using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Recurly.Properties;

namespace Recurly
{
    public class RecurlyPlanAddonList : IRecurlyPagedList<RecurlyPlanAddon>
    {
        private class RecurlyPlanAddonPager : RecurlyPager<RecurlyPlanAddon>
        {
            private readonly string _planCode;

            internal RecurlyPlanAddonPager(string planCode, int pageSize) : base(pageSize)
            {
                _planCode = planCode;
            }

            protected override string BasePath
            {
                get { return String.Format(Settings.Default.PathPlanAddonsList, _planCode); }
            }

            protected override string ParentElementName
            {
                get { return "add_ons"; }
            }

            protected override string ChildElementName
            {
                get { return RecurlyPlanAddon.ElementName; }
            }

            protected override RecurlyPlanAddon ReadChildXml(XmlTextReader reader)
            {
                return new RecurlyPlanAddon(reader);
            }
        }

        private readonly RecurlyPlanAddonPager _pager ;

        internal RecurlyPlanAddonList(string planCode, int pageSize)
        {
            _pager = new RecurlyPlanAddonPager(planCode,pageSize);
        }

        public static RecurlyPlanAddonList ListAddonsForAPlan(string planCode, int pageSize = 50)
        {
            return new RecurlyPlanAddonList(planCode, pageSize);
        }

        public List<RecurlyPlanAddon> NextPage()
        {
            return _pager.Next();
        }

        public bool EndOfPages {
            get { return _pager.EndOfPages; }
        }
    }
}
