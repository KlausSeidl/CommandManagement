using System;

namespace Skc.BestPractices.CommandManager
{
    /// <summary>Provides data for the <see cref="Command.Executed" /> event.</summary>
    public class NotifyEventArgs : EventArgs
    {
        /// <summary>Initializes a new instance of the NotifyEventArgs class with a message.</summary>
        /// <param name="message">The message we want to tell the client.</param>
        public NotifyEventArgs(string message)
        {
            Message = message;
            Discarded = false;
        }

        /// <summary>Initializes a new instance of the NotifyEventArgs class with a message and a discarded flag.</summary>
        /// <param name="message">The message we want to tell the client.</param>
        /// <param name="discarded">Flag indicating if command had been discarded.</param>
        public NotifyEventArgs(string message, bool discarded) : this(message)
        {
            Discarded = discarded;
        }

        /// <summary>Gets the message.</summary>
        /// <value>The message.</value>
        public string Message { get; }

        /// <summary>Gets flag indicating if command had been discarded.</summary>
        /// <value>Flag indicating if command had been discarded.</value>
        public bool Discarded { get; }
    }
}