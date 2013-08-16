using System;
using Recurly.Core;

namespace Recurly
{
    /// <summary>
    /// An Internal Server Error occurred on Recurly's side.
    /// </summary>
    public class RecurlyServerException : RecurlyException
    {
        internal RecurlyServerException(RecurlyError[] errors)
            : base("Recurly experienced an internal server error.", errors)
        { }

        internal RecurlyServerException(string message)
            : base(message)
        { }
    }
}
