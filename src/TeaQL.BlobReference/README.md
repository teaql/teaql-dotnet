# Optional Blob reference module

`TeaQL.BlobReference` is an explicit, passive module. The host constructs a
`SqliteBlobCatalog`, `SqliteBlobUploadService`, read and binding authorizers,
then installs `BlobReferenceModule` into its `UserContext` and calls
`ReadyAsync` before serving requests. Storage and schema installation remain
host responsibilities.

The host may also construct `SqliteBlobMaintenanceService` with a current
scope/permission policy and pass it as `maintenance:` to `BlobReferenceModule`.
It is passive until the host explicitly calls `RunBatchAsync`. The bounded
service collects expired incomplete uploads and policy-backed completed,
unbound server-stream objects after their retention deadlines. Formerly bound
objects use the final-unbind time and the longest accepted duration/minimum
floor. Active bindings, unknown-policy legacy rows and direct uploads are
excluded. An explicit catalog migration adds the unique object-key index and
nullable time/policy fields; historical rows keep unknown facts NULL. The
service claims candidates in SQLite's physical write transaction,
pins the observed ETag, deletes through a version-conditional S3 or File
provider API, and retries a failed final catalog acknowledgment. File storage
uses a persistent per-object OS lock for manifest updates and conditional
deletion across processes. Its ETag includes an immutable generation token,
so a same-byte replacement cannot match an older version. The matching
current File generation is removed. The host may separately call
`FileObjectStorage.SweepOldGenerationsAsync(key)` for a known key to remove
inaccessible older immutable data generations. This sweep waits for a
persistent writer lock before taking the manifest lock and preserves the
current generation; all processes sharing the private root must first run a
writer-lock-capable version. Scheduling and retry remain host responsibilities.
Keep Blob object keys unique and immutable; direct-upload
quiescence needs separate implementation.

For retention-aware uploads, the host can implement
`IBlobUploadPolicyAuthorization.CurrentPolicyOnLeaseAsync`. The service
snapshots its immutable `BlobRetentionPolicy` on the physical begin-upload
SQLite lease; server-stream upload rejects direct mode. Confirmation records
the completion time in the same catalog transaction, and replay does not
change the accepted snapshot. New graph bindings require a current target
policy through `IBlobBindingPolicyAuthorization` on the graph SQL lease.
They reject expired or unknown-policy sources and accumulate the maximum
accepted unreferenced duration and minimum retention floor. Final unbind
records its own time while the catalog row remains `available`; rebind clears
that time. Shared policy vectors are pinned by SHA-256 in the SQLite tests.

For JPEG/PNG uploads, pass `new StbImageBlobContentVerifier()` to
`SqliteBlobUploadService`. It accepts only `jpg`, `jpeg` and `png`. The host's
`BlobFieldSpec` must separately allow those extensions. Input bytes, decoded
RGBA output and concurrent verifications have explicit constructor limits.
The verifier consumes the whole source, checks complete JPEG/PNG containers
and PNG chunk CRCs, preflights dimensions, fully decodes pixels, and returns
the original-byte SHA-256 plus actual MIME type. A failed validation leaves
the upload unconfirmed. This is an in-process parser with resource bounds,
not a separate CPU-isolated worker.

The StbImageSharp dependency lives only in this optional project; core and
runtime projects do not depend on it. Other allowed Blob formats still need
separate trusted production verifiers. The test-only PDF verifier in the
SQLite/S3 suites is not a production parser.
