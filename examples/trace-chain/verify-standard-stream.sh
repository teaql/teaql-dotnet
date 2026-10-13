#!/usr/bin/env bash
set -euo pipefail
repo="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
library="$repo/examples/trace-chain/standard-graph/lib"
evidence="$(mktemp -d -t teaql-dotnet-standard-stream.XXXXXXXX)"
echo ".NET standard stream evidence: $evidence"
fingerprint() {
  (cd "$library" && rg --files -g '*.cs' -g '*.csproj' -g '!bin/**' -g '!obj/**' | LC_ALL=C sort | xargs -d '\n' sha256sum)
}
fingerprint > "$evidence/library-before.sha256"
dotnet build "$repo/examples/trace-chain/standard-stream/standard-stream.csproj" \
  "-p:TeaQLRuntimeSourceRoot=$repo" -p:UseSharedCompilation=false --nologo > "$evidence/build.log" 2>&1
for round in 1 2; do
  TEAQL_STANDARD_STREAM_DB="$evidence/stream.sqlite" dotnet run \
    --project "$repo/examples/trace-chain/standard-stream/standard-stream.csproj" \
    "-p:TeaQLRuntimeSourceRoot=$repo" --no-build --no-restore --nologo > "$evidence/run-$round.log" 2>&1
  rg -q 'DOTNET_STANDARD_STREAM PASSED complete=3 early=1 chunkSize=2 token=1 capturedBuilder=true' "$evidence/run-$round.log"
  if [[ "$round" == 1 ]]; then expected=1; else expected=0; fi
  rg -q "first_seed_audits=$expected repeat_seed_audits=0" "$evidence/run-$round.log"
  for name in TEAQL_TELEMETRY_ENABLED TEAQL_TELEMETRY_SERVICE_NAME TEAQL_TELEMETRY_OTLP_ENDPOINT; do
    rg -q "$name" "$evidence/run-$round.log"
  done
  cat "$evidence/run-$round.log"
done
fingerprint > "$evidence/library-after.sha256"
cmp "$evidence/library-before.sha256" "$evidence/library-after.sha256"
echo "PASS: .NET standard scalar streams twice; retained database and unchanged generated library"
