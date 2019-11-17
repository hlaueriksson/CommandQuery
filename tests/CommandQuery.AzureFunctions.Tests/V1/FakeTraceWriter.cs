#if NET472
using System.Diagnostics;
using Microsoft.Azure.WebJobs.Host;

namespace CommandQuery.AzureFunctions.Tests.V1
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