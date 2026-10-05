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
  echo "TRACE CHAIN round $round on the same retained database $database"
  run_log="$(mktemp -t teaql-dotnet-trace-chain.XXXXXX.log)"
  env -u TEAQL_TRACE_CHAIN_SCENARIO dotnet run --property:TeaQLRuntimeSourceRoot="$source_root" \
    --project "$example/trace-chain.csproj" -- --database "$database" | tee "$run_log"
  rg -Fq 'PASS: .NET generated trace-chain 6 scenarios' "$run_log"
  for logging in false true; do
    rg -q "TC-REQ-09 DOTNET GENERATED BOOTSTRAP PASSED logging=$logging first_writes=[01] repeat_writes=0" "$run_log"
  done
  rg -Fq 'PASS .NET graph identity controls: duplicate, missing and equal-ID type collapse rejected' "$run_log"
  rg -Fq 'PASS FORWARD_NOTLOADED: generated Q/E retains FK and fails closed on hidden detail' "$run_log"
  rg -Fq 'PASS: .NET generated ownership 4 scenarios' "$run_log"
  rg -Fq 'PASS: .NET generated page count, scope, privacy and independent graph saves' "$run_log"
  rg -Fq 'PASS: .NET generated stream capture, overlapping cursors, privacy and independent saves' "$run_log"
  rg -Fq 'PASS: .NET generated successful readbacks, sibling privacy and independent request intent' "$run_log"
  rg -Fq 'PASS: .NET loaded scalar privacy, snapshot refresh, deletion and independent request' "$run_log"
  rg -Fq 'PASS: .NET loaded graph privacy rollback and same-wrapper retry' "$run_log"
  rg -Fq 'PASS: .NET generated aggregates 4 cases, isolated saves and 2 failure recoveries' "$run_log"
  rg -Fq 'PASS: .NET generated Checker accepted/rejected overlap 4 cases, shared reference and retry' "$run_log"
  [[ "$before" == "$(fingerprint)" ]] || { echo 'FAIL: generated library bytes changed' >&2; exit 1; }
done
echo "PASS: .NET generated trace-chain twice without cleanup; $(wc -l <<< "$before") unchanged library files"
