using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Recurly
{
    public class RecurlyInCentsItem
    {
        public string Currency { get; set; }
        public int AmountInCents { get; set; }

        public RecurlyInCentsItem()
        {
        }

        internal RecurlyInCentsItem(XmlTextReader reader)
        {
            Currency = reader.Name;
            AmountInCents = reader.ReadElementContentAsInt();
        }

        internal void WriteXml(XmlTextWriter writer)
        {
            writer.WriteElementString(Currency,AmountInCents.ToString(CultureInfo.InvariantCulture));
        }
    }
}
