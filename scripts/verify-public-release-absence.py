#!/usr/bin/env python3
"""Refuse a NuGet release if any exact candidate version already exists."""

import json
from pathlib import Path
import re
import sys
from urllib.error import HTTPError
from urllib.parse import urlparse
from urllib.request import Request, urlopen
from xml.etree import ElementTree as ET
from zipfile import ZipFile


SERVICE_INDEX = "https://api.nuget.org/v3/index.json"
ROOT = Path(__file__).resolve().parents[1]


def package_ids(directory):
    version = ET.parse(ROOT / "Directory.Build.props").findtext(".//Version")
    if not version or re.fullmatch(r"[0-9]+\.[0-9]+\.[0-9]+(?:-[0-9A-Za-z.-]+)?", version) is None:
        raise ValueError("release source must have a normalized package version")
    packages = sorted(directory.glob("*.nupkg"))
    if not packages:
        raise ValueError("no release candidates were packed")
    names = []
    for package in packages:
        with ZipFile(package) as archive:
            manifests = [name for name in archive.namelist() if name.endswith(".nuspec")]
            if len(manifests) != 1:
                raise ValueError("candidate has no unique NuGet manifest: " + package.name)
            manifest = ET.fromstring(archive.read(manifests[0]))
        package_id = manifest.findtext("./{*}metadata/{*}id")
        packaged_version = manifest.findtext("./{*}metadata/{*}version")
        if not package_id or packaged_version != version:
            raise ValueError("candidate package identity/version mismatch: " + package.name)
        if package.name.lower() != (package_id + "." + version + ".nupkg").lower():
            raise ValueError("candidate filename/manifest mismatch: " + package.name)
        names.append(package_id)
    if len(set(name.lower() for name in names)) != len(names):
        raise ValueError("duplicate candidate package ID")
    return sorted(names), version


def fetch_json(url):
    request = Request(url, headers={"Accept": "application/json", "Cache-Control": "no-cache"})
    with urlopen(request, timeout=20) as response:
        if response.status != 200:
            raise ValueError("NuGet metadata did not return HTTP 200")
        return json.load(response)


def assert_unused(package_names, version, get_json):
    resources = get_json(SERVICE_INDEX).get("resources", [])
    bases = [row.get("@id") for row in resources
             if "PackageBaseAddress/3.0.0" in ([row.get("@type")] if isinstance(row.get("@type"), str)
                                                 else row.get("@type", []))]
    if len(bases) != 1 or not isinstance(bases[0], str) or urlparse(bases[0]).scheme != "https":
        raise ValueError("NuGet service index has no unique HTTPS package base address")
    base = bases[0].rstrip("/") + "/"
    occupied = []
    for package_id in package_names:
        try:
            metadata = get_json(base + package_id.lower() + "/index.json")
        except HTTPError as error:
            if error.code == 404:
                continue
            raise
        versions = metadata.get("versions")
        if not isinstance(versions, list) or not all(isinstance(item, str) for item in versions):
            raise ValueError("malformed NuGet version index for " + package_id)
        if version.lower() in versions:
            occupied.append(package_id)
    if occupied:
        raise ValueError("NuGet version already exists for: " + ", ".join(occupied))


def main():
    if len(sys.argv) != 2 or not Path(sys.argv[1]).is_dir():
        raise SystemExit("Usage: verify-public-release-absence.py <verified-candidate-directory>")
    names, version = package_ids(Path(sys.argv[1]))
    assert_unused(names, version, fetch_json)
    print("PASS NuGet release versions unused: " + str(len(names)) + " packages at " + version)


if __name__ == "__main__":
    main()
