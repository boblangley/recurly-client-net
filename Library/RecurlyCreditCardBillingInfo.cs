using System;
using System.Globalization;
using System.Xml;

namespace Recurly
{
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

        protected override void ReadExtendedElements(XmlTextReader reader)
        {
            switch (reader.Name)
            {
                case FirstSixElement:
                    FirstSix = reader.ReadElementContentAsString();
                    break;

                case LastFourElement:
                    LastFour = reader.ReadElementContentAsString();
                    break;

                case CardTypeElement:
                    CardType = reader.ReadElementContentAsString();
                    break;

                case ExpirationMonthElement:
                    ExpirationMonth = reader.ReadElementContentAsInt();
                    break;

                case ExpirationYearElement:
                    ExpirationYear = reader.ReadElementContentAsInt();
                    break;
            }
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
