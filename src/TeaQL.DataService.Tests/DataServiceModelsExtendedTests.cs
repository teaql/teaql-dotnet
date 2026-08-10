using Xunit;
using TeaQL.DataService;
using TeaQL.Core;
using Record = TeaQL.Core.Record;
using System.Collections.Generic;

namespace TeaQL.DataService.Tests
{
    public class DataServiceModelsExtendedTests
    {
        [Fact]
        public void StreamChunk_Properties_Work()
        {
            var chunk = new StreamChunk
            {
                Rows = new List<Record>(),
                ChunkIndex = 1,
                IsLast = true
            };

            Assert.Empty(chunk.Rows);
            Assert.Equal(1, chunk.ChunkIndex);
            Assert.True(chunk.IsLast);
        }

        [Fact]
        public void SchemaRequest_Properties_Work()
        {
            var req = new SchemaRequest { EntityName = "test" };
            Assert.Equal("test", req.EntityName);
        }

        [Fact]
        public void SchemaResult_Properties_Work()
        {
            var res = new SchemaResult { Changed = true };
            Assert.True(res.Changed);
        }

        [Fact]
        public void MutationResult_Properties_Work()
        {
            var res = new MutationResult
            {
                AffectedRows = 10,
                GeneratedValues = new Record(),
                Metadata = new ExecutionMetadata()
            };

            Assert.Equal(10ul, res.AffectedRows);
            Assert.NotNull(res.GeneratedValues);
            Assert.NotNull(res.Metadata);
        }
        
        [Fact]
        public void ExecutionMetadata_TraceChain_Works()
        {
            var meta = new ExecutionMetadata
            {
                TraceChain = new List<TraceNode> { new TraceNode("t", null, "c") }
            };
            Assert.Single(meta.TraceChain);
        }
    }
}
