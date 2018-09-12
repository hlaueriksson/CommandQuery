#if NETCOREAPP2_0
using System;
using Amazon.Lambda.Core;

namespace CommandQuery.Sample.Specs.AWSLambda
{
    public class FakeLambdaContext : ILambdaContext
    {
        public string AwsRequestId { get; set; }
        public IClientContext ClientContext { get; set; }
        public string FunctionName { get; set; }
        public string FunctionVersion { get; set; }
        public ICognitoIdentity Identity { get; set; }
        public string InvokedFunctionArn { get; set; }
        public ILambdaLogger Logger { get; set; }
        public string LogGroupName { get; set; }
        public string LogStreamName { get; set; }
        public int MemoryLimitInMB { get; set; }
        public TimeSpan RemainingTime { get; set; }

        public FakeLambdaContext()
        {
            Logger = new FakeLambdaLogger();
        }
    }

    public class FakeLambdaLogger : ILambdaLogger
    {
        public void Log(string message)
        {
        }

        public void LogLine(string message)
        {
        }
    }
}
#endif