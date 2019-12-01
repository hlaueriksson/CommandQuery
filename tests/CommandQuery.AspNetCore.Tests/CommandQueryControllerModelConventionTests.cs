using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using CommandQuery.Tests;
using FluentAssertions;
using LoFuUnit.NUnit;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using NUnit.Framework;

namespace CommandQuery.AspNetCore.Tests
{
    public class CommandQueryControllerModelConventionTests
    {
        [LoFu, Test]
        public async Task when_Apply()
        {
            Subject = new CommandQueryControllerModelConvention();

            void should_handle_CommandControllers()
            {
                var result = new ControllerModel(typeof(CommandController<FakeCommand>).GetTypeInfo(), new List<object>());
                Subject.Apply(result);
                result.ControllerName.Should().Be("FakeCommand");
            }

            void should_handle_CommandWithResultControllers()
            {
                var result = new ControllerModel(typeof(CommandWithResultController<FakeResultCommand, FakeResult>).GetTypeInfo(), new List<object>());
                Subject.Apply(result);
                result.ControllerName.Should().Be("FakeResultCommand");
            }

            void should_handle_QueryControllers()
            {
                var result = new ControllerModel(typeof(QueryController<FakeQuery, FakeResult>).GetTypeInfo(), new List<object>());
                Subject.Apply(result);
                result.ControllerName.Should().Be("FakeQuery");
            }
        }

        CommandQueryControllerModelConvention Subject;
    }
}