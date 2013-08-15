using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Xml;
using Recurly.Properties;

namespace Recurly
{
    public class RecurlyPlanAddon
    {
        internal const string ElementName = "add_on";

        private const string AddonCodeElement = "add_on_code";
        public string AddonCode { get; set; }
        private const string PlanCodeElement = "plan";
        public string PlanCode { get; private set; }
        private const string NameElement = "name";
        public string Name { get; set; }
        private const string DisplayQuantityOnHostedPageElement = "display_quantity_on_hosted_page";
        public bool DisplayQuantityOnHostedPage { get; set; }
        private const string DefaultQuantityElement = "default_quantity";
        public int DefaultQuantity { get; set; }
        private const string UnitAmountInCentsElement = "unit_amount_in_cents";
        public RecurlyInCentsMapping UnitAmountInCents { get; set; }
        private const string AccountingCodeElement = "accounting_code";
        public string AccountingCode { get; set; }
        private const string CreatedAtElement = "created_at";
        public DateTime CreateAt { get; private set; }

        public RecurlyPlanAddon()
        {
            DefaultQuantity = 1;
            UnitAmountInCents = new RecurlyInCentsMapping(UnitAmountInCentsElement);
        }

        internal RecurlyPlanAddon(XmlTextReader reader) : this()
        {
            ReadXml(reader);
        }

        /// <summary>
        /// Returns information about an add-on.
        /// </summary>
        /// <param name="planCode"></param>
        /// <param name="addonCode"></param>
        /// <returns></returns>
        public static RecurlyPlanAddon Get(string planCode, string addonCode)
        {
            var addon = new RecurlyPlanAddon();

            var statusCode = RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Get,
                                         String.Format(Settings.Default.PathPlanAddonGet, planCode.UrlEncode(), addonCode.UrlEncode()),
                                         addon.ReadXml);

            return statusCode == HttpStatusCode.OK ? addon : null;
        }

        public bool Create(string planCode)
        {
            if (string.IsNullOrWhiteSpace(planCode)) throw new InvalidOperationException("A Plan Code to apply this add-on to must be provided.");
            if (string.IsNullOrWhiteSpace(AddonCode)) throw new InvalidOperationException("An AddonCode must be set to create this add-on");
            if (AddonCode.Length > 50) throw new InvalidOperationException("The AddonCode cannot be greater than 50 characters");
            if (!UnitAmountInCents.Any()) throw new InvalidOperationException("At least one UnitAmountInCents element must be provided to create this add-on");
            if(UnitAmountInCents.Any(i => i.AmountInCents > 10000000))
            {
                var item = UnitAmountInCents.First(i => i.AmountInCents > 10000000);
                throw new InvalidOperationException(String.Format("UnitAmount {0} cannot be greater than 10000000 (It was set at {1}", item.Currency, item.AmountInCents));
            }
            if (!string.IsNullOrWhiteSpace(AccountingCode) && AccountingCode.Length > 20) throw new InvalidOperationException("The AccountingCode cannot be more than 20 characters");

            var statusCode = RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Post,
                                         String.Format(Settings.Default.PathPlanAddonCRUD, planCode.UrlEncode()), WriteXml, ReadXml);

            return statusCode == HttpStatusCode.Created;
        }

        public bool Update()
        {
            var statusCode = RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Put,
                                         String.Format(Settings.Default.PathPlanAddonCRUD, PlanCode.UrlEncode()),
                                         WriteXml,
                                         ReadXml);
            return statusCode == HttpStatusCode.OK;
        }

        public void Delete()
        {
            RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Delete,
                                         String.Format(Settings.Default.PathPlanAddonCRUD, PlanCode.UrlEncode()));
        }

        public void ReadXml(XmlTextReader reader)
        {
            while(reader.Read())
            {
                // End of account element, get out of here
                if (reader.Name == ElementName && reader.NodeType == XmlNodeType.EndElement)
                    break;

                if(reader.NodeType != XmlNodeType.Element) continue;
                switch(reader.Name)
                {
                    case AddonCodeElement:
                        AddonCode = reader.ReadElementContentAsString();
                        break;

                    case PlanCodeElement:
                        PlanCode = reader.ReadElementAttribute("href").Split('/').Last();
                        break;

                    case NameElement:
                        Name = reader.ReadElementContentAsString();
                        break;

                    case DisplayQuantityOnHostedPageElement:
                        DisplayQuantityOnHostedPage = reader.ReadElementContentAsBoolean();
                        break;

                    case DefaultQuantityElement:
                        DefaultQuantity = reader.ReadElementContentAsInt();
                        break;

                    case UnitAmountInCentsElement:
                        UnitAmountInCents.ReadXml(reader);
                        break;

                    case CreatedAtElement:
                        CreateAt = reader.ReadElementContentAsDateTime();
                        break;
                }
            }
        }

        private void WriteXml(XmlTextWriter writer)
        {
            writer.WriteStartElement(ElementName);
                writer.WriteElementString(AddonCodeElement,AddonCode);
                writer.WriteElementString(NameElement,Name);
                UnitAmountInCents.WriteXml(writer);
                writer.WriteElementString(DefaultQuantityElement, DefaultQuantity.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementString(DisplayQuantityOnHostedPageElement, DisplayQuantityOnHostedPage.ToString());
                writer.WriteElementStringIfProvided(AccountingCodeElement, AccountingCode);
            writer.WriteEndElement();
        }
    }
}
