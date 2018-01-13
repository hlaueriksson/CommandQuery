using System.Diagnostics;
using Microsoft.Azure.WebJobs.Host;

namespace CommandQuery.Sample.Specs.AzureFunctions.Vs2
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