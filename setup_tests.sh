#!/bin/bash
cd /home/ubuntu/githome/teaql-dotnet/src

PROJECTS=(
  "TeaQL.Sql"
  "TeaQL.Runtime"
  "TeaQL.DataService"
  "TeaQL.Provider.Sqlite"
  "TeaQL.Provider.PostgreSql"
  "TeaQL.Provider.MySql"
  "TeaQL.CacheIntegration.Redis"
  "TeaQL.WebIntegration.AspNetCore"
)

for PROJ in "${PROJECTS[@]}"; do
  TEST_PROJ="${PROJ}.Tests"
  echo "Setting up ${TEST_PROJ}..."
  dotnet new xunit -n "${TEST_PROJ}"
  dotnet sln ../TeaQL.sln add "${TEST_PROJ}/${TEST_PROJ}.csproj"
  dotnet add "${TEST_PROJ}/${TEST_PROJ}.csproj" reference "${PROJ}/${PROJ}.csproj"
  dotnet add "${TEST_PROJ}/${TEST_PROJ}.csproj" package Moq
  dotnet add "${TEST_PROJ}/${TEST_PROJ}.csproj" package coverlet.collector
  dotnet add "${TEST_PROJ}/${TEST_PROJ}.csproj" package coverlet.msbuild
done
