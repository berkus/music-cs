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
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MusiC.Extensions
{
	class ExtensionCache : MusiCObject
	{
		// Attributes
		private Configurator _objConfig;
		
		#region Static Attributes
		static readonly private Dictionary<String, ExtensionInfo> _extensionList = new Dictionary<String, ExtensionInfo>();
		static readonly private LinkedList<ExtensionInfo> _configList = new LinkedList<ExtensionInfo>();
		static readonly private LinkedList<IHandler> _handlerList = new LinkedList<IHandler>();
		#endregion
		
		#region Extension Handling
		public void Load(String extensionsDir)
		{
			foreach(String ext in Directory.GetFiles(extensionsDir, "*.dll"))
			{
				// FIX: When MusiC.dll is loaded the Type.IsSubClassOf() fails.
				if(Path.GetFileName(ext) == "MusiC.dll")
					continue;
					
				try {
					Assembly l = Assembly.LoadFrom(ext);
					BeginReportSection(ext+" ... [LOADED]");
					
					foreach(Type t in l.GetExportedTypes())
					{
						Add(t);
					}
					
					EndReportSection(true);
				}
				catch {
					// Probably trying to load an unmanaged assembly.
					Warning(ext+" ... [LOADING FAILED]");
				}
			}
		}
		
		public void Add(Type extensionType)
		{
			if(ExtensionInfo.Identify(extensionType) == ExtensionKind.Error)
			{
				Message(extensionType.ToString() + " ... [REJECTED]");
				return;
			}
			
			ExtensionInfo info = new ExtensionInfo(extensionType);
			
			switch(info.Kind)
			{
				case ExtensionKind.Configuration :
					bool unique = true;
					
					// Compare Type objects to find different objects representing the same type.
					foreach(ExtensionInfo i in _configList)
					{
						if(i.Class == info.Class) 
						{	
							unique = false;
							break;
						}	
					}
					
					if(unique)
						_configList.AddLast(info);
					
					break;
				
				case ExtensionKind.Classifier:
				case ExtensionKind.Feature:
				case ExtensionKind.Window:
					if (!_extensionList.ContainsKey(extensionType.FullName))
						_extensionList.Add(extensionType.FullName, info);
					break;
					
				case ExtensionKind.FileHandler:
					_handlerList.AddLast(info.Instantiate(null) as IHandler);
					break;
				
				case ExtensionKind.NotSet:
				case ExtensionKind.Error:
					Error("Please report this event at our site, http://sites.google.com/site/music-cs.");
					break;
			}
			
			Message(extensionType.ToString() + " ... [ADDED]");
		}
		#endregion
		
		#region FileHandlers Handling
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
		#endregion
		
		#region Configurator Handling
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
		#endregion
		
		#region Info Handling
		static public ExtensionInfo GetInfo(String className)
		{
			ExtensionInfo info;
			
			// info is null if there isn't a class with that name.
			_extensionList.TryGetValue(className, out info);
			
			return info;
		}
		#endregion
		
		#region Checkers
		public bool HasConfig()
		{
			return (_configList.Count > 0);
		}
		
		public bool HasFileHandler()
		{
			return (_handlerList.Count > 0);
		}
		#endregion
	}
}
