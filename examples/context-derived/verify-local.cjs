#!/usr/bin/env node
'use strict';
const fs = require('node:fs'), path = require('node:path'), os = require('node:os'), cp = require('node:child_process'), crypto = require('node:crypto'), assert = require('node:assert/strict');
const {createOwnedRunner, ownedBytes} = require('../fix-e-alignment/owned-command.cjs');
const example = __dirname, repo = path.resolve(example, '../..'), fixture = path.join(example, 'fixture');
const sha = bytes => crypto.createHash('sha256').update(bytes).digest('hex');
function files(root) {
  const result = [];
  function visit(dir) { for (const item of fs.readdirSync(dir, {withFileTypes: true})) {
    assert(!item.isSymbolicLink(), 'symlink is forbidden'); const file = path.join(dir, item.name);
    if (item.isDirectory()) { if (!['bin', 'obj'].includes(item.name)) visit(file); }
    else if (item.isFile()) result.push(path.relative(root, file));
  } }
  visit(root); return result.sort();
}
function fingerprint() {
  const manifest = JSON.parse(fs.readFileSync(path.join(fixture, 'generated-files.json')));
  // Fixture directories may never acquire build outputs; source is immutable.
  function noBuild(dir) { for (const item of fs.readdirSync(dir, {withFileTypes:true})) {
    assert(!item.isSymbolicLink()); if (item.isDirectory()) { assert(!['bin','obj'].includes(item.name)); noBuild(path.join(dir,item.name)); }
  } } noBuild(fixture);
  assert.deepEqual(files(fixture), [...Object.keys(manifest), 'generated-files.json', 'model.xml'].sort());
  for (const [name, hash] of Object.entries(manifest)) {
    assert(!path.isAbsolute(name) && !name.split('/').includes('..'));
    assert.equal(sha(fs.readFileSync(path.join(fixture, name))), hash, name);
  }
  return {manifestSha256:sha(fs.readFileSync(path.join(fixture,'generated-files.json'))), modelSha256:sha(fs.readFileSync(path.join(fixture,'model.xml')))};
}
function sourceHash() {
  const digest = crypto.createHash('sha256');
  for (const name of files(path.join(repo,'src')).filter(name => name.endsWith('.cs') || name.endsWith('.csproj') || name.endsWith('.json'))) {
    digest.update(name + '\0'); digest.update(fs.readFileSync(path.join(repo,'src',name)));
  }
  digest.update(fs.readFileSync(path.join(repo,'Directory.Build.props'))); return digest.digest('hex');
}
const requested = process.env.TEAQL_CONTEXT_FIX_EVIDENCE_DIR;
const evidence = requested ? path.resolve(requested) : fs.mkdtempSync(path.join(os.tmpdir(),'teaql-dotnet-context-evidence-'));
if (requested) { assert(path.isAbsolute(requested) && !fs.existsSync(evidence)); fs.mkdirSync(evidence,{recursive:true}); }
const scratch = fs.mkdtempSync(path.join(os.tmpdir(),'teaql-dotnet-context-build-'));
const runner = createOwnedRunner();
const report = {schema:'teaql.dotnet-context-derived-mutation/v1',passed:false,commands:[],rounds:[],publicPublication:false,databaseRetained:true};
async function main() {
  try {
    const disk=fs.statfsSync(scratch);assert(disk.bavail*disk.bsize > 4*1024**3);
    report.fixture=fingerprint(); report.sourceSha256=sourceHash();
    report.sourceCommit=cp.execFileSync('git',['-C',repo,'rev-parse','HEAD'],{encoding:'utf8'}).trim();
    report.applicationSha256=sha(fs.readFileSync(path.join(example,'Program.cs')));
    const generated=path.join(scratch,'generated'), consumer=path.join(scratch,'consumer'), artifacts=path.join(scratch,'artifacts');
    fs.cpSync(fixture,generated,{recursive:true}); fs.mkdirSync(consumer); fs.copyFileSync(path.join(example,'Program.cs'),path.join(consumer,'Program.cs'));
    const project=path.join(consumer,'ContextFixExample.csproj');
    fs.writeFileSync(project,'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net8.0</TargetFramework><OutputType>Exe</OutputType><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup><ItemGroup><ProjectReference Include="../generated/context-fix-service-lib.csproj" /></ItemGroup></Project>\n');
    const properties=['-p:UseArtifactsOutput=true','-p:ArtifactsPath='+artifacts,'-p:TeaQLRuntimeSourceRoot='+repo];
    const env={...process.env,DOTNET_CLI_USE_MSBUILD_SERVER:'0',TEAQL_CONTEXT_FIX_DB:path.join(evidence,'fixture.sqlite')};
    async function run(command,log,timeout=120000) {
      const file=path.join(evidence,log),fd=fs.openSync(file,'wx');let result;
      try {result=await runner.run(command,{cwd:consumer,env,timeout,stdio:['ignore',fd,fd]});} finally {fs.closeSync(fd);}
      const bytes=fs.readFileSync(file); report.commands.push({command,exitCode:result.status,signal:result.signal,logSha256:sha(bytes)});
      assert.equal(result.status,0,bytes.toString().slice(-8000)); return bytes.toString();
    }
    await run(['dotnet','build',project,'--disable-build-servers','-m:2','--verbosity','minimal',...properties],'build.log',300000);
    const binary=(await run(['dotnet','msbuild',project,'-nologo','-getProperty:TargetPath',...properties],'target-path.log')).trim();
    assert(binary.startsWith(artifacts+path.sep)&&fs.existsSync(binary));
    const dependencies=JSON.parse(fs.readFileSync(binary.replace(/\.dll$/,'.deps.json')));
    for (const name of ['TeaQL.Core','TeaQL.Runtime','TeaQL.DataService','TeaQL.Sql','TeaQL.Provider.Sqlite']) {
      const entries=Object.entries(dependencies.libraries).filter(([key])=>key.startsWith(name+'/'));
      assert.equal(entries.length,1);assert.equal(entries[0][1].type,'project',name);
    }
    report.localProjectDependencies=true; report.binarySha256=sha(fs.readFileSync(binary));
    for (const round of [1,2]) {
      const log=await run(['dotnet',binary],`round-${round}.log`);
      const rows=log.split(/\r?\n/).filter(line=>line.startsWith('DOTNET_CONTEXT_FIX_SQLITE=')).map(line=>JSON.parse(line.split('=')[1]));
      assert.equal(rows.length,1);const row=rows[0];
      for(const key of ['passed','typedQ','typedE','objectLocation','graphRollback','fixOnce'])assert.equal(row[key],true,key);
      assert.equal(row.noOpProviderCalls,0);assert.equal(row.failedCheckProviderCalls,0);assert.equal(row.updateAudits,1);
      report.rounds.push({round,observations:row}); assert.deepEqual(fingerprint(),report.fixture);assert.equal(sourceHash(),report.sourceSha256);
    }
    report.passed=true;report.sameDatabase=true;report.databaseSha256=sha(fs.readFileSync(env.TEAQL_CONTEXT_FIX_DB));
  } catch(error) {report.error=String(error);process.exitCode=1;console.error(error);}
  finally {
    // reap only this verifier's children before removing reproducible outputs.
    report.disposableBytesRemoved=runner.safeToClean ? ownedBytes(scratch) : 0;
    if(runner.safeToClean) fs.rmSync(scratch,{recursive:true});
    else report.unsafeBuildRetention={path:scratch,processGroup:runner.unsafeGroup};
    runner.dispose();
    fs.writeFileSync(path.join(evidence,'receipt.json'),JSON.stringify(report,null,2)+'\n');console.log(JSON.stringify({passed:report.passed,evidence,rounds:report.rounds.length}));
  }
}
main().catch(error=>{console.error(error);process.exitCode=1;});
