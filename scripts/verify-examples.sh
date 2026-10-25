#!/usr/bin/env bash
set -euo pipefail

repo="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
expected=(business-id conformance facet-trace fix-e-alignment internal-id-customization mutation-policy object-storage order-management runtime-logging scalar-type-probe schema-customization school-management security-foundations task_board trace-chain)
mapfile -t actual < <(find "$repo/examples" -mindepth 1 -maxdepth 1 -type d ! -name '.*' -printf '%f\n' | sort)
if [[ "${actual[*]}" != "${expected[*]}" ]]; then
  echo "example inventory changed; update scripts/verify-examples.sh: ${actual[*]}" >&2
  exit 1
fi

mapfile -t embedded_runtime_files < <(find "$repo/examples" -type f -name TeaQLCore.cs -print)
if ((${#embedded_runtime_files[@]})); then
  printf 'generated examples must depend on the packaged runtime; embedded TeaQLCore.cs: %s\n' \
    "${embedded_runtime_files[@]}" >&2
  exit 1
fi

runtime_source_root="${TeaQLRuntimeSourceRoot:-$repo}"
node "$repo/scripts/verify-generated-fixtures.cjs"
inventory_evidence="${TEAQL_EXAMPLES_EVIDENCE_DIR:-$(mktemp -d -t teaql-dotnet-examples-evidence.XXXXXXXX)}"
mkdir -p "$inventory_evidence"
export TEAQL_CONFORMANCE_DB="$inventory_evidence/conformance.sqlite"
export DOTNET_CLI_USE_MSBUILD_SERVER=0
export MSBUILDDISABLENODEREUSE=1
export UseSharedCompilation=false
node "$repo/examples/fix-e-alignment/verify-local.cjs"
TeaQLRuntimeSourceRoot="$runtime_source_root" bash "$repo/examples/scalar-type-probe/verify.sh"
bash "$repo/examples/object-storage/verify.sh"
dotnet run --property:TeaQLRuntimeSourceRoot="$runtime_source_root" --project "$repo/examples/conformance/runtime-example-conformance-service-console.csproj"
dotnet run --property:TeaQLRuntimeSourceRoot="$runtime_source_root" --project "$repo/examples/business-id/business-id.csproj"
dotnet run --property:TeaQLRuntimeSourceRoot="$runtime_source_root" --project "$repo/examples/runtime-logging/runtime-logging.csproj"
dotnet run --property:TeaQLRuntimeSourceRoot="$runtime_source_root" --project "$repo/examples/schema-customization/schema-customization.csproj"
dotnet run --property:TeaQLRuntimeSourceRoot="$runtime_source_root" --project "$repo/examples/internal-id-customization/internal-id-customization.csproj"
dotnet run --property:TeaQLRuntimeSourceRoot="$runtime_source_root" --project "$repo/examples/mutation-policy/mutation-policy.csproj"
dotnet run --property:TeaQLRuntimeSourceRoot="$runtime_source_root" --project "$repo/examples/school-management/school-management-service-lib.csproj"
TeaQLRuntimeSourceRoot="$runtime_source_root" bash "$repo/scripts/verify-school-bootstrap-example.sh"
dotnet run --property:TeaQLRuntimeSourceRoot="$runtime_source_root" --project "$repo/examples/security-foundations/security-foundations.csproj"
TEAQL_ORDER_MANAGEMENT_DB="$inventory_evidence/order.db" \
  dotnet run --property:TeaQLRuntimeSourceRoot="$runtime_source_root" --project "$repo/examples/order-management/dotnet-app-console/dotnet-app-console.csproj"
dotnet run --property:TeaQLRuntimeSourceRoot="$runtime_source_root" --project "$repo/examples/task_board/TaskBoardExample/TaskBoardExample.csproj"
TeaQLRuntimeSourceRoot="$runtime_source_root" bash "$repo/scripts/verify-trace-chain-example.sh"
bash "$repo/examples/facet-trace/verify.sh"
bash "$repo/examples/trace-chain/verify-standard-telemetry.sh"
bash "$repo/examples/trace-chain/verify-standard-order.sh"
node "$repo/scripts/verify-generated-fixtures.cjs"
bash "$repo/examples/trace-chain/verify-standard-graph.sh"
bash "$repo/examples/trace-chain/verify-standard-stream.sh"
echo "PASS: all .NET examples"
echo "Retained compact example evidence: $inventory_evidence"
