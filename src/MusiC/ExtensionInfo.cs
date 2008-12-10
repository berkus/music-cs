using System;
using System.Xml;
using System.Collections.Generic;

using MCModule.Exceptions;

namespace MusiC
{
	public class Instantiable : MusiCObject
	{
		String _name = null;
		String _class = null;
		Dictionary<String, Instantiable> _paramCache = new Dictionary<String, Instantiable>();
		
		public String Name
		{
			get { return _name; }
			set {_name = value; }
		}
		
		public String Class
		{
			get { return _class; }
			set { _class = value; }
		}
		
		public Instantiable(String paramName, String tParam)
		{
			_name=paramName;
			_class=tParam;
		}
		
		public virtual void AddParam(String paramName, String paramClass)
		{
			_paramCache.Add(paramName, new Instantiable(paramName, paramClass));
		}
		
		public virtual String[] GetTypes()
		{
			return null;
		}
		
		public virtual String[] GetParamsValue()
		{
			return null;
		}
	}
	
	public enum ExtensionType
	{
		Classifier,
		Feature,
		Window,
		Configuration,
		FileHandler,
		NotSet
	}
	
	public class ExtensionInfo : Instantiable
	{
		ExtensionType _type = ExtensionType.NotSet;
		
		public ExtensionType Type
		{
			get { return _type; }
			set { _type = value; }
		}
		
		public ExtensionInfo() : base(null, null)
		{
		}
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
