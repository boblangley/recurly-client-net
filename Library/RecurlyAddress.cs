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

        internal RecurlyAddress(XElement element)
        {
            ReadElement(element);
        }

        protected override void ReadElement(XElement element)
        {
            
            element.ProcessChild(Address1Element, e => Address1 = e.Value);
            element.ProcessChild(Address2Element, e => Address2 = e.Value);
            element.ProcessChild(CityElement, e => City = e.Value);
            element.ProcessChild(StateElement, e => State = e.Value);
            element.ProcessChild(ZipElement, e => Zip = e.Value);
            element.ProcessChild(CountryElement, e => Country = e.Value);
            element.ProcessChild(PhoneElement, e => Phone = e.Value);
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