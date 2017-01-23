﻿// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
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
using System.IO;
using System.Text;
using ExtendedXmlSerialization.Configuration;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization
{
	public static class Extensions
	{
		public static IExtendedXmlSerializer Create<T>(this T @this, Action<T> configure) where T : IExtendedXmlConfiguration
		{
			configure(@this);
			var result = @this.Create();
			return result;
		}

		public static string Serialize(this IExtendedXmlSerializer @this, object instance)
		{
			using (var stream = new MemoryStream())
			{
				@this.Serialize(stream, instance);
				stream.Seek(0, SeekOrigin.Begin);
				var result = new StreamReader(stream).ReadToEnd();
				return result;
			}
		}

		public static T Deserialize<T>(this IExtendedXmlSerializer @this, string xml)
		{
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
			{
				var result = @this.Deserialize(stream).AsValid<T>();
				return result;
			}
		}
	}
}