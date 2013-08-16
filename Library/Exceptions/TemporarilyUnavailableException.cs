using System;

namespace Recurly
{
    public class TemporarilyUnavailableException : RecurlyServerException
    {
        internal TemporarilyUnavailableException()
            : base("Recurly is temporarily unavailable. Please try again.")
        { }
    }
}
