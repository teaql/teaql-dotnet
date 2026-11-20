#!/usr/bin/env bash
set -euo pipefail
repo="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
generated="$repo/examples/trace-chain/standard-order/dotnet-lib-core"
evidence="${TEAQL_TRACE_CHAIN_STANDARD_EVIDENCE_DIR:-$(mktemp -d -t teaql-dotnet-standard-order.XXXXXXXX)}"
mkdir -p "$evidence"
echo ".NET standard Order evidence: $evidence"
# This generated snapshot is checked in so the public runtime gate needs no
# private generator checkout or remote service. Maintainers regenerate it from
# model.xml with the current producer; never repair its source by hand.
fingerprint() {
  (cd "$generated" && rg --files -g '*.cs' -g '*.csproj' -g '!bin/**' -g '!obj/**' | LC_ALL=C sort | xargs -d '\n' sha256sum)
}
fingerprint > "$evidence/library-before.sha256"
dotnet build "$repo/examples/trace-chain/standard-order/dotnet-app-console/request-tree-order-service-console.csproj" \
  "-p:TeaQLRuntimeSourceRoot=$repo" "-p:TeaQLOrderGeneratedRoot=$generated" \
  -p:UseSharedCompilation=false --nologo > "$evidence/build.log" 2>&1
for round in 1 2; do
  TEAQL_STANDARD_ORDER_DB="$evidence/orders.sqlite" dotnet run \
    --project "$repo/examples/trace-chain/standard-order/dotnet-app-console/request-tree-order-service-console.csproj" \
    "-p:TeaQLRuntimeSourceRoot=$repo" "-p:TeaQLOrderGeneratedRoot=$generated" \
    --no-build --no-restore --nologo > "$evidence/run-$round.log" 2>&1
  rg -q 'DOTNET_STANDARD_ORDER PASSED queries=2 total=3 preview=3 itemCount=5 facets=3' "$evidence/run-$round.log"
  if [[ "$round" == 1 ]]; then expected_seed=4; else expected_seed=0; fi
  rg -q "first_seed_audits=$expected_seed repeat_seed_audits=0" "$evidence/run-$round.log"
  if [[ "${TEAQL_TELEMETRY_ENABLED:-false}" == "true" ]]; then
    rg -Fq 'TeaQL telemetry: enabled' "$evidence/run-$round.log"
  else
    for name in TEAQL_TELEMETRY_ENABLED TEAQL_TELEMETRY_SERVICE_NAME TEAQL_TELEMETRY_OTLP_ENDPOINT; do
      rg -q "$name" "$evidence/run-$round.log"
    done
  fi
  cat "$evidence/run-$round.log"
done
fingerprint > "$evidence/library-after.sha256"
cmp "$evidence/library-before.sha256" "$evidence/library-after.sha256"
echo "PASS: generated .NET complete Order twice without database cleanup; library unchanged"
