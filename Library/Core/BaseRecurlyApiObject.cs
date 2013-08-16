using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Recurly.Core
{
    /// <summary>
    /// Base class for Recurly API Objects
    /// </summary>
    public abstract class BaseRecurlyApiObject
    {
        protected abstract void ReadElement( XElement element);

        protected virtual void PreReadInitialization()
        {
        }

        internal virtual void ReadXml(XmlTextReader reader)
        {
            PreReadInitialization();

            var element = XDocument.Load(reader).Root;

            ReadElement(element);
        }
    }
}
