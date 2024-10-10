using System;
using FluentAssertions;
using NUnit.Framework;

namespace Skc.BestPractices.CommandManager.Tests;

[TestFixture]
public class CommandValidationExceptionTest
{
    [Test]
    public void ConstructorTest()
    {
        // Arrange
        var innerEx = new ArgumentException("invalid argument");

        // Act
        var result = new CommandValidationException(innerEx);

        // Assert
        result.InnerException.Should().BeSameAs(innerEx);
        result.Message.Should().Be(innerEx.Message);
    }
}