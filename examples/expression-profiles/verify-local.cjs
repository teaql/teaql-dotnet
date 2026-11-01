#!/usr/bin/env node
'use strict';
const fs=require('node:fs'),path=require('node:path'),os=require('node:os'),cp=require('node:child_process'),crypto=require('node:crypto'),assert=require('node:assert/strict');
const {createOwnedRunner,ownedBytes}=require('../fix-e-alignment/owned-command.cjs');
const EXAMPLE=__dirname,REPO=path.resolve(EXAMPLE,'../..'),PROFILES=['camelCase','snake_case','PascalCase'],LANGUAGE="dotnet";
const sha=b=>crypto.createHash('sha256').update(b).digest('hex'),digest=f=>sha(fs.readFileSync(f));
function sourceHash(){
 const names=[...new Set(cp.execFileSync('git',['-C',REPO,'ls-files','-z','--cached','--others','--exclude-standard']).toString().split('\0').filter(Boolean))].sort(),h=crypto.createHash('sha256');
 for(const n of names){const f=path.join(REPO,n);h.update(n+'\0');if(!fs.existsSync(f)){h.update('deleted\0');continue;}const s=fs.lstatSync(f);h.update(String(s.mode)+'\0');h.update(s.isSymbolicLink()?fs.readlinkSync(f):fs.readFileSync(f));h.update('\0');}return h.digest('hex');
}
function fixture(){
 const root=path.join(EXAMPLE,'generated'),manifest=JSON.parse(fs.readFileSync(path.join(root,'manifest.json'))),provenance=JSON.parse(fs.readFileSync(path.join(EXAMPLE,'provenance.json')));
 assert.equal(manifest.schema,'teaql.generated-expression-model-profiles/v1');assert.equal(manifest.language,"dotnet");assert.equal(manifest.localSourceOnly,true);
 assert.equal(digest(path.join(root,'manifest.json')),provenance.manifestSha256);assert.deepEqual(Object.keys(manifest.profiles).sort(),[...PROFILES].sort());
 for(const[p,v]of Object.entries(manifest.profiles)){assert.deepEqual(v.evaluation.errors,[]);assert.equal(digest(path.join(root,p,'model.xml')),v.modelSha256);
  for(const[n,h]of Object.entries(v.files)){const f=path.resolve(root,p,n);assert(f.startsWith(path.join(root,p)+path.sep));assert.equal(fs.realpathSync(f),f);assert.equal(digest(f),h);}}
 return{manifest,provenance,provenanceSha256:digest(path.join(EXAMPLE,'provenance.json'))};
}
async function main(){
 const requested=process.env.TEAQL_GENERATED_E_PROFILE_EVIDENCE_DIR,output=requested?path.resolve(requested):fs.mkdtempSync(path.join(os.tmpdir(),'teaql-'+LANGUAGE+'-expression-evidence-'));
 if(requested){assert(path.isAbsolute(requested)&&!fs.existsSync(output));fs.mkdirSync(output,{recursive:true});}
 const report={schema:'teaql.'+LANGUAGE+'-generated-e-model-profiles/v1',passed:false,localSourceOnly:true,publicPublication:false,installedArtifactAccepted:false,completeExampleInventoryAccepted:false,allP0P1Complete:false,commands:[],profiles:{}};
 let scratch,runner;
 try{
  const disk=fs.statfsSync(os.tmpdir());assert(disk.bavail*disk.bsize>8*1024**3);
  report.sourceBefore={commit:cp.execFileSync('git',['-C',REPO,'rev-parse','HEAD'],{encoding:'utf8'}).trim(),sourceContentsSha256:sourceHash()};report.fixture=fixture();report.controllerSha256=digest(__filename);
  scratch=fs.mkdtempSync(path.join(os.tmpdir(),'teaql-'+LANGUAGE+'-expression-build-'));runner=createOwnedRunner();
  const env={...process.env,TMPDIR:scratch};
  async function run(profile,name,command,cwd,timeout=300000){
   const folder=path.join(output,profile);fs.mkdirSync(folder,{recursive:true});const file=path.join(folder,name+'.log'),fd=fs.openSync(file,'wx');let result;
   try{result=await runner.run(command,{cwd,env,timeout,stdio:['ignore',fd,fd]});}finally{fs.closeSync(fd);}
   const log=fs.readFileSync(file,'utf8');report.commands.push({profile,name,command,exitCode:result.status,signal:result.signal,logSha256:digest(file)});
   assert.equal(result.status,0,log.slice(-6000));assert.equal(result.signal,null);assert(runner.safeToClean);return log;
  }
  for(const profile of PROFILES){
   const work=path.join(scratch,profile);fs.mkdirSync(work);env.TEAQL_EXPRESSION_PROFILE=profile;env.TEAQL_EXPRESSION_DB=path.join(output,profile,'fixture.sqlite');
   fs.mkdirSync(path.join(output,profile));const state={rounds:[],sameDatabase:true};report.profiles[profile]=state;

   env.DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER='1';env.DOTNET_CLI_USE_MSBUILD_SERVER='0';
   const modules=['TeaQL.Core','TeaQL.DataService','TeaQL.Runtime','TeaQL.Sql','TeaQL.Provider.Sqlite'];
   fs.cpSync(path.join(EXAMPLE,'generated',profile),path.join(work,'generated'),{recursive:true});fs.copyFileSync(path.join(EXAMPLE,'Program.cs'),path.join(work,'Program.cs'));report.applicationSha256=digest(path.join(EXAMPLE,'Program.cs'));
   const project=path.join(work,'ExpressionProfiles.csproj'),xml='<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable><EnableDefaultCompileItems>false</EnableDefaultCompileItems></PropertyGroup><ItemGroup><Compile Include="Program.cs"/><Compile Include="generated/**/*.cs" Exclude="generated/**/obj/**;generated/**/bin/**"/>'+modules.map(n=>'<Using Include="'+n+'"/><ProjectReference Include="'+path.join(REPO,'src',n,n+'.csproj')+'"/>').join('')+'</ItemGroup></Project>';
   fs.writeFileSync(project,xml);fs.writeFileSync(path.join(output,profile,'consumer.csproj'),xml);
   const artifacts=path.join(scratch,'artifacts'),feed=path.join(scratch,'empty-feed');fs.mkdirSync(feed,{recursive:true});
   const flags=['-p:UseSharedCompilation=false','-maxcpucount:2','-nodeReuse:false','-p:UseArtifactsOutput=true','-p:ArtifactsPath='+artifacts];
   await run(profile,'restore',['dotnet','restore',project,...flags,'-p:RestoreSources='+feed],work);
   await run(profile,'build',['dotnet','build',project,'--no-restore',...flags],work);
   const executable=(await run(profile,'target-path',['dotnet','msbuild',project,'-nologo','-getProperty:TargetPath',...flags],work)).trim();assert(executable.startsWith(artifacts+path.sep)&&fs.existsSync(executable));
   state.runtimeProjects=modules.map(n=>path.join(REPO,'src',n,n+'.csproj'));state.executable=executable;
   for(const round of[1,2]){env.TEAQL_EXPRESSION_ROUND=String(round);const log=await run(profile,'round-'+round,['dotnet',executable],work,90000);
    const rows=[...log.matchAll(/^GENERATED_E_MODEL_PROFILE=(.+)$/gm)].map(m=>JSON.parse(m[1]));assert.equal(rows.length,1);const v=rows[0];
    assert.equal(v.passed,true);assert.equal(v.profile,profile);assert.equal(v.round,round);assert.equal(v.providerEntries,0);assert(v.queryCalibration>0);
    assert.deepEqual(Object.keys(v.runtimeOrigins).sort(),[...modules].sort());assert(Object.values(v.runtimeOrigins).every(p=>p.startsWith(artifacts+path.sep)));state.rounds.push(v);
   }

   state.databaseSha256=digest(env.TEAQL_EXPRESSION_DB);assert.deepEqual(fixture(),report.fixture);assert.equal(sourceHash(),report.sourceBefore.sourceContentsSha256);
  }
  report.passed=true;
 }catch(error){report.error=error.stack;console.error(error.stack);process.exitCode=1;}
 finally{
  const safe=!runner||runner.safeToClean;report.cleanup={scratchRemoved:false,disposedOwnedBytes:safe&&scratch?ownedBytes(scratch):0,sharedCachesDeleted:false,databasesDeleted:false};
  if(safe&&scratch){fs.rmSync(scratch,{recursive:true});report.cleanup.scratchRemoved=true;}else if(scratch)report.unsafeRetention=scratch;
  runner?.dispose();report.sourceAfter=sourceHash();fs.writeFileSync(path.join(output,'report.json'),JSON.stringify(report,null,2)+'\n',{flag:'wx'});console.log(JSON.stringify({output,passed:report.passed,cleanup:report.cleanup}));
 }
}
if(require.main===module)main().catch(e=>{console.error(e.stack);process.exitCode=1;});
