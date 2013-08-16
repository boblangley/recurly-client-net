using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Recurly.Core;

namespace Recurly
{
    public class RecurlyAccountNote : BaseRecurlyApiObject
    {
        public const string ElementName = "note";

        private const string AccountCodeElement = "account";
        public string AccountCode { get; private set; }

        private const string MessageElement = "message";
        public string Message { get; private set; }

        private const string CreatedAtElement = "created_at";
        public DateTime CreatedAt { get; private set; }

        internal RecurlyAccountNote(XElement element)
        {
            ReadElement(element);
        }

        protected override void ReadElement(XElement element)
        {
            element.ProcessChild(AccountCodeElement, e => AccountCode = e.GetHrefLinkId());
            element.ProcessChild(MessageElement, e => Message = e.Value);
            element.ProcessChild(CreatedAtElement, e => CreatedAt = e.ToDateTime());
        }
    }
}
