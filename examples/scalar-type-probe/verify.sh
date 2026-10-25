#!/usr/bin/env bash
set -euo pipefail
example="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo="$(cd "$example/../.." && pwd)"
if [[ -n "${TEAQL_SCALAR_EVIDENCE_DIR:-}" ]]; then
  evidence="$TEAQL_SCALAR_EVIDENCE_DIR"
  mkdir -- "$evidence"
else
  evidence="$(mktemp -d -t teaql-dotnet-scalar.XXXXXXXX)"
fi
build_root="$(mktemp -d -t teaql-dotnet-scalar-build.XXXXXXXX)"
cleanup() {
  result=$?
  trap - EXIT
  rm -rf -- "$build_root"
  exit "$result"
}
trap cleanup EXIT
database="${TEAQL_SCALAR_DB:-$evidence/scalar.sqlite}"
if [[ -e "$database" ]]; then
  echo "scalar verification requires a fresh database; will not overwrite $database" >&2
  exit 1
fi
cd "$example"
sha256sum -c generated.sha256
flags=(-p:UseSharedCompilation=false -maxcpucount:2 -nodeReuse:false
  -p:UseArtifactsOutput=true "-p:ArtifactsPath=$build_root")
DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER=1 dotnet build scalar-type-probe.csproj "${flags[@]}" \
  -p:TeaQLRuntimeSourceRoot="${TeaQLRuntimeSourceRoot:-$repo}" > "$evidence/build.log" 2>&1 || {
    tail -n 40 "$evidence/build.log"; exit 1;
  }
executable="$(dotnet msbuild scalar-type-probe.csproj -nologo -getProperty:TargetPath \
  -p:UseArtifactsOutput=true "-p:ArtifactsPath=$build_root" \
  -p:TeaQLRuntimeSourceRoot="${TeaQLRuntimeSourceRoot:-$repo}")"
if [[ "$executable" != "$build_root/"* || ! -f "$executable" ]]; then
  echo "build did not produce an executable in its owned artifact directory" >&2
  exit 1
fi
for round in 1 2; do
  TEAQL_SCALAR_DB="$database" DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER=1 \
    dotnet "$executable" | tee "$evidence/round-$round.log"
  fresh=True; version=2
  if [[ "$round" == 2 ]]; then fresh=False; version=3; fi
  rg -Fq "PASS .NET generated scalar Decimal fresh=$fresh rows=3 version=$version" "$evidence/round-$round.log"
  sha256sum -c generated.sha256
done
echo "PASS .NET scalar example twice without cleanup; evidence: $evidence"
