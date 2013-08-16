using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Recurly.Core;

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

        internal RecurlyPayPalBillingInfo(string accountCode, XElement element) : base(accountCode, element)
        {
        }

        protected override void ReadElement(XElement element)
        {
            base.ReadElement(element);

            element.ProcessChild(PayPalBillingAgreementIdElement, e =>
                    PayPalBillingAgreementId = e.Value);
        }

        protected override void WriteExtendedElements(XmlTextWriter writer)
        {
        }
    }
}
