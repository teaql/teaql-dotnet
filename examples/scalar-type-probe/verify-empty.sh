#!/usr/bin/env bash
set -euo pipefail
example="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo="$(cd "$example/../.." && pwd)"
if [[ -n "${TEAQL_EMPTY_EVIDENCE_DIR:-}" ]]; then
  evidence="$TEAQL_EMPTY_EVIDENCE_DIR"
  mkdir -- "$evidence"
else
  evidence="$(mktemp -d -t teaql-dotnet-empty-evidence.XXXXXXXX)"
fi
evidence="$(cd "$evidence" && pwd)"
build_root="$(mktemp -d -t teaql-dotnet-empty-build.XXXXXXXX)"
cleanup() {
  result=$?
  trap - EXIT
  rm -rf -- "$build_root"
  exit "$result"
}
trap cleanup EXIT
database="${TEAQL_EMPTY_DB:-$evidence/empty-aggregate.sqlite}"
if [[ "$database" != /* ]]; then database="$(pwd)/$database"; fi
if [[ -e "$database" ]]; then
  echo "empty aggregate verification requires a fresh database; will not overwrite $database" >&2
  exit 1
fi
cd "$example"
sha256sum -c generated.sha256
flags=(-p:UseSharedCompilation=false -maxcpucount:2 -nodeReuse:false
  -p:UseArtifactsOutput=true "-p:ArtifactsPath=$build_root")
DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER=1 dotnet build empty-aggregates.csproj "${flags[@]}" \
  -p:TeaQLRuntimeSourceRoot="${TeaQLRuntimeSourceRoot:-$repo}" > "$evidence/build.log" 2>&1 || {
    tail -n 40 "$evidence/build.log"; exit 1;
  }
executable="$(dotnet msbuild empty-aggregates.csproj -nologo -getProperty:TargetPath \
  -p:UseArtifactsOutput=true "-p:ArtifactsPath=$build_root" \
  -p:TeaQLRuntimeSourceRoot="${TeaQLRuntimeSourceRoot:-$repo}")"
if [[ "$executable" != "$build_root/"* || ! -f "$executable" ]]; then
  echo "build did not produce an executable in its owned artifact directory" >&2
  exit 1
fi
unset TEAQL_EMPTY_ROUND
TEAQL_EMPTY_DB="$database" TEAQL_EMPTY_VECTORS="$example/empty-aggregate-vectors.json" \
  DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER=1 dotnet "$executable" | tee "$evidence/rounds.log"
test "$(rg -c '^DOTNET_GENERATED_EMPTY_AGGREGATE=' "$evidence/rounds.log")" = 2
sha256sum -c generated.sha256
echo "PASS .NET generated empty aggregates twice without cleanup; compact evidence: $evidence"
