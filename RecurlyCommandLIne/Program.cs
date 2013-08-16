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
            //var info = RecurlyBillingInfo.Get("9ec68fc2-0dc0-41b7-9d9e-d8f253b805a2") as RecurlyCreditCardBillingInfo;
            Console.ReadLine();
        }
    }
}
