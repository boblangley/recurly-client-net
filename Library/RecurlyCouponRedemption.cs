using System;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using Recurly.Core;
using Recurly.Properties;

namespace Recurly
{
    public class RecurlyCouponRedemption : BaseRecurlyApiObject
    {
        private const string ElementName = "redemption";

        private const string AccountCodeElement = "account_code";
        private const string AccountLinkElement = "account";
        public string AccountCode { get; internal set; }

        private const string CouponCodeElement = "coupon";
        public string CouponCode { get; private set; }
        private const string CurrencyElement = "currency";
        public string Currency { get; internal set; }
        private const string SingleUseElement = "single_use";
        public bool SingleUse { get; private set; }
        private const string TotalDiscountedInCentsElement = "total_discounted_in_cents";
        public int TotalDiscountedInCents { get; private set; }
        private const string StateElement = "state";
        public string State { get; private set; }
        private const string CreatedAtElement = "created_at";
        public DateTime CreatedAt { get; private set; }

        internal RecurlyCouponRedemption()
        {
        }

        internal RecurlyCouponRedemption(XElement element)
        {
            ReadElement(element);
        }

        public static RecurlyCouponRedemption GetAccountRedemption(string accountNumber)
        {
            var redemption = new RecurlyCouponRedemption();

            var statusCode = RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Get,
                                                          String.Format(Settings.Default.PathAccountCouponRedemption,
                                                                        accountNumber), redemption.ReadXml);

            return statusCode == HttpStatusCode.OK ? redemption : null;
        }

        /// <summary>
        /// Occasionally, you may want to remove a coupon from an account
        /// <remarks>Recurly will automatically remove coupons after they expire or are otherwise no longer valid for an account. If you want to remove a coupon from an account before it expires, you may use the examples below. Please note: the coupon will still count towards the "maximum redemption total" of a coupon.</remarks>
        /// </summary>
        public void Remove()
        {
            RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Delete,
                                                          String.Format(Settings.Default.PathAccountCouponRedemption,
                                                                        AccountCode));
        }

        public static RecurlyCouponRedemption GetInvoiceInvoice(int invoiceNumber)
        {
            var redemption = new RecurlyCouponRedemption();

            var statusCode = RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Get,
                                                          String.Format(Settings.Default.PathInvoiceCouponRedemption,
                                                                        invoiceNumber), redemption.ReadXml);

            return statusCode == HttpStatusCode.OK ? redemption : null;
        }

        protected override void ReadElement(XElement element)
        {
            element.ProcessChild(SingleUseElement, e =>
                SingleUse = e.ToBool());

            element.ProcessChild(TotalDiscountedInCentsElement, e =>
                TotalDiscountedInCents = e.ToInt());

            element.ProcessChild(CreatedAtElement, e =>
                CreatedAt = e.ToDateTime());

            element.ProcessChild(CouponCodeElement, e =>
                CouponCode = e.GetHrefLinkId());

            element.ProcessChild(AccountLinkElement, e =>
                AccountCode = e.GetHrefLinkId());

            element.ProcessChild(StateElement, e =>
                State = e.Value);
        }

        internal void WriteXml(XmlTextWriter writer)
        {
            writer.WriteStartElement(ElementName);
                writer.WriteElementString(AccountCodeElement, AccountCode);
                writer.WriteElementString(CurrencyElement, Currency);
            writer.WriteEndElement();
        }

        public override bool Equals(object obj)
        {
            var a = obj as RecurlyCouponRedemption;
            return a != null && Equals(a);
        }

        public bool Equals(RecurlyCouponRedemption redemption)
        {
            if(string.IsNullOrWhiteSpace(AccountCode) || string.IsNullOrWhiteSpace(CouponCode))
                return false;

            return AccountCode == redemption.AccountCode && CouponCode == redemption.CouponCode;
        }

        public override int GetHashCode()
        {
            return (AccountCode + CouponCode + CreatedAt).GetHashCode();
        }
    }
}