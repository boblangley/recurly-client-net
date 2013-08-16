using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Recurly.Core;
using Recurly.Properties;

namespace Recurly
{
    /// <summary>
    /// Generic Billing Info
    /// </summary>
    public abstract class RecurlyBillingInfo : RecurlyAddress
    {
        internal new const string ElementName = "billing_info";
        
        private const string AccountCodeElement = "account";
        public string AccountCode { get; private set; }
        private const string FirstNameElement = "first_name";
        public string FirstName { get; set; }
        private const string LastNameElement = "last_name";
        public string LastName { get; set; }
        private const string VatNumberElement = "vat_number";
        public string VatNumber { get; set; }
        private const string IpAddressElement = "ip_address";
        public string IpAddress { get; set; }
        private const string IpAddressCountryElement = "ip_address_country";
        public string IpAddressCountry { get; private set; }

        public virtual string Type
        {
            get { return "undefined"; }
        }

        protected RecurlyBillingInfo(string accountCode)
        {
            AccountCode = accountCode;
        }
        
        /// <summary>
        /// Lookup a Recurly account's billing info
        /// </summary>
        /// <param name="accountCode"></param>
        /// <returns></returns>
        public static RecurlyBillingInfo Get(string accountCode)
        {
            RecurlyBillingInfo billingInfo = new RecurlyCreditCardBillingInfo(accountCode);

            var statusCode = RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Get,
                String.Format(Settings.Default.PathAccountBillingInfoGet,HttpUtility.UrlEncode(accountCode)), 
                reader =>
                    {
                        reader.Read();
                        switch(reader.GetAttribute("type"))
                        {
                            case "credit_card":
                                billingInfo = new RecurlyCreditCardBillingInfo(accountCode);
                                break;
                            case "paypal":
                                billingInfo = new RecurlyPayPalBillingInfo(accountCode);
                                break;
                        }
                        billingInfo.ReadXml(reader);
                    });

            return statusCode == HttpStatusCode.OK ? billingInfo : null;
        }

        /// <summary>
        /// Updates the BillingInfo for the account
        /// </summary>
        public void Update()
        {
            var account = RecurlyAccount.Get(AccountCode);
            account.Update(this);
        }

        /// <summary>
        /// Delete an account's billing info.
        /// </summary>
        public static void DeleteBillingInfo(string accountCode)
        {
            var account = RecurlyAccount.Get(accountCode);
            account.Update();
        }

        protected override string RootElementName
        {
            get { return ElementName; }
        }

        protected override void ProcessElement(XElement element)
        {
            switch (element.Name.LocalName)
            {
                case AccountCodeElement:
                    AccountCode = element.GetHrefLinkId();
                    break;

                case FirstNameElement:
                    FirstName = element.Value;
                    break;

                case LastNameElement:
                    LastName = element.Value;
                    break;

                case VatNumberElement:
                    VatNumber = element.Value;
                    break;

                case IpAddressElement:
                    IpAddress = element.Value;
                    break;

                case IpAddressCountryElement:
                    IpAddressCountry = element.Value;
                    break;
            }
            base.ProcessElement(element);
        }

        internal new void WriteXml(XmlTextWriter writer)
        {
            if (String.IsNullOrEmpty(IpAddress))
                System.Diagnostics.Debug.WriteLine("Recurly Client Library: Recording IP Address is strongly recommended.");

            writer.WriteStartElement(ElementName);
                writer.WriteElementString(FirstNameElement, FirstName);
                writer.WriteElementString(LastNameElement, LastName);
                WriteAddressElements(writer);
                writer.WriteElementStringIfProvided(VatNumberElement,VatNumber);
                writer.WriteElementStringIfProvided(IpAddressElement, IpAddress);
                WriteExtendedElements(writer);
            writer.WriteEndElement();
        }

        protected virtual void WriteExtendedElements(XmlTextWriter writer)
        {
        }
    }
}
