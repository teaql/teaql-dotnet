#!/usr/bin/env bash
set -euo pipefail
example="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo="$(cd "$example/../.." && pwd)"
evidence="$(mktemp -d -t teaql-dotnet-scalar.XXXXXXXX)"
cd "$example"
sha256sum -c generated.sha256
flags=(-p:UseSharedCompilation=false -maxcpucount:2 -nodeReuse:false)
DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER=1 dotnet build scalar-type-probe.csproj "${flags[@]}" \
  -p:TeaQLRuntimeSourceRoot="${TeaQLRuntimeSourceRoot:-$repo}" > "$evidence/build.log" 2>&1 || {
    tail -n 40 "$evidence/build.log"; exit 1;
  }
for round in 1 2; do
  TEAQL_SCALAR_DB="$evidence/scalar.sqlite" DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER=1 \
    dotnet run --no-build --no-restore --project scalar-type-probe.csproj \
    -p:TeaQLRuntimeSourceRoot="${TeaQLRuntimeSourceRoot:-$repo}" | tee "$evidence/round-$round.log"
  fresh=True; version=2
  if [[ "$round" == 2 ]]; then fresh=False; version=3; fi
  rg -Fq "PASS .NET generated scalar Decimal fresh=$fresh rows=3 version=$version" "$evidence/round-$round.log"
  sha256sum -c generated.sha256
done
echo "PASS .NET scalar example twice without cleanup; evidence: $evidence"
