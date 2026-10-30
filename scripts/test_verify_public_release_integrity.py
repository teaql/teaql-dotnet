#!/usr/bin/env python3
"""Exercise signed public NuGet payload checks without a network write."""

import importlib.util
from pathlib import Path
import tempfile
import unittest
import warnings
from urllib.error import HTTPError
from zipfile import ZipFile


SCRIPT = Path(__file__).with_name("verify-public-release-integrity.py")
SPEC = importlib.util.spec_from_file_location("teaql_nuget_integrity", SCRIPT)
MODULE = importlib.util.module_from_spec(SPEC)
SPEC.loader.exec_module(MODULE)
BASE = "https://api.nuget.org/v3-flatcontainer/"


class NuGetIntegrityTests(unittest.TestCase):
    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory(prefix="teaql-nuget-integrity-")
        self.work = Path(self.temporary.name)
        self.local = self.work / "TeaQL.Core.9.8.7.nupkg"
        self.remote = self.work / "remote.nupkg"
        self.make_package(self.local, signed=False)
        self.make_package(self.remote, signed=True)

    def tearDown(self):
        self.temporary.cleanup()

    def make_package(self, path, signed, payload=b"exact DLL bytes"):
        with ZipFile(path, "w") as archive:
            archive.writestr("TeaQL.Core.nuspec", b"<package/>")
            archive.writestr("lib/net8.0/TeaQL.Core.dll", payload)
            archive.writestr("[Content_Types].xml", b"<Types/>")
            if signed:
                archive.writestr(".signature.p7s", b"mock signature")

    def test_unique_https_base_from_service_index(self):
        self.assertEqual(MODULE.package_base(lambda _: {"resources": [
            {"@id": BASE, "@type": ["PackageBaseAddress/3.0.0"]}]}), BASE)

    def test_malformed_or_unsafe_base_rejected(self):
        for resources in (None, [{"@type": None}],
                          [{"@id": "http://example.invalid/", "@type": "PackageBaseAddress/3.0.0"}]):
            with self.subTest(resources=resources), self.assertRaises(ValueError):
                MODULE.package_base(lambda _: {"resources": resources})

    def test_repository_signed_payload_matches_unsigned_candidate(self):
        checked = []
        MODULE.verify_payload(self.local, self.remote, lambda path: checked.append(path))
        self.assertEqual(checked, [self.remote])

    def test_changed_public_dll_rejected(self):
        self.make_package(self.remote, signed=True, payload=b"changed DLL bytes")
        with self.assertRaisesRegex(ValueError, "payload differs"):
            MODULE.verify_payload(self.local, self.remote, lambda _: None)

    def test_unsigned_public_package_rejected(self):
        self.make_package(self.remote, signed=False)
        with self.assertRaisesRegex(ValueError, "signature entry"):
            MODULE.verify_payload(self.local, self.remote, lambda _: None)

    def test_duplicate_zip_entry_rejected(self):
        with warnings.catch_warnings():
            warnings.simplefilter("ignore", UserWarning)
            with ZipFile(self.remote, "a") as archive:
                archive.writestr("TeaQL.Core.nuspec", b"duplicate")
        with self.assertRaisesRegex(ValueError, "duplicate ZIP entries"):
            MODULE.verify_payload(self.local, self.remote, lambda _: None)

    def test_all_visible_public_candidates(self):
        checked = []
        files = {"TeaQL.Core": self.local}
        def download(url, maximum):
            checked.append((url, maximum))
            return self.remote.read_bytes()
        MODULE.require_published(files, "9.8.7", BASE, download,
                                 lambda _: None, lambda _: None, attempts=2)
        self.assertEqual(checked[0][0],
            BASE + "teaql.core/9.8.7/teaql.core.9.8.7.nupkg")

    def test_delayed_visibility_retries_only_missing_package(self):
        calls = []
        def download(url, maximum):
            calls.append(url)
            if len(calls) == 1:
                raise HTTPError(url, 404, "missing", None, None)
            return self.remote.read_bytes()
        MODULE.require_published({"TeaQL.Core": self.local}, "9.8.7", BASE,
                                 download, lambda _: None, lambda _: None, attempts=2)
        self.assertEqual(len(calls), 2)

    def test_persistent_absence_fails_closed(self):
        def missing(url, maximum):
            raise HTTPError(url, 404, "missing", None, None)
        with self.assertRaisesRegex(RuntimeError, "not visible after retries"):
            MODULE.require_published({"TeaQL.Core": self.local}, "9.8.7", BASE,
                                     missing, lambda _: None, lambda _: None, attempts=2)

    def test_other_http_error_fails_closed(self):
        def outage(url, maximum):
            raise HTTPError(url, 503, "unavailable", None, None)
        with self.assertRaisesRegex(RuntimeError, "HTTP 503"):
            MODULE.require_published({"TeaQL.Core": self.local}, "9.8.7", BASE,
                                     outage, lambda _: None, lambda _: None, attempts=2)


if __name__ == "__main__":
    unittest.main(verbosity=2)
