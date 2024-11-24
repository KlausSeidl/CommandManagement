using FluentAssertions;
using NUnit.Framework;

namespace Skc.BestPractices.CommandManager.Tests;

[TestFixture]
public class MacroTest
{
    [Test]
    public void AddTest()
    {
        // Arrange
        var macro = new Macro();
        int executed = 0;
        int undone = 0;
        
        macro.Add(() => new TestCommandWithCallback(() => executed++, () => undone++));
        macro.Add(() => new TestCommandWithCallback(() => executed++, () => undone++));

        var commandManager = new CommandManager();

        // Act
        commandManager.Execute(macro);
        
        // Assert
        commandManager.GetUndoCommands().Count.Should().Be(1);
        executed.Should().Be(2);
        undone.Should().Be(0);
        
        // Undo
        commandManager.Undo();
        commandManager.GetUndoCommands().Count.Should().Be(0);
        executed.Should().Be(2);
        undone.Should().Be(2);
        
        // Redo
        commandManager.Redo();
        commandManager.GetUndoCommands().Count.Should().Be(1);
        executed.Should().Be(4);
        undone.Should().Be(2);
    }
}