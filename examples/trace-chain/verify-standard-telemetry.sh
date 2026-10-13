#!/usr/bin/env bash
set -euo pipefail
repo="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
evidence="$(mktemp -d -t teaql-dotnet-standard-example.XXXXXXXX)"
echo ".NET standard telemetry example evidence: $evidence"
cd "$repo"
fingerprint() { find examples/trace-chain/lib -type f ! -path '*/bin/*' ! -path '*/obj/*' -print0 | sort -z | xargs -0 sha256sum; }
fingerprint > "$evidence/library-before.sha256"
for round in 1 2; do
  dotnet run --project examples/trace-chain/standard-telemetry/standard-telemetry.csproj \
    --property:TeaQLRuntimeSourceRoot="$repo" --nologo > "$evidence/run-$round.log" 2>&1
  rg -q 'DOTNET_STANDARD_TELEMETRY_EXAMPLE PASSED first_seed_audits=1 repeat_seed_audits=0 queries=2 ordinary_sql_logs=0' "$evidence/run-$round.log"
  for name in TEAQL_TELEMETRY_ENABLED TEAQL_TELEMETRY_SERVICE_NAME TEAQL_TELEMETRY_OTLP_ENDPOINT; do
    rg -q "$name" "$evidence/run-$round.log"
  done
  cat "$evidence/run-$round.log"
done
fingerprint > "$evidence/library-after.sha256"
cmp "$evidence/library-before.sha256" "$evidence/library-after.sha256"
echo "PASS: generated .NET standard telemetry example twice; generated library unchanged"
