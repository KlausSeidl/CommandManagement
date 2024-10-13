using System;
using System.Collections.Generic;

namespace Skc.BestPractices.CommandManager;

/// <summary>
///     The CommandManager handles the history of done/undone commands and
///     the building of command groups (like macros). All responsibility for executing
///     commands that should participate in the command history framework must
///     be delegated to the CommandManager. If there are commands (method calls) outside
///     the command manager (these cannot be undone), make sure to have consistent
///     data for the commands registered in the command manager.
/// </summary>
/// <remarks>
///     To build a command group call
///     <list type="number">
///         <item>
///             <description>BeginGroup</description>
///         </item>
///         <item>
///             <description>Add commands to the group by calling DoCommand</description>
///         </item>
///         <item>
///             <description>Repeat step 2 for each single command</description>
///         </item>
///         <item>
///             <description>EndGroup</description>
///         </item>
///     </list>
/// </remarks>
public sealed class CommandManager
{
    // The history of executed commands for a redo. 
    private Stack<Command> _commandHistory;

    // If we want to build a command group 
    private CommandGroup _currentGroup;

    // Store a marker on a command
    private Command _marker;
    
    // Stores the list of undone commands
    private Stack<Command> _undoneCommands;


    /// <summary>
    ///     Initializes a new instance of the CommandManager class.
    /// </summary>
    public CommandManager()
    {
        _commandHistory = new Stack<Command>();
        _undoneCommands = new Stack<Command>();

        _marker = null;
    }

    /// <summary>
    ///     Returns true, if the CommandManager is currently in building a group.
    /// </summary>
    /// <remarks>
    ///     See <see cref="BeginGroup"/> and <see cref="EndGroup"/> for details about building command groups (macros) 
    /// </remarks>
    public bool IsGroup => _currentGroup != null;


    /// <summary>
    ///     Event raised when the history of executed commands changed.
    /// </summary>
    /// <remarks>
    ///     Handle this event to work with the list of commands that can be undone (<see cref="GetUndoCommands"/>), e.g. to update the state of an "Undo" menu item 
    /// </remarks>
    public event EventHandler<EventArgs> CommandHistoryChanged;

    /// <summary>
    ///     Event raised when future (redo list) of commands changed.
    /// </summary>
    /// <remarks>
    ///     Handle this event to work with the list of commands that can be redone (<see cref="GetRedoCommands"/>), e.g. to update the state of an "Redo" menu item 
    /// </remarks>
    public event EventHandler<EventArgs> CommandFutureChanged;

    /// <summary>
    ///     Event raised before a command will be executed. Use this event, when you want cancel the execution of a command.
    /// </summary>
    public event EventHandler<ExecutingEventArgs> Executing;

    /// <summary>
    ///     Event raised when a command was executed.
    /// </summary>
    public event EventHandler<NotifyEventArgs> Executed;

    /// <summary>
    ///     Event raise when a command was discarded.
    /// </summary>
    public event EventHandler<NotifyEventArgs> Discarded;

    /// <summary>
    ///     To build a group of commands, first call this method.
    /// </summary>
    /// <param name="description">
    ///     Description of the command group or macro
    /// </param>
    public void BeginGroup(string description)
    {
        // Create a new CommandGroup Object 
        // and collect all done commands in this group 
        _currentGroup = new CommandGroup { Description = description };
    }

    /// <summary>
    ///     After collecting all commands in a command group call EndGroup
    ///     to close the group.
    /// </summary>
    public void EndGroup()
    {
        // The group is complete with all command object, so 
        // add the command group to the history 
        _commandHistory.Push(_currentGroup);
        OnCommandHistoryChanged(EventArgs.Empty);
        // Destroy the group 
        _currentGroup = null;
    }

    /// <summary>
    ///     Executes the command object.
    /// </summary>
    /// <param name="cmd">A Command object to execute</param>
    /// <returns></returns>
    /// <remarks>
    ///     Do not call the Execute method of a command object directly,
    ///     because it then would not be registered in the command history.
    /// </remarks>
    public object Execute(Command cmd)
    {
        // Check, if this command has been discarded in the constructor
        if (cmd.Discard)
        {
            OnDiscarded(cmd, new NotifyEventArgs("Command discarded", true));
            return null;
        }

        // Check, if this is an empty command group
        if (cmd is CommandGroup commandGroup && commandGroup.Count == 0)
            return null;

        // Notify that the command will now be executed
        var executing = new ExecutingEventArgs(cmd, cmd.RequiresCannotBeUndoneUserConfirmation);
        OnExecuting(cmd, executing);

        if (executing.Cancel)
        {
            cmd.Discard = true;
            OnDiscarded(cmd, new NotifyEventArgs("Command by user discarded", true));
            return null;
        }

        // Execute the command 
        var result = cmd.Execute();

        // Check, if this command has been discarded in the execute method
        if (cmd.Discard)
        {
            OnDiscarded(cmd, new NotifyEventArgs("Command discarded", true));
            return null;
        }

        // The command has been really executed, so tell it the client
        OnExecuted(cmd, new NotifyEventArgs(cmd.Description));
        cmd.OnExecuted(cmd, new NotifyEventArgs(cmd.Description));

        // Do we have to add the command to the command group ? 
        // (i.e. has the user called BeginGroup before) 
        if (_currentGroup == null)
        {
            // Add the command to the History 
            _commandHistory.Push(cmd);
            OnCommandHistoryChanged(EventArgs.Empty);
        }
        else
        {
            _currentGroup.Add(cmd);
        }

