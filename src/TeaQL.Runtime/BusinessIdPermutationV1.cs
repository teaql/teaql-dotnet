using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using TeaQL.Core;

namespace TeaQL.Runtime;

/// <summary>Canonical TeaQL Business ID fixed-domain permutation profile V1.</summary>
public static class BusinessIdPermutationV1
{
    public const int Width = 6;
    public const ulong DomainSize = 2_176_782_336UL;
    public const ulong MaxSequence = DomainSize - 1;
    public const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static readonly byte[] Magic = Encoding.ASCII.GetBytes("teaql-business-id-fp-v1\0");

    public static string Encode(ulong sequence, BusinessIdScope scope, BusinessIdEncodingKey key)
    {
        if (sequence >= DomainSize)
            throw new BusinessIdException(BusinessIdErrorCode.BusinessIdRangeExhausted,
                $"Business ID V1 sequence must be in 0..{MaxSequence}");

        var tweak = CanonicalTweak(scope, key.Version);
        var candidate = sequence;
        do candidate = Permute32((uint)candidate, tweak, key.Bytes);
        while (candidate >= DomainSize);

        Span<char> encoded = stackalloc char[Width];
        for (var index = Width - 1; index >= 0; index--)
        {
            encoded[index] = Alphabet[(int)(candidate % 36)];
            candidate /= 36;
        }
        return new string(encoded);
    }

    private static uint Permute32(uint value, byte[] tweak, byte[] key)
    {
        var left = (ushort)(value >> 16);
        var right = (ushort)value;
        for (byte round = 0; round < 8; round++)
        {
            var input = new byte[tweak.Length + 3];
            tweak.CopyTo(input, 0);
            input[^3] = round;
            BinaryPrimitives.WriteUInt16BigEndian(input.AsSpan(input.Length - 2), right);
            var digest = HMACSHA256.HashData(key, input);
            var output = BinaryPrimitives.ReadUInt16BigEndian(digest);
            (left, right) = (right, (ushort)(left ^ output));
        }
        return ((uint)left << 16) | right;
    }

    private static byte[] CanonicalTweak(BusinessIdScope scope, uint keyVersion)
    {
        using var bytes = new MemoryStream();
        bytes.Write(Magic);
        bytes.WriteByte(1);
        Span<byte> number = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(number, keyVersion);
        bytes.Write(number);
        WriteField(bytes, scope.DomainRootKey);
        WriteField(bytes, scope.AggregateType);
        WriteField(bytes, scope.Namespace);
        WriteField(bytes, scope.PeriodKey);
        return bytes.ToArray();
    }

    private static void WriteField(Stream output, string value)
    {
        var encoded = Encoding.UTF8.GetBytes(value);
        Span<byte> length = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(length, checked((uint)encoded.Length));
        output.Write(length);
        output.Write(encoded);
    }
}
