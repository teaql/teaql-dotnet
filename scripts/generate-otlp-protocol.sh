#!/usr/bin/env bash
set -euo pipefail
repo="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
protocol="$repo/src/TeaQL.Telemetry.OpenTelemetry/Protocol"
compiler="${TEAQL_PROTOC:-protoc}"
[[ "$($compiler --version)" == 'libprotoc 3.12.4' ]] || { echo 'Use pinned protoc 3.12.4' >&2; exit 1; }
definitions=(common/v1/common.proto resource/v1/resource.proto trace/v1/trace.proto logs/v1/logs.proto metrics/v1/metrics.proto collector/trace/v1/trace_service.proto collector/logs/v1/logs_service.proto collector/metrics/v1/metrics_service.proto)
inputs=()
for definition in "${definitions[@]}"; do inputs+=("opentelemetry/proto/$definition"); done
(cd "$protocol/Definitions" && "$compiler" --experimental_allow_proto3_optional -I . --csharp_out="$protocol/Generated" --csharp_opt=internal_access "${inputs[@]}")
# Older official protoc output predates nullable annotations. Apply this only
# to compiler output, never disable nullable checking in application code.
for binding in Common Resource Trace Logs Metrics TraceService LogsService MetricsService; do
  sed -i '1i#nullable disable\n#pragma warning disable CS8981 // Official protoc aliases' "$protocol/Generated/$binding.cs"
done
