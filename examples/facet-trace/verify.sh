#!/usr/bin/env bash
set -euo pipefail
example="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo="$(cd "$example/../.." && pwd)"
if [[ -n "${TEAQL_FACET_EVIDENCE_DIR:-}" ]]; then
  evidence="$TEAQL_FACET_EVIDENCE_DIR"
  mkdir "$evidence"
else
  evidence="$(mktemp -d -t teaql-facet-evidence.XXXXXXXX)"
fi
export TEAQL_FACET_TRACE_DB="$evidence/school.sqlite"
unset TEAQL_ALLOW_SENSITIVE_PLAINTEXT_LOGS
scratch="$(mktemp -d -t teaql-dotnet-facet-build.XXXXXXXX)"
trap 'status=$?; trap - EXIT; rm -rf -- "$scratch"; exit "$status"' EXIT
project="$example/dotnet-app-console/school-management-service-console.csproj"
artifacts="$scratch/artifacts"
fingerprint() {
 (cd "$example" && find dotnet-lib-core dotnet-app-console -type f -print0 | sort -z | xargs -0 sha256sum)
}
fingerprint > "$evidence/source-before.sha256"
node "$repo/scripts/verify-generated-fixtures.cjs" > "$evidence/fixture-origins.log"
dotnet build "$project" --property:TeaQLRuntimeSourceRoot="$repo" --property:UseArtifactsOutput=true \
 --property:ArtifactsPath="$artifacts" --property:UseSharedCompilation=false > "$evidence/build.log" 2>&1
assembly="$artifacts/bin/school-management-service-console/debug/school-management-service-console.dll"
[[ -f "$assembly" && -f "${assembly%.dll}.runtimeconfig.json" ]]
for round in 1 2; do
 timeout --kill-after=5s 120s dotnet "$assembly" > "$evidence/run-$round.log" 2>&1
 [[ "$(rg -c '^FACET_OBSERVED ' "$evidence/run-$round.log")" == 24 ]]
 rg -Fq 'DOTNET_FACET_PASS scenarios=24' "$evidence/run-$round.log"
 rg -Fxq 'REVERSE_CHILD_UPDATE_PASS count=1' "$evidence/run-$round.log"
done
fingerprint > "$evidence/source-after.sha256"
cmp "$evidence/source-before.sha256" "$evidence/source-after.sha256"
python3 -B - "$evidence" "$artifacts" "$project" "$example" "$repo" "$assembly" <<'CHECK'
import hashlib,json,pathlib,sqlite3,sys
root,artifacts,project,example,runtime,assembly=map(pathlib.Path,sys.argv[1:]);assets={}
for file in artifacts.rglob('project.assets.json'):
 data=json.loads(file.read_text());assets[str(pathlib.Path(data['project']['restore']['projectPath']).resolve())]=data
app=assets[str(project.resolve())];library=(example/'dotnet-lib-core/school-management-service-lib.csproj').resolve()
assert set(app['project']['restore']['frameworks']['net8.0']['projectReferences'])=={str(library)}
domain=assets[str(library)];expected={str((runtime/'src'/name/(name+'.csproj')).resolve()) for name in ['TeaQL.Core','TeaQL.DataService','TeaQL.Runtime','TeaQL.Sql','TeaQL.Provider.Sqlite']}
assert set(domain['project']['restore']['frameworks']['net8.0']['projectReferences'])==expected
for name in ['TeaQL.Core','TeaQL.DataService','TeaQL.Runtime','TeaQL.Sql','TeaQL.Provider.Sqlite']:
 selected=[row for key,row in app['libraries'].items() if key.split('/')[0]==name];assert len(selected)==1 and selected[0]['type']=='project',name
database=root/'school.sqlite'
with sqlite3.connect(database.resolve().as_uri()+'?mode=ro&immutable=1',uri=True) as connection:
 rows=connection.execute('SELECT name,address,version FROM school_data ORDER BY id').fetchall()
 assert len(rows)==2 and all(row[0]=='DOTNET-FACET-PRIVATE-SCHOOL' for row in rows),rows
 assert [row[2] for row in rows]==[3,1] and rows[0][1]=='Facet Road B',rows
 assert connection.execute('PRAGMA quick_check').fetchone()==('ok',)
receipt=dict(schema='teaql.dotnet-facet-business/v1',passed=True,scenariosPerRun=24,runs=2,sameDatabase=True,
 originalBusinessProgramByteExact=True,generatedScopesUnchanged=True,reverseChildWritebackVerified=True,
 nativeWorkspaceEntryUsed=True,runtimeSourceProjects=sorted(expected),schools=rows,
 executedAssemblySHA256=hashlib.sha256(assembly.read_bytes()).hexdigest(),databaseSHA256=hashlib.sha256(database.read_bytes()).hexdigest())
(root/'business-receipt.json').write_text(json.dumps(receipt,indent=2,sort_keys=True)+'\n')
print(json.dumps(dict(passed=True,scenariosPerRun=24,runs=2)))
CHECK
