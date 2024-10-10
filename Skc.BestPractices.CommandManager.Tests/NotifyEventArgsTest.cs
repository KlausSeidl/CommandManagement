using FluentAssertions;
using NUnit.Framework;

namespace Skc.BestPractices.CommandManager.Tests;

[TestFixture]
public class NotifyEventArgsTest
{
    [Test]
    public void ConstructorTestWithMessage()
    {
        // Arrange
        const string message = "message";

        // Act
        var result = new NotifyEventArgs(message);

        // Assert
        result.Message.Should().Be(message);
        result.Discarded.Should().BeFalse();
    }

    [Test]
    public void ConstructorTestWithMessageAndDiscarded()
    {
        // Arrange
        const string message = "message";
        const bool discarded = true;

        // Act
        var result = new NotifyEventArgs(message, discarded);

        // Assert
        result.Message.Should().Be(message);
        result.Discarded.Should().BeTrue();
    }
}