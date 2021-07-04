using BenchmarkDotNet.Running;

namespace CommandQuery.Benchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<JsonBenchmarks>();
        }
    }
}
