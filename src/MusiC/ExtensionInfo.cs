using System;
using System.Reflection;
using System.Xml;
using System.Collections.Generic;

using MCModule.Exceptions;

namespace MusiC
{
	public class ParamList : Parametrized
	{
	}
	
	public class Parametrized : MusiCObject
	{
		Boolean _isParamInitiated = false;
		
		LinkedList<Instantiable> _paramList = new LinkedList<Instantiable>();
		
		public virtual void AddParam(String paramName, String paramClass)
		{
			AddParam(paramName, paramClass, null);
		}
		
		public virtual void AddParam(String paramName, String paramClass, String strValue)
		{
			Instantiable i = new Instantiable(paramName, paramClass);
			i.StrValue = strValue;
			_paramList.AddLast(i);
		}
		
		virtual public void Instantiate()
		{
			if(_paramList.Count != 0 && !_isParamInitiated)
			{
				foreach(Instantiable i in _paramList)
					i.Instantiate();
			}
		}
		
		public Instantiable GetParamByName(String name)
		{
			foreach(Instantiable i in _paramList)
			{
				if(i.Name == name)
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
				paramClassTypes[i] = param.TypeObj; 
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
				paramValue[j] = param.Value; 
				j++;
			}
			
			return paramValue;
		}
	}
	
	public class Instantiable : Parametrized
	{
		String _name = null;
		String _classname = null;
		Type _class = null;
		
		String _strValue = null;
		Object _value = null;
		
		Boolean _isInitiated = false;
		
		public Type TypeObj
		{
			get { return _class; }
			set { _class = value; _classname = value.FullName; }
		}
		
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
		
		public Object Value
		{
			get { return _value; }
		}
		
		public Instantiable(String paramName, String paramClass)
		{
			_name=paramName;
			_classname=paramClass;
			_class = Type.GetType(paramClass, false, false);
		}
		
		override public void Instantiate()
		{
			if( _value != null || _isInitiated)
				return;
			
			///@todo throw exception
			if(_class == null || _classname == null)
				return;
			
			base.Instantiate();
			
			if(_strValue != null)
			{
				MethodInfo parse = _class.GetMethod("Parse", new Type[]{typeof(String)});
				
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
	
	public class ExtensionInfo : MusiCObject
	{
		Type _class=null;
		ExtensionType _type = ExtensionType.NotSet;
		
		public ExtensionType Type
		{
			get { return _type; }
		}
		
		public ExtensionInfo(Type t)
		{
			_class = t;
			_type = Identify(t);
		}
		
		ExtensionType Identify(Type t)
		{
			if(typeof(Config).IsAssignableFrom(t))
				return ExtensionType.Configuration;
			
			if(typeof(Classifier).IsAssignableFrom(t))
				return ExtensionType.Classifier;
			
			if(typeof(Feature).IsAssignableFrom(t))
				return ExtensionType.Feature;
			
			//if(typeof(Window).IsAssignableFrom(t))
			//	return ExtensionType.FileHandler;
			
			if(typeof(Window).IsAssignableFrom(t))
				return ExtensionType.Window;
			
			return ExtensionType.NotSet;
		}
		
		public Extension Instantiate(ParamList pList)
		{
			Type[] paramTypes = pList.GetTypes();
			ConstructorInfo ctor = _class.GetConstructor(paramTypes);
			
			Extension e = null;
			
			if(ctor != null)
			{
				e = ctor.Invoke(pList.GetParamsValue()) as Extension;
			}
			
			return e;
		}
	}
}