using System;
using System.Collections.Generic;

using MusiC;

namespace MusiC.Extensions
{
	public class ExtensionCache : MusiCObject, IGlobal
	{
		Config _objConfig=null;
		Type _tConfig=null;
		
		//Dictionary<String, Type> _tHandlerCache = new Dictionary<String, Type>();
		Dictionary<String, ExtensionInfo> _extensionList = new Dictionary<String, ExtensionInfo>();
		
		public void Initialize()
		{
		}
		
		public void Add(Type t)
		{
			ExtensionInfo info = new ExtensionInfo(t);
			_extensionList.Add(t.FullName, info);
			
			if(info.Type == ExtensionType.Configuration)
				_tConfig = t;
			
			Message(t.ToString() + " ... [ADDED]");
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
		
		public ExtensionInfo GetInfo(String className)
		{
			ExtensionInfo info;
			_extensionList.TryGetValue(className, out info);
			
			return info;
		}
	}
}
