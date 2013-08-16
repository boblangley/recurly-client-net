using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using Recurly.Core;
using Recurly.Properties;

namespace Recurly
{
    /// <summary>
    /// A Subscription
    /// </summary>
    public class RecurlySubscription : BaseRecurlyApiObject
    {
        internal static string ElementName = "subscription";

        private string _elementName = ElementName;

        protected override string RootElementName
        {
            get { return _elementName; }
        }

        /// <summary>
        /// The state of subscription
        /// </summary>
        public enum SubscriptionState
        {
            Active,
            Canceled,
            Future,
            Expired,
            In_Trial,
            Past_Due,
            Live
        }

        public enum ChangeTimeframe
        {
            Now,
            Renewal
        }

        public enum RefundType
        {
            Full,
            Partial,
            None
        }

        public enum CollectionMethods
        {
            Automatic,
            Manual
        }

        /// <summary>
        /// 
        /// </summary>
        public class RecurlyManualInvoiceDetails
        {            
            private const string NetTermsElement = "net_terms";
            public int NetTerms { get; set; }
            private const string PurchaseOrderNumberElement = "po_number";
            public string PurchaseOrderNumber { get; set; }

            internal static void ProcessElement(XElement element, RecurlySubscription sub)
            {
                switch(element.Name.LocalName)
                {
                    case NetTermsElement:
                        if(sub.ManualInvoiceDetails == null)
                            sub.ManualInvoiceDetails = new RecurlyManualInvoiceDetails();
                        sub.ManualInvoiceDetails.NetTerms = element.ToInt();
                        break;
                    case PurchaseOrderNumberElement:
                        if(sub.ManualInvoiceDetails == null)
                            sub.ManualInvoiceDetails = new RecurlyManualInvoiceDetails();
                        sub.ManualInvoiceDetails.PurchaseOrderNumber = element.Value;
                        break;
                }
            }

            internal void WriteXmlElements(XmlTextWriter writer)
            {
                writer.WriteElementString(NetTermsElement,NetTerms.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementStringIfProvided(PurchaseOrderNumberElement,PurchaseOrderNumber);
            }
        }

        private const string PlanCodeElement = "plan_code";
        public string PlanCode { get; set; }
        private const string IdElement = "uuid";
        public string Id { get; private set; }
        private const string StateElement = "state";
        public SubscriptionState State { get; private set; }
        private const string UnitAmountInCentsElement = "unit_amount_in_cents";
        public int? UnitAmountInCents { get; set; }
        private const string QuantityElement = "quantity";
        public int Quantity { get; set; }
        private const string CurrencyElement = "currency";
        public string Currency { get; set; }
        private const string ActivatedAtElement = "activated_at";
        private const string StartsAtElement = "starts_at";
        public DateTime ActivatedAt { get; private set; }
        private const string CanceledAtElement = "canceled_at";
        public DateTime? CanceledAt { get; private set; }
        private const string ExpiresAtElement = "expires_at";
        public DateTime? ExpiresAt { get; private set; }
        private const string CurrentPeriodStartedAtElement = "current_period_started_at";
        public DateTime? CurrentPeriodStartedAt { get; private set; }
        private const string CurrentPeriodEndsAtElement = "current_period_ends_at";
        public DateTime? CurrentPeriodEndsAt { get; private set; }
        private const string TrialStartedAtElement = "trial_started_at";
        public DateTime? TrialStartedAt { get; private set; }
        private const string TrialEndsAtElement = "trial_ends_at";
        private const string AccountCodeElement = "account";
        public string AccountCode { get; private set; }
        private const string AddonsElement = "subscription_add_ons";
        public List<RecurlySubscriptionAddon> Addons { get; set; }
        private const string PendingSubscriptionElement = "pending_subscription";
        public RecurlySubscription PendingSubscription { get; private set; }
        private const string CollectionMethodElement = "collection_method";
        public CollectionMethods CollectionMethod { get; set; }
        protected RecurlyManualInvoiceDetails ManualInvoiceDetails { get; set; }

        #region Element Strings for Create
        private const string CouponCodeElement = "coupon_code";
        private const string TotalBillingCyclesElement = "total_billing_cycles";
        private const string FirstRenewalDateElement = "first_renewal_date";
        #endregion

        /// <summary>
        /// Date the trial ends, if the subscription has/had a trial.
        /// 
        /// This may optionally be set on new subscriptions to specify an exact time for the 
        /// subscription to commence.  The subscription will be active and in "trial" until
        /// this date.
        /// </summary>
        public DateTime? TrialEndsAt
        {
            get { return _trialEndsAt; }
            set
            {
                if (value.HasValue && (value < DateTime.UtcNow))
                    throw new ArgumentException("TrialEndsAt must occur in the future.");

                _trialEndsAt = value;
            }
        }
        private DateTime? _trialEndsAt;

