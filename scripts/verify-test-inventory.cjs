#!/usr/bin/env node
'use strict';
const fs = require('node:fs'), path = require('node:path'), assert = require('node:assert/strict');

function inspect(root) {
  const solution = fs.readFileSync(path.join(root, 'TeaQL.sln'), 'utf8');
  const projects = new Map([...solution.matchAll(/^Project\("[^"]+"\) = "[^"]+", "([^"]+\.csproj)", "(\{[^}]+\})"/gm)]
    .map(match => [match[1].replaceAll('\\', '/'), match[2]]));
  const discovered = [];
  function visit(directory) {
    for (const entry of fs.readdirSync(directory, {withFileTypes: true})) {
      if (entry.isSymbolicLink() || ['bin', 'obj', '.git'].includes(entry.name)) continue;
      const absolute = path.join(directory, entry.name);
      if (entry.isDirectory()) visit(absolute);
      else if (entry.isFile() && entry.name.endsWith('.csproj')
        && (entry.name.endsWith('Tests.csproj')
          || /<IsTestProject>\s*true\s*<\/IsTestProject>/.test(fs.readFileSync(absolute, 'utf8'))))
        discovered.push(path.relative(root, absolute).split(path.sep).join('/'));
    }
  }
  for (const directory of ['src', 'tests']) if (fs.existsSync(path.join(root, directory))) visit(path.join(root, directory));
  assert(discovered.length > 0, 'no test projects discovered');
  for (const file of discovered.sort()) {
    const id = projects.get(file);
    assert(id, 'test project omitted from TeaQL.sln: ' + file);
    for (const configuration of ['Debug', 'Release'])
      assert(solution.includes(id + '.' + configuration + '|Any CPU.Build.0 = ' + configuration + '|Any CPU'),
        'test project disabled for ' + configuration + ': ' + file);
  }
  return discovered;
}
if (require.main === module) {
  const projects = inspect(path.resolve(__dirname, '..'));
  console.log('PASS: .NET solution contains and enables all ' + projects.length + ' test projects');
}
module.exports = {inspect};
