using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Recurly.Core;
using Recurly.Properties;

namespace Recurly
{
    /// <summary>
    /// A coupon that can be applied to an account or subscription
    /// </summary>
    public class RecurlyCoupon : BaseRecurlyApiObject
    {
        internal const string ElementName = "coupon";

        public enum CouponState
        {
            All,
            Redeemable,
            Expired,
            Maxed_out
        }

        public enum DiscountType
        {
            Percent,
            Dollars
        }

        private const string CouponCodeElement = "coupon_code";
        public string CouponCode { get; set; }

        private const string NameElement = "name";
        public string Name { get; set; }

        private const string StateElement = "state";
        public CouponState State { get; private set; }

        private const string HostedDescriptionElement = "hosted_description";
        public string HostedDescription { get; set; }

        private const string InvoiceDescriptionElement = "invoice_description";
        public string InvoiceDescription { get; set; }

        private const string DiscountTypeElement = "discount_type";
        public DiscountType Type { get; set; }

        private const string DiscountPercentElement = "discount_percent";
        public int? DiscountPercent { get; set; }

        private const string DiscountInCentsElement = "discount_in_cents";
        private RecurlyInCentsMapping DiscountInCents { get; set; }

        private const string RedeemByDateElement = "redeem_by_date";
        public DateTime? RedeemByDate { get; set; }

        private const string SingleUseElement = "single_use";
        public bool SingleUse { get; set; }

        private const string AppliesForMonthsElement = "applies_for_months";
        public int? AppliesForMonths { get; set; }

        private const string MaxRedemptionsElement = "max_redemptions";
        public int MaxRedemptions { get; set; }

        private const string AppliesToAllPlansElement = "applies_to_all_plans";
        public bool AppliesToAllPlans { get; set; }

        private const string CreatedAtElement = "created_at";
        public DateTime CreatedAt { get; private set; }

        private const string PlanCodeListElement = "plan_codes";
        private const string PlanCodeItemElement = "plan_code";
        public List<string> PlanCodes { get; set; }

        #region Constructors

        private RecurlyCoupon()
        {
            PlanCodes = new List<string>();
            DiscountInCents = new RecurlyInCentsMapping(DiscountInCentsElement);
        }

        internal RecurlyCoupon(XmlTextReader xmlReader) : this()
        {
            ReadXml(xmlReader);
        }

        #endregion

        /// <summary>
        /// Returns information about an active coupon.
        /// </summary>
        /// <param name="couponCode"></param>
        /// <returns></returns>
        public static RecurlyCoupon Get(string couponCode)
        {
            var coupon = new RecurlyCoupon();

            var statusCode = RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Get,
                String.Format(Settings.Default.PathCouponGet, HttpUtility.UrlEncode(couponCode)),
                coupon.ReadXml);

            return statusCode == HttpStatusCode.NotFound ? null : coupon;
        }

        /// <summary>
        /// Creates a new coupon. Please note: coupons cannot be updated after being created.
        /// </summary>
        /// <returns>Success or Failure</returns>
        public bool Create()
        {
            if (String.IsNullOrWhiteSpace(CouponCode)) throw new InvalidOperationException("A CouponCode must be assigned to the coupon before it can be created.");
            if (CouponCode.Length > 50) throw new InvalidOperationException("The CouponCode cannot be more than 50 characters");
            const string couponCodeRegex = @"[a-zA-Z0-9@\-_\.]";
            if (Regex.Matches(CouponCode, couponCodeRegex).Count > 0) throw new InvalidOperationException(String.Format("The CouponCode can only contain the following characters: {0}. '{1}' was provided.", couponCodeRegex, CouponCode));
            if (String.IsNullOrWhiteSpace(Name)) throw new InvalidOperationException("A Name must be provided for the coupon before it can be created.");
            switch(Type)
            {
                case DiscountType.Percent:
                    if (!DiscountPercent.HasValue) throw new InvalidOperationException("A DiscountPercentage must be assigned to the coupon before it can be created.");
                    break;
                case DiscountType.Dollars:
                    if (!DiscountInCents.Any()) throw new InvalidOperationException("One or more DiscountInCent elements must be added before the coupon can be created.");
                    break;
            }
            if (!AppliesToAllPlans && !PlanCodes.Any()) throw new InvalidOperationException("One or more Plan Codes must be assigned to the coupon before it can be created.");

            var statusCode = RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Post, Settings.Default.PathCouponCreate,
                                         ReadXml);