        public RecurlySubscription()
        {
            
            Addons = new List<RecurlySubscriptionAddon>();
            Quantity = 1;
        }

        internal RecurlySubscription(XmlTextReader reader) : this()
        {
            ReadXml(reader);
        }

        /// <summary>
        /// Lookup a subscription's details.
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        public static RecurlySubscription Get(string subscriptionId)
        {
            var sub = new RecurlySubscription();
            var statusCode = RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Get,
                                                          String.Format(Settings.Default.PathSubscriptionGet,
                                                                        subscriptionId),
                                                          sub.ReadXml);

            return statusCode == HttpStatusCode.NotFound ? null : sub;
        }

        /// <summary>
        /// Create a new subscription.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="couponCode"></param>
        /// <param name="currency"></param>
        /// <param name="startsAt"></param>
        /// <param name="billingCycles"></param>
        /// <param name="firstRenewalDate"></param>
        /// <param name="manualInvoice"></param>
        /// <returns></returns>
        public bool Create(RecurlyAccount account, string couponCode = null, string currency = null, DateTime? startsAt = null, int? billingCycles = null, DateTime? firstRenewalDate = null, RecurlyManualInvoiceDetails manualInvoice = null)
        {            
            if (String.IsNullOrWhiteSpace(PlanCode)) throw new InvalidOperationException("PlanCode must be provided in order to create a subscription");

            Currency = String.IsNullOrWhiteSpace(currency) ? RecurlyClient.Currency : currency;

            if (billingCycles == 0)
                throw new ArgumentOutOfRangeException("billingCycles","Billing cycles must be greater than 0 if provided");

            if (Quantity < 1)
                throw new InvalidOperationException("Quantity must be greater than 0");

            if(manualInvoice != null)
            {
                CollectionMethod = CollectionMethods.Manual;
                ManualInvoiceDetails = manualInvoice;
            }

            if (CollectionMethod == CollectionMethods.Automatic && ManualInvoiceDetails != null)
                System.Diagnostics.Debug.WriteLine("Subscription has manual invoice details but is set to automatic collection. ManualInvoiceDetails will be ignored.");

            ActivatedAt = startsAt.HasValue ? startsAt.Value : DateTime.MinValue;

            if (firstRenewalDate.HasValue && startsAt.HasValue && firstRenewalDate.Value < startsAt.Value)
                    throw new ArgumentOutOfRangeException("firstRenewalDate", "First renewal date cannot be before the start date");

            var statusCode = RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Post, Settings.Default.PathSubscriptionCreate,
                                         writer => WriteCreateXml(writer,account,couponCode,billingCycles,firstRenewalDate), ReadXml);

            return statusCode == HttpStatusCode.Created;
        }

