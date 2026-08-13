using System;
using System.Collections.Generic;
using Xunit;
using TeaQL.DataService;
using TeaQL.Core;
using Record = TeaQL.Core.Record;

namespace TeaQL.DataService.Tests
{
    public class DataServiceModelsTests
    {
        [Fact]
        public void QueryRequest_Properties_Work()
        {
            var query = new SelectQuery();
            var trace = new List<TraceNode> { new TraceNode("test", null, "comment") };
            var req = new QueryRequest { Query = query, TraceChain = trace, Comment = "comment" };

            Assert.Same(query, req.Query);
            Assert.Same(trace, req.TraceChain);
            Assert.Equal("comment", req.Comment);
        }

        [Fact]
        public void QueryResult_Properties_Work()
        {
            var rows = new List<Record>();
            var meta = new ExecutionMetadata();
            var res = new QueryResult { Rows = rows, Metadata = meta };

            Assert.Same(rows, res.Rows);
            Assert.Same(meta, res.Metadata);
        }

        [Fact]
        public void InsertMutationRequest_Properties_Work()
        {
            var trace = new List<TraceNode> { new TraceNode("test", null, "c1") };
            var cmd = new InsertCommand { TraceChain = trace };
            var req = new InsertMutationRequest(cmd);

            Assert.Same(cmd, req.Command);
            Assert.Same(trace, req.TraceChain);
            Assert.Equal("c1", req.Comment);
        }

        [Fact]
        public void UpdateMutationRequest_Properties_Work()
        {
            var trace = new List<TraceNode> { new TraceNode("test", null, "c2") };
            var cmd = new UpdateCommand { TraceChain = trace };
            var req = new UpdateMutationRequest(cmd);

            Assert.Same(cmd, req.Command);
            Assert.Same(trace, req.TraceChain);
            Assert.Equal("c2", req.Comment);
        }

        [Fact]
        public void DeleteMutationRequest_Properties_Work()
        {
            var trace = new List<TraceNode> { new TraceNode("test", null, "c3") };
            var cmd = new DeleteCommand { TraceChain = trace };
            var req = new DeleteMutationRequest(cmd);

            Assert.Same(cmd, req.Command);
            Assert.Same(trace, req.TraceChain);
            Assert.Equal("c3", req.Comment);
        }

        [Fact]
        public void RecoverMutationRequest_Properties_Work()
        {
            var trace = new List<TraceNode> { new TraceNode("test", null, "c4") };
            var cmd = new RecoverCommand { TraceChain = trace };
            var req = new RecoverMutationRequest(cmd);

            Assert.Same(cmd, req.Command);
            Assert.Same(trace, req.TraceChain);
            Assert.Equal("c4", req.Comment);
        }

        [Fact]
        public void BatchMutationRequest_Properties_Work()
        {
            var innerReq = new InsertMutationRequest(new InsertCommand());
            var reqs = new List<MutationRequest> { innerReq };
            var batchReq = new BatchMutationRequest(reqs);

            Assert.Same(reqs, batchReq.Requests);
            Assert.Empty(batchReq.TraceChain);
            Assert.Null(batchReq.Comment);
        }

        [Fact]
        public void DataServiceCapabilities_Properties_Work()
        {
            var cap = new DataServiceCapabilities
            {
                Query = true,
                Mutation = false,
                Transaction = true,
                Schema = false,
                IdGeneration = true,
                BatchMutation = false,
                Returning = true
            };

            Assert.True(cap.Query);
            Assert.False(cap.Mutation);
            Assert.True(cap.Transaction);
            Assert.False(cap.Schema);
            Assert.True(cap.IdGeneration);
            Assert.False(cap.BatchMutation);
            Assert.True(cap.Returning);
        }

        [Fact]
        public void ExecutionMetadata_Properties_Work()
        {
            var meta = new ExecutionMetadata
            {
                Backend = "pg",
                Operation = DataServiceOperation.Insert,
                StartedAt = DateTimeOffset.MinValue,
                EndedAt = DateTimeOffset.MaxValue,
                AffectedRows = 10,
                ResultCount = 5,
                Comment = "hello",
                BackendRequestId = "req-1",
                DebugQuery = "select 1"
            };

            Assert.Equal("pg", meta.Backend);
            Assert.Equal(DataServiceOperation.Insert, meta.Operation);
            Assert.Equal(10ul, meta.AffectedRows);
            Assert.Equal(5, meta.ResultCount);
            Assert.Equal("hello", meta.Comment);
            Assert.Equal("req-1", meta.BackendRequestId);
            Assert.Equal("select 1", meta.DebugQuery);
        }
    }
}
