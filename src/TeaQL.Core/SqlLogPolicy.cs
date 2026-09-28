namespace TeaQL.Core;

public enum SqlParameterLogPolicy { Unknown, Plain, Masked, Credential }

/// <summary>Shared credential classification for metadata and safe log projection.</summary>
public static class SensitiveLogNames
{
    public static bool IsCredential(string text)
    {
        var normalized = new string(text.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());
        return new[] { "password", "passwd", "passphrase", "privatekey", "secret", "accesstoken",
            "refreshtoken", "idtoken", "apikey", "authorization", "credential", "sessiontoken", "magiclinktoken" }
            .Any(normalized.Contains);
    }
}
