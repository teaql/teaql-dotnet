using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using TeaQL.BlobReference;
using TeaQL.Core.Storage;
using TeaQL.Runtime;
using TeaQL.Runtime.Storage;
using Xunit;

namespace TeaQL.Provider.Sqlite.Tests;

public class BlobFileObjectStorageTests
{
    [Fact]
    public async Task ConditionalDeleteWaitsForExternalManifestLockOnLinux()
    {
        if (!OperatingSystem.IsLinux() || !File.Exists("/usr/bin/flock")) return;
        var parent = Path.Combine(Path.GetTempPath(), "teaql-file-process-lock-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(parent);
        try
        {
            var root = Path.Combine(parent, "private");
            await using var provider = new FileObjectStorage(root, RandomNumberGenerator.GetBytes(32));
            var storage = new UserContext().WithObjectStorage(provider).ObjectStorage("verify process lock");
            var key = new ObjectKey("blob/process-lock");
            var bytes = Encoding.UTF8.GetBytes("content");
            var written = await storage.PutBytesAsync(key, bytes, new(bytes.Length, ObjectWriteMode.CreateOnly));
            var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(key.Value))).ToLowerInvariant();
            var lockPath = Path.Combine(root, hash[..2], hash, "manifest.lock");
            var marker = Path.Combine(parent, "locked");
            var start = new ProcessStartInfo("/usr/bin/flock") { UseShellExecute = false };
            start.ArgumentList.Add("-x");
            start.ArgumentList.Add(lockPath);
            start.ArgumentList.Add("/bin/sh");
            start.ArgumentList.Add("-c");
            start.ArgumentList.Add("touch \"$1\"; sleep 3");
            start.ArgumentList.Add("sh");
            start.ArgumentList.Add(marker);
            using var process = Process.Start(start) ?? throw new InvalidOperationException("flock did not start");
            try
            {
                for (var attempt = 0; !File.Exists(marker) && attempt < 100; attempt++)
                    await Task.Delay(10);
                Assert.True(File.Exists(marker), "external process never acquired the manifest lock");
                var deletion = storage.DeleteIfVersionAsync(key, written.ETag!);
                await Task.Delay(100);
                Assert.False(process.HasExited, "external lock holder already exited");
                Assert.False(deletion.IsCompleted,
                    "deletion completed while locked: " + (deletion.Exception?.GetBaseException().GetType().Name ?? "success"));
                await process.WaitForExitAsync();
                Assert.Equal(0, process.ExitCode);
                Assert.Equal(ObjectConditionalDeleteResult.Deleted, await deletion);
            }
            finally
            {
                if (!process.HasExited) process.Kill();
                await process.WaitForExitAsync();
            }
        }
        finally { Directory.Delete(parent, true); }
    }

    [Fact]
    public async Task ConditionalDeleteFencesSameByteReplacementAcrossInstances()
    {
        var parent = Path.Combine(Path.GetTempPath(), "teaql-file-conditional-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(parent);
        try
        {
            var root = Path.Combine(parent, "private");
            var secret = RandomNumberGenerator.GetBytes(32);
            await using var first = new FileObjectStorage(root, secret);
            await using var second = new FileObjectStorage(root, secret);
            var writer = new UserContext().WithObjectStorage(first).ObjectStorage("write File generation");
            var collector = new UserContext().WithObjectStorage(second).ObjectStorage("delete File generation");
            var key = new ObjectKey("blob/same-byte-replacement");
            Assert.Null(await collector.HeadAsync(key));
            Assert.Equal(ObjectConditionalDeleteResult.AlreadyMissing,
                await collector.DeleteIfVersionAsync(key, "absent"));
            var bytes = Encoding.UTF8.GetBytes("same bytes");
            var initial = await writer.PutBytesAsync(key, bytes, new(bytes.Length, ObjectWriteMode.CreateOnly));
            var newer = await writer.PutBytesAsync(key, bytes, new(bytes.Length, ObjectWriteMode.Replace));
            Assert.NotEqual(initial.ETag, newer.ETag);
            Assert.Equal(ObjectConditionalDeleteResult.VersionMismatch,
                await collector.DeleteIfVersionAsync(key, initial.ETag!));
            Assert.Equal(bytes, await collector.GetBytesAsync(key, 100));
            Assert.Equal(ObjectConditionalDeleteResult.Deleted,
                await collector.DeleteIfVersionAsync(key, newer.ETag!));
            Assert.Null(await collector.HeadAsync(key));
            Assert.Equal(ObjectConditionalDeleteResult.AlreadyMissing,
                await collector.DeleteIfVersionAsync(key, newer.ETag!));
            var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(key.Value))).ToLowerInvariant();
            var version = newer.ETag!.Split('.').Last();
            Assert.False(File.Exists(Path.Combine(root, hash[..2], hash, "data-" + version)));
        }
        finally { Directory.Delete(parent, true); }
    }

    [Fact]
    public async Task RealFileBytesAndAuthenticatedLoopbackReadUrl()
    {
        var parent = Path.Combine(Path.GetTempPath(), "teaql-file-store-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(parent);
        try
        {
            await using var provider = new FileObjectStorage(Path.Combine(parent, "private"), RandomNumberGenerator.GetBytes(32));
            var key = new ObjectKey("private/report.pdf");
            var actor = new UserContext().WithObjectStorage(provider).ObjectStorage("test File provider");
            var bytes = Encoding.UTF8.GetBytes("%PDF-1.4\nreal bytes\n%%EOF");
            var beforeStart = await Assert.ThrowsAsync<StorageException>(() => actor.GetReadUrlAsync(key));
            Assert.Equal(StorageErrorCode.NotConfigured, beforeStart.Code);
            Assert.Equal("127.0.0.1", provider.Start().Host);
            Assert.Null(await actor.HeadAsync(key));
            var initial = await actor.PutBytesAsync(key, bytes,
                new(bytes.Length, ObjectWriteMode.CreateOnly, "application/pdf"));
            Assert.Equal(bytes.Length, initial.Size);
            var sameBytes = await actor.PutBytesAsync(key, bytes,
                new(bytes.Length, ObjectWriteMode.Replace, "application/pdf"));
            Assert.NotEqual(initial.ETag, sameBytes.ETag);
            Assert.StartsWith(Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant() + ".", sameBytes.ETag);
            var conflict = await Assert.ThrowsAsync<StorageException>(() => actor.PutBytesAsync(key, bytes,
                new(bytes.Length, ObjectWriteMode.CreateOnly)));
            Assert.Equal(StorageErrorCode.Conflict, conflict.Code);
            var shortWrite = await Assert.ThrowsAsync<StorageException>(() => actor.PutBytesAsync(key, [1],
                new(2, ObjectWriteMode.Replace)));
            Assert.Equal(StorageErrorCode.InvalidInput, shortWrite.Code);
            Assert.Equal(bytes, await actor.GetBytesAsync(key, 100));
            using var http = new HttpClient(new HttpClientHandler { UseProxy = false });
            var link = await actor.GetReadUrlAsync(key, new(TimeSpan.FromSeconds(5), "attachment", "report.pdf"));
            Assert.DoesNotContain(key.Value, link.Url.ToString());
            using (var response = await http.GetAsync(link.Url))
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(bytes, await response.Content.ReadAsByteArrayAsync());
                Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);
                Assert.Equal($"\"{sameBytes.ETag}\"", response.Headers.ETag?.ToString());
                Assert.Equal("attachment; filename=\"report.pdf\"", response.Content.Headers.ContentDisposition?.ToString());
                Assert.True(response.Headers.CacheControl?.Private);
                Assert.True(response.Headers.CacheControl?.NoStore);
            }
            using (var head = new HttpRequestMessage(HttpMethod.Head, link.Url))
            using (var response = await http.SendAsync(head))
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(bytes.Length, response.Content.Headers.ContentLength);
            }
            using (var request = new HttpRequestMessage(HttpMethod.Get, link.Url))
            {
                request.Headers.Range = new RangeHeaderValue(0, 3);
                using var response = await http.SendAsync(request);
                Assert.Equal(HttpStatusCode.PartialContent, response.StatusCode);
                Assert.Equal(bytes[..4], await response.Content.ReadAsByteArrayAsync());
                Assert.Equal($"bytes 0-3/{bytes.Length}", response.Content.Headers.ContentRange?.ToString());
            }
            using (var request = new HttpRequestMessage(HttpMethod.Get, link.Url))
            {
                request.Headers.Range = new RangeHeaderValue(null, 3);
                using var response = await http.SendAsync(request);
                Assert.Equal(HttpStatusCode.PartialContent, response.StatusCode);
                Assert.Equal(bytes[^3..], await response.Content.ReadAsByteArrayAsync());
            }
            using (var request = new HttpRequestMessage(HttpMethod.Get, link.Url))
            {
                request.Headers.Range = new RangeHeaderValue(bytes.Length, null);
                using var response = await http.SendAsync(request);
                Assert.Equal(HttpStatusCode.RequestedRangeNotSatisfiable, response.StatusCode);
                Assert.Equal($"bytes */{bytes.Length}", response.Content.Headers.ContentRange?.ToString());
            }
            var urlText = link.Url.ToString();
            var flip = urlText.Length - 12;
            var tampered = new Uri(urlText[..flip] + (urlText[flip] == 'A' ? 'B' : 'A') + urlText[(flip + 1)..]);
            Assert.Equal(HttpStatusCode.NotFound, (await http.GetAsync(tampered)).StatusCode);
            var expiring = await actor.GetReadUrlAsync(key, new(TimeSpan.FromSeconds(1)));
            await Task.Delay(1200);
            Assert.Equal(HttpStatusCode.NotFound, (await http.GetAsync(expiring.Url)).StatusCode);
            var replacement = Encoding.UTF8.GetBytes("replacement");
            await actor.PutBytesAsync(key, replacement, new(replacement.Length, ObjectWriteMode.Replace, "text/plain"));
            Assert.Equal(HttpStatusCode.NotFound, (await http.GetAsync(link.Url)).StatusCode);
            Assert.Equal(replacement, await actor.GetBytesAsync(key, 100));
            var currentLink = await actor.GetReadUrlAsync(key);
            var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(key.Value))).ToLowerInvariant();
            var directory = Path.Combine(parent, "private", hash[..2], hash);
            foreach (var path in Directory.GetFiles(directory, "data-*")) File.Delete(path);
            var missing = await Assert.ThrowsAsync<StorageException>(() => actor.HeadAsync(key));
            Assert.Equal(StorageErrorCode.Unavailable, missing.Code);
            Assert.Equal(HttpStatusCode.NotFound, (await http.GetAsync(currentLink.Url)).StatusCode);
            await actor.DeleteAsync(key);
            Assert.Null(await actor.HeadAsync(key));
        }
        finally { Directory.Delete(parent, true); }
    }
}
