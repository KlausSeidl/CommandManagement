using FluentAssertions;
using NUnit.Framework;

namespace Skc.BestPractices.CommandManager.Tests;

[TestFixture]
public class MacroTest
{
    [Test]
    public void Should_execute_all_contained_commands()
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
    
    [Test]
    public void Should_execute_all_not_discarded_contained_commands()
    {
        // Arrange
        var macro = new Macro();
        int executed = 0;
        int undone = 0;
        
        macro.Add(() => new TestCommandWithCallback(() => executed++, () => undone++, true));
        macro.Add(() => new TestCommandWithCallback(() => executed++, () => undone++));

        var commandManager = new CommandManager();

        // Act
        commandManager.Execute(macro);
        
        // Assert
        commandManager.GetUndoCommands().Count.Should().Be(1);
        executed.Should().Be(1);
        undone.Should().Be(0);
        
        // Undo
        commandManager.Undo();
        commandManager.GetUndoCommands().Count.Should().Be(0);
        executed.Should().Be(1);
        undone.Should().Be(1);
        
        // Redo
        commandManager.Redo();
        commandManager.GetUndoCommands().Count.Should().Be(1);
        executed.Should().Be(2);
        undone.Should().Be(1);
    }
}