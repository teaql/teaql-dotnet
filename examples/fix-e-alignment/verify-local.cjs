#!/usr/bin/env node
"use strict";
const fs = require('node:fs'), path = require('node:path'), os = require('node:os'), cp = require('node:child_process'), crypto = require('node:crypto'), assert = require('node:assert/strict');
const example = __dirname, repo = path.resolve(example, '../..'), sha = b => crypto.createHash('sha256').update(b).digest('hex');
function fingerprint() {
  const m = JSON.parse(fs.readFileSync(path.join(example, 'fixture-manifest.json')));
  assert.equal(m.language, 'dotnet'); assert.equal(m.evaluationErrors, 0);
  assert.equal(sha(fs.readFileSync(path.join(example, 'model.xml'))), m.modelSha256);
  const actual = [];
  function visit(dir, prefix = '') { for (const e of fs.readdirSync(dir, {withFileTypes: true})) {
    const name = path.join(prefix, e.name), file = path.join(dir, e.name); assert(!e.isSymbolicLink());
    if (e.isDirectory()) visit(file, name); else { assert(e.isFile()); actual.push(name); }
  } }
  visit(path.join(example, 'generated')); assert.deepEqual(actual.sort(), Object.keys(m.files).sort());
  for (const [name, expected] of Object.entries(m.files)) assert.equal(sha(fs.readFileSync(path.join(example, 'generated', name))), expected, name);
  return m;
}
const requested = process.env.TEAQL_FIX_E_EVIDENCE_DIR;
const evidence = requested ? path.resolve(requested) : fs.mkdtempSync(path.join(os.tmpdir(), 'teaql-dotnet-fix-e-evidence-'));
if (requested) { assert(path.isAbsolute(requested) && !fs.existsSync(evidence)); fs.mkdirSync(evidence, {recursive: true}); }
const scratch = fs.mkdtempSync(path.join(os.tmpdir(), 'teaql-dotnet-fix-e-build-'));
const report = {schema: 'teaql.dotnet-fix-e-example/v1', passed: false, commands: [], sameDatabase: true,
  localRuntime: repo, databaseRetained: true, sharedCacheRemoved: false};
