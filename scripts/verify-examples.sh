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
TEAQL_DISPATCH_PROFILE_EVIDENCE_DIR="$inventory_evidence/expression-profiles" bash "$repo/examples/expression-profiles/verify-dispatch.sh"
TEAQL_FREIGHT_QUOTE_EVIDENCE_DIR="$inventory_evidence/freight-quotes" \
  bash "$repo/examples/scalar-type-probe/verify-freight-quotes.sh"
bash "$repo/examples/object-storage/verify.sh"
TEAQL_CONFORMANCE_EVIDENCE_DIR="$inventory_evidence/conformance" bash "$repo/examples/conformance/verify.sh"
dotnet run --project "$repo/examples/business-id/dotnet-app-console/calibration/business-id.csproj"
TEAQL_BUSINESS_ID_KEY_V1_HEX=000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f \
TEAQL_PURCHASE_NUMBERING_ROUND=inventory TEAQL_PURCHASE_NUMBERING_DATABASE="$inventory_evidence/business-id.sqlite" \
dotnet run --property:TeaQLRuntimeSourceRoot="$runtime_source_root" --project "$repo/examples/business-id/dotnet-app-console/purchase-order-numbering-console.csproj"
TEAQL_SHIPPING_LOG_EVIDENCE_DIR="$inventory_evidence/runtime-logging" bash "$repo/examples/runtime-logging/verify.sh"
dotnet run --project "$repo/examples/schema-customization/dotnet-app-console/calibration/schema-customization.csproj"
TEAQL_SHOWCASE_ROUND=inventory TEAQL_SHOWCASE_DATABASE="$inventory_evidence/school-schema.sqlite" \
dotnet run --property:TeaQLRuntimeSourceRoot="$runtime_source_root" \
  --project "$repo/examples/schema-customization/dotnet-app-console/school-schema-service-console.csproj"
TEAQL_INTERNAL_ID_EVIDENCE_DIR="$inventory_evidence/internal-id" bash "$repo/examples/internal-id-customization/verify.sh"
dotnet run --project "$repo/examples/mutation-policy/dotnet-app-console/calibration/mutation-policy.csproj"
TEAQL_SHOWCASE_ROUND=inventory TEAQL_SHOWCASE_DATABASE="$inventory_evidence/customer-approval.sqlite" \
dotnet run --property:TeaQLRuntimeSourceRoot="$runtime_source_root" \
  --project "$repo/examples/mutation-policy/dotnet-app-console/customer-onboarding-service-console.csproj"
TEAQL_SCHOOL_EVIDENCE_DIR="$inventory_evidence/school-management" bash "$repo/examples/school-management/verify.sh"
dotnet run --property:TeaQLRuntimeSourceRoot="$runtime_source_root" --project "$repo/examples/security-foundations/security-foundations.csproj"
TEAQL_ORDER_PRIVACY_EVIDENCE_DIR="$inventory_evidence/order-privacy" bash "$repo/examples/security-foundations/verify-privacy.sh"
TEAQL_ORDER_EVIDENCE_DIR="$inventory_evidence/order-management" bash "$repo/examples/order-management/verify.sh"
TEAQL_TASK_BOARD_DB="$inventory_evidence/task-board.sqlite" dotnet run --property:TeaQLRuntimeSourceRoot="$runtime_source_root" --project "$repo/examples/task_board/dotnet-app-console/robot-task-board-service-console.csproj"
TeaQLRuntimeSourceRoot="$runtime_source_root" bash "$repo/scripts/verify-trace-chain-example.sh"
bash "$repo/examples/facet-trace/verify.sh"
bash "$repo/examples/trace-chain/verify-standard-telemetry.sh"
bash "$repo/examples/trace-chain/verify-standard-order.sh"
node "$repo/scripts/verify-generated-fixtures.cjs"
bash "$repo/examples/trace-chain/verify-standard-graph.sh"
bash "$repo/examples/trace-chain/verify-standard-stream.sh"
echo "PASS: all .NET examples"
python3 -B "example/TeaQL.ExampleApi/verify-federation.py" --runtime dotnet --output "$inventory_evidence/order-federation"
echo "Retained compact example evidence: $inventory_evidence"
