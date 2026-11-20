#!/usr/bin/env bash
set -euo pipefail
example="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo="$(cd "$example/../.." && pwd)"
project="$example/dotnet-app-console/school-management-service-console.csproj"
if [[ -n "${TEAQL_SCHOOL_EVIDENCE_DIR:-}" ]]; then
  evidence="$TEAQL_SCHOOL_EVIDENCE_DIR"
  mkdir "$evidence"
else
  evidence="$(mktemp -d -t teaql-dotnet-school-evidence.XXXXXXXX)"
fi
scratch="$(mktemp -d -t teaql-dotnet-school-build.XXXXXXXX)"
trap 'status=$?; trap - EXIT; rm -rf -- "$scratch"; exit "$status"' EXIT
export TEAQL_SCHOOL_ARTIFACTS_DIR="$scratch/artifacts"
properties=(--property:TeaQLRuntimeSourceRoot="$repo" --property:UseArtifactsOutput=true
  --property:ArtifactsPath="$TEAQL_SCHOOL_ARTIFACTS_DIR")
fingerprint() {
  (cd "$example" && find dotnet-lib-core dotnet-app-console -type f -print0 | sort -z | xargs -0 sha256sum)
}
fingerprint > "$evidence/source-before.sha256"
dotnet build "${properties[@]}" "$project" > "$evidence/build.log" 2>&1
assembly="$TEAQL_SCHOOL_ARTIFACTS_DIR/bin/school-management-service-console/debug/school-management-service-console.dll"
[[ -f "$assembly" && -f "${assembly%.dll}.runtimeconfig.json" ]]
TEAQL_SCHOOL_MANAGEMENT_DB="$evidence/showcase.sqlite" \
  dotnet "$assembly" > "$evidence/application.log" 2>&1
sha256sum "$evidence/showcase.sqlite" > "$evidence/database-before-rejection.sha256"
set +e
TEAQL_SCHOOL_MANAGEMENT_DB="$evidence/showcase.sqlite" \
  dotnet "$assembly" > "$evidence/existing-database-rejected.log" 2>&1
rejected=$?
set -e
[[ "$rejected" != 0 ]]
rg -Fq 'existing data is preserved' "$evidence/existing-database-rejected.log"
sha256sum "$evidence/showcase.sqlite" > "$evidence/database-after-rejection.sha256"
cmp "$evidence/database-before-rejection.sha256" "$evidence/database-after-rejection.sha256"
TEAQL_SCHOOL_BOOTSTRAP_EVIDENCE_DIR="$evidence/bootstrap" \
  bash "$repo/scripts/verify-school-bootstrap-example.sh" > "$evidence/bootstrap.log" 2>&1
fingerprint > "$evidence/source-after.sha256"
cmp "$evidence/source-before.sha256" "$evidence/source-after.sha256"
python3 -B - "$evidence" "$TEAQL_SCHOOL_ARTIFACTS_DIR" "$project" "$repo" "$assembly" <<'PY'
import hashlib, json, pathlib, sqlite3, sys
root, artifacts, project, runtime, assembly = map(pathlib.Path, sys.argv[1:])
matches = []
for path in artifacts.rglob('project.assets.json'):
    data = json.loads(path.read_text())
    if pathlib.Path(data['project']['restore']['projectPath']).resolve() == project.resolve():
        matches.append((path, data))
assert len(matches) == 1, [str(row[0]) for row in matches]
assets, data = matches[0]
expected = {str((runtime / 'src' / name / (name + '.csproj')).resolve()) for name in
    ['TeaQL.Core', 'TeaQL.DataService', 'TeaQL.Runtime', 'TeaQL.Sql', 'TeaQL.Provider.Sqlite']}
actual = set(data['project']['restore']['frameworks']['net8.0']['projectReferences'])
assert actual == expected, actual
for name in ['TeaQL.Core', 'TeaQL.DataService', 'TeaQL.Runtime', 'TeaQL.Sql', 'TeaQL.Provider.Sqlite']:
    dependencies = [row for key, row in data['libraries'].items() if key.split('/')[0] == name]
    assert len(dependencies) == 1 and dependencies[0]['type'] == 'project', name
assert 'PASS .NET School bootstrap and portable Query parity with logged intent' in (root / 'application.log').read_text()
for logging in ['on', 'off']:
    for round in [1, 2]:
        expected_line = f'PASS .NET generated bootstrap trace logging={logging == "on"} fresh={round == 1} originalVersion={1 if round == 1 else 3}'
        assert expected_line in (root / 'bootstrap' / f'{logging}-{round}.log').read_text()
database = root / 'showcase.sqlite'
with sqlite3.connect(database.resolve().as_uri() + '?mode=ro&immutable=1', uri=True) as connection:
    rows = connection.execute('SELECT name,version,student_capacity FROM school_data').fetchall()
    assert rows == [('Riverside Primary School', 1, 800)], rows
    assert connection.execute('PRAGMA quick_check').fetchone() == ('ok',)
receipt = dict(schema='teaql.dotnet-school-business/v1', passed=True,
    originalApplicationAssertionsPassed=True, runtimeProjectReferences=sorted(actual),
    assetsSHA256=hashlib.sha256(assets.read_bytes()).hexdigest(),
    executedAssemblySHA256=hashlib.sha256(assembly.read_bytes()).hexdigest(),
    bootstrapRoundsPerMode=2, loggingModes=['on', 'off'], sameBootstrapDatabases=True,
    preexistingDatabaseRejectedWithoutModification=True, generatedScopesUnchanged=True,
    sqliteRows=rows, databaseSHA256=hashlib.sha256(database.read_bytes()).hexdigest())
(root / 'business-receipt.json').write_text(json.dumps(receipt, indent=2, sort_keys=True) + '\n')
print(json.dumps(dict(passed=True, sourceProjects=len(actual), sameBootstrapDatabases=True)))
PY
