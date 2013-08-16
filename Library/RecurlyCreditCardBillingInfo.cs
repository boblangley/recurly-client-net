using System;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using Recurly.Core;

namespace Recurly
{
    /// <summary>
    /// Billing Info with credit card information
    /// </summary>
    public class RecurlyCreditCardBillingInfo : RecurlyBillingInfo
    {
        private const string FirstSixElement = "first_six";
        public string FirstSix { get; private set; }
        private const string LastFourElement = "last_four";
        public string LastFour { get; private set; }
        private const string CardTypeElement = "card_type";
        public string CardType { get; private set; }
        private const string ExpirationMonthElement = "month";
        public int ExpirationMonth { get; set; }
        private const string ExpirationYearElement = "year";
        public int ExpirationYear { get; set; }

        public override string Type
        {
            get { return "credit_card"; }
        }

        private const string NumberElement = "number";
        private string _number;
        public string Number {
            set { _number = value; }
        }
        private const string VerificationValueElement = "verification_value";
        private string _verificationValue;
        public string VerificationValue
        {
            set { _verificationValue = value; }
        }

        public RecurlyCreditCardBillingInfo(string accountCode) : base(accountCode)
        {
        }

        public RecurlyCreditCardBillingInfo(RecurlyAccount account) : base(account.AccountCode)
        {
        }

        internal RecurlyCreditCardBillingInfo(string accountCode, XElement element) : base(accountCode, element)
        {
        }

        protected override void ReadElement(XElement element)
        {
            base.ReadElement(element);

            element.ProcessChild(FirstSixElement, e =>
                FirstSix = e.Value);

            element.ProcessChild(LastFourElement, e =>
                LastFour = e.Value);

            element.ProcessChild(CardTypeElement, e =>
                CardType = e.Value);

            element.ProcessChild(ExpirationMonthElement, e =>
                ExpirationMonth = e.ToInt());

            element.ProcessChild(ExpirationYearElement, e =>
                ExpirationYear = e.ToInt());
        }

        protected override void WriteExtendedElements(XmlTextWriter writer)
        {
            writer.WriteElementString(NumberElement,_number);
            _number = null;
            writer.WriteElementString(VerificationValueElement,_verificationValue);
            _verificationValue = null;
            writer.WriteElementString(ExpirationMonthElement, ExpirationMonth.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString(ExpirationYearElement, ExpirationYear.ToString(CultureInfo.InvariantCulture));
        }
    }
}
