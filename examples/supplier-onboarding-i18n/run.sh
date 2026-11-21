#!/usr/bin/env bash
set -euo pipefail

example="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo="$(cd "$example/../.." && pwd)"
runtime="${TeaQLRuntimeSourceRoot:-$repo}"
scratch="$(mktemp -d -t teaql-supplier-dotnet.XXXXXXXX)"
cleanup() {
  local status=$?
  trap - EXIT
  python3 -c 'import shutil, sys; shutil.rmtree(sys.argv[1])' "$scratch"
  exit "$status"
}
trap cleanup EXIT
export DOTNET_CLI_HOME="$scratch/cli"
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1
export NUGET_PACKAGES="$scratch/nuget"
cp -R -- "$example/dotnet-app-console" "$example/dotnet-lib-core" "$scratch/"
cd -- "$scratch"
project="$scratch/dotnet-app-console/supplier-onboarding-service-console.csproj"
properties=(-p:TeaQLRuntimeSourceRoot="$runtime" -p:ArtifactsPath="$scratch/artifacts")
dotnet restore "$project" "${properties[@]}" \
  --source "${TEAQL_I18N_NUGET_SOURCE:-https://api.nuget.org/v3/index.json}" >&2
dotnet build "$project" "${properties[@]}" --no-restore -c Release -o "$scratch/output" >&2
driver=(python3 "$example/run-example.py" --catalog "$example/dotnet-lib-core/teaql-i18n.json")
binary="$scratch/output/supplier-onboarding-service-console.dll"
if [[ "${1:-}" == --verify ]]; then
  "${driver[@]}" --all --repeat 2 -- dotnet "$binary"
  "${driver[@]}" --edge-cases -- dotnet "$binary"
else
  "${driver[@]}" "$@" -- dotnet "$binary"
fi
