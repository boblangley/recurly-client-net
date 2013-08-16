using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Recurly.Core;

namespace Recurly
{
    public class RecurlyInCentsItem
    {
        public string Currency { get; set; }
        public int AmountInCents { get; set; }

        public RecurlyInCentsItem()
        {
        }

        internal RecurlyInCentsItem(XElement element)
        {
            Currency = element.Name.LocalName;
            AmountInCents = element.ToInt();
        }

        internal void WriteXml(XmlTextWriter writer)
        {
            writer.WriteElementString(Currency,AmountInCents.ToString(CultureInfo.InvariantCulture));
        }
    }
}
