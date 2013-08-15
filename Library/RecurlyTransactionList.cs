using System;
using System.Collections.Generic;
using System.Xml;
using Recurly.Properties;

namespace Recurly
{
    public class RecurlyTransactionList : IRecurlyPagedList<RecurlyTransaction>
    {
        private class RecurlyTransactionListPager : RecurlyPager<RecurlyTransaction>
        {
            private readonly string _basePath;
            public RecurlyTransactionListPager(string basePath, RecurlyTransaction.TransactionState state,
                                               RecurlyTransaction.TransactionType type, int pageSize) : base(pageSize)
            {
                _basePath = basePath;
                if (state != RecurlyTransaction.TransactionState.All)
                    CustomQueryParameters.Add("state", Enum.GetName(state.GetType(),state).ToLower());
                if (type != RecurlyTransaction.TransactionType.All)
                    CustomQueryParameters.Add("type", Enum.GetName(type.GetType(),type).ToLower());
            }

            protected override string BasePath
            {
                get { return _basePath; }
            }

            protected override string ParentElementName
            {
                get { return "transactions"; }
            }

            protected override string ChildElementName
            {
                get { return RecurlyTransaction.ElementName; }
            }

            protected override RecurlyTransaction ReadChildXml(XmlTextReader reader)
            {
                return new RecurlyTransaction(reader);
            }
        }

        private readonly RecurlyTransactionListPager _pager;

        private RecurlyTransactionList(string basePath, RecurlyTransaction.TransactionState state,
                                       RecurlyTransaction.TransactionType type, int pageSize)
        {
            _pager = new RecurlyTransactionListPager(basePath,state,type,pageSize);
        }

        /// <summary>
        /// Returns a list of all the transactions.
        /// </summary>
        /// <param name="state">The state of transactions to return</param>
        /// <param name="type">The type of transactions to return</param>
        /// <param name="pageSize">Number of records to return per page</param>
        /// <returns></returns>
        public static RecurlyTransactionList ListTransactions(
            RecurlyTransaction.TransactionState state = RecurlyTransaction.TransactionState.All,
            RecurlyTransaction.TransactionType type = RecurlyTransaction.TransactionType.All,
            int pageSize = RecurlyPager.DefaultPageSize)
        {
            return new RecurlyTransactionList(Settings.Default.PathTransactionsList, state, type, pageSize);
        }

        /// <summary>
        /// Returns a list of transactions for an account.
        /// </summary>
        /// <param name="accountCode"></param>
        /// <param name="state">The state of transactions to return</param>
        /// <param name="type">The type of transactions to return</param>
        /// <param name="pageSize">Number of records to return per page</param>
        /// <returns></returns>
        public static RecurlyTransactionList ListAccountTransactions(string accountCode,
                                                                     RecurlyTransaction.TransactionState state =
                                                                         RecurlyTransaction.TransactionState.All,
                                                                     RecurlyTransaction.TransactionType type =
                                                                         RecurlyTransaction.TransactionType.All,
                                                                     int pageSize = RecurlyPager.DefaultPageSize)
        {
            return new RecurlyTransactionList(String.Format(Settings.Default.PathAccountTransactionsList,accountCode.UrlEncode()), state, type, pageSize);
        }

        public List<RecurlyTransaction> NextPage()
        {
            return _pager.Next();
        }

        public bool EndOfPages { get { return _pager.EndOfPages; } }
    }
}