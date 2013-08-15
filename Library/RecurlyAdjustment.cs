using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Xml;
using Recurly.Properties;

namespace Recurly
{
    public class RecurlyAdjustment
    {
        internal const string ElementName = "adjustment";
        private const int MaxAdjustementUnitAmountInCents = 10000000;

        public enum AdjustmentType
        {
            All,
            Charge,
            Credit
        }

        public enum AdjustmentState
        {
            Active,
            Pending,
            Invoiced
        }

        private const string AccountCodeCreateElement = "account_code";
        private const string AccountCodeListElement = "account";
        public string AccountCode { get; set; }

        private const string TypeAttribute = "type";
        public AdjustmentType Type { get; private set; }

        private const string IdElement = "uuid";
        public string Id { get; private set; }

        private const string DescriptionElement = "description";
        public string Description { get; set; }

        private const string AccountingCodeElement = "accounting_code";
        public string AccountingCode { get; set; }

        private const string OriginElement = "origin";
        public string Origin { get; set; }

        private const string UnitAmountInCentsElement = "unit_amount_in_cents";
        public int UnitAmountInCents { get; set; }

        private const string QuantityElement = "quantity";
        public int Quantity { get; set; }

        private const string DiscountInCentsElement = "discount_in_cents";
        public int DiscountInCents { get; set; }

        private const string TaxInCentsElement = "tax_in_cents";
        public int TaxInCents { get; set; }

        private const string TotalInCentsElement = "total_in_cents";
        public int TotalInCents { get; set; }

        private const string CurrencyElement = "currency";
        public string Currency { get; set; }

        private const string TaxableElement = "taxable";
        public bool Taxable { get; set; }

        private const string ProductCodeElement = "product_code";
        public string ProductCode { get; set; }

        private const string StartDateElement = "start_date";
        public DateTime StartDate { get; set; }

        private const string EndDateElement = "end_date";
        public DateTime? EndDate { get; set; }

        private const string CreatedAtElement = "created_at";
        public DateTime CreatedAt { get; private set; }

        public RecurlyAdjustment(string accountCode, int unitAmountInCents)
        {
            if (String.IsNullOrWhiteSpace(accountCode))
                throw new InvalidOperationException("AccountCode must be assigned before an adjustment can be created");
            if (unitAmountInCents > MaxAdjustementUnitAmountInCents)
                throw new InvalidOperationException("UnitAmountInCents cannot exceed " + MaxAdjustementUnitAmountInCents);
            if (accountCode.Length > 20)
                throw new InvalidOperationException("AccountingCode cannot be longer than 20 characters");

            AccountCode = accountCode;
            UnitAmountInCents = unitAmountInCents;
            Quantity = 1;
            Currency = RecurlyClient.Currency;
        }

        internal RecurlyAdjustment(XmlTextReader reader)
        {
            ReadXml(reader);
        }

        /// <summary>
        /// Creates a one-time charge on an account. 
        /// <remarks>
        /// Charges are not invoiced or collected immediately. Non-invoiced charges will automatically be invoices when the account's subscription renews, or you trigger a collection by posting an invoice. Charges may be removed from an account if they have not been invoiced.
        /// </remarks>
        /// </summary>
        public bool Create()
        {
            if(!string.IsNullOrWhiteSpace(Id))
            {
                throw new InvalidOperationException("You cannot call Create on an existing adjustment");
            }

            if (Quantity < 0) throw new InvalidOperationException("Quantity must be greater than 0");

            var statusCode = RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Post,
                                         String.Format(Settings.Default.PathAccountAdjustmentCreate, AccountCode),
                                         WriteXml, ReadXml, null);

            return statusCode == HttpStatusCode.Created;
        }

        /// <summary>
        /// Delete a non-invoiced adjustment from an account.
        /// </summary>
        public void Delete()
        {
            RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Delete,
                                         String.Format(Settings.Default.PathAccountAdjustmentDelete, AccountCode));
        }

        private void ReadXml(XmlTextReader reader)
        {
            while(reader.Read())
            {
                if(reader.Name == ElementName && reader.NodeType == XmlNodeType.EndElement)
                    break;

                if(reader.NodeType != XmlNodeType.Element) continue;
                switch(reader.Name)
                {
                    case ElementName:
                        Type = reader.ReadElementAttributeAsEnum<AdjustmentType>(TypeAttribute);
                        break;

                    case AccountCodeListElement:
                        AccountCode = reader.ReadElementAttribute("href").Split('/').Last();
                        break;
                    case IdElement:
                        Id = reader.ReadElementContentAsString();
                        break;
                    case DescriptionElement:
                        Description = reader.ReadElementContentAsString();
                        break;
                    case AccountingCodeElement:
                        AccountingCode = reader.ReadElementContentAsString();
                        break;
                    case OriginElement:
                        Origin = reader.ReadElementContentAsString();
                        break;
                    case UnitAmountInCentsElement:
                        UnitAmountInCents = reader.ReadElementContentAsInt();
                        break;
                    case QuantityElement:
                        Quantity = reader.ReadElementContentAsInt();
                        break;
                    case DiscountInCentsElement:
                        DiscountInCents = reader.ReadElementContentAsInt();
                        break;
                    case TaxInCentsElement:
                        TaxInCents = reader.ReadElementContentAsInt();
                        break;
                    case TotalInCentsElement:
                        TotalInCents = reader.ReadElementContentAsInt();
                        break;
                    case CurrencyElement:
                        Currency = reader.ReadElementContentAsString();
                        break;
                    case TaxableElement:
                        Taxable = reader.ReadElementContentAsBoolean();
                        break;
                    case ProductCodeElement:
                        ProductCode = reader.ReadElementContentAsString();
                        break;
                    case StartDateElement:
                        StartDate = reader.ReadElementContentAsDateTime();
                        break;
                    case EndDateElement:
                        EndDate = reader.ReadElementContentAsNullable(r =>r.ReadElementContentAsDateTime());
                        break;
                    case CreatedAtElement:
                        CreatedAt = reader.ReadElementContentAsDateTime();
                        break;
                }
            }
        }

        private void WriteXml(XmlTextWriter writer)
        {
            writer.WriteStartElement(ElementName);
                writer.WriteElementString(AccountCodeCreateElement,AccountCode);
                writer.WriteElementString(CurrencyElement,Currency);
                writer.WriteElementString(UnitAmountInCentsElement,UnitAmountInCents.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementStringIfProvided(DescriptionElement,Description);
                writer.WriteElementIntIfProvided(QuantityElement,Quantity > 1 ? Quantity : new int?());
                writer.WriteElementStringIfProvided(AccountingCodeElement,AccountingCode);
            writer.WriteEndElement();
        }

    }
}
