using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using Recurly.Properties;

namespace Recurly.Core
{
    /// <summary>
    /// Class for the Recurly client library.
    /// </summary>
    public class RecurlyClient
    {
        private static string _apiKey;
        private static string _apiSubdomain;
        private static string _apiPrivateKey;
        private static string _currency;

        /// <summary>
        /// Recurly API Key
        /// </summary>
        public static string ApiKey
        {
            get { return String.IsNullOrWhiteSpace(_apiKey) ? Configuration.RecurlySection.Current.ApiKey : _apiKey; }
            set { _apiKey = value; }
        }

        /// <summary>
        /// Recurly Site Subdomain
        /// </summary>
        public static string ApiSubdomain
        {
            get { return String.IsNullOrWhiteSpace(_apiSubdomain) ? Configuration.RecurlySection.Current.Subdomain : _apiSubdomain; }
            set { _apiSubdomain = value; }
        }

        /// <summary>
        /// Recurly Private Key for Transparent Post API
        /// </summary>
        public static string PrivateKey
        {
            get
            {
                return String.IsNullOrWhiteSpace(_apiPrivateKey)
                           ? Configuration.RecurlySection.Current.PrivateKey
                           : _apiPrivateKey;
            }
            set { _apiPrivateKey = value; }
        }

        /// <summary>
        /// Sets the currency
        /// </summary>
        public static string Currency
        {
            get
            {
                return String.IsNullOrWhiteSpace(_currency)
                           ? Configuration.RecurlySection.Current.Currency
                           : _currency;
            }
            set { _currency = value; }
        }

        #region Header Helper Methods

        private static string _userAgent;
        /// <summary>
        /// User Agent header for connecting to Recurly. If an error occurs, Recurly uses this information to find
        /// better diagnose the problem.
        /// </summary>
        private static string UserAgent
        {
            get
            {
                return _userAgent ?? (_userAgent = String.Format("Recurly .NET Client v" +
                                                                 System.Reflection.Assembly.GetExecutingAssembly()
                                                                       .GetName()
                                                                       .Version));
            }
        }

        private static string _authorizationHeaderValue;
        /// <summary>
        /// Create the web request header value for the API Authorization.
        /// </summary>
        private static string AuthorizationHeaderValue
        {
            get
            {
                if (_authorizationHeaderValue == null)
                {
                    if (!String.IsNullOrEmpty(ApiKey))
                        _authorizationHeaderValue = "Basic " +
                            Convert.ToBase64String(Encoding.UTF8.GetBytes(ApiKey));
                }

                return _authorizationHeaderValue;
            }
        }

        #endregion

        public enum HttpRequestMethod
        {
            /// <summary>
            /// Lookup information about an object
            /// </summary>
            Get,
            /// <summary>
            /// Create a new object
            /// </summary>
            Post,
            /// <summary>
            /// Update an existing object
            /// </summary>
            Put,
            /// <summary>
            /// Delete an object
            /// </summary>
            Delete
        }

        public static HttpStatusCode PerformRequest(HttpRequestMethod method, string urlPath)
        {
            return PerformRequest(method, urlPath, null, null, null);
        }

        public static HttpStatusCode PerformRequest(HttpRequestMethod method, string urlPath,
            Action<XmlTextReader> readXmlDelegate)
        {
            return PerformRequest(method, urlPath, null, readXmlDelegate, null);
        }

        public static HttpStatusCode PerformRequest(HttpRequestMethod method, string urlPath,
            Action<XmlTextReader> readXmlDelegate, Action<WebHeaderCollection> headersDelegate)
        {
            return PerformRequest(method, urlPath, null, readXmlDelegate, headersDelegate);
        }

        public static HttpStatusCode PerformRequest(HttpRequestMethod method, string urlPath,
            Action<XmlTextWriter> writeXmlDelegate)
        {
            return PerformRequest(method, urlPath, writeXmlDelegate, null, null);
        }

        public static HttpStatusCode PerformRequest(HttpRequestMethod method, string urlPath,
            Action<XmlTextWriter> writeXmlDelegate, Action<XmlTextReader> readXmlDelegate)
        {
            return PerformRequest(method, urlPath, writeXmlDelegate, readXmlDelegate, null);
        }

