using System;

namespace Skc.BestPractices.CommandManager;

public class CommandValidationException : Exception
{
    public CommandValidationException(Exception innerEx) : base(innerEx.Message, innerEx)
    {
    }
}