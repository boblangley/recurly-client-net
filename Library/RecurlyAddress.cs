using System.Xml;

namespace Recurly
{
    public class RecurlyAddress
    {
        public const string ElementName = "account";

        protected const string Address1Element = "address1";
        public string Address1 { get; set; }
        protected const string Address2Element = "address2";
        public string Address2 { get; set; }
        protected const string CityElement = "city";
        public string City { get; set; }
        protected const string StateElement = "state";
        /// <summary>
        /// 2-letter state or province preferred
        /// </summary>
        public string State { get; set; }
        protected const string ZipElement = "zip";
        public string Zip { get; set; }
        protected const string CountryElement = "country";
        /// <summary>
        /// 2-letter ISO country code
        /// </summary>
        public string Country { get; set; }
        protected const string PhoneElement = "phone";
        public string Phone { get; set; }

        public RecurlyAddress()
        {
        }

        internal RecurlyAddress(XmlTextReader reader)
        {
            ReadXml(reader);
        }

        private void ReadXml(XmlTextReader reader)
        {
            while(reader.Read())
            {
                // End of account element, get out of here
                if(reader.Name == ElementName && reader.NodeType == XmlNodeType.EndElement)
                    break;

                if(reader.NodeType != XmlNodeType.Element) continue;
                ReadAddressElements(reader);
            }
        }

        protected void ReadAddressElements(XmlTextReader reader)
        {
            switch (reader.Name)
            {
                case Address1Element:
                    Address1 = reader.ReadElementContentAsString();
                    break;
                case Address2Element:
                    Address2 = reader.ReadElementContentAsString();
                    break;
                case CityElement:
                    City = reader.ReadElementContentAsString();
                    break;
                case StateElement:
                    State = reader.ReadElementContentAsString();
                    break;
                case ZipElement:
                    Zip = reader.ReadElementContentAsString();
                    break;
                case CountryElement:
                    Country = reader.ReadElementContentAsString();
                    break;
                case PhoneElement:
                    Phone = reader.ReadElementContentAsString();
                    break;
            }
        }

        public void WriteXml(XmlTextWriter writer)
        {
            writer.WriteStartElement(ElementName);
            WriteAddressElements(writer);
            writer.WriteEndElement();
        }

        protected void WriteAddressElements(XmlTextWriter writer)
        {
            writer.WriteElementString(Address1Element, Address1);
            writer.WriteElementStringAsNillable(Address2Element, Address2);
            writer.WriteElementString(CityElement, City);
            writer.WriteElementString(StateElement, State);
            writer.WriteElementString(ZipElement, Zip);
            writer.WriteElementString(CountryElement, Country);
            writer.WriteElementStringAsNillable(PhoneElement, Phone);
        }
    }
}