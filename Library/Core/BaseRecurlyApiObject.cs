using System;
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
        protected abstract string RootElementName { get; }

        protected abstract void ProcessElement( XElement element);

        protected virtual void PreLoopInitializion()
        {
        }

        internal virtual void ReadXml(XmlTextReader reader)
        {
            PreLoopInitializion();

            while(reader.Read())
            {
                if (reader.Name == RootElementName && reader.NodeType == XmlNodeType.EndElement)
                    break;

                if(reader.NodeType != XmlNodeType.Element) continue;

                var element = XElement.Parse(reader.ReadOuterXml());

                ProcessElement(element);
                ProcessReader(element.Name.LocalName, reader);
            }
        }

        protected virtual void ProcessReader(string elementName, XmlTextReader reader)
        {
            
        }

    }
}
