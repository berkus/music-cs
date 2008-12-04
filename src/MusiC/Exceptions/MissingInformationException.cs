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