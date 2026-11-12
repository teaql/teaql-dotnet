'use strict';
const test=require('node:test'), assert=require('node:assert/strict'), fs=require('node:fs');
const path=require('node:path'), os=require('node:os'), cp=require('node:child_process');
const script=path.join(__dirname,'verify-natural-messages.cjs');
for(const mode of ['failure','missing-executable']) test('owned natural-message gate cleans after '+mode,()=>{
  const owned=fs.mkdtempSync(path.join(os.tmpdir(),'teaql-dotnet-natural-runner-test-'));
  try {
    const evidence=path.join(owned,'evidence');
    const result=cp.spawnSync(process.execPath,[script,'--output',evidence],{encoding:'utf8',timeout:20000,
      env:{...process.env,DOTNET_EXE:mode==='failure'?'/usr/bin/false':path.join(owned,'absent-dotnet')}});
    assert.equal(result.status,1,result.stderr);
    const receipt=JSON.parse(fs.readFileSync(path.join(evidence,'receipt.json')));
    assert.equal(receipt.passed,false);assert.equal(receipt.rounds.length,0);
    assert.equal(receipt.unsafeBuildRetention,undefined);assert(receipt.disposableBytesRemoved>0);
    assert(receipt.scratchPath.startsWith(os.tmpdir()+path.sep+'teaql-dotnet-natural-build-'));
    assert.equal(fs.existsSync(receipt.scratchPath),false);
    assert.equal(receipt.commands.length,1);assert.notEqual(receipt.commands[0].exitCode,0);
  } finally {fs.rmSync(owned,{recursive:true});}
});
test('existing evidence is never overwritten',()=>{
  const owned=fs.mkdtempSync(path.join(os.tmpdir(),'teaql-dotnet-natural-existing-test-'));
  try {
    const marker=path.join(owned,'keep.txt');fs.writeFileSync(marker,'retained evidence');
    const result=cp.spawnSync(process.execPath,[script,'--output',owned],{encoding:'utf8',timeout:20000});
    assert.equal(result.status,1);assert.equal(fs.readFileSync(marker,'utf8'),'retained evidence');
    assert.deepEqual(fs.readdirSync(owned),['keep.txt']);
  } finally {fs.rmSync(owned,{recursive:true});}
});
