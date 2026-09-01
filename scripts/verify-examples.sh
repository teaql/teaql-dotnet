#!/usr/bin/env bash
set -euo pipefail

repo="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
expected=(conformance order-management runtime-logging school-management task_board)
mapfile -t actual < <(find "$repo/examples" -mindepth 1 -maxdepth 1 -type d -printf '%f\n' | sort)
if [[ "${actual[*]}" != "${expected[*]}" ]]; then
  echo "example inventory changed; update scripts/verify-examples.sh: ${actual[*]}" >&2
  exit 1
fi

dotnet run --project "$repo/examples/conformance/runtime-example-conformance-service-console.csproj"
dotnet run --project "$repo/examples/runtime-logging/runtime-logging.csproj"
dotnet run --project "$repo/examples/school-management/school-management-service-lib.csproj"
dotnet run --project "$repo/examples/order-management/dotnet-app-console/dotnet-app-console.csproj"
dotnet run --project "$repo/examples/task_board/TaskBoardExample/TaskBoardExample.csproj"
echo "PASS: all .NET examples"
