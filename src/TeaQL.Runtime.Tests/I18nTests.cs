using TeaQL.Runtime;
namespace TeaQL.Runtime.Tests;
public class I18nTests
{
 [Fact] public void RendersFifteenLocalesTimesFiveRules(){var rules=new[]{new CheckResult{RuleId="required",Location="name"},new CheckResult{RuleId="min",Location="age",InputValue=1,SystemValue=2},new CheckResult{RuleId="max",Location="age",InputValue=3,SystemValue=2},new CheckResult{RuleId="min_length",Location="name",InputValue="a",SystemValue=2},new CheckResult{RuleId="max_length",Location="name",InputValue="abc",SystemValue=2}};var cells=0;foreach(var locale in TeaQLLocales.All)foreach(var source in rules){var result=new CheckResult{RuleId=source.RuleId,Location=source.Location,InputValue=source.InputValue,SystemValue=source.SystemValue};I18nCatalog.Builtin.Translate(result,locale);Assert.NotNull(result.Message);Assert.False(result.Message!.StartsWith("checker."));cells++;}Assert.Equal(75,cells);}
 [Fact] public void AliasAndInvalidLocalePreserveContext(){var context=new UserContext().SetLocaleCode("ZH_hans");Assert.Equal(TeaQLLocale.ChineseSimplified,context.Locale);Assert.Throws<UnsupportedLocaleException>(()=>context.SetLocaleCode("xx"));Assert.Equal(TeaQLLocale.ChineseSimplified,context.Locale);}
}
