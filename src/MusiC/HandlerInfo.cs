/*
 * The MIT License
 * Copyright (c) 2008 Marcos Jos� Sant'Anna Magalh�es
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 * 
 */

using System;
using System.Xml;
using System.Text.RegularExpressions;

using MCModule.Exceptions;

namespace MusiC
{
	public class HandlerInfo : BinaryInfo
	{
		string mPattern;
		
		public string Pattern
		{
			get { return mPattern; }
			set { mPattern = value != null ? value : String.Empty; }
		}
		
		public bool MatchPattern(string file)
		{
			Regex r = new Regex(mPattern);
			
			bool match = r.IsMatch(file);
			
			//if(match) Console.WriteLine("{0} - OK!", file);
			//else Console.WriteLine("{0} - X", file);
			
			return match;
		}
	}
}

//namespace MCModule
//{
//	public class HandlerInfo : BinaryInfo
//	{
//		string mPattern;
//		
//		public string Pattern
//		{
//			get { return mPattern; }
//			set { mPattern = value != null ? value : String.Empty; }
//		}
//		
//		new public void Parse(XmlNode n)
//		{
//			base.Parse(n);
//			
//			XmlAttribute attr = n.Attributes["pattern"];
//			if(attr == null || attr.Value == null) throw new MissingInformationException("pattern", n);
//			else mPattern = attr.Value;
//		}
//		
//		public bool MatchPattern(string file)
//		{
//			Regex r = new Regex(mPattern);
//			
//			bool match = r.IsMatch(file);
//			
//			//if(match) Console.WriteLine("{0} - OK!", file);
//			//else Console.WriteLine("{0} - X", file);
//			
//			return match;
//		}
//	}
//}