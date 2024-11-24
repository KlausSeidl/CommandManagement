using System;

namespace Skc.BestPractices.CommandManager.Tests;

public class TestCommand : Command
{
    protected override object Execute()
    {
        return null;
    }

    protected override object Undo()
    {
        return null;
    }
}

public class TestCommandWithCallback : Command
{
    private readonly Action _executeAction;
    private readonly Action _undoAction;

    public TestCommandWithCallback(Action executeAction, Action undoAction, bool discard = false)
    {
        _executeAction = executeAction;
        _undoAction = undoAction;
        Discard = discard;
    }

    protected override object Execute()
    {
        _executeAction();
        return null;
    }

    protected override object Undo()
    {
        _undoAction();
        return null;
    }
}