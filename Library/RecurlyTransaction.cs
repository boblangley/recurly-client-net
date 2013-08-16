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

        public class TransactionDetails
        {
            public class TransactionAccount
            {
                public class TransactionBillingInfo
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

                    internal TransactionBillingInfo(XElement element)
                    {
                        element.ProcessChild(FirstNameElement, e =>
                            FirstName = e.Value);

                        element.ProcessChild(LastNameElement, e =>
                            LastName = e.Value);

                        element.ProcessChild(Address1Element, e =>
                            Address1 = e.Value);

                        element.ProcessChild(Address2Element, e =>
                            Address2 = e.Value);

                        element.ProcessChild(CityElement, e =>
                            City = e.Value);

                        element.ProcessChild(StateElement, e =>
                            State = e.Value);

                        element.ProcessChild(ZipElement, e =>
                            Zip = e.Value);

                        element.ProcessChild(CountryElement, e =>
                            Country = e.Value);

                        element.ProcessChild(PhoneElement, e =>
                            Phone = e.Value);

                        element.ProcessChild(VatNumberElement, e =>
                            VatNumber = e.Value);

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

                internal TransactionAccount(XElement element)
                {
                    element.ProcessChild(AccountCodeElement, e =>
                        AccountCode = e.Value);

                    element.ProcessChild(FirstNameElement, e =>
                        FirstName = e.Value);

                    element.ProcessChild(LastNameElement, e =>
                        LastName = e.Value);

                    element.ProcessChild(EmailElement, e =>
                        Email = e.Value);

                    element.ProcessChild(CompanyNameElement, e =>
                        CompanyName = e.Value);

                    element.ProcessChild(BillingInfoElement, e =>
                        BillingInfo = new TransactionBillingInfo(e));
                }
            }

            internal const string AccountElement = "account";
            public TransactionAccount Account { get; private set; }

            internal TransactionDetails(XElement element)
            {
                element.ProcessChild(AccountElement, e => Account = new TransactionAccount(e));
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

        internal RecurlyTransaction(XElement element)
        {
            ReadElement(element);
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

        protected override void ReadElement(XElement element)
        {
            element.ProcessChild(IdElement, e =>
                Id = e.Value);

            element.ProcessChild(AccountCodeElement, e =>
                AccountCode = e.GetHrefLinkId());

            element.ProcessChild(InvoiceNumberElement, e =>
                InvoiceNumber = e.GetHrefLinkId(int.Parse));

            element.ProcessChild(SubscriptionIdElement, e =>
                SubscriptionId = e.GetHrefLinkId());

            element.ProcessChild(ElementName, e =>
                Type = e.Attribute("type").ToEnum<TransactionType>());

            element.ProcessChild(ActionElement, e =>
                Action = e.Value);

            element.ProcessChild(AmountInCentsElement, e =>
                AmountInCents = e.ToInt());

            element.ProcessChild(TaxInCentsElement, e =>
                TaxInCents = e.ToInt());

            element.ProcessChild(CurrencyElement, e =>
                Currency = e.Value);

            element.ProcessChild(StatusElement, e =>
                Status = e.Value);

            element.ProcessChild(ReferenceElement, e =>
                Reference = e.Value);

            element.ProcessChild(TestElement, e =>
                Test = e.ToBool());

            element.ProcessChild(VoidableElement, e =>
                Voidable = e.ToBool());

            element.ProcessChild(RefundableElement, e =>
                Refundable = e.ToBool());

            element.ProcessChild(CvvResultElement, e =>
                {
                    CvvResultCode = e.Attribute("code").Value;
                    CvvResult = e.Value;
                });

            element.ProcessChild(AvsResultElement, e =>
                {
                    AvsResultCode = e.Attribute("code").Value;
                    AvsResult = e.Value;
                });

            element.ProcessChild(AvsResultStreetElement, e =>
                AvsResultStreet = e.Value);

            element.ProcessChild(AvsResultPostalElement, e =>
                AvsResultPostal = e.Value);

            element.ProcessChild(CreatedAtElement, e =>
                CreatedAt = e.ToDateTime());

            element.ProcessChild(TransactionErrorElement, e =>
                TransactionError = e.Value);

            element.ProcessChild(TransactionDetailsElement, e =>
                Details = new TransactionDetails(e));
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