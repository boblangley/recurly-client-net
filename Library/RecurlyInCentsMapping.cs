using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

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

        internal void ReadElement(XElement element)
        {
            element.Elements().ToList().ForEach(AddCurrency);
        }

        private void AddCurrency(XElement element)
        {
            _innerDic.Add(element.Name.LocalName, new RecurlyInCentsItem(element));
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