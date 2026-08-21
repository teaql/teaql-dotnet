using System.Reflection;
using System.Text.Json;

namespace TeaQL.Runtime;

public enum TeaQLLocale { English, ChineseSimplified, ChineseTraditional, Japanese, Korean, German, French, Spanish, Portuguese, Arabic, Thai, Indonesian, Filipino, Ukrainian, Vietnamese }
public sealed class UnsupportedLocaleException(string code) : ArgumentException($"Unsupported locale: {code}") { public string LocaleCode { get; } = code; }

public static class TeaQLLocales
{
    public static readonly TeaQLLocale[] All = Enum.GetValues<TeaQLLocale>();
    private static readonly Dictionary<TeaQLLocale,string> Codes = new(){{TeaQLLocale.English,"en"},{TeaQLLocale.ChineseSimplified,"zh-CN"},{TeaQLLocale.ChineseTraditional,"zh-TW"},{TeaQLLocale.Japanese,"ja"},{TeaQLLocale.Korean,"ko"},{TeaQLLocale.German,"de"},{TeaQLLocale.French,"fr"},{TeaQLLocale.Spanish,"es"},{TeaQLLocale.Portuguese,"pt"},{TeaQLLocale.Arabic,"ar"},{TeaQLLocale.Thai,"th"},{TeaQLLocale.Indonesian,"id"},{TeaQLLocale.Filipino,"fil"},{TeaQLLocale.Ukrainian,"uk"},{TeaQLLocale.Vietnamese,"vi"}};
    private static readonly Dictionary<string,TeaQLLocale> Aliases = new(StringComparer.OrdinalIgnoreCase){{"en-us",TeaQLLocale.English},{"en-gb",TeaQLLocale.English},{"zh",TeaQLLocale.ChineseSimplified},{"zh-hans",TeaQLLocale.ChineseSimplified},{"zh-sg",TeaQLLocale.ChineseSimplified},{"cn",TeaQLLocale.ChineseSimplified},{"zh-hant",TeaQLLocale.ChineseTraditional},{"zh-hk",TeaQLLocale.ChineseTraditional},{"zh-mo",TeaQLLocale.ChineseTraditional},{"tw",TeaQLLocale.ChineseTraditional},{"ja-jp",TeaQLLocale.Japanese},{"ko-kr",TeaQLLocale.Korean},{"de-de",TeaQLLocale.German},{"fr-fr",TeaQLLocale.French},{"es-mx",TeaQLLocale.Spanish},{"pt-br",TeaQLLocale.Portuguese},{"pt-pt",TeaQLLocale.Portuguese},{"ar-sa",TeaQLLocale.Arabic},{"th-th",TeaQLLocale.Thai},{"id-id",TeaQLLocale.Indonesian},{"tl",TeaQLLocale.Filipino},{"fil-ph",TeaQLLocale.Filipino},{"uk-ua",TeaQLLocale.Ukrainian},{"vi-vn",TeaQLLocale.Vietnamese}};
    public static string Code(this TeaQLLocale locale)=>Codes[locale];
    public static TeaQLLocale Parse(string code){if(string.IsNullOrWhiteSpace(code))throw new UnsupportedLocaleException(code);var normalized=code.Trim().Replace('_','-');foreach(var pair in Codes)if(string.Equals(pair.Value,normalized,StringComparison.OrdinalIgnoreCase))return pair.Key;if(Aliases.TryGetValue(normalized,out var locale))return locale;throw new UnsupportedLocaleException(code);}
}

public sealed class CheckResult
{
    public required string RuleId {get;init;} public required string Location {get;init;}
    public object? InputValue {get;init;} public object? SystemValue {get;init;} public string? Message {get;set;}
}

public sealed class CheckException(IReadOnlyList<CheckResult> violations)
    : Exception("Check failed: " + string.Join("; ", violations.Select(x => x.Message ?? $"{x.RuleId}:{x.Location}")))
{
    public IReadOnlyList<CheckResult> Violations { get; } = violations;
}

public sealed class I18nCatalog
{
    private sealed record LocaleData(Dictionary<string,string> Messages, Dictionary<string,string> Vocabulary);
    private readonly Dictionary<string,LocaleData> _locales; private readonly I18nCatalog? _fallback;
    private static readonly Lazy<I18nCatalog> BuiltinValue=new(()=>{using var stream=Assembly.GetExecutingAssembly().GetManifestResourceStream("TeaQL.Runtime.builtin-messages-v1.json")??throw new InvalidOperationException("Missing built-in i18n catalog");return FromJson(stream);});
    private I18nCatalog(Dictionary<string,LocaleData> locales,I18nCatalog? fallback){_locales=locales;_fallback=fallback;}
    public static I18nCatalog Builtin=>BuiltinValue.Value;
    public static I18nCatalog FromJson(Stream stream,I18nCatalog? fallback=null){using var doc=JsonDocument.Parse(stream);if(doc.RootElement.GetProperty("schema").GetString()!="teaql.i18n/v1")throw new ArgumentException("Unsupported i18n schema");var locales=new Dictionary<string,LocaleData>();foreach(var locale in doc.RootElement.GetProperty("locales").EnumerateObject()){var code=TeaQLLocales.Parse(locale.Name).Code();var messages=locale.Value.GetProperty("messages").EnumerateObject().ToDictionary(x=>x.Name,x=>x.Value.GetString()!);var vocabulary=locale.Value.GetProperty("vocabulary").EnumerateObject().ToDictionary(x=>x.Name,x=>x.Value.GetString()!);locales[code]=new(messages,vocabulary);}return new(locales,fallback);}
    private string? Find(string code,string key)=>_locales.TryGetValue(code,out var locale)&&locale.Messages.TryGetValue(key,out var value)?value:null;
    public string Message(TeaQLLocale locale,string key)=>Find(locale.Code(),key)??_fallback?.Find(locale.Code(),key)??Find("en",key)??_fallback?.Find("en",key)??key;
    public CheckResult Translate(CheckResult result,TeaQLLocale locale){var keys=new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase){{"required","checker.required"},{"min","checker.min"},{"max","checker.max"},{"min_str_len","checker.minLength"},{"min_length","checker.minLength"},{"max_str_len","checker.maxLength"},{"max_length","checker.maxLength"}};var key=keys.TryGetValue(result.RuleId,out var found)?found:$"checker.{result.RuleId.ToLowerInvariant()}";var input=result.InputValue?.ToString()??"null";result.Message=Message(locale,key).Replace("{location}",result.Location).Replace("{system}",result.SystemValue?.ToString()??"null").Replace("{input}",input).Replace("{input_len}",input.Length.ToString());return result;}
}