const {createOwnedRunner, ownedBytes} = require('./owned-command.cjs'), runner = createOwnedRunner();
async function main() {
try {
  const disk = fs.statfsSync(scratch); assert(disk.bavail * disk.bsize > 4 * 1024 ** 3);
  const m = fingerprint(); report.modelSha256 = m.modelSha256;
  report.expressionBoundaryApplicationSha256 = sha(fs.readFileSync(path.join(example, 'ExpressionBoundaryProbe.cs')));
  report.partialAncestorApplicationSha256 = sha(fs.readFileSync(path.join(example, 'PartialAncestorProbe.cs')));
  report.partialRootApplicationSha256 = sha(fs.readFileSync(path.join(example, 'PartialRootProbe.cs')));
  report.projectionApplicationSha256 = sha(fs.readFileSync(path.join(example, 'ProjectionViewsProbe.cs')));
  report.unchangedViewApplicationSha256 = sha(fs.readFileSync(path.join(example, 'UnchangedViewsProbe.cs')));
  report.checkerBoundaryApplicationSha256 = sha(fs.readFileSync(path.join(example, 'CheckerBoundaryProbe.cs')));
  report.graphFixLifecycleApplicationSha256 = sha(fs.readFileSync(path.join(example, 'GraphFixLifecycleProbe.cs')));
  report.concurrentQueryApplicationSha256 = sha(fs.readFileSync(path.join(example, 'ConcurrentQueryProbe.cs')));
  report.cancelledNewApplicationSha256 = sha(fs.readFileSync(path.join(example, 'CancelledNewProbe.cs')));
  const generated = path.join(scratch, 'generated'), consumer = path.join(scratch, 'consumer');
  fs.cpSync(path.join(example, 'generated'), generated, {recursive: true}); fs.mkdirSync(consumer);
  for (const file of fs.readdirSync(example).filter(file => file.endsWith('.cs'))) fs.copyFileSync(path.join(example, file), path.join(consumer, file));
  const project = '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net8.0</TargetFramework><OutputType>Exe</OutputType><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup><ItemGroup><ProjectReference Include="../generated/checker-fix-advanced-lib.csproj" /></ItemGroup></Project>\n';
  const projectPath = path.join(consumer, 'TeaqlFixEExample.csproj'), artifacts = path.join(scratch, 'artifacts');
  fs.writeFileSync(projectPath, project); fs.writeFileSync(path.join(evidence, 'consumer-project.xml'), project);
  const properties = ['-p:UseArtifactsOutput=true', '-p:ArtifactsPath=' + artifacts, '-p:TeaQLRuntimeSourceRoot=' + repo];
  const env = {...process.env, DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER: '1', TEAQL_FIX_E_DB: path.join(evidence, 'fixture.sqlite')};
  async function run(command, log, timeout = 120000) {
    const file = path.join(evidence, log), fd = fs.openSync(file, 'w'); let result;
    try { result = await runner.run(command, {cwd: consumer, env, timeout, stdio: ['ignore', fd, fd]}); }
    finally { fs.closeSync(fd); }
    const bytes = fs.readFileSync(file);
    report.commands.push({command, exitCode: result.status, signal: result.signal, logSha256: sha(bytes)});
    if (result.status !== 0) process.exitCode = result.status || 1;
    assert.equal(result.status, 0, bytes.toString().slice(-12000)); return bytes.toString();
  }
  await run(['dotnet', 'build', projectPath, '--disable-build-servers', '-m:2', '--verbosity', 'minimal', ...properties], 'build.log', 300000);
  const executable = (await run(['dotnet', 'msbuild', projectPath, '-nologo', '-getProperty:TargetPath', ...properties], 'target-path.log')).trim();
  assert(executable.startsWith(artifacts + path.sep) && fs.existsSync(executable), 'execute the actual owned build, not a default bin fallback');
  const deps = JSON.parse(fs.readFileSync(executable.replace(/\.dll$/, '.deps.json')));
  for (const name of ['TeaQL.Core', 'TeaQL.Runtime', 'TeaQL.DataService', 'TeaQL.Sql', 'TeaQL.Provider.Sqlite']) {
    const entries = Object.entries(deps.libraries).filter(([key]) => key.startsWith(name + '/'));
    assert.equal(entries.length, 1, name); assert.equal(entries[0][1].type, 'project', 'not local source: ' + name);
  }
  report.binarySha256 = sha(fs.readFileSync(executable)); report.localProjectDependencies = true;
  for (const round of [1, 2]) {
    env.TEAQL_FIX_E_ROUND = String(round);
    const log = await run(['dotnet', executable], 'round-' + round + '.log');
    for (const marker of ['FIX_E_EXAMPLE=', 'CORE_LOAD_EXAMPLE=', 'CORE_MUTATION_OWNERSHIP=', 'CORE_MUTATION_PAGING=', 'DOTNET_VERSION_BOUNDARIES=', 'DOTNET_CHECKER_METADATA=', 'DOTNET_CHECKER_BOUNDARY_RESULT=', 'CORE_DOTNET_E_BOUNDARY_RESULT=', 'CORE_DOTNET_PARTIAL_ANCESTORS_RESULT=', 'CORE_DOTNET_PARTIAL_ROOT_RESULT=']) {
      const values = log.split(/\r?\n/).filter(line => line.startsWith(marker)).map(line => JSON.parse(line.slice(marker.length)));
      assert.equal(values.length, 1, marker); assert.equal(values[0].passed, true, marker);
    }
    const boundary = log.split(/\r?\n/).filter(line => line.startsWith('DOTNET_CHECKER_BOUNDARY_CASE='))
      .map(line => JSON.parse(line.slice('DOTNET_CHECKER_BOUNDARY_CASE='.length)));
    assert.equal(boundary.length, 28); assert(boundary.every(v => v.passed && v.round === round));
    const projections = log.split(/\r?\n/).filter(line => line.startsWith('DOTNET_PROJECTION_VIEW_CASE='))
      .map(line => JSON.parse(line.slice('DOTNET_PROJECTION_VIEW_CASE='.length)));
    assert.equal(projections.length, 3);
    assert.deepEqual(projections.map(v => v.case).sort(), ['independent-query-projection-boundary',
      'same-identity-independent-mutation-ledgers', 'intra-query-repeated-identity-projection-boundary'].sort());
    assert(projections.every(v => v.passed && v.round === round));
    const unchanged = log.split(/\r?\n/).filter(line => line.startsWith('DOTNET_UNCHANGED_VIEW_CASE='))
      .map(line => JSON.parse(line.slice('DOTNET_UNCHANGED_VIEW_CASE='.length)));
    assert.equal(unchanged.length, 5);
    assert.equal(new Set(unchanged.map(v => v.case)).size, 5);
    assert(unchanged.every(v => v.passed && v.round === round && !v.pending && v.jsonPreserved
      && Object.values(v.effects).every(count => count === 0)));
    for (const marker of ['DOTNET_UNCHANGED_VIEW_WRITE=', 'DOTNET_UNCHANGED_VIEW_PHYSICAL=']) {
      const rows = log.split(/\r?\n/).filter(line => line.startsWith(marker)).map(line => JSON.parse(line.slice(marker.length)));
      assert.equal(rows.length, 1); assert(rows[0].passed && rows[0].round === round);
    }
    const graph = log.split(/\r?\n/).filter(line => line.startsWith('DOTNET_GRAPH_LIFECYCLE_CASE='))
      .map(line => JSON.parse(line.slice('DOTNET_GRAPH_LIFECYCLE_CASE='.length)));
    assert.equal(graph.length, 12); assert.equal(new Set(graph.map(row => row.case)).size, 12);
    assert(graph.every(row => row.passed && row.round === round && row.clockCalls === 1));
    const graphResult = log.split(/\r?\n/).filter(line => line.startsWith('DOTNET_GRAPH_LIFECYCLE_RESULT='))
      .map(line => JSON.parse(line.slice('DOTNET_GRAPH_LIFECYCLE_RESULT='.length)));
    assert.equal(graphResult.length, 1); assert(graphResult[0].passed && graphResult[0].round === round && graphResult[0].cases === 12);
    const concurrency = log.split(/\r?\n/).filter(line => line.startsWith('DOTNET_CONCURRENT_QUERY='))
      .map(line => JSON.parse(line.slice('DOTNET_CONCURRENT_QUERY='.length)));
    assert.equal(concurrency.length, 1); assert(concurrency[0].passed && concurrency[0].observation.round === round);
    assert(concurrency[0].observation.bothLive && concurrency[0].observation.sameContext && concurrency[0].observation.distinctRoots);
    assert.equal(concurrency[0].observation.physicalQueries, 6); assert.equal(concurrency[0].observation.providerRootCalls, 3);
    assert.deepEqual(fingerprint(), m);
    const cancelled = log.split(/\r?\n/).filter(line => line.startsWith('DOTNET_CANCELLED_NEW='))
      .map(line => JSON.parse(line.slice('DOTNET_CANCELLED_NEW='.length)));
    assert.deepEqual(cancelled.map(row => row.scenario), ['root', 'complete-child', 'incomplete-child',
      'already-composed-child', 'invalid-live-sibling', 'rollback-retry']);
    assert(cancelled.every(row => row.passed && row.round === round));
    const summary = log.split(/\r?\n/).filter(line => line.startsWith('DOTNET_CANCELLED_NEW_RESULT='))
      .map(line => JSON.parse(line.slice('DOTNET_CANCELLED_NEW_RESULT='.length)));
    assert.deepEqual(summary, [{round, passed: true, cases: 6, passedCases: 6}]);
  }
  report.databaseSha256 = sha(fs.readFileSync(path.join(evidence, 'fixture.sqlite'))); report.passed = true;
  report.expressionBoundaryDatabase = {file: 'fixture.sqlite.expression-boundaries.sqlite',
    sha256: sha(fs.readFileSync(path.join(evidence, 'fixture.sqlite.expression-boundaries.sqlite')))};
  report.partialAncestorDatabase = {file: 'fixture.sqlite.partial-ancestors.sqlite',
    sha256: sha(fs.readFileSync(path.join(evidence, 'fixture.sqlite.partial-ancestors.sqlite')))};
  report.partialRootDatabase = {file: 'fixture.sqlite.partial-root.sqlite',
    sha256: sha(fs.readFileSync(path.join(evidence, 'fixture.sqlite.partial-root.sqlite')))};
  report.projectionDatabase = {file: 'fixture.sqlite.projection-views.sqlite',
    sha256: sha(fs.readFileSync(path.join(evidence, 'fixture.sqlite.projection-views.sqlite')))};
  report.unchangedViewDatabase = {file: 'fixture.sqlite.unchanged-views.sqlite',
    sha256: sha(fs.readFileSync(path.join(evidence, 'fixture.sqlite.unchanged-views.sqlite')))};
  report.checkerBoundaryDatabase = {file: 'fixture.sqlite.checker-boundaries.sqlite',
    sha256: sha(fs.readFileSync(path.join(evidence, 'fixture.sqlite.checker-boundaries.sqlite')))};
  report.graphFixLifecycleDatabase = {file: 'fixture.sqlite.graph-fix-lifecycle.sqlite',
    sha256: sha(fs.readFileSync(path.join(evidence, 'fixture.sqlite.graph-fix-lifecycle.sqlite')))};
  report.concurrentQueryDatabase = {file: 'fixture.sqlite.concurrent-query.sqlite',
    sha256: sha(fs.readFileSync(path.join(evidence, 'fixture.sqlite.concurrent-query.sqlite')))};
  report.cancelledNewDatabase = {file: 'fixture.sqlite.cancelled-new.sqlite',
    sha256: sha(fs.readFileSync(path.join(evidence, 'fixture.sqlite.cancelled-new.sqlite')))};
  console.log('PASS .NET retained Fix/E/loading/ownership example twice on one database');
} catch (error) { console.error(error.stack); process.exitCode ||= 1; }
finally {
  report.disposedOwnedBytes = runner.safeToClean ? ownedBytes(scratch) : 0;
  if (runner.safeToClean) fs.rmSync(scratch, {recursive: true});
  else report.unsafeBuildRetention = {path: scratch, processGroup: runner.unsafeGroup};
  report.scratchRemoved = !fs.existsSync(scratch); runner.dispose();
  fs.writeFileSync(path.join(evidence, 'receipt.json'), JSON.stringify(report, null, 2) + '\n');
  console.log('Retained compact example evidence: ' + evidence);
}
}
main().catch(error => { console.error(error.stack); process.exitCode ||= 1; });
