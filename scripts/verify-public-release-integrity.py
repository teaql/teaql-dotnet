#!/usr/bin/env python3
"""Verify every public NuGet payload against its exact local release candidate."""

from hashlib import sha256
import importlib.util
from pathlib import Path
import subprocess
import sys
import tempfile
import time
from urllib.error import HTTPError, URLError
from urllib.parse import urlparse
from urllib.request import Request, urlopen
from zipfile import BadZipFile, ZipFile


ROOT = Path(__file__).resolve().parents[1]
SOURCE = Path(__file__).with_name("verify-public-release-absence.py")
SPEC = importlib.util.spec_from_file_location("teaql_nuget_absence", SOURCE)
ABSENCE = importlib.util.module_from_spec(SPEC)
SPEC.loader.exec_module(ABSENCE)


def package_base(get_json):
    resources = get_json(ABSENCE.SERVICE_INDEX).get("resources", [])
    if not isinstance(resources, list) or not all(isinstance(row, dict) for row in resources):
        raise ValueError("NuGet service index resources are malformed")
    def types(row):
        value = row.get("@type")
        return [value] if isinstance(value, str) else value if isinstance(value, list) else []
    bases = [row.get("@id") for row in resources
             if "PackageBaseAddress/3.0.0" in types(row)]
    if len(bases) != 1 or not isinstance(bases[0], str):
        raise ValueError("NuGet service index has no unique package base address")
    parsed = urlparse(bases[0])
    if parsed.scheme != "https" or not parsed.netloc or parsed.username or parsed.query or parsed.fragment:
        raise ValueError("NuGet package base address is not a plain HTTPS URL")
    return bases[0].rstrip("/") + "/"


def candidate_files(directory):
    names, version = ABSENCE.package_ids(directory)
    if len(names) != 15:
        raise ValueError("Expected all 15 release candidates")
    files = {name: directory / f"{name}.{version}.nupkg" for name in names}
    if any(not path.is_file() or path.is_symlink() for path in files.values()):
        raise ValueError("Release candidate is missing or symlinked")
    return files, version


def fetch_package(url, maximum_bytes):
    request = Request(url, headers={"Accept": "application/octet-stream",
                                    "Cache-Control": "no-cache"})
    with urlopen(request, timeout=30) as response:
        if response.status != 200:
            raise RuntimeError(f"Unexpected NuGet package HTTP {response.status}")
        data = response.read(maximum_bytes + 1)
    if len(data) > maximum_bytes:
        raise RuntimeError("Public NuGet package is unexpectedly large")
    return data


def verify_repository_signature(path):
    result = subprocess.run(["dotnet", "nuget", "verify", "--all", str(path)],
                            capture_output=True, text=True, timeout=90)
    if result.returncode != 0:
        raise RuntimeError("NuGet repository signature verification failed: " + path.name)


def payload_hashes(path):
    with ZipFile(path) as archive:
        names = archive.namelist()
        if len(names) != len(set(names)):
            raise ValueError("NuGet package has duplicate ZIP entries")
        return {name: sha256(archive.read(name)).hexdigest()
                for name in names if name != ".signature.p7s"}


def verify_payload(local, remote, signature_check=verify_repository_signature):
    with ZipFile(remote) as archive:
        if archive.namelist().count(".signature.p7s") != 1:
            raise ValueError("Public NuGet package lacks a unique repository signature entry")
    signature_check(remote)
    if payload_hashes(local) != payload_hashes(remote):
        raise ValueError("Public NuGet package payload differs from local candidate: " + local.name)


def require_published(files, version, base, download=fetch_package,
                      signature_check=verify_repository_signature, sleeper=time.sleep,
                      attempts=30):
    pending = dict(files)
    with tempfile.TemporaryDirectory(prefix="teaql-public-nuget-integrity-") as work:
        scratch = Path(work)
        for attempt in range(attempts):
            for name, local in list(pending.items()):
                lower_id, lower_version = name.lower(), version.lower()
                url = (f"{base}{lower_id}/{lower_version}/"
                       f"{lower_id}.{lower_version}.nupkg")
                try:
                    remote_bytes = download(url, local.stat().st_size + 2_000_000)
                except HTTPError as exc:
                    if exc.code != 404:
                        exc.close()
                        raise RuntimeError(f"NuGet package retrieval failed: {name} HTTP {exc.code}") from exc
                    exc.close()
                    continue
                except (URLError, TimeoutError) as exc:
                    raise RuntimeError(f"NuGet package retrieval failed: {name}") from exc
                remote = scratch / local.name
                remote.write_bytes(remote_bytes)
                try:
                    verify_payload(local, remote, signature_check)
                finally:
                    remote.unlink(missing_ok=True)
                del pending[name]
                print(f"PASS exact signed public NuGet payload: {name} {version}", flush=True)
            if not pending:
                return
            if attempt < attempts - 1:
                sleeper(30)
    raise RuntimeError("Public NuGet packages not visible after retries: " + ", ".join(pending))


def main(argv):
    if len(argv) != 2 or not Path(argv[1]).is_dir():
        raise SystemExit("Usage: verify-public-release-integrity.py <verified-candidate-directory>")
    files, version = candidate_files(Path(argv[1]))
    base = package_base(ABSENCE.fetch_json)
    require_published(files, version, base)
    print(f"PASS all {len(files)} exact signed public NuGet payloads at {version}")


if __name__ == "__main__":
    try:
        main(sys.argv)
    except (OSError, ValueError, RuntimeError, BadZipFile, subprocess.TimeoutExpired) as exc:
        raise SystemExit(str(exc)) from exc
