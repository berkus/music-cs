/*
 * The MIT License
 * Copyright (c) 2008 Marcos José Sant'Anna Magalhães
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

namespace MCModule
{
	abstract public class Exception : ApplicationException
	{
		abstract public void WriteMessage();
	}
}

namespace MCModule.Exceptions
{	
	public class MissingInformationException : MCModule.Exception
	{
		string mMissingElement = String.Empty;
		string mFileName = String.Empty;
		string mOuterXml = String.Empty;
		
		public MissingInformationException(string elementName, XmlNode n)
		{
			mMissingElement = elementName;
			mFileName = n.OwnerDocument.Name;
			mOuterXml = n.OuterXml;
		}
		
		override public void WriteMessage()
		{
			Console.WriteLine("** Required Information Missing **");
			Console.WriteLine("What: {0}", mMissingElement);
			Console.WriteLine("Where: {0}", mOuterXml);
			Console.WriteLine("When:\n{0}", StackTrace);
			Console.WriteLine("");
			Console.WriteLine("Solution add the missing attribute to the element shown. You can browse de documentation to find more help.");
			Console.WriteLine("http://www.lps.ufrj.br/~mjsmagalhaes/musiC");
			Console.WriteLine("************************");
			Console.WriteLine("");
		}
	}
}

namespace MusiC.Exceptions
{	
	public class MissingAttributeException : MCException
	{
		string mMissingElement = String.Empty;
		string mFileName = String.Empty;
		string mOuterXml = String.Empty;
		
		public MissingAttributeException(string nodeMissing) : base("Couldn't find the obligatory attribute "
			+ nodeMissing + ". Please check case and spelling.")
		{
		}
	}
}