        /// <summary>
        /// Request an update to a subscription that takes place immediately or at renewal.
        /// </summary>
        /// <param name="timeframe"></param>
        /// <param name="newPlanCode"></param>
        public void Update(ChangeTimeframe timeframe, string newPlanCode = null)
        {
            if (newPlanCode != null && String.IsNullOrWhiteSpace(newPlanCode)) throw new ArgumentException("newPlanCode", "A new PlanCode cannot be empty if provided");
            
            RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Put,
                                         String.Format(Settings.Default.PathSubscriptionUpdate, Id),
                                         writer => WriteChangeXml(writer, timeframe, newPlanCode), ReadXml);
        }

        /// <summary>
        /// Cancel a subscription so it remains active and then expires at the end of the current bill cycle.
        /// </summary>
        public void Cancel()
        {
            RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Put,
                                         String.Format(Settings.Default.PathSubscriptionCancel, Id));
        }

        /// <summary>
        /// Reactivate a canceled subscription so it renews at the end of the current bill cycle.
        /// </summary>
        public void Reactivate()
        {
            RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Put,
                                         String.Format(Settings.Default.PathSubscriptionReactivate, Id));
        }

        /// <summary>
        /// You may remove any stored billing information for an account. If the account has a subscription, the renewal will go into past due unless you update the billing info before the renewal occurs.
        /// </summary>
        /// <param name="refund"></param>
        public void Terminate(RefundType refund)
        {
            RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Put,
                                         String.Format(Settings.Default.PathSubscriptionTerminate, Id,
                                                       Enum.GetName(refund.GetType(), refund).ToLower()));
        }

        /// <summary>
        /// Delays renewal of a subscription
        /// </summary>
        /// <param name="nextRenewalDate"></param>
        public void Postpone(DateTime nextRenewalDate)
        {
            if (nextRenewalDate.Date <= DateTime.Now.Date) throw new ArgumentOutOfRangeException("nextRenewalDate","Renewal date must be in the future to postpone");
            RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Put,
                                         String.Format(Settings.Default.PathSubscriptionPostpone, Id,
                                                       nextRenewalDate.ToString("s")));
        }

        #region Read and Write XML documents

        protected override void PreLoopInitializion()
        {
            Addons.Clear();
        }

        protected override void ProcessElement(XElement element)
        {
            switch(element.Name.LocalName)
            {
                case AccountCodeElement:
                    AccountCode = element.Attribute("href").Value.Split('/').Last();
                    break;

                case PlanCodeElement:
                    PlanCode = element.Value;
                    break;

                case IdElement:
                    Id = element.Value;
                    break;

                case StateElement:
                    State = element.ToEnum<SubscriptionState>();
                    break;

                case UnitAmountInCentsElement:
                    UnitAmountInCents = element.ToInt();
                    break;

                case CurrencyElement:
                    Currency = element.Value;
                    break;

                case QuantityElement:
                    Quantity = element.ToInt();
                    break;

                case ActivatedAtElement:
                    ActivatedAt = element.ToDateTime();
                    break;

                case CanceledAtElement:
                    CanceledAt = element.ToNullable(DateTime.Parse);
                    break;

                case ExpiresAtElement:
                    ExpiresAt = element.ToNullable(DateTime.Parse);
                    break;

                case CurrentPeriodStartedAtElement:
                    CurrentPeriodStartedAt = element.ToNullable(DateTime.Parse);
                    break;

                case CurrentPeriodEndsAtElement:
                    CurrentPeriodEndsAt = element.ToNullable(DateTime.Parse);
                    break;

                case TrialStartedAtElement:
                    TrialStartedAt = element.ToNullable(DateTime.Parse);
                    break;

                case TrialEndsAtElement:
                    _trialEndsAt = element.ToNullable(DateTime.Parse);
                    break;

                case CollectionMethodElement:
                    CollectionMethod = element.ToEnum<CollectionMethods>();
                    break;
            }
            RecurlyManualInvoiceDetails.ProcessElement(element, this);
        }

        protected override void ProcessReader(string elementName, XmlTextReader reader)
        {
 	        switch(elementName)
 	        {
                case RecurlySubscriptionAddon.ElementName:
                    Addons.Add(new RecurlySubscriptionAddon(reader));
                    break;

                case PendingSubscriptionElement:
                    PendingSubscription = new RecurlySubscription {_elementName = PendingSubscriptionElement};
 	                PendingSubscription.ReadXml(reader);
                    break;
 	        }            
        }

        protected void WriteCreateXml(XmlTextWriter writer, RecurlyAccount account, string couponCode, int? billingCycles, DateTime? firstRenewalDate)
        {
            writer.WriteStartElement(ElementName); // Start: subscription
                writer.WriteElementString(PlanCodeElement, PlanCode); //Required
                account.WriteXml(writer); //Required
                writer.WriteElementStringIfProvided(CouponCodeElement,couponCode);
                writer.WriteElementIntIfProvided(UnitAmountInCentsElement, UnitAmountInCents);
                writer.WriteElementString(CurrencyElement,Currency); //Required
                writer.WriteElementIntIfProvided(QuantityElement, Quantity > 1 ? Quantity : new int?());
                writer.WriteElementDateTimeIfProvided(TrialEndsAtElement, TrialEndsAt);
                writer.WriteElementDateTimeIfFuture(StartsAtElement,ActivatedAt);
                writer.WriteElementIntIfProvided(TotalBillingCyclesElement, billingCycles);
                writer.WriteElementDateTimeIfProvided(FirstRenewalDateElement, firstRenewalDate);
                writer.WriteElementListIfAny(AddonsElement, Addons, (w, a) => a.WriteXml(w));
                WriteManualInvoiceElement(writer);
            writer.WriteEndElement(); // End: subscription
        }

        protected void WriteChangeXml(XmlTextWriter writer, ChangeTimeframe timeframe, string newPlanCode)
        {
            writer.WriteStartElement(ElementName); // Start: subscription
                writer.WriteElementEnum("timeframe", timeframe); //Required
                writer.WriteElementStringIfProvided(PlanCodeElement, newPlanCode);
                writer.WriteElementIntIfProvided(QuantityElement,Quantity);
                writer.WriteElementIntIfProvided(UnitAmountInCentsElement,UnitAmountInCents);
                writer.WriteElementListIfAny(AddonsElement, Addons, (w, a) => a.WriteXml(w));
                WriteManualInvoiceElement(writer);
            writer.WriteEndElement(); // End: subscription
        }

        private void WriteManualInvoiceElement(XmlTextWriter writer)
        {
            if(CollectionMethod != CollectionMethods.Manual || ManualInvoiceDetails == null) return;

            writer.WriteElementEnum(CollectionMethodElement,CollectionMethod);
            ManualInvoiceDetails.WriteXmlElements(writer);
        }

        #endregion
    }
}