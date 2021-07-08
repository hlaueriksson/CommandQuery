using System;
using System.Reflection;
using System.Threading.Tasks;
using CommandQuery.Tests;
using FluentAssertions;
using LoFuUnit.NUnit;
using Microsoft.AspNetCore.Mvc.Controllers;
using NUnit.Framework;

namespace CommandQuery.AspNetCore.Tests
{
    public class CommandControllerFeatureProviderTests
    {
        [LoFu, Test]
        public async Task when_PopulateFeature()
        {
            Subject = new CommandControllerFeatureProvider(typeof(FakeCommand).Assembly);
            Result = new ControllerFeature();
            Subject.PopulateFeature(null, Result);

            void should_add_CommandControllers() =>
                Result.Controllers.Should().Contain(typeof(CommandController<FakeCommand>).GetTypeInfo());

            void should_add_CommandWithResultControllers() =>
                Result.Controllers.Should().Contain(typeof(CommandWithResultController<FakeResultCommand, FakeResult>).GetTypeInfo());

            void should_throw_when_feature_is_null() =>
                Subject.Invoking(x => x.PopulateFeature(null, null)).Should().Throw<ArgumentNullException>();
        }

        CommandControllerFeatureProvider Subject;
        ControllerFeature Result;
    }
}
