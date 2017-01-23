// MIT License
// 
// Copyright (c) 2016 Wojciech Nag�rski
//                    Michael DeMond
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.ElementModel;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	public class XmlReadContext : XmlReadContext<IContainerElement>
	{
		public XmlReadContext(XElement data) : base(data) {}

		public XmlReadContext(IXmlReadContextFactory factory, IContainerElement container, IElement element,
		                      XElement data)
			: base(factory, container, element, data) {}

		public XmlReadContext(IXmlReadContextFactory factory, IContainerElement container, IElement element,
		                      IElement selected, XElement data) : base(factory, container, element, selected, data) {}
	}

	public abstract class XmlReadContext<T> : IReadContext<T>, IXmlReadContext where T : class, IContainerElement
	{
		readonly IXmlReadContextFactory _factory;

		protected XmlReadContext(XElement data)
			: this(XmlReadContextFactory.Default, Elements.Default.Get(Types.Default.Get(data.Name)), data) {}

		protected XmlReadContext(IXmlReadContextFactory factory, IElement element, XElement data, params object[] services)
			: this(factory, null, element, element, data)
		{
			for (var i = 0; i < services.Length; i++)
			{
				Add(services[i]);
			}
		}

		protected XmlReadContext(IXmlReadContextFactory factory, T container, IElement element, XElement data)
			: this(factory, container, element, container, data) {}

		protected XmlReadContext(IXmlReadContextFactory factory, T container, IElement element, IElement selected,
		                         XElement data)
		{
			_factory = factory;
			Container = container;
			Element = element;
			Selected = selected;
			Data = data;
		}

		public XElement Data { get; }

		public T Container { get; }
		IContainerElement IContext.Container => Container;

		public IElement Element { get; }
		public IElement Selected { get; }

		public object GetService(Type serviceType) => Data.AnnotationAll(serviceType);

		public void Add(object service) => Data.AddAnnotation(service);

		public string Read() => Data.Value;

		public IEnumerable<IReadContext> Items()
		{
			var container = (ICollectionElement) Element;
			foreach (var child in Data.Elements())
			{
				yield return _factory.Create(container.Element, child);
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public IEnumerator<IReadMemberContext> GetEnumerator()
		{
			var members = ((IMemberedElement) Element).Members;
			foreach (var source in Data.Elements())
			{
				var member = members.Get(source.Name.LocalName);
				if (member != null)
				{
					yield return (IReadMemberContext) _factory.Create(member, source);
				}
			}
		}

		public string this[IName name] => _factory.Value(name, Data);
		public IContext Select() => _factory.Select(this);
	}
}