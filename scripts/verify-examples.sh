#!/usr/bin/env bash
set -euo pipefail

repo="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
expected=(conformance order-management runtime-logging school-management task_board)
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
order_management_tmp="$(mktemp -d)"
trap 'rm -rf "$order_management_tmp"' EXIT
dotnet run --property:TeaQLRuntimeSourceRoot="$runtime_source_root" --project "$repo/examples/conformance/runtime-example-conformance-service-console.csproj"
dotnet run --property:TeaQLRuntimeSourceRoot="$runtime_source_root" --project "$repo/examples/runtime-logging/runtime-logging.csproj"
dotnet run --property:TeaQLRuntimeSourceRoot="$runtime_source_root" --project "$repo/examples/school-management/school-management-service-lib.csproj"
TEAQL_ORDER_MANAGEMENT_DB="$order_management_tmp/order.db" \
  dotnet run --property:TeaQLRuntimeSourceRoot="$runtime_source_root" --project "$repo/examples/order-management/dotnet-app-console/dotnet-app-console.csproj"
dotnet run --property:TeaQLRuntimeSourceRoot="$runtime_source_root" --project "$repo/examples/task_board/TaskBoardExample/TaskBoardExample.csproj"
echo "PASS: all .NET examples"
