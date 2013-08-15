using System;
using System.Collections.Generic;
using System.Linq;

namespace Recurly
{
    public interface IRecurlyPagedList<T>
    {
        List<T> NextPage();
        bool EndOfPages { get; }
    }
}
