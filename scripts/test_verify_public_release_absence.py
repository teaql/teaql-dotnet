#!/usr/bin/env python3
"""Exercise exact NuGet package/version preflight without a network write."""

import importlib.util
from pathlib import Path
import tempfile
import unittest
from urllib.error import HTTPError
from zipfile import ZipFile


path = Path(__file__).with_name("verify-public-release-absence.py")
spec = importlib.util.spec_from_file_location("verify_public_release_absence", path)
subject = importlib.util.module_from_spec(spec)
spec.loader.exec_module(subject)


class NuGetAbsenceTests(unittest.TestCase):
    def test_package_ids_come_from_exact_candidate_manifests(self):
        with tempfile.TemporaryDirectory(prefix="teaql-nuget-absence-test-") as work:
            directory = Path(work)
            for package_id in ("TeaQL.Core", "TeaQL.BlobReference"):
                package = directory / (package_id + ".0.3.1.nupkg")
                xml = ("<package><metadata><id>" + package_id +
                       "</id><version>0.3.1</version></metadata></package>")
                with ZipFile(package, "w") as archive:
                    archive.writestr(package_id + ".nuspec", xml)
            self.assertEqual(subject.package_ids(directory),
                             (["TeaQL.BlobReference", "TeaQL.Core"], "0.3.1"))

    def test_unlisted_versions_are_checked_with_the_flat_container(self):
        requests = []
        base = "https://api.nuget.org/v3-flatcontainer/"
        def get(url):
            requests.append(url)
            if url == subject.SERVICE_INDEX:
                return {"resources": [{"@id": base, "@type": "PackageBaseAddress/3.0.0"}]}
            return {"versions": ["0.3.0"]}
        subject.assert_unused(["TeaQL.BlobReference", "TeaQL.Core"], "0.3.1", get)
        self.assertEqual(requests, [subject.SERVICE_INDEX,
            base + "teaql.blobreference/index.json", base + "teaql.core/index.json"])

    def test_existing_package_version_rejects_the_entire_release(self):
        base = "https://api.nuget.org/v3-flatcontainer/"
        def get(url):
            if url == subject.SERVICE_INDEX:
                return {"resources": [{"@id": base, "@type": ["PackageBaseAddress/3.0.0"]}]}
            return {"versions": ["0.3.1"] if "teaql.core" in url else []}
        with self.assertRaisesRegex(ValueError, "TeaQL.Core"):
            subject.assert_unused(["TeaQL.BlobReference", "TeaQL.Core"], "0.3.1", get)

    def test_missing_id_is_new_but_registry_errors_fail_closed(self):
        base = "https://api.nuget.org/v3-flatcontainer/"
        def get(url):
            if url == subject.SERVICE_INDEX:
                return {"resources": [{"@id": base, "@type": "PackageBaseAddress/3.0.0"}]}
            raise HTTPError(url, 404, "missing", {}, None)
        subject.assert_unused(["TeaQL.New"], "0.3.1", get)
        def outage(url):
            if url == subject.SERVICE_INDEX:
                return {"resources": [{"@id": base, "@type": "PackageBaseAddress/3.0.0"}]}
            raise HTTPError(url, 503, "unavailable", {}, None)
        with self.assertRaises(HTTPError):
            subject.assert_unused(["TeaQL.New"], "0.3.1", outage)

    def test_malformed_service_or_version_index_fails_closed(self):
        with self.assertRaisesRegex(ValueError, "base address"):
            subject.assert_unused(["TeaQL.Core"], "0.3.1", lambda _: {"resources": []})
        def malformed(url):
            if url == subject.SERVICE_INDEX:
                return {"resources": [{"@id": "https://api.nuget.org/v3-flatcontainer/",
                                      "@type": "PackageBaseAddress/3.0.0"}]}
            return {"versions": None}
        with self.assertRaisesRegex(ValueError, "malformed"):
            subject.assert_unused(["TeaQL.Core"], "0.3.1", malformed)


if __name__ == "__main__":
    unittest.main(verbosity=2)
