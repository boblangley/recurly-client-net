using System;
using System.Linq;
using System.Xml;

namespace Recurly
{
    public class RecurlySubscriptionAddon
    {
        public const string ElementName = "subscription_add_on";

        private const string AddonCodeElement = "add_on_code";
        public string AddonCode { get; private set; }
        private const string UnitAmountInCentsElement = "unit_amount_in_cents";
        public int UnitAmountInCents { get; private set; }
        private const string QuantityElement = "quantity";
        public int? Quantity { get; private set; }

        internal RecurlySubscriptionAddon()
        {
            
        }

        internal RecurlySubscriptionAddon(XmlTextReader reader)
        {
            
        }

        public static RecurlySubscriptionAddon Initialize(string addonCode, int unitAmountInCents, int quantity = 1)
        {
            if(quantity < 1)
                throw new ArgumentOutOfRangeException("quantity","Quantity must be 1 or greater");

            return new RecurlySubscriptionAddon()
                {
                    AddonCode = addonCode,
                    UnitAmountInCents = unitAmountInCents,
                    Quantity = quantity > 1 ? quantity : new int?()
                };            
        }

        public void Update(int? unitAmountIntCents = null, int? quantity = null)
        {
            if (unitAmountIntCents.HasValue && unitAmountIntCents.Value > 10000000)
                throw new ArgumentOutOfRangeException("unitAmountIntCents", "Unit amount cannot be greater than 10000000");

            if(unitAmountIntCents.HasValue)
                UnitAmountInCents = unitAmountIntCents.Value;

            if (quantity.HasValue && quantity.Value < 1)
                throw new ArgumentOutOfRangeException("quantity", "Quantity, if provided, must be 1 or greater");

            if (quantity.HasValue)
                Quantity = quantity;
        }

        protected void ReadXml(XmlTextReader reader)
        {
            while(reader.Read())
            {
                // End of subscription element, get out of here
                if(reader.Name == ElementName && reader.NodeType == XmlNodeType.EndElement)
                    break;

                if(reader.NodeType != XmlNodeType.Element) continue;
                switch(reader.Name)
                {
                    case AddonCodeElement:
                        AddonCode = reader.ReadElementContentAsString();
                        break;

                    case UnitAmountInCentsElement:
                        UnitAmountInCents = reader.ReadElementContentAsInt();
                        break;

                    case QuantityElement:
                        Quantity = reader.ReadElementContentAsInt();
                        break;
                }
            }
        }

        internal void WriteXml(XmlTextWriter writer)
        {
            writer.WriteStartElement(ElementName);
                writer.WriteElementString(AddonCodeElement,AddonCode);
                writer.WriteElementString(UnitAmountInCentsElement, UnitAmountInCents.ToString());
                writer.WriteElementIntIfProvided(QuantityElement,Quantity);
            writer.WriteEndElement();
        }
    }
}
