using System;
using System.Collections.Generic;

using MusiC;

namespace MusiC.Extensions
{
	public class ExtensionCache : MusiCObject, IGlobal
	{
		Config _objConfig=null;
		Type _tConfig=null;
		
		Dictionary<String, Type> _tHandlerCache = new Dictionary<String, Type>();
		
		public void Initialize()
		{
		}
		
		public void Add(Type t)
		{
			string id = Identify(t);
			switch(id)
			{
				case "Config":
					_tConfig=t;
					break;
				
				case "Handler":
					_tHandlerCache.Add(t.ToString(), t);
					break;
					
				default:
					Message(t.ToString() + " ... [REJECTED]");
					return;
			}
			
			Message(t.ToString() + " ... [ADDED]");
		}
		
		string Identify(Type t)
		{
			if(t.IsSubclassOf(typeof(Config)))
				return "Config";
			
			if(t.IsSubclassOf(typeof(Config)))
				return "Config";
			
			return null;
		}
		
		public Config GetConfig()
		{
			if(_objConfig!=null)
				return _objConfig;
			
			if(_tConfig == null)
				throw new Exceptions.MissingExtensionException("MusiC doesn't know how to load a configuration extension. Seems none was provided or aproved.");
			
			_objConfig = Invoker.LoadConfig(_tConfig);
			return _objConfig;
		}
		
		public void GetWindow(String wName)
		{
		}
		
		public void GetFeature(String fName)
		{
		}
		
		public void GetClassifier(String cName)
		{
		}
		
		public void GetHandler(String hName)
		{
		}
		
		public Type GetHandlerType(String hClassName)
		{
			Type hType;
			
			if(!_tHandlerCache.TryGetValue(hClassName, out hType))
				/// @todo Throw exception.
				return null;
			
			return hType;
		}
	}
}
