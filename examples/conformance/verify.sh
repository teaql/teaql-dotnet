#!/usr/bin/env bash
set -euo pipefail
example="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo="$(cd "$example/../.." && pwd)"
if [[ -n "${TEAQL_CONFORMANCE_EVIDENCE_DIR:-}" ]]; then
 evidence="$TEAQL_CONFORMANCE_EVIDENCE_DIR"
 mkdir "$evidence"
else
 evidence="$(mktemp -d -t teaql-work-item-evidence.XXXXXXXX)"
fi
unset TEAQL_ALLOW_SENSITIVE_PLAINTEXT_LOGS
scratch="$(mktemp -d -t teaql-dotnet-work-item-build.XXXXXXXX)"
trap 'status=$?; trap - EXIT; rm -rf -- "$scratch"; exit "$status"' EXIT
project="$example/dotnet-app-console/runtime-example-conformance-service-console.csproj"
artifacts="$scratch/artifacts"
fingerprint() {
 (cd "$example" && find dotnet-lib-core dotnet-app-console -type f -print0 | sort -z | xargs -0 sha256sum)
}
fingerprint > "$evidence/source-before.sha256"
node "$repo/scripts/verify-generated-fixtures.cjs" > "$evidence/fixture-origins.log"
dotnet build "$project" --property:TeaQLRuntimeSourceRoot="$repo" --property:UseArtifactsOutput=true \
 --property:ArtifactsPath="$artifacts" --property:UseSharedCompilation=false > "$evidence/build.log" 2>&1
assembly="$artifacts/bin/runtime-example-conformance-service-console/debug/runtime-example-conformance-service-console.dll"
[[ -f "$assembly" && -f "${assembly%.dll}.runtimeconfig.json" ]]
for round in 1 2; do
 TEAQL_CONFORMANCE_DB="$evidence/smoke-$round.sqlite" dotnet "$assembly" > "$evidence/run-$round.log" 2>&1
 rg -Fq 'PASS .NET minimum runtime conformance: 8/8' "$evidence/run-$round.log"
done
sha256sum "$evidence/smoke-1.sqlite" > "$evidence/before-rejection.sha256"
set +e
TEAQL_CONFORMANCE_DB="$evidence/smoke-1.sqlite" dotnet "$assembly" > "$evidence/existing-database-rejected.log" 2>&1
rejected=$?
set -e
[[ "$rejected" != 0 ]]
rg -Fq 'existing data is never deleted' "$evidence/existing-database-rejected.log"
sha256sum "$evidence/smoke-1.sqlite" > "$evidence/after-rejection.sha256"
cmp "$evidence/before-rejection.sha256" "$evidence/after-rejection.sha256"
fingerprint > "$evidence/source-after.sha256"
cmp "$evidence/source-before.sha256" "$evidence/source-after.sha256"
python3 -B - "$evidence" "$artifacts" "$project" "$example" "$repo" "$assembly" <<'CHECK'
import hashlib,json,pathlib,sqlite3,sys
root,artifacts,project,example,runtime,assembly=map(pathlib.Path,sys.argv[1:]);assets={}
for file in artifacts.rglob('project.assets.json'):
 data=json.loads(file.read_text());assets[str(pathlib.Path(data['project']['restore']['projectPath']).resolve())]=data
app=assets[str(project.resolve())];library=(example/'dotnet-lib-core/runtime-example-conformance-service-lib.csproj').resolve()
assert str(library) in app['project']['restore']['frameworks']['net8.0']['projectReferences']
domain=assets[str(library)];expected={str((runtime/'src'/name/(name+'.csproj')).resolve()) for name in ['TeaQL.Core','TeaQL.DataService','TeaQL.Runtime','TeaQL.Sql','TeaQL.Provider.Sqlite']}
assert set(domain['project']['restore']['frameworks']['net8.0']['projectReferences'])==expected
for name in ['TeaQL.Core','TeaQL.DataService','TeaQL.Runtime','TeaQL.Sql','TeaQL.Provider.Sqlite']:
 selected=[row for key,row in app['libraries'].items() if key.split('/')[0]==name];assert len(selected)==1 and selected[0]['type']=='project',name
rows=[]
for round in [1,2]:
 database=root/f'smoke-{round}.sqlite'
 with sqlite3.connect(database.resolve().as_uri()+'?mode=ro&immutable=1',uri=True) as connection:
  result=connection.execute('SELECT title,version FROM work_item_data').fetchall();assert len(result)==1 and result[0][1]==-3,result
  assert connection.execute('PRAGMA quick_check').fetchone()==('ok',)
 rows.append(dict(round=round,workItems=result,sha256=hashlib.sha256(database.read_bytes()).hexdigest()))
receipt=dict(schema='teaql.dotnet-work-item-business/v1',passed=True,smokeRuns=2,smokeDatabasesFreshAndRetained=True,
 originalBusinessProgramAndManagedConstructionChecksByteExact=True,generatedScopesUnchanged=True,
 preexistingSmokeDatabaseRejectedWithoutModification=True,nativeWorkspaceEntryUsed=True,
 runtimeSourceProjects=sorted(expected),smokeResults=rows,executedAssemblySHA256=hashlib.sha256(assembly.read_bytes()).hexdigest())
(root/'business-receipt.json').write_text(json.dumps(receipt,indent=2,sort_keys=True)+'\n')
print(json.dumps(dict(passed=True,smokeRuns=2,existingDatabasePreserved=True)))
CHECK
