using System;

namespace Skc.BestPractices.CommandManager;

/// <summary>
///     The abstract Command class is the base class for all commands.
///     To create a command inherit from Command, add all data needed
///     to perform the undo action (i.e. write a constructor) and implement
///     the main commands in the <see cref="Execute" /> and <see cref="Undo" /> functions.
/// </summary>
public abstract class Command
{
    /// <summary>
    ///     The Executed event is raised by the <see cref="CommandManager"/> when this command has been executed.
    /// </summary>
    /// <remarks>
    ///     Usually there is no need to subscribe to this event. Use the <see cref="CommandManager.Executed"/> event for standard use cases.
    /// </remarks>
    public event EventHandler<NotifyEventArgs> Executed;

    /// <summary>
    ///     Use the Description Property to describe what this command does
    /// </summary>
    public string Description = "";

    /// <summary>
    ///     Use the Discard Property to cancel execution and to tell the command manager not to put the command on the stack
    /// </summary>
    protected internal bool Discard;

    /// <summary>Gets, if this command can be undone. By default, all commands can be undone</summary>
    /// <returns>True, if the command can be undone</returns>
    public bool CanBeUndone { get; protected set; } = true;

    /// <summary>
    ///     Gets an explanatory message why this command cannot be undone. By default all commands can be undone and
    ///     therefor this message is empty
    /// </summary>
    /// <returns>Message why this command cannot be undone</returns>
    public string CannotBeUndoneMessage { get; protected set; }

    /// <summary>
    ///     Gets, if this command requires a user confirmation that it cannot be undone before it will be executed.
    ///     By default, all commands can be undone and do not require user interaction.
    /// </summary>
    /// <returns>True, if the command requires user interaction before execution</returns>
    public bool RequiresCannotBeUndoneUserConfirmation { get; protected set; } = false;
    
    /// <summary>
    /// Invokes the <see cref="Executed"/> event
    /// </summary>
    /// <param name="sender">This command itself</param>
    /// <param name="e">Provides additional information about the command execution</param>
    protected internal void OnExecuted(object sender, NotifyEventArgs e)
    {
        Executed?.Invoke(sender, e);
    }

    /// <summary>Implement the main action for the command in this procedure.</summary>
    /// <returns>Any return value from the main action</returns>
    protected internal abstract object Execute();

    /// <summary>Implement the main action for undoing the command in this procedure.</summary>
    /// <returns>Any return value from the main action</returns>
    protected internal abstract object Undo();

    /// <summary>Gives the user-friendly description of the command</summary>
    /// <returns>The description of the command</returns>
    public override string ToString()
    {
        return Description;
    }
}