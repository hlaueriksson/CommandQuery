using System.Reflection;
using CommandQuery.Tests;
using FluentAssertions;
using LoFuUnit.NUnit;
using Microsoft.AspNetCore.Mvc.Controllers;
using NUnit.Framework;

namespace CommandQuery.AspNetCore.Tests
{
    public class QueryControllerFeatureProviderTests
    {
        [LoFu, Test]
        public async Task when_PopulateFeature()
        {
            Subject = new QueryControllerFeatureProvider(typeof(FakeQuery).Assembly);
            Result = new ControllerFeature();
            Subject.PopulateFeature(null, Result);

            void should_add_QueryControllers() =>
                Result.Controllers.Should().Contain(typeof(QueryController<FakeQuery, FakeResult>).GetTypeInfo());

            void should_throw_when_feature_is_null() =>
                Subject.Invoking(x => x.PopulateFeature(null, null)).Should().Throw<ArgumentNullException>();
        }

        QueryControllerFeatureProvider Subject;
        ControllerFeature Result;
    }
}
