using System;
using System.Collections.Generic;

namespace Skc.BestPractices.CommandManager;

/// <summary>
///     The CommandGroup class is a Command object
///     that can store sub-commands to build groups or macros.
/// </summary>
public class CommandGroup : Command
{
    private readonly List<Command> _commands;

    /// <summary>
    ///     Initializes a new instance of the CommandGroup class.
    /// </summary>
    public CommandGroup()
    {
        _commands = new List<Command>();
    }

    /// <summary>
    ///     Gets number of commands included in the CommandGroup.
    /// </summary>
    public int Count => _commands.Count;

    protected internal override object Execute()
    {
        Command cmd;
        int i;

        for (i = 0; i <= _commands.Count - 1; i++)
        {
            cmd = _commands[i];
            cmd.Execute();
        }

        // Remove discarded commands to save memory
        for (i = _commands.Count - 1; i >= 0; i += -1)
        {
            cmd = _commands[i];
            if (cmd.Discard) _commands.RemoveAt(i);
        }

        return null;
    }

    protected internal override object Undo()
    {
        for (var i = _commands.Count - 1; i >= 0; i += -1) _commands[i].Undo();

        return null;
    }

    /// <summary>
    ///     Adds a command to the CommandGroup.
    /// </summary>
    /// <param name="command">A <see cref="Command" /> object.</param>
    /// <exception cref="ArgumentNullException">Thrown when command is a null reference (Nothing in Visual Basic)</exception>
    public void Add(Command command)
    {
        // Check parameters
        if (command == null) throw new ArgumentNullException(nameof(command), "command is null");

        // Add to group if not discarded
        if (command.Discard == false) _commands.Add(command);
    }
}