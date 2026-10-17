#!/usr/bin/env bash
set -euo pipefail
repo="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
: "${TEAQL_S3_ENDPOINT:?dedicated local test endpoint required}"
: "${TEAQL_S3_BUCKET:?dedicated test bucket required}"
: "${TEAQL_S3_ACCESS_KEY:?test access key required}"
: "${TEAQL_S3_SECRET_KEY:?test secret key required}"
cd "$repo"
dotnet test src/TeaQL.Runtime.Tests/TeaQL.Runtime.Tests.csproj --filter FullyQualifiedName~ObjectStorage --nologo -v quiet -m:2
dotnet test src/TeaQL.ObjectStorage.S3.Tests/TeaQL.ObjectStorage.S3.Tests.csproj --nologo -v quiet -m:2
dotnet run --project examples/object-storage/object-storage.csproj --no-launch-profile -v quiet
