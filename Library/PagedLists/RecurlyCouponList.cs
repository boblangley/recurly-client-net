using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Recurly.Core;
using Recurly.Properties;

namespace Recurly
{
    public class RecurlyCouponList : IRecurlyPagedList<RecurlyCoupon>
    {
        private class RecurlyCouponListPager : RecurlyPager<RecurlyCoupon>
        {
            public RecurlyCouponListPager(RecurlyCoupon.CouponState state, int pageSize) : base(pageSize)
            {
                if (state != RecurlyCoupon.CouponState.All)
                    CustomQueryParameters.Add("state",Enum.GetName(state.GetType(),state).ToLower());
            }

            protected override string BasePath
            {
                get { return Settings.Default.PathCouponsList; }
            }

            protected override string ParentElementName
            {
                get { return "coupons"; }
            }

            protected override string ChildElementName
            {
                get { return RecurlyCoupon.ElementName; }
            }

            protected override RecurlyCoupon ReadChildXml(XmlTextReader reader)
            {
                return new RecurlyCoupon(reader);
            }
        }

        private readonly RecurlyCouponListPager _pager;

        private RecurlyCouponList(RecurlyCoupon.CouponState state, int pageSize)
        {
            _pager = new RecurlyCouponListPager(state, pageSize);
        }

        public RecurlyCouponList ListActiveCoupons(RecurlyCoupon.CouponState state = RecurlyCoupon.CouponState.All,
                                                   int pageSize = RecurlyPager.DefaultPageSize)
        {
            return new RecurlyCouponList(state, pageSize);
        }

        public List<RecurlyCoupon> NextPage()
        {
            return _pager.Next();
        }

        public bool EndOfPages
        {
            get { return _pager.EndOfPages; }
        }
    }
}
