using System;
using System.Xml;

using MCModule.Exceptions;

namespace MusiC
{
	public class ExtensionInfo : BinaryInfo
	{
		// Atributtes
		
		String mTag = String.Empty;
		
		// Properties
		
		public String Tag
		{
			get { return mTag; }
			set { if (value != null) { mTag = value; } }
		}
		
		// Methods
	}
}

namespace MCModule
{
	class ExtensionInfo : BinaryInfo
	{
		// Atributtes
		
		String mTag = String.Empty;
		
		// Properties
		
		public String Tag
		{
			get { return mTag; }
			set { if (value != null) { mTag = value; } }
		}
		
		// Methods
		
		new public void Parse(XmlNode n)
		{
			base.Parse(n);
			
			XmlAttribute attr = n.Attributes["tag"];
			if(attr == null || attr.Value == null) throw new MissingInformationException("tag", n);
			else mTag = attr.Value;
			
			XmlNodeList nList = n.ChildNodes;
			foreach (XmlNode param in nList)
			{
				if(param.Name == "Param")
				{
					AddParam(param.Attributes["name"].Value, Type.GetType(param.Attributes["type"].Value, true, true));
					///@note the parameter type must be in the GAC (or known in runtime).
					///@todo add try-catch to Type.GetType and throw new exception
					//Console.WriteLine("Param must have attribute type or the specified type is unknown.");
					//Console.WriteLine("System Message:{0}", ex.Message);
				}
			}
		}
	}
}
