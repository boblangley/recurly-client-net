using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace Recurly.Core
{
    internal static class RecurlyUtilities
    {        
        public static string ReadElementAttribute(this XmlTextReader reader, string attribute)
        {
            var value = reader.GetAttribute(attribute);
            return value ?? String.Empty;
        }

        public static T ReadElementContentAsEnum<T>(this XmlTextReader reader) where T: struct
        {
            var value = reader.ReadElementContentAsString();
            return ParseEnumString<T>(value);
        }

        public static void ProcessChild(this XElement element, string name, Action<XElement> processDelegate)
        {
            var child = element.Element(name);
            if(child != null)
                processDelegate(child);
            else
            {
                Console.WriteLine("Child {0} not found in {1}",name,element);
            }
        }

        public static T ToEnum<T>(this XElement element) where T : struct
        {
            return ParseEnumString<T>(element.Value);
        }

        public static string GetHrefLinkId(this XElement element)
        {
            return element.Attribute("href").Value.Split('/').Last();
        }

        public static T GetHrefLinkId<T>(this XElement element, Func<string, T> parseDelegate)
        {
            return parseDelegate(GetHrefLinkId(element));
        }

        public static T ToEnum<T>(this XAttribute attribute) where T : struct
        {
            return ParseEnumString<T>(attribute.Value);
        }

        public static int ToInt(this XElement element)
        {
            return int.Parse(element.Value);
        }

        public static DateTime ToDateTime(this XElement element)
        {
            return DateTime.Parse(element.Value);
        }

        public static bool ToBool(this XElement element)
        {
            return bool.Parse(element.Value);
        }

        public static T? ToNullable<T>(this XElement element, Func<string, T> parseElementDelegate) where T : struct
        {
            return String.IsNullOrWhiteSpace(element.Value) ? new T?() : parseElementDelegate(element.Value);
        }

        public static T ReadElementAttributeAsEnum<T>(this XmlTextReader reader, string attributeName) where T : struct
        {
            var value = reader.GetAttribute(attributeName);
            return ParseEnumString<T>(value);
        }

        private static T ParseEnumString<T>(string value) where T: struct
        {
            T val;
            if (!Enum.TryParse(value, true, out val))
                throw new Exception(String.Format("Cannot parse value {0} of the enum {1}", value, val.GetType().Name));

            return val;
        }

        public static T? ReadElementContentAsNullable<T>(this XmlTextReader reader, Func<XmlTextReader,T> elementHasValueReadDelegate) where T: struct, IComparable
        {
            var value = reader.Value;
            return String.IsNullOrWhiteSpace(value) ? new T?() : elementHasValueReadDelegate(reader);
        }

        public static void WriteElementStringIfProvided(this XmlTextWriter writer, string elementName, string value)
        {
            if (!String.IsNullOrWhiteSpace(value))
                writer.WriteElementString(elementName, value);
        }

        public static void WriteElementStringAsNillable(this XmlTextWriter writer, string elementName, string value)
        {            
            writer.WriteStartElement(elementName);
            if (String.IsNullOrWhiteSpace(value))
                writer.WriteAttributeString("nil","nil");       
            else
                writer.WriteString(value);
            writer.WriteEndElement();
        }

        public static void WriteElementIntIfProvided(this XmlTextWriter writer, string elementName, int? value)
        {
            if (value.HasValue)
                writer.WriteElementString(elementName,value.Value.ToString());
        }

        public static void WriteElementDateTimeIfProvided(this XmlTextWriter writer, string elementName, DateTime? value)
        {
            if(value.HasValue)
                writer.WriteElementString(elementName,value.Value.ToString("s"));
        }

        public static void WriteElementDateTimeIfFuture(this XmlTextWriter writer, string elementName, DateTime value)
        {
            if (value > DateTime.Now)
                writer.WriteElementString(elementName, value.ToString("s"));
        }

        public static void WriteElementEnum(this XmlTextWriter writer, string elementName, Enum value)
        {
            writer.WriteElementString(elementName, Enum.GetName(value.GetType(), value).ToLower());
        }

        public static void WriteElementList<T>(this XmlTextWriter writer, string elementName, List<T> list, Action<XmlTextWriter, T> itemWriteDelegate)
        {
            writer.WriteStartElement(elementName);
            list.ForEach(i => itemWriteDelegate(writer, i));
            writer.WriteEndElement();
        }

        public static void WriteElementListIfAny<T>(this XmlTextWriter writer, string elementName, List<T> list, Action<XmlTextWriter,T> itemWriteDelegate)
        {
            if(!list.Any()) return;
            WriteElementList(writer, elementName,list,itemWriteDelegate);
        }

        public static string UrlEncode(this string val)
        {
            return HttpUtility.UrlEncode(val);
        }
    }
}