        // Clear the Undone Command List 
        _undoneCommands.Clear();
        OnCommandFutureChanged(EventArgs.Empty);


        // Check, if we have to clear all stacks
        if (cmd.CanBeUndone == false)
        {
            _commandHistory.Clear();
            OnCommandHistoryChanged(EventArgs.Empty);
        }

        return result;
    }

    /// <summary>
    ///     To undo the last command in the command history call this method.
    /// </summary>
    /// <returns></returns>
    public object Undo()
    {
        var commandToBeUndone = _commandHistory.Pop();

        OnCommandHistoryChanged(EventArgs.Empty);

        var result = commandToBeUndone.Undo();

        // The command has been executed, so tell it the client
        OnExecuted(commandToBeUndone, new NotifyEventArgs(commandToBeUndone.Description + " - undone")); 
        commandToBeUndone.OnExecuted(commandToBeUndone, new NotifyEventArgs(commandToBeUndone.Description + " - undone"));

        // Store for a redo 
        _undoneCommands.Push(commandToBeUndone);
        OnCommandFutureChanged(EventArgs.Empty);

        return result;
    }

    /// <summary>
    ///     Calls Undo on all command in the command history.
    /// </summary>
    public void UndoEverything()
    {
        while (_commandHistory.Count != 0)
        {
            Undo();
        }
    }

    /// <summary>
    ///     To redo the last undone command call this method.
    /// </summary>
    public object Redo()
    {
        var commandToBeRedone = _undoneCommands.Pop();

        OnCommandFutureChanged(EventArgs.Empty);

        // Undo the command 
        var result = commandToBeRedone.Execute();

        // The command has been really executed, so tell it the client
        OnExecuted(commandToBeRedone, new NotifyEventArgs(commandToBeRedone.Description + " - redone"));
        commandToBeRedone.OnExecuted(commandToBeRedone, new NotifyEventArgs(commandToBeRedone.Description + " - redone"));

        // Store for an undo 
        _commandHistory.Push(commandToBeRedone);
        OnCommandHistoryChanged(EventArgs.Empty);
        return result;
    }

    /// <summary>
    ///     Returns the list of command objects in the Undo Stack (i.e. all commands that can be undone)
    /// </summary>
    /// <returns>
    ///     List of commands in the Undo stack
    /// </returns>
    public IReadOnlyList<Command> GetUndoCommands()
    {
        return new List<Command>(_commandHistory).AsReadOnly();
    }

    /// <summary>
    ///     Returns the list of all undone command objects, i.e. all commands that can be redone.
    /// </summary>
    /// <returns>
    ///     List of all undone command objects, i.e. all commands that can be redone
    /// </returns>
    public IReadOnlyList<Command> GetRedoCommands()
    {
        return new List<Command>(_undoneCommands).AsReadOnly();
    }

    /// <summary>
    ///     Sets a marker on the last executed command.
    ///     We can use the marker to check, if there are executed commands
    ///     before or after the marker to know, if there are unsaved changes.
    /// </summary>
    public void SetMarker()
    {
        _marker = _commandHistory.Count > 0
            ? _commandHistory.Peek()
            : null;
    }

    /// <summary>
    ///     Checks, if the last executed command has the marker set
    /// </summary>
    public bool IsAtMarker()
    {
        // If we have never set the marker, and there are no commands in the
        // history, then we have never done anything
        if (_marker == null)
            return _commandHistory.Count == 0;

        var lastCmd = _commandHistory.Count > 0
            ? _commandHistory.Peek()
            : null;

        return lastCmd == _marker;
    }

    public void Clear()
    {
        _commandHistory = new Stack<Command>();
        _undoneCommands = new Stack<Command>();

        _currentGroup = null;
        _marker = null;
    }
    
    /// <summary>
    ///     Raises the CommandHistoryChanged event.
    /// </summary>
    /// <param name="e">An <see cref="EventArgs" /> object.</param>
    private void OnCommandHistoryChanged(EventArgs e)
    {
        CommandHistoryChanged?.Invoke(this, e);
    }

    /// <summary>
    ///     Raises the CommandFutureChanged event.
    /// </summary>
    /// <param name="e">An <see cref="EventArgs" /> object.</param>
    private void OnCommandFutureChanged(EventArgs e)
    {
        CommandFutureChanged?.Invoke(this, e);
    }

    /// <summary>Raises the Executing event.</summary>
    /// <param name="sender"></param>
    /// <param name="e">An <see cref="ExecutingEventArgs" /> object.</param>
    private void OnExecuting(object sender, ExecutingEventArgs e)
    {
        Executing?.Invoke(sender, e);
    }

    /// <summary>Raises the Executed event.</summary>
    /// <param name="sender"></param>
    /// <param name="e">An <see cref="NotifyEventArgs" /> object.</param>
    private void OnExecuted(object sender, NotifyEventArgs e)
    {
        Executed?.Invoke(sender, e);
    }

    /// <summary>Raises the Discarded event.</summary>
    /// <param name="sender"></param>
    /// <param name="e">An <see cref="NotifyEventArgs" /> object.</param>
    private void OnDiscarded(object sender, NotifyEventArgs e)
    {
        Discarded?.Invoke(sender, e);
        Executed?.Invoke(sender, e);
    }
}