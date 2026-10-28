#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
pack_root="$(mktemp -d /tmp/teaql-dotnet-reference-packages.XXXXXX)"
trap 'rm -rf -- "$pack_root"' EXIT

for project in TeaQL.BlobReference TeaQL.SecureDocumentReference; do
  dotnet pack "$repo_root/src/$project/$project.csproj" -c Release \
    -p:UseSharedCompilation=false -o "$pack_root" --nologo -v:q
done

python3 - "$repo_root" "$pack_root" <<'PY'
from hashlib import sha256
from pathlib import Path
import sys
from xml.etree import ElementTree as ET
from zipfile import ZipFile

source, packed = map(Path, sys.argv[1:])
version = ET.parse(source / 'Directory.Build.props').findtext('.//Version')
if not version:
    raise SystemExit('Source package version is missing')
expected = {
    'TeaQL.BlobReference': {'TeaQL.Core', 'TeaQL.DataService',
                            'TeaQL.Runtime', 'TeaQL.Sql', 'StbImageSharp'},
    'TeaQL.SecureDocumentReference': {'TeaQL.Core', 'TeaQL.Runtime',
                                      'TeaQL.Sql'},
}
packages = sorted(packed.glob('*.nupkg'))
if len(packages) != len(expected):
    raise SystemExit(f'Expected two optional NuGet packages, found {len(packages)}')
for package in packages:
    with ZipFile(package) as archive:
        manifest_name, = [name for name in archive.namelist()
                          if name.endswith('.nuspec')]
        manifest = ET.fromstring(archive.read(manifest_name))
        name = manifest.findtext('.//{*}id')
        actual_version = manifest.findtext('.//{*}version')
        if name not in expected or actual_version != version:
            raise SystemExit(f'Unexpected package identity: {name} {actual_version}')
        dependencies = {entry.attrib.get('id') for entry in
                        manifest.findall('.//{*}dependency')}
        if dependencies != expected[name]:
            raise SystemExit(f'Unexpected {name} dependencies: {dependencies}')
        libraries = [entry for entry in archive.namelist()
                     if entry.startswith('lib/') and entry.endswith('.dll')]
        dll = f'lib/net8.0/{name}.dll'
        if libraries != [dll]:
            raise SystemExit(f'Unexpected {name} DLLs: {libraries}')
        source_dll = source / 'src' / name / 'bin' / 'Release' / 'net8.0' / f'{name}.dll'
        if archive.read(dll) != source_dll.read_bytes():
            raise SystemExit(f'{name} packed DLL differs from the built source')
    print(f'PASS {name} {version} isolated NuGet candidate '
          f'sha256={sha256(package.read_bytes()).hexdigest()}')
PY
