using System;
using System.Collections.Generic;
using Recurly.Core;

namespace Recurly
{
    /// <summary>
    /// The requested object is not defined in Recurly.
    /// </summary>
    public class NotFoundException : RecurlyException
    {
        internal NotFoundException(string message, List<RecurlyError> errors)
            : base(message, errors)
        { }
    }
}
