using System;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using Recurly.Core;
using Recurly.Properties;

namespace Recurly
{
    /// <summary>
    /// An account in Recurly.
    /// </summary>
    public class RecurlyAccount : BaseRecurlyApiObject
    {
        /// <summary>
        /// State of the account
        /// </summary>
        public enum AccountState
        {
            Active,
            Closed,
            Past_Due
        }
        
        internal const string ElementName = "account";

        private const string AccountCodeElement = "account_code";
        public string AccountCode { get; private set; }
        private const string EmailElement = "email";
        public string Email { get; set; }
        private const string UsernameElement = "username";
        public string Username { get; set; }
        private const string FirstNameElement = "first_name";
        public string FirstName { get; set; }
        private const string LastNameElement = "last_name";
        public string LastName { get; set; }

        public const string StateElement = "state";
        public AccountState State { get; private set; }

        //public string VatNumber { get; set; }
        private const string CompanyNameElement = "company_name";
        public string CompanyName { get; set; }
        private const string AcceptLanguageElement = "accept_language";
        public string AcceptLanguage { get; set; }

        private const string CreatedAtElement = "created_at";
        public DateTime CreatedAt { get; private set; }
                
        public RecurlyAddress Address { get; set; }

        private RecurlyBillingInfo _billingInfo;

        private const string HostedLoginTokenElement = "hosted_login_token";
        public string HostedLoginToken { get; private set; }

        internal const string UrlPrefix = "/accounts/";

        public RecurlyAccount(string accountCode)
        {
            AccountCode = accountCode;
        }

        internal RecurlyAccount(XElement element)
        {
            ReadElement(element);
        }

        private RecurlyAccount()
        { }

        /// <summary>
        /// Lookup a Recurly account
        /// </summary>
        /// <param name="accountCode"></param>
        /// <returns></returns>
        public static RecurlyAccount Get(string accountCode)
        {
            var account = new RecurlyAccount();

            var statusCode = RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Get,
                UrlPrefix + System.Web.HttpUtility.UrlEncode(accountCode),
                account.ReadXml);

            return statusCode == HttpStatusCode.NotFound ? null : account;
        }

        /// <summary>
        /// Create a new account in Recurly
        /// </summary>
        public bool Create(RecurlyBillingInfo billingInfo = null)
        {
            _billingInfo = billingInfo;

            var statusCode = RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Post, 
                Settings.Default.PathAccountCreate,
                writer => WriteXml(writer),
                ReadXml,
                null);

