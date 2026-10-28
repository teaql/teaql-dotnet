#!/usr/bin/env python3
"""Fail closed on the exact NuGet package set before release authentication."""

from hashlib import sha256
from pathlib import Path
import sys
from xml.etree import ElementTree as ET
from zipfile import ZipFile


ROOT = Path(__file__).resolve().parents[1]
PACKED = Path(sys.argv[1]).resolve() if len(sys.argv) == 2 else None
if PACKED is None or not PACKED.is_dir():
    raise SystemExit("Usage: verify-release-candidates.py <existing-package-directory>")

version = ET.parse(ROOT / "Directory.Build.props").findtext(".//Version")
if not version:
    raise SystemExit("Source package version is missing")

projects = (
    "TeaQL.Core", "TeaQL.DataService", "TeaQL.Sql", "TeaQL.Runtime",
    "TeaQL.BlobReference", "TeaQL.SecureDocumentReference",
    "TeaQL.Provider.Sqlite", "TeaQL.Provider.PostgreSql", "TeaQL.Provider.MySql",
    "TeaQL.Provider.SqlServer", "TeaQL.CacheIntegration.Redis",
    "TeaQL.WebIntegration.AspNetCore", "TeaQL.TfpEndpoint",
    "TeaQL.Telemetry.OpenTelemetry",
)
expected = {f"{name}.{version}.nupkg": name for name in projects}
actual = {path.name: path for path in PACKED.glob("*.nupkg")}
if set(actual) != set(expected):
    raise SystemExit(
        f"Release package set mismatch: missing={sorted(set(expected) - set(actual))}, "
        f"unexpected={sorted(set(actual) - set(expected))}"
    )

optional_dependencies = {
    "TeaQL.BlobReference": {
        "TeaQL.Core", "TeaQL.DataService", "TeaQL.Runtime", "TeaQL.Sql",
        "StbImageSharp",
    },
    "TeaQL.SecureDocumentReference": {
        "TeaQL.Core", "TeaQL.Runtime", "TeaQL.Sql",
    },
}
for filename, name in expected.items():
    package = actual[filename]
    with ZipFile(package) as archive:
        manifest_names = [entry for entry in archive.namelist()
                          if entry.endswith(".nuspec")]
        if len(manifest_names) != 1:
            raise SystemExit(f"Unexpected manifest count in {filename}")
        manifest = ET.fromstring(archive.read(manifest_names[0]))
        if (manifest.findtext("./{*}metadata/{*}id"),
                manifest.findtext("./{*}metadata/{*}version")) != (name, version):
            raise SystemExit(f"Package identity mismatch in {filename}")
        if name not in optional_dependencies:
            continue
        dependencies = {entry.attrib.get("id") for entry in
                        manifest.findall(".//{*}dependency")}
        if dependencies != optional_dependencies[name]:
            raise SystemExit(f"Optional dependency mismatch in {filename}")
        libraries = [entry for entry in archive.namelist()
                     if entry.startswith("lib/") and entry.endswith(".dll")]
        dll = f"lib/net8.0/{name}.dll"
        if libraries != [dll]:
            raise SystemExit(f"Optional DLL isolation mismatch in {filename}")
        source_dll = ROOT / "src" / name / "bin" / "Release" / "net8.0" / f"{name}.dll"
        if archive.read(dll) != source_dll.read_bytes():
            raise SystemExit(f"Optional DLL bytes differ from source build in {filename}")
    print(f"PASS exact optional release candidate {filename} "
          f"sha256={sha256(package.read_bytes()).hexdigest()}")

print(f"PASS exact release package set: {len(expected)} candidates at {version}")
