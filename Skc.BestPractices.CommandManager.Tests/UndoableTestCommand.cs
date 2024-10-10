namespace Skc.BestPractices.CommandManager.Tests;

public class UndoableTestCommand : Command
{

    public UndoableTestCommand()
    {
        CanBeUndone = false;
        CannotBeUndoneMessage = "To complicated";
        RequiresCannotBeUndoneUserConfirmation = true;
    }

    protected override object Execute()
    {
        return null;
    }

    protected override object Undo()
    {
        return null;
    }
}