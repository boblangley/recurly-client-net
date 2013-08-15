using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using Recurly.Properties;

namespace Recurly
{
    public class RecurlyCoupon
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
        private Dictionary<string, int> DiscountInCents { get; set; }

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
            DiscountInCents = new Dictionary<string, int>();
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
                redemption.ReadXml,
                null);

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

        internal void ReadXml(XmlTextReader reader)
        {
            while (reader.Read())
            {
                if (reader.Name == ElementName && reader.NodeType == XmlNodeType.EndElement)
                    break;

                if(reader.NodeType != XmlNodeType.Element) continue;
                switch (reader.Name)
                {
                    case CouponCodeElement:
                        CouponCode = reader.ReadElementContentAsString();
                        break;

                    case NameElement:
                        Name = reader.ReadElementContentAsString();
                        break;

                    case StateElement:
                        State = reader.ReadElementContentAsEnum<CouponState>();
                        break;

                    case HostedDescriptionElement:
                        HostedDescription = reader.ReadElementContentAsString();
                        break;

                    case InvoiceDescriptionElement:
                        InvoiceDescription = reader.ReadElementContentAsString();
                        break;

                    case DiscountTypeElement:
                        Type = reader.ReadElementContentAsEnum<DiscountType>();
                        break;

                    case DiscountPercentElement:
                        DiscountPercent = reader.ReadElementContentAsInt();
                        break;

                    case DiscountInCentsElement:
                        ReadDollarsMapping(reader);
                        break;

                    case RedeemByDateElement:
                        RedeemByDate = reader.ReadElementContentAsNullable(r => r.ReadElementContentAsDateTime());
                        break;

                    case SingleUseElement:
                        SingleUse = reader.ReadElementContentAsBoolean();
                        break;

                    case AppliesForMonthsElement:
                        AppliesForMonths = reader.ReadElementContentAsNullable(r => r.ReadElementContentAsInt());
                        break;

                    case MaxRedemptionsElement:
                        MaxRedemptions = reader.ReadElementContentAsInt();
                        break;

                    case AppliesToAllPlansElement:
                        AppliesToAllPlans = reader.ReadElementContentAsBoolean();
                        break;

                    case CreatedAtElement:
                        CreatedAt = reader.ReadElementContentAsDateTime();
                        break;

                    case PlanCodeItemElement:
                        PlanCodes.Add(reader.ReadElementContentAsString());
                        break;
                }
            }
        }

        private void ReadDollarsMapping(XmlTextReader reader)
        {
            while(reader.Read())
            {
                if(reader.Name == DiscountInCentsElement && reader.NodeType == XmlNodeType.EndElement)
                    break;

                if(reader.NodeType != XmlNodeType.Element) continue;

                DiscountInCents.Add(reader.Name, reader.ReadElementContentAsInt());
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
                    foreach(var kvp in DiscountInCents)
                    {
                        writer.WriteElementString(kvp.Key, kvp.Value.ToString());
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
