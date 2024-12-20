﻿using System;
using System.Collections.Generic;

namespace Skc.BestPractices.CommandManager;

/// <summary>
///     The CommandGroup class is a Command object
///     that can store sub-commands to build groups or macros.
/// </summary>
public class CommandGroup : Command
{
    private readonly List<Command> _commands = [];

    /// <summary>
    ///     Adds a command to the CommandGroup.
    /// </summary>
    /// <param name="command">A <see cref="Command" /> object.</param>
    /// <exception cref="ArgumentNullException">Thrown when command is a null reference (Nothing in Visual Basic)</exception>
    public void Add(Command command)
    {
        // Check parameters
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command), "command is null");
        }

        // Add to group if not discarded
        if (command.Discard == false)
        {
            _commands.Add(command);
        }
    }

    /// <summary>
    ///     Gets number of commands included in the CommandGroup.
    /// </summary>
    public int Count => _commands.Count;

    /// <inheritdoc />
    protected internal override object Execute()
    {
        for (var i = 0; i <= _commands.Count - 1; i++)
        {
            var cmd = _commands[i];
            cmd.Execute();
        }

        // Remove discarded commands to save memory
        for (var i = _commands.Count - 1; i >= 0; i += -1)
        {
            var cmd = _commands[i];
            if (cmd.Discard) _commands.RemoveAt(i);
        }

        return null;
    }

    /// <inheritdoc />
    protected internal override object Undo()
    {
        for (var i = _commands.Count - 1; i >= 0; i += -1) _commands[i].Undo();

        return null;
    }
}