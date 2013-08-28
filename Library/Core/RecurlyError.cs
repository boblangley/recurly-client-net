using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;

namespace Recurly.Core
{
    /// <summary>
    /// An individual error message.
    /// For more information, please visit http://docs.recurly.com/api/errors
    /// </summary>
    public class RecurlyError
    {
        public string Description { get; internal set; }

        public string Symbol { get; internal set; }

        internal RecurlyError(XElement element)
        {
            element.ProcessChild("symbol",x => Symbol = x.Value);
            element.ProcessChild("description",x => Description = x.Value);
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Symbol, Description);
        }

        internal static List<RecurlyError> ReadResponseAndParseErrors(HttpWebResponse response)
        {
            if (response == null)
                return new List<RecurlyError>();

            using (var responseStream = response.GetResponseStream())
            {
                var errors = new List<RecurlyError>();

                XDocument doc = null;

                try
                {
                    if(responseStream == null)
                        throw new Exception("The response stream returned is null");

                    doc = XDocument.Load(responseStream);

                    if(doc.Root != null) errors = doc.Root.Elements("error").Select(e => new RecurlyError(e)).ToList();
                }
                catch (XmlException ex)
                {
                    if(doc != null)
                    {
                        System.Diagnostics.Debug.WriteLine(doc.ToString(),"Response Body");
                    }
                    System.Diagnostics.Debug.WriteLine(ex.ToString(),"Error");
                }

                return errors;
            }
        }
    }
}
