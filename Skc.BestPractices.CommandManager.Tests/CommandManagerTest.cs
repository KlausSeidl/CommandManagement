using System;
using FluentAssertions;
using NUnit.Framework;

namespace Skc.BestPractices.CommandManager.Tests;

[TestFixture]
public class CommandManagerTest
{
    [SetUp]
    public void TestInitialize()
    {
        _testClass = new CommandManager();

        _isDiscarded = false;
        _isExecutingRaised = false;
        _isExecutedRaised = false;
        _isCommandHistoryChangedRaised = false;
        _isCommandFutureChangedRaised = false;
    }

    private CommandManager _testClass;

    private bool _isDiscarded;
    private bool _isExecutingRaised;
    private bool _isExecutedRaised;
    private bool _isCommandHistoryChangedRaised;
    private bool _isCommandFutureChangedRaised;


    private void CommandManager_Executing(object sender, ExecutingEventArgs e)
    {
        _isExecutingRaised = true;
    }

    private void CommandManager_ExecutingWithUserInteraction(object sender, ExecutingEventArgs e)
    {
        _isExecutingRaised = true;
        e.Cancel = false;
    }

    private void CommandManager_Executed(object sender, NotifyEventArgs e)
    {
        _isExecutedRaised = true;
        _isDiscarded = e.Discarded;
    }

    private void CommandManager_CommandHistoryChanged(object sender, EventArgs e)
    {
        _isCommandHistoryChangedRaised = true;
    }

    private void CommandManager_CommandFutureChanged(object sender, EventArgs e)
    {
        _isCommandFutureChangedRaised = true;
    }

    [Test]
    public void ConstructorTest()
    {
        // Arrange
        // -

        // Act
        // (done in SetUp)

        // Assert
        _testClass.IsGroup.Should().BeFalse();
    }

    [Test]
    public void ExecuteTest()
    {
        // Arrange
        var command = new TestCommand();
        _testClass.Executed += CommandManager_Executed;
        _testClass.CommandHistoryChanged += CommandManager_CommandHistoryChanged;
        _isExecutedRaised = false;
        _isCommandHistoryChangedRaised = false;

        // Act
        _testClass.Execute(command);

        // Assert
        _isExecutedRaised.Should().BeTrue();
        _isCommandHistoryChangedRaised.Should().BeTrue();
    }

    [Test]
    public void UndoTest()
    {
        // Arrange
        _testClass.Execute(new TestCommand());
        _testClass.Executed += CommandManager_Executed;
        _testClass.CommandHistoryChanged += CommandManager_CommandHistoryChanged;
        _testClass.CommandFutureChanged += CommandManager_CommandFutureChanged;


        // Act
        _testClass.Undo();

        // Assert
        _isExecutedRaised.Should().BeTrue();
        _isCommandHistoryChangedRaised.Should().BeTrue();
        _isCommandFutureChangedRaised.Should().BeTrue();
    }

    [Test]
    public void RedoTest()
    {
        // Arrange
        _testClass.Execute(new TestCommand());
        _testClass.Undo();
        _testClass.Executed += CommandManager_Executed;
        _testClass.CommandHistoryChanged += CommandManager_CommandHistoryChanged;
        _testClass.CommandFutureChanged += CommandManager_CommandFutureChanged;
        _isExecutedRaised = false;
        _isCommandHistoryChangedRaised = false;
        _isCommandFutureChangedRaised = false;

        // Act
        _testClass.Redo();

        // Assert
        _isExecutedRaised.Should().BeTrue();
        _isCommandHistoryChangedRaised.Should().BeTrue();
        _isCommandFutureChangedRaised.Should().BeTrue();
    }

    [Test]
    public void GetRedoCommandsTest()
    {
        // Arrange
        var command = new TestCommand();
        _testClass.Execute(command);
        _testClass.Undo();

        // Act
        var result = _testClass.GetRedoCommands();

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(1);
        result[0].Should().BeSameAs(command);
    }

    [Test]
    public void GetUndoCommandsTest()
    {
        // Arrange
        var command = new TestCommand();
        _testClass.Execute(command);

        // Act
        var result = _testClass.GetUndoCommands();

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(1);
        result[0].Should().BeSameAs(command);
    }

    [Test]
    public void SetMarkterTest()
    {
        // Arrange
        // -

        // Act
        _testClass.SetMarker();

        // Assert
        _testClass.IsAtMarker().Should().BeTrue();
    }

    [Test]
    public void IsAtMarkerTest()
    {
        // Arrange
        _testClass.SetMarker();
        _testClass.Execute(new TestCommand());

        // Act
        var result = _testClass.IsAtMarker();

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsGroupTest()
    {
        _testClass.IsGroup.Should().BeFalse();
    }

    [Test]
    public void ExecuteUndoableCommandTest()
    {
        // Arrange
        var command = new UndoableTestCommand();
        _testClass.Executing += CommandManager_Executing;
        _testClass.Executed += CommandManager_Executed;
        _testClass.CommandHistoryChanged += CommandManager_CommandHistoryChanged;

        // Act
        _testClass.Execute(command);

        // Assert
        _isExecutingRaised.Should().BeTrue();
        _isDiscarded.Should().BeTrue();
        _isExecutedRaised.Should().BeTrue();
        _isCommandHistoryChangedRaised.Should().BeFalse();
    }

    [Test]
    public void ExecuteUndoableCommandWithUserInteractionTest()
    {
        // Arrange
        var commandNormal = new TestCommand();
        _testClass.Execute(commandNormal);
        _testClass.GetUndoCommands().Count.Should().Be(1);

        _isDiscarded = false;
        _isExecutingRaised = false;
        _isExecutedRaised = false;
        _isCommandHistoryChangedRaised = false;
        _isCommandFutureChangedRaised = false;

        var command = new UndoableTestCommand();
        _testClass.Executing += CommandManager_ExecutingWithUserInteraction;
        _testClass.Executed += CommandManager_Executed;
        _testClass.CommandHistoryChanged += CommandManager_CommandHistoryChanged;

        // Act
        _testClass.Execute(command);

        // Assert
        _isExecutingRaised.Should().BeTrue();
        _isDiscarded.Should().BeFalse();
        _isExecutedRaised.Should().BeTrue();
        _isCommandHistoryChangedRaised.Should().BeTrue();
        _testClass.GetUndoCommands().Count.Should().Be(0);
        _testClass.GetRedoCommands().Count.Should().Be(0);
    }
}