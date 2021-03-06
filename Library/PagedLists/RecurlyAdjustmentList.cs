﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Recurly.Core;
using Recurly.Properties;

namespace Recurly
{
    public class RecurlyAdjustmentList : IRecurlyPagedList<RecurlyAdjustment>
    {
        private class RecurlyAdjustmentListPager : RecurlyPager<RecurlyAdjustment>
        {
            private readonly string _baseUrl;
            public RecurlyAdjustmentListPager(RecurlyAdjustment.AdjustmentType type, RecurlyAdjustment.AdjustmentState state, string accountCode, int pageSize) : base(pageSize)
            {
                _baseUrl = String.Format(Settings.Default.PathAccountAdjustmentsList, HttpUtility.UrlEncode(accountCode));
                if (type != RecurlyAdjustment.AdjustmentType.All)                    
                    CustomQueryParameters.Add("type",Enum.GetName(type.GetType(),type).ToLower());
                if(state != RecurlyAdjustment.AdjustmentState.Active)
                    CustomQueryParameters.Add("state",Enum.GetName(state.GetType(),state).ToLower());
            }

            protected override string BasePath
            {
                get { return _baseUrl; }
            }

            protected override string ParentElementName
            {
                get { return "adjustments"; }
            }

            protected override string ChildElementName
            {
                get { return RecurlyAdjustment.ElementName; }
            }

            protected override RecurlyAdjustment InitializeChild(XElement element)
            {
                return new RecurlyAdjustment(element);
            }
        }

        private readonly RecurlyAdjustmentListPager _pager;

        private RecurlyAdjustmentList(RecurlyAdjustment.AdjustmentType type, RecurlyAdjustment.AdjustmentState state,
                                      string accountCode, int pageSize)
        {
            _pager = new RecurlyAdjustmentListPager(type,state,accountCode,pageSize);
        }

        public static RecurlyAdjustmentList ListAccountAdjustments(string accountCode,
                                                            RecurlyAdjustment.AdjustmentType type =
                                                                RecurlyAdjustment.AdjustmentType.All,
                                                            RecurlyAdjustment.AdjustmentState state =
                                                                RecurlyAdjustment.AdjustmentState.Active,
                                                            int pageSize = RecurlyPager.DefaultPageSize)
        {
            return new RecurlyAdjustmentList(type,state,accountCode,pageSize);
        }

        public List<RecurlyAdjustment> NextPage()
        {
            return _pager.Next();
        }

        public bool EndOfPages { get { return _pager.EndOfPages; } }


        public int TotalCount
        {
            get { return _pager.TotalRecords; }
        }
    }
}
