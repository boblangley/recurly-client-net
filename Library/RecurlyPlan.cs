using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Recurly.Core;
using Recurly.Properties;

namespace Recurly
{
    public class RecurlyPlan : BaseRecurlyApiObject
    {
        internal const string ElementName = "plan";

        public enum IntervalUnit
        {
            Days,
            Months
        }

        private const string PlanCodeElement = "plan_code";
        public string PlanCode { get; set; }
        private const string NameElement = "name";
        public string Name { get; set; }
        private const string DescriptionElement = "description";
        public string Description { get; set; }
        private const string SuccessUrlElement = "success_url";
        public string SuccessUrl { get; set; }
        private const string CancelUrlElement = "cancel_url";
        public string CancelUrl { get; set; }
        private const string DisplayDonationAmountsElement = "display_donation_amounts";
        public bool DisplayDonationAmounts { get; private set; }
        private const string DisplayQuantityElement = "display_quantity";
        public bool DisplayQuantity { get; set; }
        private const string DisplayPhoneNumberElement = "display_phone_number";
        public bool DisplayPhoneNumber { get; private set; }
        private const string BypassHostedConfirmationElement = "bypass_hosted_confirmation";
        public bool BypassHostedConfirmation { get; private set; }
        private const string UnitNameElement = "unit_name";
        public string UnitName { get; set; }
        private const string PaymentPageTosLinkElement = "payment_page_tos_link";
        public string PaymentPageTosLink { get; private set; }
        private const string PlanIntervalLengthElement = "plan_interval_length";
        public int PlanIntervalLength { get; set; }
        private const string PlanIntervalUnitElement = "plan_interval_unit";
        public IntervalUnit PlanIntervalUnit { get; set; }
        private const string TrialIntervalLengthElement = "trial_interval_length";
        public int TrialIntervalLength { get; set; }
        private const string TrialIntervalUnitElement = "trial_interval_unit";
        public IntervalUnit TrialIntervalUnit { get; set; }
        private const string AccountingCodeElement = "accounting_code";
        public string AccountingCode { get; set; }
        private const string CreatedAtElement = "created_at";
        public DateTime CreatedAt { get; private set; }
        private const string UnitAmountInCentsElement = "unit_amount_in_cents";
        
        /// <summary>
        /// Collection of Amounts in cents and a currency code. At least 1 element is required.
        /// </summary>
        public RecurlyInCentsMapping UnitAmountInCents { get; set; }
        private const string SetupFeeInCentsElement = "setup_fee_in_cents";
        public RecurlyInCentsMapping SetupFeeInCents { get; set; }
        private const string TotalBillingCyclesElement = "total_billing_cycles";
        public int? TotalBillingCycles { get; set; }
        
        public RecurlyPlan()
        {
            PlanIntervalLength = 1;
            UnitAmountInCents = new RecurlyInCentsMapping(UnitAmountInCentsElement);
            SetupFeeInCents = new RecurlyInCentsMapping(SetupFeeInCentsElement, true);
            PlanIntervalUnit = IntervalUnit.Months;
            TrialIntervalUnit = IntervalUnit.Months;
        }

        internal RecurlyPlan(XElement element) : this()
        {
            ReadElement(element);
        }

        public static RecurlyPlan Get(string planCode)
        {
            var plan = new RecurlyPlan();

            var statusCode = RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Get,
                String.Format(Settings.Default.PathPlanGet,planCode.UrlEncode()),
                plan.ReadXml);

            return statusCode == HttpStatusCode.OK ? plan : null;
        }

        public bool Create()
        {
            if (String.IsNullOrWhiteSpace(PlanCode)) throw new InvalidOperationException("A PlanCode must be assigned before this plan can be created.");
            if (PlanCode.Length > 50) throw new InvalidOperationException("The PlanCode cannot be more than 50 characters");
            const string regex = @"[a-z0-9@\-_\.]";
            if (Regex.Matches(PlanCode, regex).Count > 0) throw new InvalidOperationException(String.Format("The PlanCode can only contain the following characters: {0}. '{1}' was provided.", regex, PlanCode));
            
            if (String.IsNullOrWhiteSpace(Name)) throw new InvalidOperationException("A Name must be assigned before this plan can be created.");
            if (Name.Length > 255) throw new InvalidOperationException("The Name can be no more than 255 characters");
            if (!String.IsNullOrWhiteSpace(AccountingCode) && AccountingCode.Length > 20) throw new InvalidOperationException("The AccountingCode cannot be more than 20 characters.");
            if (Regex.Matches(AccountingCode, regex).Count > 0) throw new InvalidOperationException(String.Format("The AccountingCode can only contain the following characters: {0}. '{1}' was provided.", regex, AccountingCode));
            
            if (!UnitAmountInCents.Any()) throw new InvalidOperationException("At least one UnitAmountInCents must be provided before this Plan can be created");

            var statusCode = RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Post,
                                         Settings.Default.PathPlanCreate,
                                         WriteXml,
                                         ReadXml);

