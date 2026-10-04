#!/usr/bin/env bash
set -euo pipefail
example="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo="$(cd "$example/../.." && pwd)"
facet_run_directory="$(mktemp -d -t teaql-dotnet-facet.XXXXXX)"
export TEAQL_FACET_TRACE_DB="$facet_run_directory/facet.db"
unset TEAQL_ALLOW_SENSITIVE_PLAINTEXT_LOGS
fingerprint() {
  (cd "$example/lib" && rg --files -g '!**/bin/**' -g '!**/obj/**' | LC_ALL=C sort | xargs -d '\n' sha256sum)
}
fingerprint > "$facet_run_directory/library-before.sha256"
# Runtime examples always depend on repository sources. Artifact acceptance
# belongs in a separate workspace with a pinned version and load-hash checks.
if ! dotnet build "$example/Consumer.csproj" -p:TeaQLRuntimeSourceRoot="$repo" \
    -p:UseSharedCompilation=false --verbosity minimal > "$facet_run_directory/build.log" 2>&1; then
  tail -80 "$facet_run_directory/build.log" >&2
  echo "FAIL: Facet build; evidence $facet_run_directory" >&2
  exit 1
fi
for round in first second; do
  log="$facet_run_directory/$round.log"
  if ! timeout --kill-after=5s 60s dotnet run --project "$example/Consumer.csproj" \
      --no-build --no-restore > "$log" 2>&1; then
    tail -80 "$log" >&2
    echo "FAIL: Facet $round; evidence $facet_run_directory" >&2
    exit 1
  fi
  [[ "$(rg -c '^FACET_OBSERVED ' "$log")" == 24 ]]
  rg -Fq 'DOTNET_FACET_PASS scenarios=24' "$log"
  rg -Fxq 'REVERSE_CHILD_UPDATE_PASS count=1' "$log"
  echo "PASS: generated Facet $round, 24 scenarios and reverse-child save; same database $TEAQL_FACET_TRACE_DB; log $log"
done
fingerprint > "$facet_run_directory/library-after.sha256"
cmp "$facet_run_directory/library-before.sha256" "$facet_run_directory/library-after.sha256"
echo "PASS: generated Facet library unchanged; retained evidence $facet_run_directory"
