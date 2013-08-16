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

        internal RecurlyAccountNote(XmlTextReader reader)
        {
            ReadXml(reader);
        }

        protected override string RootElementName
        {
            get { return ElementName; }
        }

        protected override void ProcessElement(XElement element)
        {
            switch (element.Name.LocalName)
            {
                case AccountCodeElement:
                    AccountCode = element.Attribute("href").Value.Split('/').Last();
                    break;
                case MessageElement:
                    Message = element.Value;
                    break;
                case CreatedAtElement:
                    CreatedAt = element.ToDateTime();
                    break;
            }
        }

        private void ReadXml(XmlTextReader reader)
        {
            while(reader.Read())
            {
                if(reader.Name == ElementName && reader.NodeType == XmlNodeType.EndElement)
                    break;

                if(reader.NodeType != XmlNodeType.Element) continue;

            }
        }
    }
}