            return statusCode == HttpStatusCode.Created;
        }

        public bool Update()
        {
            var statusCode = RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Put,
                                         String.Format(Settings.Default.PathPlanUpdate,PlanCode.UrlEncode()),
                                         WriteXml,
                                         ReadXml);

            return statusCode == HttpStatusCode.OK;
        }

        public void Delete()
        {
            RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Delete,
                                         String.Format(Settings.Default.PathPlanUpdate, PlanCode.UrlEncode()));
        }

        /// <summary>
        /// Returns a list of all the add-ons for this plan.
        /// </summary>
        /// <param name="pageSize">Number of records to return per page</param>
        /// <returns></returns>
        public RecurlyPlanAddonList ListAddons(int pageSize = RecurlyPager.DefaultPageSize)
        {
            return RecurlyPlanAddonList.ListAddonsForAPlan(PlanCode, pageSize);
        }

        public override string ToString()
        {
            return "Recurly Plan: " + PlanCode;
        }

        protected override void ReadElement(XElement element)
        {
            element.ProcessChild(PlanCodeElement, e =>
                PlanCode = element.Value);

            element.ProcessChild(NameElement, e =>
                Name = element.Value);

            element.ProcessChild(DescriptionElement, e =>
                Description = element.Value);

            element.ProcessChild(SuccessUrlElement, e =>
                SuccessUrl = element.Value);

            element.ProcessChild(CancelUrlElement, e =>
                CancelUrl = element.Value);

            element.ProcessChild(DisplayDonationAmountsElement, e =>
                DisplayDonationAmounts = element.ToBool());

            element.ProcessChild(DisplayQuantityElement, e =>
                DisplayQuantity = element.ToBool());

            element.ProcessChild(DisplayPhoneNumberElement, e =>
                DisplayPhoneNumber = element.ToBool());

            element.ProcessChild(BypassHostedConfirmationElement, e =>
                BypassHostedConfirmation = element.ToBool());

            element.ProcessChild(UnitNameElement, e =>
                UnitName = element.Value);

            element.ProcessChild(PaymentPageTosLinkElement, e =>
                PaymentPageTosLink = element.Value);

            element.ProcessChild(PlanIntervalLengthElement, e =>
                PlanIntervalLength = element.ToInt());

            element.ProcessChild(PlanIntervalUnitElement, e =>
                PlanIntervalUnit = element.ToEnum<IntervalUnit>());

            element.ProcessChild(TrialIntervalLengthElement, e =>
                TrialIntervalLength = element.ToInt());

            element.ProcessChild(TrialIntervalUnitElement, e =>
                TrialIntervalUnit = element.ToEnum<IntervalUnit>());

            element.ProcessChild(AccountingCodeElement, e =>
                AccountingCode = element.Value);

            element.ProcessChild(CreatedAtElement, e =>
                CreatedAt = element.ToDateTime());

            element.ProcessChild(UnitAmountInCentsElement, e =>
                UnitAmountInCents.ReadElement(e));

            element.ProcessChild(SetupFeeInCentsElement, e =>
                SetupFeeInCents.ReadElement(e));
        }

        internal void WriteXml(XmlTextWriter writer)
        {
            writer.WriteStartElement(ElementName);
                writer.WriteElementString(PlanCodeElement, PlanCode);
                writer.WriteElementString(NameElement, Name);
                writer.WriteElementStringIfProvided(DescriptionElement,Description);
                writer.WriteElementStringIfProvided(SuccessUrlElement,SuccessUrl);
                writer.WriteElementStringIfProvided(CancelUrlElement,CancelUrl);   
                writer.WriteElementString(DisplayQuantityElement, DisplayQuantity.ToString());
                writer.WriteElementStringIfProvided(UnitNameElement, UnitName);
                writer.WriteElementString(PlanIntervalLengthElement, PlanIntervalLength.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementEnum(PlanIntervalUnitElement,PlanIntervalUnit);
                writer.WriteElementString(TrialIntervalLengthElement, TrialIntervalLength.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementEnum(TrialIntervalUnitElement, TrialIntervalUnit);
                writer.WriteElementStringIfProvided(AccountingCodeElement,AccountingCode);
                UnitAmountInCents.WriteXml(writer);
                SetupFeeInCents.WriteXml(writer);
            writer.WriteEndElement();
        }
    }
}