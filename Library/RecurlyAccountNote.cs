using System;
using System.Linq;
using System.Xml;

namespace Recurly
{
    public class RecurlyAccountNote
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

        private void ReadXml(XmlTextReader reader)
        {
            while(reader.Read())
            {
                if(reader.Name == ElementName && reader.NodeType == XmlNodeType.EndElement)
                    break;

                if(reader.NodeType != XmlNodeType.Element) continue;
                switch(reader.Name)
                {
                    case AccountCodeElement:
                        AccountCode = reader.ReadElementAttribute("href").Split('/').Last();
                        break;
                    case MessageElement:
                        Message = reader.ReadElementContentAsString();
                        break;
                    case CreatedAtElement:
                        CreatedAt = reader.ReadElementContentAsDateTime();
                        break;
                }
            }
        }
    }
}
