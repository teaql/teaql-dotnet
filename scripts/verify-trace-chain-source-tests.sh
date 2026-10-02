#!/usr/bin/env bash
# Distinct result directories prevent parallel solution TRX files from overwriting each other.
set -euo pipefail
repo="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
results="${1:?pass a retained test result directory}"
mapfile -t projects < <(rg --files "$repo/src" "$repo/tests" -g '*Tests.csproj' | LC_ALL=C sort)
for project in "${projects[@]}"; do
  name="$(basename "$project" .csproj)"
  dotnet test "$project" --logger 'console;verbosity=normal' \
    --logger 'trx;LogFileName=tests.trx' --results-directory "$results/$name"
done
echo "PASS: .NET source test projects ${#projects[@]}; distinct retained TRX per project"
