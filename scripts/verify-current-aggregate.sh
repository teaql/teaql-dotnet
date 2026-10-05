#!/usr/bin/env bash
set -euo pipefail
repo="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
database="${TEAQL_TRACE_CHAIN_DB:-$repo/examples/.local/current-aggregate.sqlite}"
source_root="${TeaQLRuntimeSourceRoot:-$repo}"
for round in 1 2; do
  echo "CURRENT AGGREGATE native round $round"
  dotnet test "$repo/src/TeaQL.Provider.Sqlite.Tests/TeaQL.Provider.Sqlite.Tests.csproj" \
    --filter 'FullyQualifiedName~RelationAggregateTraceTests' \
    -p:UseSharedCompilation=false -maxcpucount:2
done
TEAQL_TRACE_CHAIN_DB="$database" TeaQLRuntimeSourceRoot="$source_root" \
  bash "$repo/scripts/verify-trace-chain-example.sh"
echo 'PASS .NET current aggregate gate: native twice and full generated trace-chain twice without cleanup'
