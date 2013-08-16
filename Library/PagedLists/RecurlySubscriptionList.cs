using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Recurly.Core;
using Recurly.Properties;

namespace Recurly
{
    public class RecurlySubscriptionList : IRecurlyPagedList<RecurlySubscription> 
    {
        private class RecurlySubscriptionListPager : RecurlyPager<RecurlySubscription>
        {
            private readonly string _basePath;
            public RecurlySubscriptionListPager(string basePath, RecurlySubscription.SubscriptionState state, int pageSize) : base(pageSize)
            {
                _basePath = basePath;

                if(state != RecurlySubscription.SubscriptionState.Live)
                {
                    CustomQueryParameters.Add("state",Enum.GetName(state.GetType(),state).ToLower());
                }
            }

            protected override string BasePath
            {
                get { return String.Format(_basePath + ""); }
            }

            protected override string ParentElementName
            {
                get { return "subscriptions"; }
            }

            protected override string ChildElementName
            {
                get { return "subscription"; }
            }

            protected override RecurlySubscription ReadChildXml(XmlTextReader reader)
            {
                return new RecurlySubscription(reader);
            }
        }

        private readonly RecurlySubscriptionListPager _pager;

        private RecurlySubscriptionList(string basePath, RecurlySubscription.SubscriptionState state, int pageSize)
        {
            _pager = new RecurlySubscriptionListPager(basePath, state, pageSize);
        }

        public static RecurlySubscriptionList GetSubscriptions(
            RecurlySubscription.SubscriptionState state = RecurlySubscription.SubscriptionState.Live,
            int pageSize = RecurlyPager.DefaultPageSize)
        {
            return new RecurlySubscriptionList(Settings.Default.PathSubscriptionsList, state, pageSize);
        }

        public static RecurlySubscriptionList GetAccountSubscriptions(string accountCode,
                                                                      RecurlySubscription.SubscriptionState state =
                                                                          RecurlySubscription.SubscriptionState.Live,
                                                                      int pageSize = RecurlyPager.DefaultPageSize)
        {
            return new RecurlySubscriptionList(string.Format(Settings.Default.PathAccountSubscriptionsList,accountCode.UrlEncode()),
                state,pageSize);
        }

        public List<RecurlySubscription> NextPage()
        {
            return _pager.Next();
        }

        public bool EndOfPages {
            get { return _pager.EndOfPages; }
        }
    }
}
