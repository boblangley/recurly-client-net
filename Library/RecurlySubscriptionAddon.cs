using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Recurly.Core;

namespace Recurly
{
    public class RecurlySubscriptionAddon : BaseRecurlyApiObject
    {
        internal const string ElementName = "subscription_add_on";

        private const string AddonCodeElement = "add_on_code";
        public string AddonCode { get; private set; }
        private const string UnitAmountInCentsElement = "unit_amount_in_cents";
        public int UnitAmountInCents { get; private set; }
        private const string QuantityElement = "quantity";
        public int? Quantity { get; private set; }

        internal RecurlySubscriptionAddon()
        {
            
        }

        internal RecurlySubscriptionAddon(XElement element)
        {
            ReadElement(element);
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

        protected override void ReadElement(XElement element)
        {
            element.ProcessChild(AddonCodeElement, e =>
                AddonCode = e.Value);

            element.ProcessChild(UnitAmountInCentsElement, e =>
                UnitAmountInCents = e.ToInt());

            element.ProcessChild(QuantityElement, e =>
                Quantity = e.ToInt());
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
