#!/usr/bin/env bash
set -euo pipefail
repo="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
example="$repo/examples/school-management"
evidence="$(mktemp -d -t teaql-dotnet-bootstrap.XXXXXXXX)"
fingerprint() {
  { rg --files "$example/Models" "$example/Requests"; printf '%s\n' "$example/Q.cs" "$example/E.cs" "$example/GeneratedRuntimeModule.cs"; } |
    LC_ALL=C sort | xargs -d '\n' sha256sum
}
fingerprint > "$evidence/library-before.sha256"
dotnet build --property:TeaQLRuntimeSourceRoot="${TeaQLRuntimeSourceRoot:-$repo}" \
  "$example/school-management-service-lib.csproj"
for logging in on off; do
  for round in 1 2; do
    TEAQL_SCHOOL_BOOTSTRAP_DB="$evidence/$logging.sqlite" TEAQL_SCHOOL_BOOTSTRAP_LOGGING="$logging" \
      dotnet run --no-build --no-restore --property:TeaQLRuntimeSourceRoot="${TeaQLRuntimeSourceRoot:-$repo}" \
      --project "$example/school-management-service-lib.csproj" | tee "$evidence/$logging-$round.log"
    expected_logging=True; [[ "$logging" == off ]] && expected_logging=False
    fresh=True; version=1
    if [[ "$round" == 2 ]]; then fresh=False; version=3; fi
    rg -Fq "PASS .NET generated bootstrap trace logging=$expected_logging fresh=$fresh originalVersion=$version" "$evidence/$logging-$round.log"
    fingerprint > "$evidence/library-after.sha256"
    cmp "$evidence/library-before.sha256" "$evidence/library-after.sha256"
  done
done
echo "PASS .NET generated bootstrap twice per logging mode; retained evidence: $evidence"
