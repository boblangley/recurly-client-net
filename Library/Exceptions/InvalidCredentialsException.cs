using System;
using System.Collections.Generic;
using Recurly.Core;

namespace Recurly
{
    /// <summary>
    /// The API credentials for Recurly are invalid.
    /// </summary>
    public class InvalidCredentialsException : RecurlyException
    {
        internal InvalidCredentialsException(List<RecurlyError> errors)
            : base("The API credentials for Recurly are invalid. Please check the credentials and try again.", errors)
        { }
    }
}
