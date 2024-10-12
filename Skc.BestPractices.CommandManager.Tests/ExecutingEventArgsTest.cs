using FluentAssertions;
using NUnit.Framework;

namespace Skc.BestPractices.CommandManager.Tests;

[TestFixture]
public class ExecutingEventArgsTest
{
    [Test]
    public void ConstructorTest()
    {
        // Arrange
        var cmd = new UndoableTestCommand();
        const bool cancel = false;

        // Act
        var result = new ExecutingEventArgs(cmd, cancel);

        // Assert
        result.Command.Should().BeSameAs(cmd);
        result.Cancel.Should().BeFalse();
    }
}