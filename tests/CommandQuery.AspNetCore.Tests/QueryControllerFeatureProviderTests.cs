using System.Reflection;
using System.Threading.Tasks;
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
            var subject = new QueryControllerFeatureProvider(typeof(FakeQuery).Assembly);
            Result = new ControllerFeature();
            subject.PopulateFeature(null, Result);

            void should_add_QueryControllers() =>
                Result.Controllers.Should().Contain(typeof(QueryController<FakeQuery, FakeResult>).GetTypeInfo());
        }

        ControllerFeature Result;
    }
}