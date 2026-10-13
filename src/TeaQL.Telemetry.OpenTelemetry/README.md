# Standard OpenTelemetry module for .NET

This optional package enables a TeaQL-owned SDK through process configuration.
Generated Q/E and audited mutation code need no SDK initialization or manual
spans. The implementation is a local, unreleased candidate.

Add `TeaQL.Telemetry.OpenTelemetry` to the application dependencies, then set:

```bash
TEAQL_TELEMETRY_ENABLED=true
TEAQL_TELEMETRY_SERVICE_NAME=payment-service
TEAQL_TELEMETRY_OTLP_ENDPOINT=http://localhost:4318
```

Without enablement, context startup prints these three settings and does not
load the optional SDK assembly. Enabled startup prints validated effective
settings, full recording and masking. URL credentials and token query strings
are rejected without echoing them. The module does not inherit a customer's
OTLP authentication headers or change global SDK configuration.

The standard SDK exports traces, metrics, safe expanded SQL and business
TraceChain. Ordinary SQL logging switches do not disable telemetry. Each
physical statement captures its own trace/span identity before execution;
delayed completion uses that identity rather than the currently active span.
The existing runtime masking algorithm also applies during plaintext debugging.

Private ownership filtering keeps the shared .NET Activity/Meter registries
from cross-exporting other SDK instances. The logical instrumentation scope
remains `io.teaql.runtime`. The module owns only its providers and exporters;
customer SDKs remain usable after TeaQL shutdown. Native exporter resource
ownership uses the SDK's public processor extension, not reflection into its
internal properties. The [official export processor implementation](https://github.com/open-telemetry/opentelemetry-dotnet/blob/core-1.17.0/src/OpenTelemetry/BaseExportProcessor.cs)
defines that resource binding.

Trace/log admission keeps exact record identities until actual export completion.
Queued, staged and exporting records share a 2,048-record limit per signal. SDK
queues and batching remain native. Private log tickets survive the SDK's snapshot
copy and are stripped before export; they are not business IDs or correlation IDs.
`ExportQueues` reports exact admission, completion, retirement and late offers.
`ExportHealth`
reports failed spans, log records, Metric objects and queue losses.
`teaql.telemetry.export.lost`, `teaql.telemetry.export.rejected` and
`teaql.telemetry.export.response` use only signal/reason dimensions. Receiver
rejection counts spans, log records or metric data points; it is separate from
failed-export counts. Fixed notices never include exporter exceptions.
Business outcomes and durable audit delivery remain independent of export.
Flush waits for its captured accepted-record prefix, including records already
dequeued for export. Later completions cannot substitute for older records.
Shutdown closes admission, drains for five seconds and retires only unattempted
records if necessary. It cancels active owned HTTP, settles trace/log failures
before the final metric snapshot and reserves cleanup within a six-second budget.
Concurrent shutdown or flush-during-shutdown callers join the same cleanup.
Closed pipelines do not advertise new SQL correlation; late offers remain visible
without exporting invented records. Close the shared pipeline once
at application shutdown with `await context.RuntimeTelemetry.ShutdownAsync()`.

The owned HTTP boundary uses the public exporter client factory while retaining
official SDK encoding and resource binding. Each exporter receives a private
client with redirects and cookies disabled, a three-second request/body budget,
and a four-MiB streamed response limit. Unknown-length streams stop after the
first over-limit byte. Original remote bodies, headers, status descriptions and
transport exceptions never enter the SDK's global diagnostics. Customer
EventSource listeners remain active; the module does not silence them.

The HTTP boundary now validates acknowledgements using internal C# bindings
generated from official OpenTelemetry protocol definitions. It counts submitted
records in each actual serialized request, rejects malformed, negative,
above-submitted and invalid-wire responses, and distinguishes partial success
from warnings without retaining remote explanation text. Empty responses,
default partial-success messages and valid unknown fields remain compatible.
Accepted portions are not resubmitted as a whole batch.

The local gate has 703 solution passes, 36 SDK cases and all ten example groups.
Fifteen actual three-signal HTTP modes replay generated Q/E, audited seeds and
mixed graph saves on retained SQLite. Startup readback has twelve SQL logs.
Failed metrics count Metric objects; partial rejection counts data points.
The final metric export's own acknowledgement updates local diagnostics after
that snapshot was sent. Four retained-database pressure runs each execute 1,500
generated Q/E queries. Recovery releases all permits and admits the next query;
a never-acknowledging receiver closes at about 5.9 seconds, with 256 failed
attempts and 1,792 queued retirements per signal. No permits or active HTTP remain,
and all three owned transports close. Typed queries and audited graph saves still
work after telemetry shutdown. Advanced lifecycle/graph/stream and internal
artifact qualification remain open.

## Generated local example

```bash
bash examples/trace-chain/verify-standard-telemetry.sh
```

The example depends on this repository's runtime, creates a fresh retained
SQLite database, verifies audited seed/repeated-seed behavior and executes two
bounded generated Q queries with E assertions. Generated library source remains
unchanged. The root `scripts/verify-examples.sh` includes this gate.
