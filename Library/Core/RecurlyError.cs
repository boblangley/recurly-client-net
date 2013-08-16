using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;

namespace Recurly.Core
{
    /// <summary>
    /// An individual error message.
    /// For more information, please visit http://docs.recurly.com/api/errors
    /// </summary>
    public class RecurlyError
    {
        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; internal set; }
        /// <summary>
        /// Field causing the error, if appropriate.
        /// </summary>
        public string Field { get; internal set; }
        /// <summary>
        /// Error code set for certain transaction failures.
        /// </summary>
        public string Code { get; internal set; }

        internal RecurlyError(XmlTextReader reader)
        {
            if (reader.HasAttributes)
            {
                try
                {
                    Field = reader.GetAttribute("field");
                }
                catch (ArgumentOutOfRangeException)
                { }
                try
                {
                    Code = reader.GetAttribute("code");
                }
                catch (ArgumentOutOfRangeException)
                { }
            }

            Message = reader.ReadElementContentAsString();
        }

        public override string ToString()
        {
            if (!String.IsNullOrEmpty(Field))
                return String.Format("{0} (Field: {1})", Message, Field);
            return !String.IsNullOrEmpty(Code) ? String.Format("{0} (Code: {1})", Message, Code) : Message;
        }

        internal static RecurlyError[] ReadResponseAndParseErrors(HttpWebResponse response)
        {
            if (response == null)
                return new RecurlyError[0];

            using (var responseStream = response.GetResponseStream())
            {
                var errors = new List<RecurlyError>();

                try
                {
                    if(responseStream == null)
                        throw new Exception("The response stream returned is null");

                    using (var xmlReader = new XmlTextReader(responseStream))
                    {
                        // Parse errors collection
                        while (xmlReader.Read())
                        {
                            if (xmlReader.Name == "errors" && xmlReader.NodeType == XmlNodeType.EndElement)
                                break;

                            if (xmlReader.Name == "error" && xmlReader.NodeType == XmlNodeType.Element)
                                errors.Add(new RecurlyError(xmlReader));
                        }
                    }
                }
                catch (XmlException)
                {
                    // Do nothing
                }

                return errors.ToArray();
            }
        }
    }
}