            return statusCode == HttpStatusCode.Created;
        }

        /// <summary>
        /// Redeem a coupon on an account.
        /// </summary>
        /// <param name="accountCode"></param>
        /// <param name="couponCode"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        public static RecurlyCouponRedemption Redeem(string couponCode, string accountCode, string currency = null)
        {
            var redemption = new RecurlyCouponRedemption() { AccountCode = accountCode, Currency = couponCode};

            var statusCode = RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Post,
                String.Format(Settings.Default.PathRedeemCoupon,HttpUtility.UrlEncode(couponCode)),
                redemption.WriteXml,
                redemption.ReadXml);

            return statusCode == HttpStatusCode.Created ? redemption : null;
        }

        /// <summary>
        /// Deactivate the coupon so customers can no longer redeem the coupon
        /// </summary>
        public void Deactivate()
        {
            RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Delete, Settings.Default.PathCouponDeactivate);
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
                case CouponCodeElement:
                    CouponCode = element.Value;
                    break;

                case NameElement:
                    Name = element.Value;
                    break;

                case StateElement:
                    State = element.ToEnum<CouponState>();
                    break;

                case HostedDescriptionElement:
                    HostedDescription = element.Value;
                    break;

                case InvoiceDescriptionElement:
                    InvoiceDescription = element.Value;
                    break;

                case DiscountTypeElement:
                    Type = element.ToEnum<DiscountType>();
                    break;

                case DiscountPercentElement:
                    DiscountPercent = element.ToInt();
                    break;

                case RedeemByDateElement:
                    RedeemByDate = element.ToNullable(DateTime.Parse);
                    break;

                case SingleUseElement:
                    SingleUse = element.ToBool();
                    break;

                case AppliesForMonthsElement:
                    AppliesForMonths = element.ToNullable(int.Parse);
                    break;

                case MaxRedemptionsElement:
                    MaxRedemptions = element.ToInt();
                    break;

                case AppliesToAllPlansElement:
                    AppliesToAllPlans = element.ToBool();
                    break;

                case CreatedAtElement:
                    CreatedAt = element.ToDateTime();
                    break;

                case PlanCodeItemElement:
                    PlanCodes.Add(element.Value);
                    break;
            }
        }

        protected override void ProcessReader(string elementName, XmlTextReader reader)
        {
            switch(elementName)
            {
                case DiscountInCentsElement:
                    DiscountInCents.ReadXml(reader);
                    break;
            }
        }

        internal void WriteXml(XmlTextWriter writer)
        {
            writer.WriteStartElement(ElementName);
                writer.WriteElementString(CouponCodeElement, CouponCode);
                writer.WriteElementString(NameElement,Name);
                writer.WriteElementStringIfProvided(HostedDescriptionElement,HostedDescription);
                writer.WriteElementStringIfProvided(InvoiceDescriptionElement, InvoiceDescription);
                writer.WriteElementDateTimeIfProvided(RedeemByDateElement, RedeemByDate);
                writer.WriteElementString(SingleUseElement, SingleUse.ToString());
                writer.WriteElementIntIfProvided(AppliesForMonthsElement,AppliesForMonths);
                writer.WriteElementString(MaxRedemptionsElement, MaxRedemptions.ToString(CultureInfo.InvariantCulture));                
                WriteDiscountElements(writer);
                if(!AppliesToAllPlans)
                {
                    writer.WriteElementString(AppliesToAllPlansElement, AppliesToAllPlans.ToString());                   
                    writer.WriteElementList(PlanCodeListElement, PlanCodes, WritePlanCodeItem);
                }
                    
            writer.WriteEndElement();
        }

        private void WriteDiscountElements(XmlTextWriter writer)
        {
            writer.WriteElementEnum(DiscountTypeElement, Type);
            switch(Type)
            {
                case DiscountType.Percent:
                    writer.WriteElementIntIfProvided(DiscountPercentElement, DiscountPercent);
                    break;
                case DiscountType.Dollars:                    
                    writer.WriteStartElement(DiscountInCentsElement);
                    foreach(var item in DiscountInCents)
                    {
                        writer.WriteElementString(item.Currency, item.AmountInCents.ToString(CultureInfo.InvariantCulture));
                    }
                    writer.WriteEndElement();
                    break;
            }
        }

        private static void WritePlanCodeItem(XmlTextWriter writer, string planCode)
        {
            writer.WriteElementString(PlanCodeItemElement, planCode);
        }

        #endregion

        #region Object Overrides

        public override string ToString()
        {
            return "Recurly Coupon: " + CouponCode;
        }

        public override bool Equals(object obj)
        {
            var a = obj as RecurlyCoupon;
            return a != null && Equals(a);
        }

        public bool Equals(RecurlyCoupon coupon)
        {
            return CouponCode == coupon.CouponCode;
        }

        public override int GetHashCode()
        {
            return CouponCode.GetHashCode();
        }

        #endregion
    }
}
