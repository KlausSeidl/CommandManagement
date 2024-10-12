using System;

namespace Skc.BestPractices.CommandManager;

/// <summary>
///     Provides data for the <see cref="CommandManager.Executing" /> event.
/// </summary>
public class ExecutingEventArgs : EventArgs
{
    /// <summary>Initializes a new instance of the ExecutingEventArgs class with a command and a cancel flag.</summary>
    /// <param name="command">The command that will be executed.</param>
    /// <param name="cancel">Flag indicating if command execution should be aborted.</param>
    public ExecutingEventArgs(Command command, bool cancel)
    {
        Command = command;
        Cancel = cancel;
    }

    /// <summary>Gets the Command which is about to be executed.</summary>
    public Command Command { get; }

    /// <summary>Gets or sets flag if the execution of the command should be canceled.</summary>
    /// <value>Flag if the execution of the command should be canceled.</value>
    public bool Cancel { get; set; }
}