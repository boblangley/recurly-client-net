using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace Recurly.Core
{
    internal abstract class RecurlyPager
    {
        public const int DefaultPageSize = 50;
        public const int MaxPageSize = 200;
    }

    internal abstract class RecurlyPager<T> : RecurlyPager
    {
        private const string MatchPattern = "/<([^>]+)>; rel=\"next\"/";
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
            if (_nextLink == END)
                return null;

            var items = new List<T>();

            if(string.IsNullOrWhiteSpace(_nextLink))
                _nextLink = DefaultPath;

            RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Get, _nextLink, reader => ReadXml(items,reader), ReadHeaders);

            return items;
        }

        private void ReadHeaders(WebHeaderCollection webHeaderCollection)
        {
            if(webHeaderCollection == null) return;

            if(!string.IsNullOrWhiteSpace(webHeaderCollection.Get("X-Records")))
                TotalRecords = int.Parse(webHeaderCollection.Get("X-Records"));

            _nextLink = END;

            var link = webHeaderCollection.Get("Link");

            if(string.IsNullOrWhiteSpace(link)) return;
            var match = Regex.Match(webHeaderCollection.Get("Link"), MatchPattern);
            _nextLink = match.Success ? match.Groups[1].Value : END;
        }

        internal void ReadXml(List<T> items, XmlTextReader reader)
        {
            while (reader.Read())
            {
                // End of items element, get out of here
                if (reader.Name == ParentElementName && reader.NodeType == XmlNodeType.EndElement)
                    break;

                if (reader.NodeType != XmlNodeType.Element) continue;
                if(reader.Name != ChildElementName) continue;

                items.Add(ReadChildXml(reader));
            }
        }

        protected abstract T ReadChildXml(XmlTextReader reader);
    }
}
