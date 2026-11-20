#!/usr/bin/env bash
set -euo pipefail

repo="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
expected=(business-id conformance context-derived expression-profiles facet-trace fix-e-alignment internal-id-customization mutation-policy natural-messages object-storage order-management runtime-logging scalar-type-probe schema-customization school-management security-foundations task_board trace-chain)
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
node "$repo/scripts/verify-test-inventory.cjs"
node "$repo/scripts/verify-generated-fixtures.cjs"
inventory_evidence="${TEAQL_EXAMPLES_EVIDENCE_DIR:-$(mktemp -d -t teaql-dotnet-examples-evidence.XXXXXXXX)}"
mkdir -p "$inventory_evidence"
export TEAQL_CONFORMANCE_DB="$inventory_evidence/conformance.sqlite"
export DOTNET_CLI_USE_MSBUILD_SERVER=0
export MSBUILDDISABLENODEREUSE=1
export UseSharedCompilation=false
node "$repo/examples/fix-e-alignment/verify-local.cjs"
TEAQL_CONTEXT_FIX_EVIDENCE_DIR="$inventory_evidence/context-derived" node "$repo/examples/context-derived/verify-local.cjs"
node --test "$repo/scripts/verify-natural-messages.test.cjs"
node "$repo/scripts/verify-natural-messages.cjs" --output "$inventory_evidence/natural-messages"
TEAQL_GENERATED_E_PROFILE_EVIDENCE_DIR="$inventory_evidence/expression-profiles" node "$repo/examples/expression-profiles/verify-local.cjs"
TeaQLRuntimeSourceRoot="$runtime_source_root" bash "$repo/examples/scalar-type-probe/verify.sh"
TEAQL_EMPTY_EVIDENCE_DIR="$inventory_evidence/empty-aggregates" TeaQLRuntimeSourceRoot="$runtime_source_root" bash "$repo/examples/scalar-type-probe/verify-empty.sh"
bash "$repo/examples/object-storage/verify.sh"
TEAQL_CONFORMANCE_EVIDENCE_DIR="$inventory_evidence/conformance" bash "$repo/examples/conformance/verify.sh"
dotnet run --project "$repo/examples/business-id/dotnet-app-console/calibration/business-id.csproj"
TEAQL_BUSINESS_ID_KEY_V1_HEX=000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f \
TEAQL_PURCHASE_NUMBERING_ROUND=inventory TEAQL_PURCHASE_NUMBERING_DATABASE="$inventory_evidence/business-id.sqlite" \
dotnet run --property:TeaQLRuntimeSourceRoot="$runtime_source_root" --project "$repo/examples/business-id/dotnet-app-console/purchase-order-numbering-console.csproj"
dotnet run --property:TeaQLRuntimeSourceRoot="$runtime_source_root" --project "$repo/examples/runtime-logging/runtime-logging.csproj"
dotnet run --project "$repo/examples/schema-customization/dotnet-app-console/calibration/schema-customization.csproj"
TEAQL_SHOWCASE_ROUND=inventory TEAQL_SHOWCASE_DATABASE="$inventory_evidence/school-schema.sqlite" \
dotnet run --property:TeaQLRuntimeSourceRoot="$runtime_source_root" \
  --project "$repo/examples/schema-customization/dotnet-app-console/school-schema-service-console.csproj"
dotnet run --property:TeaQLRuntimeSourceRoot="$runtime_source_root" --project "$repo/examples/internal-id-customization/internal-id-customization.csproj"
dotnet run --project "$repo/examples/mutation-policy/dotnet-app-console/calibration/mutation-policy.csproj"
TEAQL_SHOWCASE_ROUND=inventory TEAQL_SHOWCASE_DATABASE="$inventory_evidence/customer-approval.sqlite" \
dotnet run --property:TeaQLRuntimeSourceRoot="$runtime_source_root" \
  --project "$repo/examples/mutation-policy/dotnet-app-console/customer-onboarding-service-console.csproj"
dotnet run --property:TeaQLRuntimeSourceRoot="$runtime_source_root" --project "$repo/examples/school-management/dotnet-app-console/school-management-service-console.csproj"
TeaQLRuntimeSourceRoot="$runtime_source_root" bash "$repo/scripts/verify-school-bootstrap-example.sh"
dotnet run --property:TeaQLRuntimeSourceRoot="$runtime_source_root" --project "$repo/examples/security-foundations/security-foundations.csproj"
TEAQL_ORDER_EVIDENCE_DIR="$inventory_evidence/order-management" bash "$repo/examples/order-management/verify.sh"
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
