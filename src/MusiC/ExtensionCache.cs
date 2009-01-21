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
using System.Collections.Generic;

using MusiC;

namespace MusiC.Extensions
{
	class ExtensionCache : MusiCObject
	{
		Configurator _objConfig;
		
		static readonly private Dictionary<String, ExtensionInfo> _extensionList = new Dictionary<String, ExtensionInfo>();
		static readonly private LinkedList<ExtensionInfo> _configList = new LinkedList<ExtensionInfo>();
		static readonly private LinkedList<IHandler> _handlerList = new LinkedList<IHandler>();
		
		public void Add(Type extensionType)
		{
			if(!typeof(Extension).IsAssignableFrom(extensionType))
			{
				Message(extensionType.ToString() + " ... [REJECTED]");
				return;
			}
			
			ExtensionInfo info = new ExtensionInfo(extensionType);
			
			if(info.Kind != ExtensionKind.FileHandler)
				_extensionList.Add(extensionType.FullName, info);
			else
				_handlerList.AddLast(info.Instantiate(null) as IHandler);
			
			if(info.Kind == ExtensionKind.Configuration)
			{
				_configList.AddLast(info);
			}
			
			Message(extensionType.ToString() + " ... [ADDED]");
		}
		
		public Managed.Handler GetManagedHandler(String file)
		{
			foreach(IHandler h in _handlerList)
			{
				if(h.CanHandle(file) && ExtensionManagement.Managed == ExtensionInfo.IdentifyManagement(h.GetType()))
				{
					return h as Managed.Handler;
				}
			}
			
			return null;
		}
		
		public Unmanaged.Handler GetUnmanagedHandler(String file)
		{
			foreach(IHandler h in _handlerList)
			{
				if(h.CanHandle(file) && ExtensionManagement.Unmanaged == ExtensionInfo.IdentifyManagement(h.GetType()))
				{
					return h as Unmanaged.Handler;
				}
			}
			
			return null;
		}
	
		/// <summary>
		/// 
		/// </summary>
		/// <exception cref="MusiC.Exceptions.MissingExtensionException"></exception>
		/// <returns></returns>
		public Configurator GetConfigurator()
		{
			if(_objConfig!=null)
				return _objConfig;
			
			if(_configList.Count == 0)
				throw new Exceptions.MissingExtensionException("MusiC doesn't know how to load a configuration extension. Seems none was provided or aproved.");
			
			_objConfig = Invoker.LoadConfig(_configList.First.Value.Class);
			
			return _objConfig;
		}
		
		public void SetConfigurator(Configurator configObj)
		{
			_objConfig = configObj;
		}
		
		static public IHandler GetHandler(String file)
		{
			foreach(IHandler h in _handlerList)
			{
				if(h.CanHandle(file))
				{
					return h;
				}
			}
			
			return null;
		}
		
		static public ExtensionInfo GetInfo(String className)
		{
			ExtensionInfo info;
			_extensionList.TryGetValue(className, out info);
			
			return info;
		}
	}
}
