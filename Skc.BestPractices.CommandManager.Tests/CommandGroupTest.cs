using FluentAssertions;
using NUnit.Framework;

namespace Skc.BestPractices.CommandManager.Tests;

[TestFixture]
public class CommandGroupTest
{

    [Test]
    public void ConstructorTest()
    {
        // Arrange
        // -

        // Act
        var result = new CommandGroup();

        // Assert
        result.Count.Should().Be(0);
    }

    [Test]
    public void AddTest()
    {
        // Arrange
        var commandGroup = new CommandGroup();
        var command = new TestCommand();

        // Act
        commandGroup.Add(command);

        // Assert
        commandGroup.Count.Should().Be(1);
    }
}