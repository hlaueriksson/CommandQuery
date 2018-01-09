using System.Diagnostics;
using Microsoft.Azure.WebJobs.Host;

namespace CommandQuery.Specs.AzureFunctions
{
    public class FakeTraceWriter : TraceWriter
    {
        public FakeTraceWriter() : base(TraceLevel.Off)
        {
        }

        public override void Trace(TraceEvent traceEvent)
        {
        }
    }
}