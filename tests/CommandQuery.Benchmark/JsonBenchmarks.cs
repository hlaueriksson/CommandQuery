using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using CommandQuery.NewtonsoftJson;

namespace CommandQuery.Benchmark
{
    public class JsonBenchmarks
    {
        private FakeComplexObject _object;
        private string _newtonsoftJsonString;
        private string _systemTextJsonString;
        private Dictionary<string, object> _dictionary;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _object = new FakeComplexObject
            {
                String = "String",
                Int = 1,
                Bool = true,
                DateTime = DateTime.Parse("2021-07-04 20:31:48"),
                Guid = Guid.Parse("b665da2a-60fe-4f2b-baf1-9d766e2542d3"),
                NullableDouble = 2.1,
                Array = new[] { 1, 2 },
                IEnumerable = new[] { 3, 4 },
                List = new List<int> { 5, 6 }
            };

            _newtonsoftJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(_object);
            _systemTextJsonString = System.Text.Json.JsonSerializer.Serialize(_object);

            _dictionary = new Dictionary<string, object>
            {
                { "String", "String" },
                { "Int", "1" },
                { "Bool", "true" },
                { "DateTime", "2021-07-04 20:31:48" },
                { "Guid", "b665da2a-60fe-4f2b-baf1-9d766e2542d3" },
                { "NullableDouble", "2.1" },
                { "Array", new[] { "1", "2" } },
                { "IEnumerable", new[] { "3", "4" } },
                { "List", new[] { "5", "6" } }
            };
        }

        // NewtonsoftJson

        [Benchmark]
        public object NewtonsoftJson_JsonExtensions_SafeToObject() => JsonExtensions.SafeToObject(_newtonsoftJsonString, typeof(FakeComplexObject));

        [Benchmark]
        public object NewtonsoftJson_DictionaryExtensions_SafeToObject() => DictionaryExtensions.SafeToObject(_dictionary, typeof(FakeComplexObject));

        // SystemTextJson

        [Benchmark]
        public object SystemTextJson_JsonExtensions_SafeToObject() => SystemTextJson.JsonExtensions.SafeToObject(_systemTextJsonString, typeof(FakeComplexObject));

        [Benchmark]
        public object SystemTextJson_DictionaryExtensions_SafeToObject() => SystemTextJson.DictionaryExtensions.SafeToObject(_dictionary, typeof(FakeComplexObject));
    }

    public class FakeComplexObject
    {
        public string String { get; set; }
        public int Int { get; set; }
        public bool Bool { get; set; }
        public DateTime DateTime { get; set; }
        public Guid Guid { get; set; }
        public double? NullableDouble { get; set; }
        public int[] Array { get; set; }
        public IEnumerable<int> IEnumerable { get; set; }
        public List<int> List { get; set; }
    }
}
