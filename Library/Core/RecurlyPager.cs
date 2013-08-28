using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace Recurly.Core
{
    internal abstract class RecurlyPager
    {
        public const int DefaultPageSize = 50;
        public const int MaxPageSize = 200;
    }

    internal abstract class RecurlyPager<T> : RecurlyPager
    {
        private const string MatchPattern = "\\<(.*?)\\>";
        private const string END = "EOP";
        private readonly int _pageSize;

        private string _nextLink = "";

        /// <summary>
        /// Number of records retrieved in each page
        /// </summary>
        public int PageSize
        {
            get { return _pageSize; }
        }

        /// <summary>
        /// Number of records retrievable in all pages. Is not initialized until the first page is retrieved.
        /// </summary>
        public int TotalRecords { get; private set; }

        /// <summary>
        /// True when all pages have been retrieved
        /// </summary>
        public bool EndOfPages
        {
            get { return _nextLink == END; }
        }

        private string DefaultPath
        {
            get
            {
                var urlBuilder = new StringBuilder(BasePath);

                var parameters = CustomQueryParameters;

                if (_pageSize != DefaultPageSize)
                {
                    parameters.Add("per_page", _pageSize.ToString(CultureInfo.InvariantCulture));
                }

                if (parameters.Any())
                {
                    var paramStrings = parameters.Select(
                        p => HttpUtility.UrlEncode(p.Key) + "=" + HttpUtility.UrlEncode(p.Value));
                    urlBuilder.AppendFormat("?{0}", paramStrings.First());
                    paramStrings.Skip(1).ToList().ForEach(p => urlBuilder.AppendFormat("&{0}", p));
                }
                System.Diagnostics.Debug.WriteLine(urlBuilder.ToString(),"Default Pager Path");
                return urlBuilder.ToString();
            }
        }

        protected abstract string BasePath { get; }
        protected abstract string ParentElementName { get; }
        protected abstract string ChildElementName { get; }
        protected Dictionary<string, string> CustomQueryParameters = new Dictionary<string, string>();

        protected RecurlyPager(int pageSize)
        {
            if (pageSize > MaxPageSize) throw new ArgumentOutOfRangeException("pageSize", "Page size cannot be greater than" + MaxPageSize);
            _pageSize = pageSize;            
        }

        /// <summary>
        /// Retrieve the next page
        /// </summary>
        /// <returns>New list of items or null if the last page has been retrieved</returns>
        public List<T> Next()
        {
            System.Diagnostics.Debug.WriteLine(_nextLink,"_nextLink");
            if (_nextLink == END)
                return null;

            var items = new List<T>();

            if(string.IsNullOrWhiteSpace(_nextLink))
                _nextLink = DefaultPath;

            var statusCode = RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Get, _nextLink, reader => ReadXml(items,reader), ReadHeaders);

            if(statusCode == HttpStatusCode.NotFound)
                _nextLink = END;

            return items;
        }

        private void ReadHeaders(WebHeaderCollection webHeaderCollection)
        {
            if(webHeaderCollection == null) return;

            if(!string.IsNullOrWhiteSpace(webHeaderCollection.Get("X-Records")))
                TotalRecords = int.Parse(webHeaderCollection.Get("X-Records"));

            _nextLink = END;

            var link = webHeaderCollection.Get("Link");
            System.Diagnostics.Debug.WriteLine(link, "Link Header");
            if(string.IsNullOrWhiteSpace(link)) return;
            var next = link.Split(',').FirstOrDefault(l => l.Contains("next"));
            if(next == null) return;
            var match = Regex.Match(next, MatchPattern);
            _nextLink = match.Success ? match.Groups[1].Value : END;
        }

        internal void ReadXml(List<T> items, XmlTextReader reader)
        {
            var root = XDocument.Load(reader).Element(ParentElementName);

            root.Elements(ChildElementName).ToList().ForEach(e => items.Add(InitializeChild(e)));
        }

        protected abstract T InitializeChild(XElement element);
    }
}
