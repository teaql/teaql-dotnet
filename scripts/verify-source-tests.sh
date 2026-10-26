#!/usr/bin/env bash
set -euo pipefail
repo="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
node --test "$repo/scripts/verify-test-inventory.test.cjs"
node "$repo/scripts/verify-test-inventory.cjs"
dotnet test "$repo/TeaQL.sln" "$@"
