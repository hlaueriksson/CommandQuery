using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using CommandQuery.Client;
using FluentAssertions;
using NUnit.Framework;

namespace CommandQuery.Tests.Client
{
    [Ignore("TODO: Deserialization risks in use of BinaryFormatter and related types")]
    public class CommandQueryExceptionTests
    {
        [Test]
        public void CommandQueryException_should_be_Serializable()
        {
            new CommandQueryException().Should().BeBinarySerializable();
            new CommandQueryException("fail").Should().BeBinarySerializable();
            new CommandQueryException("fail", new Exception()).Should().BeBinarySerializable();

            var error = new CommandQuery.Client.Error { Message = "fail", Details = new Dictionary<string, object> { { "foo", "bar" } } };
            new CommandQueryException("fail", error).Should().BeBinarySerializable();

            new CommandQueryException().Invoking(x => x.GetObjectData(null, new StreamingContext())).Should().Throw<ArgumentNullException>();
        }
    }
}
