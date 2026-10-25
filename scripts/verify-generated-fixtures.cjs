#!/usr/bin/env node
"use strict";
const fs = require('node:fs'), path = require('node:path'), crypto = require('node:crypto'), assert = require('node:assert/strict');
const repo = path.resolve(__dirname, '..'), sha = bytes => crypto.createHash('sha256').update(bytes).digest('hex');
const roots = ['examples/conformance', 'examples/trace-chain/lib', 'examples/facet-trace/lib',
  'examples/school-management', 'examples/order-management/dotnet-lib-core',
  'examples/trace-chain/standard-graph/lib', 'examples/trace-chain/standard-order/lib'];
let files = 0;
for (const relative of roots) {
  const root = path.join(repo, relative), manifest = JSON.parse(fs.readFileSync(path.join(root, 'fixture-manifest.json')));
  assert.equal(manifest.schema, 'teaql.retained-example-fixture/v1');
  assert.equal(manifest.evaluatedScope, 'dotnet-lib-core'); assert.equal(manifest.evaluationErrors, 0);
  const model = path.resolve(repo, manifest.modelSource); assert(model.startsWith(repo + path.sep));
  assert.equal(sha(fs.readFileSync(model)), manifest.modelSha256, 'model changed: ' + relative);
  const generated = Object.entries(manifest.files); assert(generated.length > 0);
  for (const [file, expected] of generated) {
    assert(!path.isAbsolute(file) && !file.split(/[\\/]/).includes('..'));
    const source = path.join(root, file);
    assert(fs.lstatSync(source).isFile() && !fs.lstatSync(source).isSymbolicLink());
    assert.equal(sha(fs.readFileSync(source)), expected, 'generated library changed: ' + relative + '/' + file);
    files++;
  }
}
console.log('PASS .NET retained validated generation: ' + roots.length + ' libraries, ' + files + ' byte-exact files and original model hashes');
