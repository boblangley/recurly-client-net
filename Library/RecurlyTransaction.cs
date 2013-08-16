using System;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using Recurly.Core;
using Recurly.Properties;

namespace Recurly
{
    public class RecurlyTransaction : BaseRecurlyApiObject
    {
        internal const string ElementName = "transaction";

        public class TransactionDetails : BaseRecurlyApiObject
        {
            public class TransactionAccount : BaseRecurlyApiObject
            {
                public class TransactionBillingInfo : BaseRecurlyApiObject
                {
                    private const string FirstNameElement = "first_name";
                    public string FirstName { get; private set; }
                    private const string LastNameElement = "last_name";
                    public string LastName { get; private set; }
                    protected const string Address1Element = "address1";
                    public string Address1 { get; private set; }
                    protected const string Address2Element = "address2";
                    public string Address2 { get; private set; }
                    protected const string CityElement = "city";
                    public string City { get; private set; }
                    protected const string StateElement = "state";
                    public string State { get; private set; }
                    protected const string ZipElement = "zip";
                    public string Zip { get; private set; }
                    protected const string CountryElement = "country";
                    public string Country { get; private set; }
                    protected const string PhoneElement = "phone";
                    public string Phone { get; private set; }
                    private const string VatNumberElement = "vat_number";
                    public string VatNumber { get; private set; }
                    private const string CardTypeElement = "card_type";
                    public string CardType { get; private set; }
                    private const string ExpirationMonthElement = "month";
                    public int ExpirationMonth { get; private set; }
                    private const string ExpirationYearElement = "year";
                    public int ExpirationYear { get; private set; }
                    private const string FirstSixElement = "first_six";
                    public string FirstSix { get; private set; }
                    private const string LastFourElement = "last_four";
                    public string LastFour { get; private set; }

                    internal TransactionBillingInfo(XmlTextReader reader)
                    {
                        ReadXml(reader);
                    }

                    protected override string RootElementName
                    {
                        get { return BillingInfoElement; }
                    }

                    protected override void ProcessElement(XElement element)
                    {
                        switch (element.Name.LocalName)
                        {
                            case FirstNameElement:
                                FirstName = element.Value;
                                break;
                            case LastNameElement:
                                LastName = element.Value;
                                break;
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
                            case VatNumberElement:
                                VatNumber = element.Value;
                                break;
                            case FirstSixElement:
                                FirstSix = element.Value;
                                break;
                            case LastFourElement:
                                LastFour = element.Value;
                                break;
                            case CardTypeElement:
                                CardType = element.Value;
                                break;
                            case ExpirationMonthElement:
                                ExpirationMonth = element.ToInt();
                                break;
                            case ExpirationYearElement:
                                ExpirationYear = element.ToInt();
                                break;
                        }
                    }
                }

                private const string AccountCodeElement = "account_code";
                public string AccountCode { get; private set; }
                private const string FirstNameElement = "first_name";
                public string FirstName { get; private set; }
                private const string LastNameElement = "last_name";
                public string LastName { get; private set; }
                private const string CompanyNameElement = "company_name";
                public string CompanyName { get; private set; }
                private const string EmailElement = "email";
                public string Email { get; private set; }

                private const string BillingInfoElement = "billing_info";
                public TransactionBillingInfo BillingInfo { get; private set; }

                internal TransactionAccount(XmlTextReader reader)
                {
                    ReadXml(reader);
                }

                protected override string RootElementName
                {
                    get { return AccountElement; }
                }

                protected override void ProcessElement(XElement element)
                {
                    switch (element.Name.LocalName)
                    {
                        case AccountCodeElement:
                            AccountCode = element.Value;
                            break;

                        case FirstNameElement:
                            FirstName = element.Value;
                            break;

                        case LastNameElement:
                            LastName = element.Value;
                            break;

                        case EmailElement:
                            Email = element.Value;
                            break;

                        case CompanyNameElement:
                            CompanyName = element.Value;
                            break;
                    }
                }

                protected override void ProcessReader(string elementName, XmlTextReader reader)
                {
                    switch(elementName)
                    {
                        case BillingInfoElement:
                            BillingInfo = new TransactionBillingInfo(reader);
                            break;
                    }
                }
            }

            internal const string AccountElement = "account";
            public TransactionAccount Account { get; private set; }

            internal TransactionDetails(XmlTextReader reader)
            {
                ReadXml(reader);
            }

            protected override string RootElementName
            {
                get { return TransactionDetailsElement; }
            }

            protected override void ProcessElement(XElement element) {}

            protected override void ProcessReader(string elementName, XmlTextReader reader)
            {
                switch (elementName)
                {
                    case AccountElement:
                        Account = new TransactionAccount(reader);
                        break;
                }
            }
        }

        public enum TransactionType
        {
            All,
            Authorization,
            Refund,
            Purchase
        }

        public enum TransactionState
        {
            All,
            Sucessful,
            Failed,
            Voided
        }

        private const string AccountCodeElement = "account";
        public string AccountCode { get; private set; }

        private const string InvoiceNumberElement = "invoice";
        public int InvoiceNumber { get; private set; }

        private const string SubscriptionIdElement = "subscription";
        public string SubscriptionId { get; private set; }

        private const string IdElement = "uuid";
        public string Id { get; private set; }
        
        public TransactionType Type { get; private set; }

        private const string ActionElement = "action";
        public string Action { get; private set; }

        private const string AmountInCentsElement = "amount_in_cents";
        public int AmountInCents { get; private set; }

