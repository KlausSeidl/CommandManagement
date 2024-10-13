using System;

namespace Skc.BestPractices.CommandManager;

/// <summary>
///     Use the <see cref="CommandValidationException"/> whenever you need to raise an exception in validating a commands input data. 
/// </summary>
/// <remarks>
///     This exception must be handled in calling code and is not handled by the <see cref="CommandManager"/>
/// </remarks>
public class CommandValidationException : Exception
{
    /// <summary>
    ///     Constructor. Initializes a new instance of the class
    /// </summary>
    /// <param name="innerEx">The wrapped inner exception</param>
    public CommandValidationException(Exception innerEx) : base(innerEx.Message, innerEx)
    {
    }
}