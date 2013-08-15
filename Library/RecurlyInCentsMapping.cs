using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Recurly
{
    public class RecurlyInCentsMapping : ICollection<RecurlyInCentsItem>
    {
        private readonly Dictionary<string, RecurlyInCentsItem> _innerDic = new Dictionary<string, RecurlyInCentsItem>();
        private readonly bool _optional;
        private readonly string _elementName;
        
        internal RecurlyInCentsMapping(string elementName)
        {
            _elementName = elementName;
        }
        
        internal RecurlyInCentsMapping(string elementName, bool optional) : this(elementName)
        {
            _optional = true;
        }

        public IEnumerator<RecurlyInCentsItem> GetEnumerator()
        {
            return _innerDic.Select(i => i.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(RecurlyInCentsItem item)
        {
            if(_innerDic.ContainsKey(item.Currency))
                _innerDic[item.Currency] = item;
            else
            {
                _innerDic.Add(item.Currency,item);
            }
        }

        public void Clear()
        {
            _innerDic.Clear();
        }

        public bool Contains(RecurlyInCentsItem item)
        {
            return _innerDic.ContainsKey(item.Currency);
        }

        public void CopyTo(RecurlyInCentsItem[] array, int arrayIndex)
        {
            _innerDic.Select(i => i.Value).ToList().CopyTo(array, arrayIndex);
        }

        public bool Remove(RecurlyInCentsItem item)
        {
            return _innerDic.Remove(item.Currency);
        }

        public int Count {
            get { return _innerDic.Count; }
        }

        public bool IsReadOnly { get { return false; } }

        public void ForEach(Action<RecurlyInCentsItem> itemActionDelegate)
        {
            _innerDic.Select(i => i.Value).ToList().ForEach(itemActionDelegate);
        }

        internal void ReadXml(XmlTextReader reader)
        {
            while(reader.Read())
            {
                if (reader.Name == _elementName && reader.NodeType == XmlNodeType.EndElement)
                    break;

                if(reader.NodeType != XmlNodeType.Element) continue;

                _innerDic.Add(reader.Name, new RecurlyInCentsItem(reader));
            }
        }

        internal void WriteXml(XmlTextWriter writer)
        {
            if(_optional && !_innerDic.Any()) return;
            writer.WriteStartElement(_elementName);
            ForEach(i => i.WriteXml(writer));
            writer.WriteEndElement();
        }
    }
}