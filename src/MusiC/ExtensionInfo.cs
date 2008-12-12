using System;
using System.Reflection;
using System.Xml;
using System.Collections.Generic;

using MCModule.Exceptions;

namespace MusiC
{
	public class Instantiable : MusiCObject
	{
		String _name = null;
		String _classname = null;
		Type _class = null;
		
		String _strValue = null;
		Object _value = null;
		
		Boolean _isParamInitiated = false;
		Boolean _isInitiated = false;
		
		LinkedList<Instantiable> _paramList = new LinkedList<Instantiable>();
		
		public String Name
		{
			get { return _name; }
			set {_name = value; }
		}
		
		public String Class
		{
			get { return _classname; }
			set { _class = Type.GetType(value, false, false); _classname = value; }
		}
		
		public String StrValue
		{
			get { return _strValue; }
			set { _strValue=value; }
		}
		
		public Instantiable(String paramName)
		{
			_name=paramName;
		}
		
		public Instantiable(String paramName, String tParam)
		{
			_name=paramName;
			_classname=tParam;
			_class = Type.GetType(tParam, false, false);
		}
		
		public virtual void AddParam(String paramName, String paramClass)
		{
			_paramList.AddLast(new Instantiable(paramName, paramClass));
		}
		
		public void Instantiate()
		{
			if( _value != null || _isInitiated)
				return;
			
			///@todo throw exception
			if(_class == null || _classname == null)
				return;
			
			if(_paramList.Count != 0 && !_isParamInitiated)
			{
				foreach(Instantiable i in _paramList)
					i.Instantiate();
			}
			
			if(_strValue != null)
			{
				MethodInfo parse = _class.GetMethod("Parse");
				
				if(parse == null)
				{
					Warning(_classname+".Parse wasn't found. This must be a static method. Using declared constructor.");
				}
				else
				{
					_value = parse.Invoke(null, new Object[] { StrValue } );
					return;
				}
			}
			
			ConstructorInfo defaultCtor = _class.GetConstructor(GetTypes());
			defaultCtor.Invoke(GetParamsValue());
		}
		
		public Instantiable GetParamByName(String name)
		{
			foreach(Instantiable i in _paramList)
			{
				if(i._name == name)
					return i;
			}
			
			return null;
			
			///@todo error handling
		}
		
		public virtual Type[] GetTypes()
		{
			int paramCount = _paramList.Count;
			Type[] paramClassTypes = new Type[paramCount];
			
			int i = 0;
			foreach(Instantiable param in _paramList)
			{
				paramClassTypes[i] = param._class; 
				i++;
			}
			
			return paramClassTypes;
		}
		
		public virtual Object[] GetParamsValue()
		{
			if(_paramList.Count != 0 && !_isParamInitiated)
			{
				foreach(Instantiable i in _paramList)
					i.Instantiate();
			}
			
			Int32 paramCount = _paramList.Count;
			Object[] paramValue = new Object[paramCount];
			
			Int32 j = 0;
			foreach(Instantiable param in _paramList)
			{
				paramValue[j] = param._value; 
				j++;
			}
			
			return paramValue;
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
		
		public ExtensionInfo() : base(null)
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
