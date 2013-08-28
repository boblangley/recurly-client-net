using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Recurly.Core;
using Recurly.Properties;

namespace Recurly
{
    public class RecurlyPlanList : IRecurlyPagedList<RecurlyPlan>
    {
        private class RecurlyPlanListPager : RecurlyPager<RecurlyPlan> 
        {
            internal RecurlyPlanListPager(int pageSize) : base(pageSize)
            {
            }

            protected override string BasePath
            {
                get { return Settings.Default.PathPlansList; }
            }

            protected override string ParentElementName
            {
                get { return "plans"; }
            }

            protected override string ChildElementName
            {
                get { return RecurlyPlan.ElementName; }
            }

            protected override RecurlyPlan InitializeChild(XElement element)
            {
                return new RecurlyPlan(element);
            }
        }

        private readonly RecurlyPlanListPager _pager;

        internal RecurlyPlanList(int pageSize)
        {
            _pager = new RecurlyPlanListPager(pageSize);
        }

        /// <summary>
        /// Lists all your active subscription plans.
        /// </summary>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static RecurlyPlanList GetPlanList(int pageSize = 50)
        {
            return new RecurlyPlanList(pageSize);
        }

        public List<RecurlyPlan> NextPage()
        {
            return _pager.Next();
        }

        public bool EndOfPages {
            get { return _pager.EndOfPages; }
        }

        public int TotalCount
        {
            get { return _pager.TotalRecords; }
        }
    }
}
