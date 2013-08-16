using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Recurly;

namespace RecurlyCommandLIne
{
    class Program
    {
        static void Main(string[] args)
        {
            var subs = RecurlySubscriptionList.GetAccountSubscriptions("ft2137").NextPage();            

            Console.ReadLine();
        }
    }
}
