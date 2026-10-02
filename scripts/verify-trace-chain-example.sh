#!/usr/bin/env bash
set -euo pipefail
repo="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
example="$repo/examples/trace-chain"
database="${TEAQL_TRACE_CHAIN_DB:-$repo/examples/.local/trace-chain.sqlite}"
source_root="${TeaQLRuntimeSourceRoot:-$repo}"
fingerprint() {
  rg --files "$example/lib" -g '*.cs' -g '*.csproj' -g '!**/obj/**' -g '!**/bin/**' |
    LC_ALL=C sort | xargs -d '\n' sha256sum
}
before="$(fingerprint)"
for round in 1 2; do
  echo "TRACE CHAIN round $round on the same retained database"
  run_log="$(mktemp -t teaql-dotnet-trace-chain.XXXXXX.log)"
  env -u TEAQL_TRACE_CHAIN_SCENARIO dotnet run --property:TeaQLRuntimeSourceRoot="$source_root" \
    --project "$example/trace-chain.csproj" -- --database "$database" | tee "$run_log"
  rg -Fq 'PASS: .NET generated trace-chain 6 scenarios' "$run_log"
  rg -Fq 'PASS: .NET generated ownership 4 scenarios' "$run_log"
  [[ "$before" == "$(fingerprint)" ]] || { echo 'FAIL: generated library bytes changed' >&2; exit 1; }
done
echo "PASS: .NET generated trace-chain twice without cleanup; $(wc -l <<< "$before") unchanged library files"
