using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recurly
{
    public interface IRecurlyPagedList<T>
    {
        List<T> NextPage();
        bool EndOfPages { get; }
    }
}
