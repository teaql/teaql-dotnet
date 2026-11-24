# Supplier onboarding in 15 locales with TeaQL C# and .NET

The [KSML model](model.xml) describes a warehouse with suppliers. The [semantic model](semantics.json) sends two registrations through the generated workspace. One lacks a supplier name. The other submits 3 as the minimum order quantity, below the allowed value of 10. The application attempts audited saves and uses generated Q to confirm that SQLite stores neither invalid supplier.

## Run and select a locale

A .NET 8 SDK, SQLite-capable platform, and Python 3.9 or newer. The verified SDK was 8.0.428 with runtime 8.0.34. This example uses local .NET runtime package 0.6.0 source. The SDK restores public dependencies and compiles the generated console with a project reference to the generated domain library. Restore packages and all build outputs go into temporary storage.

```sh
bash examples/supplier-onboarding-i18n/run.sh
bash examples/supplier-onboarding-i18n/run.sh --locale fr
bash examples/supplier-onboarding-i18n/run.sh --all
bash examples/supplier-onboarding-i18n/run.sh --edge-cases
bash examples/supplier-onboarding-i18n/run.sh --verify
```

The commands run from the runtime repository root. The output is UTF-8 JSON Lines. Each validation row gives the effective locale, stable message key, canonical field, native rule, arguments and localized message. `--all` uses the 15 locales from the semantic model in one process. The final command runs the full set twice against one SQLite file and runs the locale boundary checks.

The app selects the locale with `context.SetLocaleCode(code)` and reads `context.Locale.Code()`. A new context defaults to `en`. The parser trims, normalizes underscores and case, and resolves known aliases such as `ZH_hant` to `zh-TW`. An empty or unsupported code raises `UnsupportedLocaleException` and preserves the previous locale. Single-language mode prints the rejection and exits 2. Locale selection comes from the trusted CLI and context. Environment variables and business input do not set it.

`TeaQLRuntimeSourceRoot` defaults to the enclosing repository. `TEAQL_I18N_NUGET_SOURCE` defaults to `https://api.nuget.org/v3/index.json`. Empty values select the defaults.

## How the message is produced

1. The console builds its `UserContext`, installs generated model metadata, and explicitly initializes SQLite. It sets the locale before the generated business call.
2. The real model check rejects the invalid registration. `CheckException.Violations` exposes `RuleId` and `ModelPath`. `ToWire()` supplies the safe error message and native arguments. The numeric bound is present and the erroneous input value is omitted by the generated Checker.
3. The renderer chooses the locale and a template for the field's location shape. It binds the rendered field, model path and applicable checker parameters, including the lower bound.
4. The sample attaches the generated application catalog through `context.InstallI18nCatalog(I18nCatalog.FromJson(stream, I18nCatalog.Builtin))`. Its Vietnamese message entries are absent. Built-in Vietnamese supplies the same two messages after attachment. For this natural checker message, selection tries the application's shape-specific override, the application legacy template, and the built-in natural template at the active locale, then repeats that order for English. The generic invalid rule can supply the final natural template. Direct lookup through the legacy message catalog selects application and built-in entries at the current locale first, then application and built-in English, then the stable key.

The independent French context remains French through the main context's Japanese, Arabic and Japanese sequence. Domain labels without an available translation can remain in English. The new contexts share one locally opened SQLite connection. The native minimum response omits the input value, while the original test input is still visible in the shared scenario. [Locale source](../../src/TeaQL.Runtime/I18n.cs) · [Message lookup source](../../src/TeaQL.Runtime/I18n.cs) · [Natural templates source](../../src/TeaQL.Runtime/CheckerMessageCatalog.cs)

## Run the same scene in another runtime

Keep the same KSML and semantic inputs when switching to its sibling checkout. Each runtime uses its own setup and call style, while the error keys, field paths and minimum bound stay stable.

[Java](../../../java/examples/supplier-onboarding-i18n/README.md) · [Rust](../../../rust/examples/supplier-onboarding-i18n/README.md) · [TypeScript](../../../typescript/examples/supplier-onboarding-i18n/README.md) · [Go](../../../go/examples/supplier-onboarding-i18n/README.md) · [Python](../../../python/examples/supplier-onboarding-i18n/README.md) · [C#/.NET](./README.md) · [Swift](../../../swift/Examples/SupplierOnboardingI18n/README.md)

## Add a locale

Add the `Italian` value to `TeaQLLocale` and `TeaQLLocale.Italian` with code `it` to `TeaQLLocales.Codes` in `I18n.cs`. Add any aliases in `Aliases`. Update the built-in resources `builtin-messages-v1.json`, `natural-messages-v1.json` and `checker-presentation-v1.json` under `src/TeaQL.Runtime`. Supply natural forms for the supported shapes and add presentation words, such as the name for the current object and the measurement units. For a legacy required-message entry the identifier and placeholder stay the same.

```json
{
  "it": {
    "messages": {"checker.required": "Il campo {location} è obbligatorio"},
    "vocabulary": {}
  }
}
```

Keep placeholders such as `{field}`, `{location}` and `{system}` aligned with their respective catalog schemas. Add the locale to the generator and shared semantic model, regenerate the workspaces, then run the same save, alias, fallback and isolation checks. To customize text for an already supported locale, attach an explicit application catalog.
