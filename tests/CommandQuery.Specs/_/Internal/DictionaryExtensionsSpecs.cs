using System;
using System.Collections.Generic;
using CommandQuery.Internal;
using Machine.Specifications;

namespace CommandQuery.Specs._.Internal
{
    public class DictionaryExtensionsSpecs
    {
        [Subject(typeof(DictionaryExtensions))]
        public class when_converting_a_dictionary_to_object
        {
            Establish context = () =>
            {
                Subject = new Dictionary<string, string>
                {
                    { "String", "Value" },
                    { "Int", "1" },
                    { "Bool", "true" },
                    { "DateTime", "2018-07-06" },
                    //{ "Guid", "3B10C34C-D423-4EC3-8811-DA2E0606E241" },
                    { "NullableDouble", "2" },
                    { "UndefinedProperty", "should_not_be_used" }
                };
            };

            It should_set_the_property_values = () =>
            {
                var result = Subject.SafeToObject(typeof(FakeQuery)) as FakeQuery;

                result.String.ShouldEqual("Value");
                result.Int.ShouldEqual(1);
                result.Bool.ShouldEqual(true);
                result.DateTime.ShouldEqual(DateTime.Parse("2018-07-06"));
                //result.Guid.ShouldEqual(new Guid("3B10C34C-D423-4EC3-8811-DA2E0606E241"));
                result.NullableDouble.ShouldEqual(2);
            };

            static IDictionary<string, string> Subject;
        }

        private class FakeQuery
        {
            public string String { get; set; }
            public int Int { get; set; }
            public bool Bool { get; set; }
            public DateTime DateTime { get; set; }
            //public Guid Guid { get; set; }
            public double? NullableDouble { get; set; }
        }
    }
}