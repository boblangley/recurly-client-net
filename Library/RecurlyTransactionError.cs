using System.Xml.Linq;
using Recurly.Core;

namespace Recurly
{
    public class RecurlyTransactionError
    {
        public string Code { get; private set; }
        public string Category { get; private set; }
        public string MerchantMessage { get; private set; }
        public string CustomerMessage { get; private set; }

        internal RecurlyTransactionError(XElement element)
        {
            element.ProcessChild("error_code", e => Code = e.Value);
            element.ProcessChild("error_category", e => Category = e.Value);
            element.ProcessChild("merchant_message", e => MerchantMessage = e.Value);
            element.ProcessChild("customer_message", e => CustomerMessage = e.Value);
        }
    }
}