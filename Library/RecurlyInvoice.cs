﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using Recurly.Core;
using Recurly.Properties;

namespace Recurly
{
    public class RecurlyInvoice : BaseRecurlyApiObject
    {
        internal const string ElementName = "invoice";

        public enum InvoiceState
        {
            All,
            Open,
            Collected,
            Failed,
            Past_Due
        }

        private const string IdElement = "uuid";
        public string Id { get; private set; }
        private const string StateElement = "state";
        public InvoiceState State { get; private set; }
        private const string InvoiceNumberElement = "invoice_number";
        public int InvoiceNumber { get; private set; }
        private const string PurchaseOrderNumberElement = "po_number";
        public string PurchaseOrderNumber { get; set; }
        private const string VatNumberElement = "vat_number";
        public string VatNumber { get; private set; }
        private const string SubTotalInCentsElement = "subtotal_in_cents";
        public int SubTotalInCents { get; private set; }
        private const string TaxInCentsElement = "tax_in_cents";
        public int TaxInCents { get; private set; }
        private const string TotalInCentsElement = "total_in_cents";
        public int TotalInCents { get; private set; }
        private const string CurrencyElement = "currency";
        public string Currency { get; private set; }
        private const string CreatedAtElement = "created_at";
        public DateTime CreatedAt { get; private set; }
        private const string TransactionsElement = "transactions";
        public List<RecurlyTransaction> Transactions { get; private set; }
        private const string AccountCodeElement = "account";
        public string AccountCode { get; private set; }

        private RecurlyInvoice()
        {
            Transactions = new List<RecurlyTransaction>();
        }

        internal RecurlyInvoice(XmlTextReader reader) : this()
        {
            ReadXml(reader);
        }

        /// <summary>
        /// Look up an Invoice.
        /// </summary>
        /// <param name="invoiceNumber">Invoice Number</param>
        /// <returns></returns>
        public static RecurlyInvoice Get(int invoiceNumber)
        {
            var invoice = new RecurlyInvoice();

            var statusCode = RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Get,
                                                          String.Format(Settings.Default.PathInvoiceGet, invoiceNumber),
                invoice.ReadXml);

            return statusCode == HttpStatusCode.OK ? invoice : null;
        }

        public void MarkSuccessful()
        {
            RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Put,
                                         String.Format(Settings.Default.PathInvoiceMarkSuccessful, InvoiceNumber));
        }

        public void MarkFailed()
        {
            RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Put,
                                         String.Format(Settings.Default.PathInvoiceMarkFailed, InvoiceNumber));
        }

        /// <summary>
        /// Create an Invoice if there are outstanding charges on an account. If there are no outstanding
        /// charges, null is returned.
        /// </summary>
        /// <param name="accountCode">Account code</param>
        /// <returns></returns>
        public static RecurlyInvoice InvoicePendingCharges(string accountCode)
        {
            var invoice = new RecurlyInvoice();

            var statusCode = RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Post,
                String.Format(Settings.Default.PathAccountInvoicePendingCharges,accountCode.UrlEncode()),
                invoice.ReadXml);

            return statusCode == HttpStatusCode.OK ? invoice : null;
        }

        /// <summary>
        /// Lookup information about a coupon redemption applied to the invoice
        /// </summary>
        /// <returns></returns>
        public RecurlyCouponRedemption CouponRedemption()
        {
            return RecurlyCouponRedemption.GetInvoiceInvoice(InvoiceNumber);
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
                case IdElement:
                    Id = element.Value;
                    break;

                case StateElement:
                    State = element.ToEnum<InvoiceState>();
                    break;

                case InvoiceNumberElement:
                    InvoiceNumber = element.ToInt();
                    break;

                case PurchaseOrderNumberElement:
                    PurchaseOrderNumber = element.Value;
                    break;

                case VatNumberElement:
                    VatNumber = element.Value;
                    break;

                case SubTotalInCentsElement:
                    SubTotalInCents = element.ToInt();
                    break;

                case TaxInCentsElement:
                    TaxInCents = element.ToInt();
                    break;

                case TotalInCentsElement:
                    TotalInCents = element.ToInt();
                    break;

                case CurrencyElement:
                    Currency = element.Value;
                    break;

                case CreatedAtElement:
                    CreatedAt = element.ToDateTime();
                    break;

                case AccountCodeElement:
                    AccountCode = element.Value.Split('/').Last();
                    break;

                case TransactionsElement:
                    //Transactions.ReadXml(reader); 
                    break;
            }
        }

        #endregion

        #region Object Overrides

        public override string ToString()
        {
            return "Recurly Invoice: " + Id;
        }

        public override bool Equals(object obj)
        {
            var a = obj as RecurlyInvoice;
            return a != null && Equals(a);
        }

        public bool Equals(RecurlyInvoice invoice)
        {
            return Id == invoice.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        #endregion
    }
}