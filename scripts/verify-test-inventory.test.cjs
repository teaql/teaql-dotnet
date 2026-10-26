'use strict';
const {test} = require('node:test'), assert = require('node:assert/strict');
const fs = require('node:fs'), os = require('node:os'), path = require('node:path');
const {inspect} = require('./verify-test-inventory.cjs');
function fixture(run) {
  const root = fs.mkdtempSync(path.join(os.tmpdir(), 'teaql-dotnet-test-inventory-'));
  try {
    const file = path.join(root, 'src/Core.Tests/Core.Tests.csproj');
    fs.mkdirSync(path.dirname(file), {recursive: true});
    fs.writeFileSync(file, '<Project><IsTestProject>true</IsTestProject></Project>');
    const declaration = 'Project("{TYPE}") = "Core.Tests", "src\\Core.Tests\\Core.Tests.csproj", "{CORE}"\nEndProject\n';
    const configurations = ['Debug', 'Release'].map(name => '{CORE}.' + name + '|Any CPU.Build.0 = ' + name + '|Any CPU').join('\n');
    fs.writeFileSync(path.join(root, 'TeaQL.sln'), declaration + configurations);
    run(root, declaration, configurations);
  } finally { fs.rmSync(root, {recursive: true}); }
}
test('the real repository inventories Core and every other native test project', () => {
  const projects = inspect(path.resolve(__dirname, '..'));
  assert(projects.includes('src/TeaQL.Core.Tests/TeaQL.Core.Tests.csproj'));
  assert(projects.includes('tests/TeaQL.Streaming.Tests/TeaQL.Streaming.Tests.csproj'));
});
test('an omitted project fails instead of returning an incomplete green suite', () => fixture((root, _, configurations) => {
  fs.writeFileSync(path.join(root, 'TeaQL.sln'), configurations);
  assert.throws(() => inspect(root), /omitted.*Core.Tests/);
}));
test('a disabled configuration fails even if the project appears in the solution', () => fixture((root, declaration, configurations) => {
  fs.writeFileSync(path.join(root, 'TeaQL.sln'), declaration + configurations.split('\n')[0]);
  assert.throws(() => inspect(root), /disabled for Release/);
}));
test('ordinary source projects are not mistaken for tests', () => fixture(root => {
  fs.writeFileSync(path.join(root, 'src/Regular.csproj'), '<Project/>');
  assert.deepEqual(inspect(root), ['src/Core.Tests/Core.Tests.csproj']);
}));
test('test SDK projects without an explicit IsTestProject flag are still discovered', () => fixture(root => {
  const file = path.join(root, 'src/Extra.Tests/Extra.Tests.csproj');
  fs.mkdirSync(path.dirname(file)); fs.writeFileSync(file, '<Project/>');
  assert.throws(() => inspect(root), /omitted.*Extra.Tests/);
}));
