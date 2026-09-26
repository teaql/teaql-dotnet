using TeaQL.Runtime;
using Xunit;

namespace TeaQL.Runtime.Tests;

public class EntityReferenceCodecTests
{
    private const string Golden = "tqr1.AAAAAjMzMzMzMzMzMzMzM3bKiZgRSQQhfIj2cBXRDZIloUGHWLBp8QrXL_aejwIXPFtvV_E71O7wbOXy3cvYo_SwxvuS-89x572T9CO_pDAY4tbjWCNv";

    [Fact]
    public void EntityReferenceIsPortableOpaqueBoundRotatableAndExpiring()
    {
        var now = new DateTimeOffset(2026, 9, 26, 12, 0, 0, TimeSpan.Zero);
        var codec = new AeadEntityReferenceCodec(2, new Dictionary<uint, byte[]> {
            [1] = Enumerable.Repeat((byte)0x11, 32).ToArray(), [2] = Enumerable.Repeat((byte)0x22, 32).ToArray()
        }).WithClock(() => now).WithNonceSource(() => Enumerable.Repeat((byte)0x33, 12).ToArray());
        var token = codec.Encode("OrderItem", 42, 7, "edit-order", TimeSpan.FromHours(1));
        Assert.Equal(Golden, token);
        Assert.DoesNotContain("OrderItem", token);
        var claims = codec.Decode(token, "OrderItem", "edit-order");
        Assert.Equal((ulong)42, claims.Id); Assert.Equal(7, claims.Version); Assert.Equal((uint)2, claims.KeyVersion);
        Assert.Throws<EntityReferenceTokenException>(() => codec.Decode(token, "InvoiceItem", "edit-order"));
        Assert.Throws<EntityReferenceTokenException>(() => codec.Decode(token, "OrderItem", "other-purpose"));
        Assert.Throws<EntityReferenceTokenException>(() => codec.Decode(token[..^1] + "A", "OrderItem", "edit-order"));
        codec.WithClock(() => now.AddHours(2));
        Assert.Throws<EntityReferenceTokenException>(() => codec.Decode(token, "OrderItem", "edit-order"));
    }

    [Fact]
    public void RawReferencesRequireExactDevelopmentAcknowledgement()
    {
        var previous = Environment.GetEnvironmentVariable("TEAQL_UNSAFE_RAW_ENTITY_REFERENCES");
        try
        {
            Environment.SetEnvironmentVariable("TEAQL_UNSAFE_RAW_ENTITY_REFERENCES", null);
            var context = new UserContext();
            Assert.Throws<EntityReferenceTokenException>(() => context.EncodeEntityReference("Order", 1, 1, "edit", TimeSpan.FromMinutes(1)));
            Environment.SetEnvironmentVariable("TEAQL_UNSAFE_RAW_ENTITY_REFERENCES", "I_UNDERSTAND_RAW_ENTITY_IDS_ARE_VISIBLE_FOR_LOCAL_DEVELOPMENT_ONLY");
            var token = context.EncodeEntityReference("Order", 1, 1, "edit", TimeSpan.FromMinutes(1));
            Assert.StartsWith("tqr0.", token);
            Assert.Equal((ulong)1, context.DecodeEntityReference(token, "Order", "edit").Id);
        }
        finally { Environment.SetEnvironmentVariable("TEAQL_UNSAFE_RAW_ENTITY_REFERENCES", previous); }
    }
}
