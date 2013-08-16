using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Recurly.Core;
using Recurly.Properties;

namespace Recurly
{
    public class RecurlyInvoiceList : IRecurlyPagedList<RecurlyInvoice>
    {
        private class RecurlyInvoiceListPager : RecurlyPager<RecurlyInvoice>
        {
            private readonly string _basePath;

            public RecurlyInvoiceListPager(string basePath, RecurlyInvoice.InvoiceState state, int pageSize) : base(pageSize)
            {
                _basePath = basePath;
                if (state != RecurlyInvoice.InvoiceState.All)
                    CustomQueryParameters.Add("state",Enum.GetName(state.GetType(),state).ToLower());
            }

            protected override string BasePath
            {
                get { return _basePath; }
            }

            protected override string ParentElementName
            {
                get { return "invoices"; }
            }

            protected override string ChildElementName
            {
                get { return RecurlyInvoice.ElementName; }
            }

            protected override RecurlyInvoice InitialzeChild(XElement element)
            {
                return new RecurlyInvoice(element);                
            }
        }

        private readonly RecurlyInvoiceListPager _pager;

        internal RecurlyInvoiceList(string basePath, RecurlyInvoice.InvoiceState state, int pageSize)
        {
            _pager = new RecurlyInvoiceListPager(basePath, state, pageSize);
        }

        public static RecurlyInvoiceList ListInvoices(RecurlyInvoice.InvoiceState state = RecurlyInvoice.InvoiceState.All, int pageSize = RecurlyPager.DefaultPageSize)
        {
            return new RecurlyInvoiceList(Settings.Default.PathInvoicesList,state,pageSize);
        }

        public static RecurlyInvoiceList ListAccountInvoices(string accountCode, RecurlyInvoice.InvoiceState state = RecurlyInvoice.InvoiceState.All, int pageSize = RecurlyPager.DefaultPageSize)
        {
            return new RecurlyInvoiceList(String.Format(Settings.Default.PathAccountInvoicesList, accountCode.UrlEncode()),
                                              state, pageSize);
        }

        public List<RecurlyInvoice> NextPage()
        {
            return _pager.Next();
        }

        public bool EndOfPages { get { return _pager.EndOfPages; }}
    }
}