        private const string TaxInCentsElement = "tax_in_cents";
        public int TaxInCents { get; private set; }

        private const string CurrencyElement = "currency";
        public string Currency { get; private set; }

        private const string StatusElement = "status";
        public string Status { get; private set; }

        private const string ReferenceElement = "reference";
        public string Reference { get; private set; }

        private const string TestElement = "test";
        public bool Test { get; private set; }

        private const string VoidableElement = "voidable";
        public bool Voidable { get; private set; }

        private const string RefundableElement = "refundable";
        public bool Refundable { get; private set; }

        private const string CvvResultElement = "cvv_result";
        public string CvvResultCode { get; private set; }
        public string CvvResult { get; private set; }

        private const string AvsResultElement = "avs_result";
        public string AvsResultCode { get; private set; }
        public string AvsResult { get; private set; }

        private const string AvsResultStreetElement = "avs_result_street";
        public string AvsResultStreet { get; private set; }

        private const string AvsResultPostalElement = "avs_result_postal";
        public string AvsResultPostal { get; private set; }

        private const string CreatedAtElement = "created_at";
        public DateTime CreatedAt { get; private set; }

        private const string TransactionErrorElement = "transaction_error";
        public string TransactionError { get; private set; }

        private const string TransactionDetailsElement = "details";
        /// <summary>
        /// The details section contains the account and billing information at the time the transaction was submitted. It may not reflect the latest account information.
        /// </summary>
        public TransactionDetails Details { get; private set; }

        internal RecurlyTransaction()
        {
        }

        internal RecurlyTransaction(XmlTextReader reader)
        {
            ReadXml(reader);
        }

        /// <summary>
        /// Lists details for an individual transaction.
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public static RecurlyTransaction Get(string transactionId)
        {
            var transaction = new RecurlyTransaction();

            var statusCode = RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Get,
                String.Format(Settings.Default.PathTransactionGet, transactionId.UrlEncode()),
                transaction.ReadXml);

            return statusCode == HttpStatusCode.OK ? transaction : null;
        }

        /// <summary>
        /// If the transaction has not settled and you perform a full refund, the transaction will be voided instead. Voided transactions typically do not show up on the customer's card statement. If the transaction has settled, a refund will be performed.
        /// </summary>
        public void Refund()
        {
            RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Delete,
                                         String.Format(Settings.Default.PathTransactionFullRefund, Id.UrlEncode()));
        }

        /// <summary>
        /// If the transaction has not settled and you attempt a partial refund, the request will fail. Please wait until the transaction has settled (typically 24 hours) before performing a partial refund. This advice varies depending on when your payment gateway settles the transaction.
        /// </summary>
        /// <param name="amountInCents"></param>
        public void Refund(int amountInCents)
        {
            RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Delete,
                                         String.Format(Settings.Default.PathTransactionPartialRefund, Id.UrlEncode(), amountInCents));
        }

        #region Read and Write XML documents

        protected override string RootElementName
        {
            get { return ElementName; }
        }

        protected override void ProcessElement(XElement element)
        {
            switch (element.Name.LocalName)
            {
                case IdElement:
                    Id = element.Value;
                    break;
                case AccountCodeElement:
                    AccountCode = element.GetHrefLinkId();
                    break;
                case InvoiceNumberElement:
                    InvoiceNumber = element.GetHrefLinkId(int.Parse);
                    break;
                case SubscriptionIdElement:
                    SubscriptionId = element.GetHrefLinkId();
                    break;
                case ElementName:
                    Type = element.Attribute("type").ToEnum<TransactionType>();
                    break;
                case ActionElement:
                    Action = element.Value;
                    break;
                case AmountInCentsElement:
                    AmountInCents = element.ToInt();
                    break;
                case TaxInCentsElement:
                    TaxInCents = element.ToInt();
                    break;
                case CurrencyElement:
                    Currency = element.Value;
                    break;
                case StatusElement:
                    Status = element.Value;
                    break;
                case ReferenceElement:
                    Reference = element.Value;
                    break;
                case TestElement:
                    Test = element.ToBool();
                    break;
                case VoidableElement:
                    Voidable = element.ToBool();
                    break;
                case RefundableElement:
                    Refundable = element.ToBool();
                    break;
                case CvvResultElement:
                    CvvResultCode = element.Attribute("code").Value;
                    CvvResult = element.Value;
                    break;
                case AvsResultElement:
                    AvsResultCode = element.Attribute("code").Value;
                    AvsResult = element.Value;
                    break;
                case AvsResultStreetElement:
                    AvsResultStreet = element.Value;
                    break;
                case AvsResultPostalElement:
                    AvsResultPostal = element.Value;
                    break;
                case CreatedAtElement:
                    CreatedAt = element.ToDateTime();
                    break;
                case TransactionErrorElement:
                    TransactionError = element.Value;
                    break;
            }
        }

        protected override void ProcessReader(string elementName, XmlTextReader reader)
        {
            switch(elementName)
            {
                case TransactionDetailsElement:
                    Details = new TransactionDetails(reader);
                    break;
            }
        }

        #endregion

        #region Object Overrides

        public override string ToString()
        {
            return "Recurly Transaction: " + Id;
        }

        public override bool Equals(object obj)
        {
            var a = obj as RecurlyTransaction;
            return a != null && Equals(a);
        }

        public bool Equals(RecurlyTransaction transaction)
        {
            return Id == transaction.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        #endregion
    }
}