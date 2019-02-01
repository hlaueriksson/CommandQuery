#if NET461
using System.Diagnostics;
using Microsoft.Azure.WebJobs.Host;

namespace CommandQuery.Specs.AzureFunctions.V1
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
#endif