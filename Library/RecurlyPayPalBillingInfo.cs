using System;
using System.Linq;
using System.Xml;

namespace Recurly
{
    public class RecurlyPayPalBillingInfo : RecurlyBillingInfo
    {
        private const string PayPalBillingAgreementIdElement = "paypal_billing_agreement_id";
        public string PayPalBillingAgreementId { get; private set; }

        public override string Type
        {
            get { return "paypal"; }
        }

        internal RecurlyPayPalBillingInfo(string accountCode) : base(accountCode)
        {
        }

        internal RecurlyPayPalBillingInfo(RecurlyAccount account)
            : base(account.AccountCode)
        {
        }

        protected override void ReadExtendedElements(XmlTextReader reader)
        {
            switch (reader.Name)
            {
                case PayPalBillingAgreementIdElement:
                    PayPalBillingAgreementId = reader.ReadElementContentAsString();
                    break;
            }
        }

        protected override void WriteExtendedElements(XmlTextWriter writer)
        {
        }
    }
}
