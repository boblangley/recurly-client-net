using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using Recurly.Properties;

namespace Recurly
{
    public class RecurlyAccountNoteList : IRecurlyPagedList<RecurlyAccountNote>
    {
        private class RecurlyAccountNoteListPager : RecurlyPager<RecurlyAccountNote>
        {
            private readonly string _baseUrl;
            public RecurlyAccountNoteListPager(string accountCode) : base(DefaultPageSize)
            {
                _baseUrl = String.Format(Settings.Default.PathAccountNotesList, HttpUtility.UrlEncode(accountCode));
            }

            protected override string BasePath
            {
                get { return _baseUrl; }
            }

            protected override string ParentElementName
            {
                get { return "notes"; }
            }

            protected override string ChildElementName
            {
                get { return RecurlyAccountNote.ElementName; }
            }

            protected override RecurlyAccountNote ReadChildXml(XmlTextReader reader)
            {
                return new RecurlyAccountNote(reader);
            }
        }

        private readonly RecurlyAccountNoteListPager _pager;

        private RecurlyAccountNoteList(string accountCode)
        {
            _pager = new RecurlyAccountNoteListPager(accountCode);
        }

        public static RecurlyAccountNoteList ListAccountNotes(string accountCode)
        {
            return new RecurlyAccountNoteList(accountCode);
        }

        public List<RecurlyAccountNote> NextPage()
        {
            return _pager.Next();
        }

        public bool EndOfPages {
            get { return _pager.EndOfPages; }
        }
    }
}
