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

namespace MCModule
{
	public class HandlerInfo : BinaryInfo
	{
		string mPattern;
		
		public string Pattern
		{
			get { return mPattern; }
			set { mPattern = value != null ? value : String.Empty; }
		}
		
		new public void Parse(XmlNode n)
		{
			base.Parse(n);
			
			XmlAttribute attr = n.Attributes["pattern"];
			if(attr == null || attr.Value == null) throw new MissingInformationException("pattern", n);
			else mPattern = attr.Value;
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