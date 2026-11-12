#!/usr/bin/env node
'use strict';
const fs = require('node:fs'), path = require('node:path'), os = require('node:os');
const cp = require('node:child_process'), crypto = require('node:crypto'), assert = require('node:assert/strict');
const {createOwnedRunner, ownedBytes} = require('../examples/fix-e-alignment/owned-command.cjs');
const {inspect} = require('./verify-test-inventory.cjs');
const repo = path.resolve(__dirname, '..'), example = path.join(repo, 'examples/natural-messages');
const hash = bytes => crypto.createHash('sha256').update(bytes).digest('hex');
function files(root) {
  const result = [];
  function visit(dir) {
    for (const entry of fs.readdirSync(dir, {withFileTypes:true})) {
      assert(!entry.isSymbolicLink(), 'Unexpected symlink: '+entry.name);
      const file = path.join(dir, entry.name);
      if (entry.isDirectory()) { if (!['bin','obj'].includes(entry.name)) visit(file); }
      else if (entry.isFile()) result.push(path.relative(root,file));
    }
  }
  visit(root); return result.sort();
}
function fingerprint() {
  const result = {};
  for (const root of ['src','tests','examples/natural-messages']) for (const name of files(path.join(repo,root)))
    if (/\.(cs|csproj|json)$/.test(name)) result[root+'/'+name] = hash(fs.readFileSync(path.join(repo,root,name)));
  for (const name of ['TeaQL.sln','Directory.Build.props','scripts/verify-natural-messages.cjs','scripts/verify-natural-messages.test.cjs',
    'scripts/verify-test-inventory.cjs','examples/fix-e-alignment/owned-command.cjs']) result[name] = hash(fs.readFileSync(path.join(repo,name)));
  return result;
}
function fixtureGuard() {
  const root = path.join(example,'fixture'), manifest = JSON.parse(fs.readFileSync(path.join(root,'generated-files.json')));
  function noBuild(dir) { for (const entry of fs.readdirSync(dir,{withFileTypes:true})) {
    assert(!entry.isSymbolicLink()); if (entry.isDirectory()) { assert(!['bin','obj'].includes(entry.name)); noBuild(path.join(dir,entry.name)); }
  } } noBuild(root);
  assert.deepEqual(files(root), [...Object.keys(manifest),'generated-files.json','model.xml'].sort());
  for (const [name, expected] of Object.entries(manifest)) {
    assert(!path.isAbsolute(name) && !name.split(/[\\/]/).includes('..'));
    assert.equal(hash(fs.readFileSync(path.join(root,name))),expected,name);
  }
  return hash(fs.readFileSync(path.join(root,'generated-files.json')));
}
async function main() {
  const args = process.argv.slice(2); let output, full=false;
  for (let i=0;i<args.length;i++) {
    if (args[i]==='--full') full=true;
    else if (args[i]==='--output' && args[i+1]) output=args[++i];
    else throw new Error('Usage: node scripts/verify-natural-messages.cjs [--full] [--output /new/evidence]');
  }
  if (output) { assert(path.isAbsolute(output) && !fs.existsSync(output)); fs.mkdirSync(output,{recursive:true}); }
  else output=fs.mkdtempSync(path.join(os.tmpdir(),'teaql-dotnet-natural-evidence-'));
  const scratch=fs.mkdtempSync(path.join(os.tmpdir(),'teaql-dotnet-natural-build-')), runner=createOwnedRunner();
  const receipt={schema:'teaql.dotnet-natural-messages/v1',passed:false,commands:[],rounds:[],publicPublication:false,scratchPath:scratch};
  try {
    const disk=fs.statfsSync(scratch); assert(disk.bavail*disk.bsize>4*1024**3);
    receipt.sourceCommit=cp.execFileSync('git',['-C',repo,'rev-parse','HEAD'],{encoding:'utf8'}).trim();
    receipt.sourceFiles=fingerprint(); receipt.fixtureSha256=fixtureGuard();
    const generated=path.join(scratch,'generated'), consumer=path.join(scratch,'consumer'), artifacts=path.join(scratch,'artifacts');
    fs.cpSync(path.join(example,'fixture'),generated,{recursive:true}); fs.mkdirSync(consumer);
    fs.copyFileSync(path.join(example,'Program.cs'),path.join(consumer,'Program.cs'));
    const project=path.join(consumer,'NaturalMessagesExample.csproj');
    fs.writeFileSync(project,'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net8.0</TargetFramework><OutputType>Exe</OutputType><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup><ItemGroup><ProjectReference Include="../generated/context-fix-service-lib.csproj" /></ItemGroup></Project>\n');
    const dotnet=process.env.DOTNET_EXE||'dotnet';
    const properties=['-p:UseArtifactsOutput=true','-p:ArtifactsPath='+artifacts,'-p:TeaQLRuntimeSourceRoot='+repo,'-p:UseSharedCompilation=false'];
    const env={...process.env,DOTNET_CLI_USE_MSBUILD_SERVER:'0',MSBUILDDISABLENODEREUSE:'1',TEAQL_NATURAL_MESSAGES_DB:path.join(output,'fixture.sqlite')};
    async function run(command,name,timeout=300000) {
      const target=path.join(output,name),fd=fs.openSync(target,'wx'); let result;
      try { result=await runner.run(command,{cwd:consumer,env,timeout,stdio:['ignore',fd,fd]}); } finally {fs.closeSync(fd);}
      const bytes=fs.readFileSync(target);receipt.commands.push({command,log:name,logSha256:hash(bytes),exitCode:result.status,signal:result.signal});
      assert.equal(result.status,0,bytes.toString().slice(-12000));return bytes.toString();
    }
    receipt.testProjects=inspect(repo);
    const tests=full ? path.join(repo,'TeaQL.sln') : path.join(repo,'src/TeaQL.Runtime.Tests/TeaQL.Runtime.Tests.csproj');
    await run([dotnet,'test',tests,'--disable-build-servers','-m:2','--verbosity','minimal',
      ...(full?[]:['--filter','FullyQualifiedName~Checker']),...properties], 'native-tests.log',600000);
    receipt.fullSolution=full;
    await run([dotnet,'build',project,'--disable-build-servers','-m:2','--verbosity','minimal',...properties],'build.log');
    const binary=(await run([dotnet,'msbuild',project,'-nologo','-getProperty:TargetPath',...properties],'target-path.log')).trim();
    assert(binary.startsWith(artifacts+path.sep)&&fs.existsSync(binary));
    const dependencies=JSON.parse(fs.readFileSync(binary.replace(/\.dll$/,'.deps.json')));
    for (const name of ['TeaQL.Core','TeaQL.Runtime','TeaQL.DataService','TeaQL.Sql','TeaQL.Provider.Sqlite']) {
      const entries=Object.entries(dependencies.libraries).filter(([key])=>key.startsWith(name+'/'));
      assert.equal(entries.length,1);assert.equal(entries[0][1].type,'project',name);
    }
    receipt.localProjectDependencies=true;receipt.binarySha256=hash(fs.readFileSync(binary));
    for (const round of [1,2]) {
      const log=await run([dotnet,binary],`round-${round}.log`);
      const rows=log.split(/\r?\n/).filter(line=>line.startsWith('DOTNET_NATURAL_SQLITE=')).map(line=>JSON.parse(line.slice('DOTNET_NATURAL_SQLITE='.length)));
      assert.equal(rows.length,1);const row=rows[0];
      for(const key of ['passed','typedQ','typedE','unchangedParent'])assert.equal(row[key],true,key);
      assert.equal(row.failedCheckProviderCalls,0);assert.equal(row.correctedWrites,1);assert.equal(row.correctedAudits,1);assert.equal(row.originalLength,10);
      receipt.rounds.push({round,observations:row});assert.deepEqual(fingerprint(),receipt.sourceFiles);assert.equal(fixtureGuard(),receipt.fixtureSha256);
    }
    receipt.databaseSha256=hash(fs.readFileSync(env.TEAQL_NATURAL_MESSAGES_DB));receipt.sameDatabase=true;receipt.passed=true;
  } catch(error) {receipt.error=String(error);process.exitCode=1;console.error(error);}
  finally {
    receipt.disposableBytesRemoved=runner.safeToClean?ownedBytes(scratch):0;
    if(runner.safeToClean)fs.rmSync(scratch,{recursive:true});else receipt.unsafeBuildRetention={path:scratch,processGroup:runner.unsafeGroup};
    runner.dispose();fs.writeFileSync(path.join(output,'receipt.json'),JSON.stringify(receipt,null,2)+'\n');
    console.log(JSON.stringify({passed:receipt.passed,evidence:output,rounds:receipt.rounds.length,fullSolution:receipt.fullSolution}));
  }
}
main().catch(error=>{console.error(error);process.exitCode=1;});
