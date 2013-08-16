using System.Xml;
using System.Xml.Linq;
using Recurly.Core;

namespace Recurly
{
    public class RecurlyAddress : BaseRecurlyApiObject
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

        protected override string RootElementName
        {
            get { return ElementName; }
        }

        protected override void ProcessElement(XElement element)
        {
            switch (element.Name.LocalName)
            {
                case Address1Element:
                    Address1 = element.Value;
                    break;
                case Address2Element:
                    Address2 = element.Value;
                    break;
                case CityElement:
                    City = element.Value;
                    break;
                case StateElement:
                    State = element.Value;
                    break;
                case ZipElement:
                    Zip = element.Value;
                    break;
                case CountryElement:
                    Country = element.Value;
                    break;
                case PhoneElement:
                    Phone = element.Value;
                    break;
            }
        }

        internal void WriteXml(XmlTextWriter writer)
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