            return statusCode == HttpStatusCode.Created;
        }

        /// <summary>
        /// Update an existing account in Recurly
        /// </summary>
        public bool Update(RecurlyBillingInfo billingInfo = null)
        {
            _billingInfo = billingInfo;

            var statusCode = RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Put, 
                String.Format(Settings.Default.PathAccountUpdate, System.Web.HttpUtility.UrlEncode(AccountCode)),
                writer => WriteXml(writer,false));

            return RecurlyClient.OkOrAccepted(statusCode);
        }

        /// <summary>
        /// Marks an account as closed and cancels any active subscriptions. Any saved billing information will also be permanently removed from the account.
        /// </summary>
        public bool Close()
        {
            var statusCode = RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Delete,
                                         String.Format(Settings.Default.PathAccountClose, AccountCode));

            return RecurlyClient.OkOrAccepted(statusCode);
        }

        /// <summary>
        ///  Transitions a closed account back to active.
        /// </summary>
        public bool Reopen()
        {
            var statusCode = RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Put,
                                         String.Format(Settings.Default.PathAccountReopen, AccountCode));

            return RecurlyClient.OkOrAccepted(statusCode);
        }

        /// <summary>
        /// Retrieves the accounts BillingInfo
        /// </summary>
        /// <returns></returns>
        public RecurlyBillingInfo GetBillingInfo()
        {
            return RecurlyBillingInfo.Get(AccountCode);
        }

        /// <summary>
        /// Deletes the account BillingInfo
        /// </summary>
        public void DeleteBillingInfo()
        {
            RecurlyBillingInfo.DeleteBillingInfo(AccountCode);
        }

        /// <summary>
        /// Returns a list of the notes on an account sorted in descending order.
        /// </summary>
        /// <returns>Paged list of notes</returns>
        public RecurlyAccountNoteList ListAccountNotes()
        {
            return RecurlyAccountNoteList.ListAccountNotes(AccountCode);
        }

        /// <summary>
        /// Lists all charges and credits issued for a given account.
        /// </summary>
        /// <param name="type">The type of adjustment to return</param>
        /// <param name="state">The state of the adjustments to return</param>
        /// <param name="pageSize">Number of records to return per page</param>
        /// <returns>Paged list of adjustments</returns>
        public RecurlyAdjustmentList ListAdjustments(
            RecurlyAdjustment.AdjustmentType type = RecurlyAdjustment.AdjustmentType.All,
            RecurlyAdjustment.AdjustmentState state = RecurlyAdjustment.AdjustmentState.Active,
            int pageSize = RecurlyPager.DefaultPageSize)
        {
            return RecurlyAdjustmentList.ListAccountAdjustments(AccountCode, type, state, pageSize);
        }        

        private RecurlyAdjustment Adjust(int unitAmountInCents, string description, string accountingCode = null,
                                        string currency = null, int quantity = 1)
        {
            var adjustment = new RecurlyAdjustment(AccountCode, unitAmountInCents)
                {
                    Description = description,
                    Currency = string.IsNullOrWhiteSpace(currency) ? RecurlyClient.Currency : currency,
                    Quantity = quantity
                };
            return adjustment.Create() ? adjustment : null;
        }

        /// <summary>
        /// Make a charge to the account
        /// </summary>
        /// <param name="unitAmountInCents"></param>
        /// <param name="description"></param>
        /// <param name="accountingCode"></param>
        /// <param name="currency"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public RecurlyAdjustment Charge(int unitAmountInCents, string description, string accountingCode = null,
                                string currency = null, int quantity = 1)
        {
            if (unitAmountInCents < 1) throw new ArgumentOutOfRangeException("unitAmountInCents","Unit amount must be greater than 0");
            return Adjust(unitAmountInCents, description, accountingCode, currency, quantity);
        }

        /// <summary>
        /// Apply a credit to the account
        /// </summary>
        /// <param name="unitAmountInCents"></param>
        /// <param name="description"></param>
        /// <param name="accountingCode"></param>
        /// <param name="currency"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public RecurlyAdjustment Credit(int unitAmountInCents, string description, string accountingCode = null,
                                        string currency = null, int quantity = 1)
        {
            if(unitAmountInCents > -1) throw new ArgumentOutOfRangeException("unitAmountInCents", "Unit amount must be less than 0");
            return Adjust(unitAmountInCents, description, accountingCode, currency, quantity);
        }

        /// <summary>
        /// Lookup information about the 'active' coupon redemption on an account
        /// </summary>
        /// <returns></returns>
        public RecurlyCouponRedemption GetCouponRedemption()
        {
            return RecurlyCouponRedemption.GetAccountRedemption(AccountCode);
        }

        /// <summary>
        /// Occasionally, you may want to remove a coupon from an account.
        /// </summary>
        public void RemoveCouponRedemption()
        {
            var redemption = RecurlyCouponRedemption.GetAccountRedemption(AccountCode);
            redemption.Remove();
        }

        /// <summary>
        /// Returns a list of transactions for an account.
        /// </summary>
        /// <param name="state">The state of transactions to return</param>
        /// <param name="type">The type of transactions to return</param>
        /// <param name="pageSize">Number of records to return per page</param>
        /// <returns></returns>
        public RecurlyTransactionList ListTransactions(RecurlyTransaction.TransactionState state =
                                                           RecurlyTransaction.TransactionState.All,
                                                       RecurlyTransaction.TransactionType type =
                                                           RecurlyTransaction.TransactionType.All,
                                                       int pageSize = RecurlyPager.DefaultPageSize)
        {
            return RecurlyTransactionList.ListAccountTransactions(AccountCode, state, type, pageSize);
        }

        /// <summary>
        /// Returns a list of subscriptions for an account.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public RecurlySubscriptionList ListSubscriptions(RecurlySubscription.SubscriptionState state = RecurlySubscription.SubscriptionState.Live, int pageSize = RecurlyPager.DefaultPageSize)
        {
            return RecurlySubscriptionList.GetAccountSubscriptions(AccountCode, state, pageSize);
        }

        #region Read and Write XML documents

        protected override void ReadElement(XElement element)
        {
                element.ProcessChild(AccountCodeElement,e => AccountCode = e.Value);
                element.ProcessChild(StateElement, e => State = e.ToEnum<AccountState>());
                element.ProcessChild(UsernameElement, e => Username = e.Value);
                element.ProcessChild(FirstNameElement, e => FirstName = e.Value);
                element.ProcessChild(LastNameElement, e => LastName = e.Value);
                element.ProcessChild(EmailElement, e => Email = e.Value);
                element.ProcessChild(CompanyNameElement, e => CompanyName = e.Value);
                element.ProcessChild(AcceptLanguageElement, e => AcceptLanguage = e.Value);
                element.ProcessChild(HostedLoginTokenElement, e => HostedLoginToken = e.Value);
                element.ProcessChild(CreatedAtElement, e => CreatedAt = e.ToDateTime());
                element.ProcessChild(RecurlyAddress.ElementName, e => Address = new RecurlyAddress(e));
        }

        internal void WriteXml(XmlTextWriter xmlWriter, bool includeAccountCode = true)
        {
            xmlWriter.WriteStartElement(ElementName);
                if (includeAccountCode)    
                    xmlWriter.WriteElementString(AccountCodeElement, AccountCode);
                xmlWriter.WriteElementStringIfProvided(UsernameElement, Username);
                xmlWriter.WriteElementString(EmailElement, Email);
                xmlWriter.WriteElementString(FirstNameElement, FirstName);
                xmlWriter.WriteElementString(LastNameElement, LastName);
                xmlWriter.WriteElementStringIfProvided(CompanyNameElement, CompanyName);
                xmlWriter.WriteElementStringIfProvided(AcceptLanguageElement, AcceptLanguage);
                if (_billingInfo != null)
                    _billingInfo.WriteXml(xmlWriter);
                if (Address != null)
                    Address.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement(); // End: account
        }

        #endregion

        #region Object Overrides

        public override string ToString()
        {
            return "Recurly Account: " + AccountCode;
        }

        public override bool Equals(object obj)
        {
            var a = obj as RecurlyAccount;
            return a != null && Equals(a);
        }

        public bool Equals(RecurlyAccount account)
        {
            return AccountCode == account.AccountCode;
        }

        public override int GetHashCode()
        {
            return AccountCode.GetHashCode();
        }

        #endregion
    }
}