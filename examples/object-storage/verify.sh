#!/usr/bin/env bash
set -euo pipefail
example="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo="$(cd "$example/../.." && pwd)"
: "${TEAQL_S3_ENDPOINT:?dedicated local test endpoint required}"
: "${TEAQL_S3_BUCKET:?dedicated test bucket required}"
: "${TEAQL_S3_ACCESS_KEY:?test access key required}"
: "${TEAQL_S3_SECRET_KEY:?test secret key required}"
if [[ -n "${TEAQL_ORDER_ATTACHMENT_EVIDENCE_DIR:-}" ]]; then
  evidence="$TEAQL_ORDER_ATTACHMENT_EVIDENCE_DIR"
  mkdir -p "$(dirname "$evidence")"
  mkdir "$evidence"
else
  evidence="$(mktemp -d -t teaql-dotnet-order-attachment.XXXXXXXX)"
fi
evidence="$(cd "$evidence" && pwd)"
build="$(mktemp -d -t teaql-dotnet-attachment-build.XXXXXXXX)"
trap 'status=$?; trap - EXIT; rm -rf -- "$build"; exit "$status"' EXIT
trap 'exit 130' INT
trap 'exit 143' TERM
flags=(-p:UseArtifactsOutput=true "-p:ArtifactsPath=$build" "-p:TeaQLRuntimeSourceRoot=$repo" -p:UseSharedCompilation=false)
app="$example/dotnet-app-console"
project="$app/order-attachment-service-console.csproj"
calibration="$app/calibration/object-storage-calibration.csproj"
fingerprint() {
  (cd "$example" && find dotnet-lib-core dotnet-app-console -type f -print0 | sort -z | xargs -0 sha256sum)
}
fingerprint > "$evidence/source-before.sha256"
cd "$repo"
dotnet test src/TeaQL.Runtime.Tests/TeaQL.Runtime.Tests.csproj --filter FullyQualifiedName~ObjectStorage --nologo -v quiet -m:2 "${flags[@]}" 2>&1 | tee "$evidence/runtime-tests.log"
dotnet test src/TeaQL.ObjectStorage.S3.Tests/TeaQL.ObjectStorage.S3.Tests.csproj --nologo -v quiet -m:2 "${flags[@]}" 2>&1 | tee "$evidence/provider-tests.log"
for item in "$calibration" "$project"; do
  dotnet build --disable-build-servers -m:2 "${flags[@]}" "$item"
done
legacy="$(dotnet msbuild "$calibration" -nologo -getProperty:TargetPath "${flags[@]}")"
binary="$(dotnet msbuild "$project" -nologo -getProperty:TargetPath "${flags[@]}")"
[[ "$legacy" == "$build"/* && "$binary" == "$build"/* ]]
cp "${binary%.dll}.deps.json" "$evidence/origins.log"
dotnet "$legacy" | tee "$evidence/s3-calibration.log"
for round in 1 2; do
  dotnet "$binary" "$evidence/orders.sqlite" "$evidence/receipt-$round.txt" "$round" | tee "$evidence/run-$round.log"
done
fingerprint > "$evidence/source-after.sha256"
cmp "$evidence/source-before.sha256" "$evidence/source-after.sha256"
printf 'Order attachment evidence retained at %s\n' "$evidence"
