#!/usr/bin/env bash
# Distinct result directories prevent parallel solution TRX files from overwriting each other.
set -euo pipefail
repo="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
results="${1:?pass a retained test result directory}"
shift
mapfile -t projects < <(rg --files "$repo/src" "$repo/tests" -g '*Tests.csproj' | LC_ALL=C sort)
((${#projects[@]} > 0)) || { echo 'FAIL: no test projects discovered' >&2; exit 1; }
for project in "${projects[@]}"; do
  name="$(basename "$project" .csproj)"
  test ! -e "$results/$name/tests.trx" || { echo "FAIL: refuse to overwrite $name evidence" >&2; exit 1; }
  dotnet test "$project" "$@" --logger 'console;verbosity=normal' \
    --logger 'trx;LogFileName=tests.trx' --results-directory "$results/$name"
  trx="$results/$name/tests.trx"
  counters="$(rg -o '<Counters[^>]*/>' "$trx")"
  [[ "$counters" != *$'\n'* ]] || { echo "FAIL: $name has ambiguous counters" >&2; exit 1; }
  declare -A test_counts=()
  for counter in total executed passed failed error timeout aborted notRunnable notExecuted; do
    counter_pattern="$counter=\"([0-9]+)\""
    [[ "$counters" =~ $counter_pattern ]] || { echo "FAIL: $name has no $counter counter" >&2; exit 1; }
    test_counts[$counter]="${BASH_REMATCH[1]}"
  done
  ((test_counts[total] > 0 && test_counts[total] == test_counts[executed] && test_counts[total] == test_counts[passed])) ||
    { echo "FAIL: $name did not execute and pass every test" >&2; exit 1; }
  for counter in failed error timeout aborted notRunnable notExecuted; do
    ((test_counts[$counter] == 0)) || { echo "FAIL: $name has $counter cases" >&2; exit 1; }
  done
done
echo "PASS: .NET source test projects ${#projects[@]}; distinct retained TRX per project"
