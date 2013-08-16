using System;
using System.Linq;
using System.Xml;

namespace Recurly
{
    /// <summary>
    /// Read-only payment details for PayPal
    /// </summary>
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

        protected override void ProcessElement(System.Xml.Linq.XElement element)
        {
            base.ProcessElement(element);
            switch (element.Name.LocalName)
            {
                case PayPalBillingAgreementIdElement:
                    PayPalBillingAgreementId = element.Value;
                    break;
            }
        }

        protected override void WriteExtendedElements(XmlTextWriter writer)
        {
        }
    }
}
