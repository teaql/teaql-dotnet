# Optional Blob reference module

`TeaQL.BlobReference` is an explicit, passive module. The host constructs a
`SqliteBlobCatalog`, `SqliteBlobUploadService`, read and binding authorizers,
then installs `BlobReferenceModule` into its `UserContext` and calls
`ReadyAsync` before serving requests. Storage and schema installation remain
host responsibilities.

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
