using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Recurly
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
            return reader.IsEmptyElement ? new T?() : elementHasValueReadDelegate(reader);
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
