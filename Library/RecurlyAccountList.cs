using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Recurly.Properties;

namespace Recurly
{
    public class RecurlyAccountList : IRecurlyPagedList<RecurlyAccount>
    {
        private class RecurlyAccountListPager : RecurlyPager<RecurlyAccount>
        {
            private readonly string _basePath;

            public RecurlyAccountListPager(RecurlyAccount.AccountState state, int pageSize) : base(pageSize)
            {
                _basePath = Settings.Default.PathAccountsList;
                if (state != RecurlyAccount.AccountState.Active)
                    CustomQueryParameters.Add("state",Enum.GetName(state.GetType(),state).ToLower());
            }

            protected override string BasePath
            {
                get { return _basePath; }
            }

            protected override string ParentElementName
            {
                get { return "accounts"; }
            }

            protected override string ChildElementName
            {
                get { return RecurlyAccount.ElementName; }
            }

            protected override RecurlyAccount ReadChildXml(XmlTextReader reader)
            {
                return new RecurlyAccount(reader);
            }
        }

        private readonly RecurlyAccountListPager _pager;

        private RecurlyAccountList(RecurlyAccount.AccountState state, int pageSize)
        {
            _pager = new RecurlyAccountListPager(state, pageSize);
        }

        public RecurlyAccountList ListAccounts(RecurlyAccount.AccountState state = RecurlyAccount.AccountState.Active,
                                               int pageSize = RecurlyPager.DefaultPageSize)
        {
            return new RecurlyAccountList(state,pageSize);
        }

        public List<RecurlyAccount> NextPage()
        {
            return _pager.Next();
        }

        public bool EndOfPages {
            get { return _pager.EndOfPages; }
        }
    }
}
