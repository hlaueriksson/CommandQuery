using System;
using CommandQuery.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace CommandQuery.Tests.Exceptions
{
    [Ignore("TODO: Deserialization risks in use of BinaryFormatter and related types")]
    public class ExceptionTests
    {
        [Test]
        public void CommandException_should_be_Serializable()
        {
            new CommandException().Should().BeBinarySerializable();
            new CommandException("fail").Should().BeBinarySerializable();
            new CommandException("fail", new Exception()).Should().BeBinarySerializable();
        }

        [Test]
        public void CommandProcessorException_should_be_Serializable()
        {
            new CommandProcessorException().Should().BeBinarySerializable();
            new CommandProcessorException("fail").Should().BeBinarySerializable();
            new CommandProcessorException("fail", new Exception()).Should().BeBinarySerializable();
        }

        [Test]
        public void CommandTypeException_should_be_Serializable()
        {
            new CommandTypeException().Should().BeBinarySerializable();
            new CommandTypeException("fail").Should().BeBinarySerializable();
            new CommandTypeException("fail", new Exception()).Should().BeBinarySerializable();
        }

        [Test]
        public void QueryException_should_be_Serializable()
        {
            new QueryException().Should().BeBinarySerializable();
            new QueryException("fail").Should().BeBinarySerializable();
            new QueryException("fail", new Exception()).Should().BeBinarySerializable();
        }

        [Test]
        public void QueryProcessorException_should_be_Serializable()
        {
            new QueryProcessorException().Should().BeBinarySerializable();
            new QueryProcessorException("fail").Should().BeBinarySerializable();
            new QueryProcessorException("fail", new Exception()).Should().BeBinarySerializable();
        }

        [Test]
        public void QueryTypeException_should_be_Serializable()
        {
            new QueryTypeException().Should().BeBinarySerializable();
            new QueryTypeException("fail").Should().BeBinarySerializable();
            new QueryTypeException("fail", new Exception()).Should().BeBinarySerializable();
        }
    }
}