        public static HttpStatusCode PerformRequest(HttpRequestMethod method, string urlPath,
            Action<XmlTextWriter> writeXmlDelegate, Action<XmlTextReader> readXmlDelegate, Action<WebHeaderCollection> headersDelegate)
        {
            var request = (HttpWebRequest)WebRequest.Create(ServerUrl + urlPath);
            request.Accept = "application/xml";      // Tells the server to return XML instead of HTML
            request.ContentType = "application/xml; charset=utf-8"; // The request is an XML document
            request.SendChunked = false;             // Send it all as one request
            request.UserAgent = UserAgent;
            request.Headers.Add(HttpRequestHeader.Authorization, AuthorizationHeaderValue);
            request.Method = method.ToString().ToUpper();

            System.Diagnostics.Debug.WriteLine("Recurly: Requesting {0} {1}", 
                request.Method, request.RequestUri);

            if ((method == HttpRequestMethod.Post || method == HttpRequestMethod.Put) && (writeXmlDelegate != null))
            {
                // 60 second timeout -- some payment gateways (e.g. PayPal) can take a while to respond
                request.Timeout = 60000;

                // Write POST/PUT body
                using (var requestStream = request.GetRequestStream())
                    WritePostParameters(requestStream, writeXmlDelegate);
            }

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                    return ReadWebResponse(response, readXmlDelegate, headersDelegate);
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    var response = (HttpWebResponse)ex.Response;
                    var statusCode = response.StatusCode;
                    RecurlyError[] errors;

                    System.Diagnostics.Debug.WriteLine("Recurly Library Received: {0} - {1}",
                                                       (int) statusCode, statusCode);

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                        case HttpStatusCode.Accepted:
                        case HttpStatusCode.Created:
                        case HttpStatusCode.NoContent:
                            return ReadWebResponse(response, readXmlDelegate, headersDelegate);

                        case HttpStatusCode.NotFound:
                            errors = RecurlyError.ReadResponseAndParseErrors(response);
                            if (errors.Length >= 0)
                                throw new NotFoundException(errors[0].Message, errors);
                            throw new NotFoundException("The requested object was not found.", errors);

                        case HttpStatusCode.Unauthorized:
                        case HttpStatusCode.Forbidden:
                            errors = RecurlyError.ReadResponseAndParseErrors(response);
                            throw new InvalidCredentialsException(errors);

                        case HttpStatusCode.PreconditionFailed:
                            errors = RecurlyError.ReadResponseAndParseErrors(response);
                            throw new ValidationException(errors);

                        case HttpStatusCode.ServiceUnavailable:
                            throw new TemporarilyUnavailableException();

                        case HttpStatusCode.InternalServerError:
                            errors = RecurlyError.ReadResponseAndParseErrors(response);
                            throw new RecurlyServerException(errors);
                    }

                    if ((int)statusCode == ValidationException.HttpStatusCode) // Unprocessable Entity
                    {
                        errors = RecurlyError.ReadResponseAndParseErrors(response);
                        throw new ValidationException(errors);
                    }
                }

                throw;
            }
        }

        internal static string ServerUrl
        {
            get
            {
                return String.Format(Settings.Default.BaseServerUrl, ApiSubdomain);
            }
        }

        private static HttpStatusCode ReadWebResponse(HttpWebResponse response, Action<XmlTextReader> readXmlDelegate, Action<WebHeaderCollection> headersDelegate)
        {
            var statusCode = response.StatusCode;

            if(headersDelegate != null)
            {
                headersDelegate(response.Headers);
            }

            if (readXmlDelegate != null)
            {
                using (var responseStream = response.GetResponseStream())
                {
                    if(responseStream == null)
                        throw new Exception("The response stream returned is null");

                    using (var xmlReader = new XmlTextReader(responseStream))
                        readXmlDelegate(xmlReader);
                }
            }

            return statusCode;
        }

        private static void WritePostParameters(Stream outputStream, Action<XmlTextWriter> writeXmlDelegate)
        {
            using (var xmlWriter = new XmlTextWriter(outputStream, Encoding.UTF8))
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.Formatting = Formatting.Indented;

                writeXmlDelegate(xmlWriter);

                xmlWriter.WriteEndDocument();
            }
        }
    }
}