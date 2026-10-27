# Optional Blob reference module

`TeaQL.BlobReference` is an explicit, passive module. The host constructs a
`SqliteBlobCatalog`, `SqliteBlobUploadService`, read and binding authorizers,
then installs `BlobReferenceModule` into its `UserContext` and calls
`ReadyAsync` before serving requests. Storage and schema installation remain
host responsibilities.

The host may also construct `SqliteBlobMaintenanceService` with a current
scope/permission policy and pass it as `maintenance:` to `BlobReferenceModule`.
It is passive until the host explicitly calls `RunBatchAsync`. The bounded
service collects only expired, uncompleted, unbound server-stream uploads. An
explicit catalog migration adds the unique object-key index required for this
path. The service claims candidates in SQLite's physical write transaction,
pins the observed ETag, deletes through the optional S3 version-conditional
provider API, and retries a failed final catalog acknowledgment. File storage
is rejected until it has cross-process version fencing. Keep Blob object keys
unique and immutable; completed unbound and old bound retention, File fencing
and direct-upload quiescence need separate implementation.

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
