using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Xml;
using Recurly.Properties;

namespace Recurly
{
    public class RecurlySubscription
    {
        internal const string ElementName = "subscription";

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

        public class RecurlyManualInvoiceDetails
        {
            private const string NetTermsElement = "net_terms";
            public int NetTerms { get; set; }
            private const string PurchaseOrderNumberElement = "po_number";
            public string PurchaseOrderNumber { get; set; }

            internal static void ProcessElement(XmlTextReader reader, RecurlySubscription sub)
            {
                switch(reader.Name)
                {
                    case NetTermsElement:
                        if(sub.ManualInvoiceDetails == null)
                            sub.ManualInvoiceDetails = new RecurlyManualInvoiceDetails();
                        sub.ManualInvoiceDetails.NetTerms = reader.ReadElementContentAsInt();                        
                        break;
                    case PurchaseOrderNumberElement:
                        if(sub.ManualInvoiceDetails == null)
                            sub.ManualInvoiceDetails = new RecurlyManualInvoiceDetails();
                        sub.ManualInvoiceDetails.PurchaseOrderNumber = reader.ReadElementContentAsString();
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

        public static RecurlySubscription Get(string subscriptionId)
        {
            var sub = new RecurlySubscription();
            var statusCode = RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Get,
                                                          String.Format(Settings.Default.PathSubscriptionGet,
                                                                        subscriptionId),
                                                          sub.ReadXml);

            return statusCode == HttpStatusCode.NotFound ? null : sub;
        }

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

        public void Update(ChangeTimeframe timeframe, string newPlanCode = null)
        {
            if (newPlanCode != null && String.IsNullOrWhiteSpace(newPlanCode)) throw new ArgumentException("newPlanCode", "A new PlanCode cannot be empty if provided");
            
            RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Put,
                                         String.Format(Settings.Default.PathSubscriptionUpdate, Id),
                                         writer => WriteChangeXml(writer, timeframe, newPlanCode), ReadXml);
        }

        public void Cancel()
        {
            RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Put,
                                         String.Format(Settings.Default.PathSubscriptionCancel, Id));
        }

        public void Reactivate()
        {
            RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Put,
                                         String.Format(Settings.Default.PathSubscriptionReactivate, Id));
        }

        public void Terminate(RefundType refund)
        {
            RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Put,
                                         String.Format(Settings.Default.PathSubscriptionTerminate, Id,
                                                       Enum.GetName(refund.GetType(), refund).ToLower()));
        }

        public void Postpone(DateTime nextRenewalDate)
        {
            if (nextRenewalDate.Date <= DateTime.Now.Date) throw new ArgumentOutOfRangeException("nextRenewalDate","Renewal date must be in the future to postpone");
            RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Put,
                                         String.Format(Settings.Default.PathSubscriptionPostpone, Id,
                                                       nextRenewalDate.ToString("s")));
        }

        #region Read and Write XML documents

        internal void ReadXml(XmlTextReader reader)
        {
            ReadXml(reader, "subscription");
        }

        internal void ReadXml(XmlTextReader reader, string elementname)
        {
            Addons.Clear();

            while (reader.Read())
            {
                // End of subscription element, get out of here
                if (reader.Name == elementname && reader.NodeType == XmlNodeType.EndElement)
                    break;

                if(reader.NodeType != XmlNodeType.Element) continue;
                switch (reader.Name)
                {
                    case AccountCodeElement:
                        var accountHref = reader.ReadElementAttribute("href");
                        AccountCode = accountHref.Split('/').Last();
                        break;

                    case PlanCodeElement:
                        PlanCode = reader.ReadElementContentAsString();
                        break;

                    case IdElement:
                        Id = reader.ReadElementContentAsString();
                        break;

                    case StateElement:
                        State = reader.ReadElementContentAsEnum<SubscriptionState>();
                        break;

                    case UnitAmountInCentsElement:
                        UnitAmountInCents = reader.ReadElementContentAsInt();
                        break;

                    case CurrencyElement:
                        Currency = reader.ReadElementContentAsString();
                        break;

                    case QuantityElement:
                        Quantity = reader.ReadElementContentAsInt();
                        break;

                    case ActivatedAtElement:
                        ActivatedAt = reader.ReadElementContentAsDateTime();
                        break;

                    case CanceledAtElement:
                        CanceledAt = reader.ReadElementContentAsNullable(r => r.ReadElementContentAsDateTime());
                        break;

                    case ExpiresAtElement:
                        ExpiresAt = reader.ReadElementContentAsNullable(r => r.ReadElementContentAsDateTime());
                        break;

                    case CurrentPeriodStartedAtElement:
                        CurrentPeriodStartedAt = reader.ReadElementContentAsNullable(r => r.ReadElementContentAsDateTime());
                        break;

                    case CurrentPeriodEndsAtElement:
                        CurrentPeriodEndsAt = reader.ReadElementContentAsNullable(r => r.ReadElementContentAsDateTime());
                        break;

                    case TrialStartedAtElement:
                        TrialStartedAt = reader.ReadElementContentAsNullable(r => r.ReadElementContentAsDateTime());
                        break;

                    case TrialEndsAtElement:
                        _trialEndsAt = reader.ReadElementContentAsNullable(r => r.ReadElementContentAsDateTime());
                        break;

                    case RecurlySubscriptionAddon.ElementName:
                        Addons.Add(new RecurlySubscriptionAddon(reader));
                        break;

                    case PendingSubscriptionElement:
                        PendingSubscription = new RecurlySubscription();
                        PendingSubscription.ReadXml(reader,PendingSubscriptionElement);
                        break;

                    case CollectionMethodElement:
                        CollectionMethod = reader.ReadElementContentAsEnum<CollectionMethods>();
                        break;
                }
                RecurlyManualInvoiceDetails.ProcessElement(reader, this);
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