using System.Reflection;
using System.Text.RegularExpressions;
using TeaQL.Core;

namespace TeaQL.Runtime.Tests;

public class BusinessIdPermutationV1Tests
{
    [Fact]
    public void MatchesEveryCrossLanguageGoldenVector()
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("business-id-permutation-v1.csv");
        Assert.NotNull(stream);
        using var reader = new StreamReader(stream!);
        Assert.Equal(
            "case_id,key_hex,key_version,domain_root_key,aggregate_type,namespace,period_key,sequence,expected_code,expected_business_id",
            reader.ReadLine());
        var rows = 0;
        while (reader.ReadLine() is { } line)
        {
            var fields = line.Split(',', StringSplitOptions.None);
            Assert.Equal(10, fields.Length);
            var scope = new BusinessIdScope(fields[3], fields[4], fields[5], fields[6]);
            var key = new BusinessIdEncodingKey(uint.Parse(fields[2]), Convert.FromHexString(fields[1]));
            var actual = BusinessIdPermutationV1.Encode(ulong.Parse(fields[7]), scope, key);
            Assert.Equal(fields[8], actual);
            Assert.Equal(fields[9], $"ORD-{fields[6]}-{actual}");
            rows++;
        }
        Assert.Equal(10, rows);
    }

    [Fact]
    public void IsDeterministicUniqueAndAlwaysCanonicalForRetainedRange()
    {
        var scope = new BusinessIdScope("tenant-a", "commerce_order", "order_number", "20260925");
        var key = new BusinessIdEncodingKey(1,
            Convert.FromHexString("000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f"));
        var values = new HashSet<string>();
        for (ulong sequence = 0; sequence < 20_000; sequence++)
        {
            var first = BusinessIdPermutationV1.Encode(sequence, scope, key);
            var afterRestart = BusinessIdPermutationV1.Encode(sequence,
                new BusinessIdScope("tenant-a", "commerce_order", "order_number", "20260925"),
                new BusinessIdEncodingKey(1, key.Bytes));
            Assert.Equal(first, afterRestart);
            Assert.Matches(new Regex("^[0-9A-Z]{6}$"), first);
            Assert.True(values.Add(first), $"duplicate at sequence {sequence}");
        }
    }

    [Fact]
    public void RejectsOutOfDomainSequenceAndMalformedDefinitions()
    {
        var scope = new BusinessIdScope("tenant-a", "commerce_order", "order_number", "20260925");
        var key = new BusinessIdEncodingKey(1, new byte[32]);
        var range = Assert.Throws<BusinessIdException>(() =>
            BusinessIdPermutationV1.Encode(BusinessIdPermutationV1.DomainSize, scope, key));
        Assert.Equal(BusinessIdErrorCode.BusinessIdRangeExhausted, range.Code);
        Assert.Throws<BusinessIdException>(() => new BusinessIdEncodingKey(0, new byte[32]));
        Assert.Throws<BusinessIdException>(() => new BusinessIdEncodingKey(1, new byte[31]));
        Assert.Throws<BusinessIdException>(() =>
            new BusinessIdScope(" ", "commerce_order", "order_number", "20260925"));
    }
}
