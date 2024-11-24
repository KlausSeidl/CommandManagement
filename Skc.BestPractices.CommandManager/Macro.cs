using System;
using System.Collections.Generic;

namespace Skc.BestPractices.CommandManager;

/// <summary>
///     The Macro class is a Command object that can store sub-commands to build macros.
///     The sub-commands are created and executed deferred, so that all commands are really executed
/// </summary>
public class Macro : Command
{
    private readonly List<Func<Command>> _commandFactories = [];
    private readonly List<Command> _commands = [];
    
    /// <summary>
    ///     Adds a command factory to the Macro.
    /// </summary>
    /// <param name="commandFactory">A <see cref="Command" /> object.</param>
    /// <exception cref="ArgumentNullException">Thrown when command is a null reference (Nothing in Visual Basic)</exception>
    public void Add(Func<Command> commandFactory)
    {
        // Check parameters
        if (commandFactory == null)
        {
            throw new ArgumentNullException(nameof(commandFactory), "command is null");
        }

        _commandFactories.Add(commandFactory);
    }
    
    /// <inheritdoc />
    protected internal override object Execute()
    {
        for (var i = 0; i <= _commandFactories.Count - 1; i++)
        {
            var cmd = _commandFactories[i]();
            if (!cmd.Discard)
            {
                cmd.Execute();
                _commands.Add(cmd);
            }
        }

        return null;
    }

    /// <inheritdoc />
    protected internal override object Undo()
    {
        for (var i = _commands.Count - 1; i >= 0; i += -1)
        {
            _commands[i].Undo();
        }

        return null;
    }
}