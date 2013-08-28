using System;
using System.Collections.Generic;
using Recurly.Core;

namespace Recurly
{
    /// <summary>
    /// Base class for exceptions thrown by Recurly's API.
    /// </summary>
    public class RecurlyException : ApplicationException
    {
        /// <summary>
        /// Error details from Recurly
        /// </summary>
        public List<RecurlyError> Errors { get; private set; }

        internal RecurlyException(List<RecurlyError> errors)
        {
            Errors = errors;
        }

        internal RecurlyException(string message)
            : base(message)
        { }

        internal RecurlyException(string message, Exception innerException)
            : base(message, innerException)
        { }

        internal RecurlyException(string message, List<RecurlyError> errors)
            : base(message)
        {
            Errors = errors;
        }
    }
}
