#!/usr/bin/env python3
"""Run four isolated apps from the exact local NuGet candidate bytes."""

import atexit
import json
import os
import re
import shutil
import subprocess
import sys
import tempfile
from pathlib import Path
from shutil import copy2
from xml.etree import ElementTree as ET
from zipfile import ZipFile

if len(sys.argv) != 3:
    raise SystemExit('Usage: verify-optional-reference-consumers.py <source-root> <candidates>')
source, candidates = map(Path, sys.argv[1:])
version = ET.parse(source / 'Directory.Build.props').findtext('.//Version')
if not version or not candidates.is_dir():
    raise SystemExit('Source version or candidate directory is missing')
root = Path(tempfile.mkdtemp(prefix='teaql-dotnet-reference-consumers-'))
atexit.register(shutil.rmtree, root, ignore_errors=True)
for name in ('TeaQL.Core', 'TeaQL.DataService', 'TeaQL.Runtime', 'TeaQL.Sql',
             'TeaQL.BlobReference', 'TeaQL.SecureDocumentReference'):
    package = candidates / f'{name}.{version}.nupkg'
    if not package.is_file():
        raise SystemExit(f'Missing exact candidate: {package.name}')
    copy2(package, root / package.name)
variants = {
    'base': ('TeaQL.Runtime',),
    'secure-only': ('TeaQL.Runtime', 'TeaQL.SecureDocumentReference'),
    'blob-only': ('TeaQL.Runtime', 'TeaQL.BlobReference'),
    'combined': ('TeaQL.Runtime', 'TeaQL.BlobReference',
                 'TeaQL.SecureDocumentReference'),
}
candidate_dlls = {}
candidate_packages = sorted(root.glob('TeaQL.*.nupkg'))
if len(candidate_packages) != 6:
    raise SystemExit(f'Expected six TeaQL candidate packages, found {len(candidate_packages)}')
for package in candidate_packages:
    name = package.stem.rsplit('.', 3)[0]
    with ZipFile(package) as archive:
        dll = f'lib/net8.0/{name}.dll'
        candidate_dlls[name] = archive.read(dll)

# Keep restore in a fresh package cache. Copy the transitive third-party
# packages already restored for the source build into this local feed.
global_line = subprocess.check_output(
    ['dotnet', 'nuget', 'locals', 'global-packages', '--list'], text=True).strip()
if not global_line.startswith('global-packages: '):
    raise SystemExit('Cannot locate the source build NuGet cache')
global_packages = Path(global_line.split(': ', 1)[1])
seen = set()
queue = list(candidate_packages)
while queue:
    package = queue.pop()
    with ZipFile(package) as archive:
        manifest_name, = [name for name in archive.namelist()
                          if name.endswith('.nuspec')]
        manifest = ET.fromstring(archive.read(manifest_name))
        dependency_root = manifest.find('.//{*}dependencies')
        groups = [] if dependency_root is None else dependency_root.findall('{*}group')
        if groups:
            compatible = ('net8.0', '.netcoreapp8.0', '.netstandard2.1',
                          '.netstandard2.0')
            selected = next((group for target in compatible for group in groups
                             if group.attrib.get('targetFramework', '').lower() == target), None)
            if selected is None:
                raise SystemExit(f'No net8.0 dependency group in {package.name}')
            dependencies = selected.findall('{*}dependency')
        else:
            dependencies = [] if dependency_root is None else dependency_root.findall('{*}dependency')
    for dependency in dependencies:
        name, spec = dependency.attrib['id'], dependency.attrib['version']
        if not re.fullmatch(r'[A-Za-z0-9_.-]+', name):
            raise SystemExit(f'Invalid dependency ID: {name}')
        match = re.search(r'\d+(?:\.\d+){1,3}(?:-[A-Za-z0-9.-]+)?', spec)
        if not match:
            raise SystemExit(f'Unpinned dependency: {name} {spec}')
        dependency_version = match.group(0)
        key = (name.lower(), dependency_version.lower())
        if key in seen:
            continue
        seen.add(key)
        if name.startswith('TeaQL.'):
            if not (root / f'{name}.{dependency_version}.nupkg').is_file():
                raise SystemExit(f'Missing local TeaQL candidate: {name} {spec}')
            continue
        cached = (global_packages / name.lower() / dependency_version.lower()
                  / f'{name.lower()}.{dependency_version.lower()}.nupkg')
        if not cached.is_file():
            raise SystemExit(f'Missing restored dependency: {name} {spec}')
        local = root / cached.name
        copy2(cached, local)
        queue.append(local)

environment = os.environ.copy()
environment['NUGET_PACKAGES'] = str(root / 'installed')
environment['DOTNET_CLI_HOME'] = str(root / 'dotnet-home')

for variant, packages in variants.items():
    consumer = root / variant
    consumer.mkdir()
    references = ''.join(f'<PackageReference Include="{name}" Version="{version}" />'
                         for name in packages)
    (consumer / 'Consumer.csproj').write_text(
        '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup>'
        '<OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework>'
        '<Nullable>enable</Nullable></PropertyGroup><ItemGroup>'
        f'{references}</ItemGroup></Project>', encoding='utf-8')
    imports = ['using System;', 'using TeaQL.Runtime;']
    type_names = ['typeof(RuntimeModule)']
    if 'TeaQL.BlobReference' in packages:
        imports.append('using TeaQL.BlobReference;')
        type_names.append('typeof(BlobReferenceModule)')
    if 'TeaQL.SecureDocumentReference' in packages:
        imports.append('using TeaQL.SecureDocumentReference;')
        type_names.append('typeof(SecureDocumentReferenceModule)')
    type_expression = ' + " " + '.join(
        name + '.Assembly.GetName().Name' for name in type_names)
    imports.append(f'Console.WriteLine("PASS {variant} " + {type_expression});')
    (consumer / 'Program.cs').write_text('\n'.join(imports) + '\n', encoding='utf-8')

    def run(*args):
        process = subprocess.run(args, text=True, capture_output=True,
                                 env=environment)
        if process.returncode:
            raise SystemExit(f'{variant}: {args[1]} failed: '
                             + (process.stdout + process.stderr)[-1200:])
        return process.stdout

    project = str(consumer / 'Consumer.csproj')
    run('dotnet', 'restore', project, '--source', str(root),
        '--ignore-failed-sources', '-p:NuGetAudit=false', '--nologo', '-v:q')
    output = consumer / 'out'
    run('dotnet', 'publish', project, '-c', 'Release', '--no-restore',
        '-p:UseSharedCompilation=false', '-o', str(output), '--nologo', '-v:q')
    result = run('dotnet', str(output / 'Consumer.dll'))
    if f'PASS {variant} ' not in result:
        raise SystemExit(f'{variant}: consumer did not run')
    libraries = json.loads((output / 'Consumer.deps.json').read_text(encoding='utf-8'))['libraries']
    for optional in ('TeaQL.BlobReference', 'TeaQL.SecureDocumentReference'):
        expected = optional in packages
        actual = (output / f'{optional}.dll').is_file()
        recorded = any(name.startswith(f'{optional}/') for name in libraries)
        if actual != expected or recorded != expected:
            raise SystemExit(f'{variant}: {optional} optional package presence mismatch')
    for name, expected_bytes in candidate_dlls.items():
        installed = output / f'{name}.dll'
        if installed.is_file() and installed.read_bytes() != expected_bytes:
            raise SystemExit(f'{variant}: {name} does not match the local candidate')
    print(f'PASS {variant} installed candidate consumer')
