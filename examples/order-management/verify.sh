#!/usr/bin/env bash
set -euo pipefail
example="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo="$(cd "$example/../.." && pwd)"
project="$example/dotnet-app-console/order-management-service-console.csproj"
if [[ -n "${TEAQL_ORDER_EVIDENCE_DIR:-}" ]]; then
  evidence="$TEAQL_ORDER_EVIDENCE_DIR"
  mkdir "$evidence"
else
  evidence="$(mktemp -d -t teaql-dotnet-order-evidence.XXXXXXXX)"
fi
scratch="$(mktemp -d -t teaql-dotnet-order-build.XXXXXXXX)"
trap 'status=$?; trap - EXIT; rm -rf -- "$scratch"; exit "$status"' EXIT
artifacts="$scratch/artifacts"
properties=(--property:TeaQLRuntimeSourceRoot="$repo" --property:UseArtifactsOutput=true
  --property:ArtifactsPath="$artifacts" --property:UseSharedCompilation=false)
fingerprint() {
  (cd "$example" && find dotnet-lib-core dotnet-app-console -type f -print0 | sort -z | xargs -0 sha256sum)
}
fingerprint > "$evidence/source-before.sha256"
dotnet build "${properties[@]}" "$project" > "$evidence/build.log" 2>&1
assembly="$artifacts/bin/order-management-service-console/debug/order-management-service-console.dll"
[[ -f "$assembly" && -f "${assembly%.dll}.runtimeconfig.json" ]]
for round in 1 2; do
  TEAQL_ORDER_MANAGEMENT_DB="$evidence/order.sqlite" \
    dotnet "$assembly" > "$evidence/run-$round.log" 2>&1
done
fingerprint > "$evidence/source-after.sha256"
cmp "$evidence/source-before.sha256" "$evidence/source-after.sha256"
python3 -B - "$evidence" "$artifacts" "$project" "$example" "$repo" "$assembly" <<'PY'
import datetime, decimal, hashlib, json, pathlib, sqlite3, sys
root, artifacts, project, example, runtime, assembly = map(pathlib.Path, sys.argv[1:])
assets = {}
for file in artifacts.rglob('project.assets.json'):
    data = json.loads(file.read_text())
    assets[str(pathlib.Path(data['project']['restore']['projectPath']).resolve())] = data
app = assets[str(project.resolve())]
library = (example / 'dotnet-lib-core/order-management-service-lib.csproj').resolve()
assert set(app['project']['restore']['frameworks']['net8.0']['projectReferences']) == {str(library)}
domain = assets[str(library)]
expected = {str((runtime / 'src' / name / (name + '.csproj')).resolve()) for name in
    ['TeaQL.Core', 'TeaQL.DataService', 'TeaQL.Runtime', 'TeaQL.Sql', 'TeaQL.Provider.Sqlite']}
actual = set(domain['project']['restore']['frameworks']['net8.0']['projectReferences'])
assert actual == expected, actual
for name in ['TeaQL.Core', 'TeaQL.DataService', 'TeaQL.Runtime', 'TeaQL.Sql', 'TeaQL.Provider.Sqlite']:
    selected = [row for key, row in app['libraries'].items() if key.split('/')[0] == name]
    assert len(selected) == 1 and selected[0]['type'] == 'project', name
for round in [1, 2]:
    text = (root / f'run-{round}.log').read_text()
    assert '[query] matched 1 order(s)' in text and 'WEB-2026-001  2026-08-12  129.95' in text
    assert '[audit] governed mutation path verified' in text
    expected_seed = '[seed] inserted deterministic platform, customer, and order' if round == 1 else 'no duplicate rows added'
    assert expected_seed in text
database = root / 'order.sqlite'
with sqlite3.connect(database.resolve().as_uri() + '?mode=ro&immutable=1', uri=True) as connection:
    rows = connection.execute('SELECT order_number,order_date,total_amount,version FROM customer_order_data').fetchall()
    assert len(rows) == 1 and rows[0][0] == 'WEB-2026-001', rows
    assert datetime.date.fromisoformat(str(rows[0][1])[:10]) == datetime.date(2026, 8, 12)
    assert decimal.Decimal(str(rows[0][2])) == decimal.Decimal('129.95') and rows[0][3] == 1
    presets = connection.execute('SELECT request_id,version FROM order_search_preset_data').fetchall()
    assert presets == [('quick-start-pending-orders', 1)], presets
    assert connection.execute('SELECT count(*) FROM customer_data').fetchone() == (1,)
    assert connection.execute('PRAGMA quick_check').fetchone() == ('ok',)
receipt = dict(schema='teaql.dotnet-order-business/v1', passed=True,
    sameTerminalDatabase=True, terminalRuns=2, generatedScopesUnchanged=True,
    namedSiblingDomainLibraryConsumed=str(library), runtimeSourceProjects=sorted(actual),
    orders=rows, presets=presets, executedAssemblySHA256=hashlib.sha256(assembly.read_bytes()).hexdigest(),
    databaseSHA256=hashlib.sha256(database.read_bytes()).hexdigest(), auditEventCountsClaimed=False)
(root / 'business-receipt.json').write_text(json.dumps(receipt, indent=2, sort_keys=True) + '\n')
print(json.dumps(dict(passed=True, terminalRuns=2, sourceProjects=len(actual))))
PY
