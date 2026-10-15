# Context owned object storage

Tracking: [.NET #52](https://github.com/teaql/teaql-dotnet/issues/52),
[shared contract #122](https://github.com/teaql/teaql-conformance/issues/122).

The optional `TeaQL.ObjectStorage.S3` project uses official AWSSDK.S3 4.0.104.3.
Core DTOs contain no SDK types; the runtime owns the provider SPI and Context
facade. Trusted startup configures the shared client, credentials provider,
bucket, endpoint, region and timeouts. Dispose the provider at shutdown.

```csharp
using TeaQL.Core.Storage;
using TeaQL.Runtime.Storage;
using TeaQL.ObjectStorage.S3;

var provider = new S3ObjectStorage(sdkClient, "attachments");
var storage = context.WithObjectStorage(provider)
    .ObjectStorage("store payment receipt attachment");
var key = new ObjectKey("receipts/opaque-reference.pdf");
await storage.PutAsync(key, source, new(contentLength, ObjectWriteMode.CreateOnly));
var link = await storage.GetReadUrlAsync(key);
await using var response = await storage.GetAsync(key);
await response.Stream.CopyToAsync(destination);
```

Upload inputs are disposed. The adapter stages with bounded buffers into a
private temporary file, validates exact length before requesting S3 and supports
SDK replay from that seekable body. The single-PUT limit is 5 GiB. Downloads
stream and require disposal; `GetBytesAsync(key, maximumBytes)` requires an
explicit bound. Create-only is atomic; replacement is explicit. Permission,
timeout and outage failures never become false absence.

Keys are durable; five-minute read URLs are reusable bearer links and can expire
earlier through credential/provider policy. URL issuance does not check existence
or authorize a business operation. Diagnostic representations/events exclude
private storage values and nested SDK messages. Provider selection is trusted
Context setup; S3 effects are separate from entity-ledger SQL transactions.

Configure `TEAQL_S3_ENDPOINT`, `TEAQL_S3_BUCKET`, `TEAQL_S3_ACCESS_KEY`,
`TEAQL_S3_SECRET_KEY` for a dedicated local fixture and run
`bash examples/object-storage/verify.sh` twice. All TeaQL dependencies are project
references to this checkout. Tests include contract boundaries, real SDK HTTP
failures and the live lifecycle. The example provisions its test bucket and
deletes only unique test keys. The runtime provider never creates buckets.
This gate qualifies the local fixture; other services, range/list/copy/multipart
and signed upload need separate work.
