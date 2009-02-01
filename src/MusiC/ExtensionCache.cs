/*
 * The MIT License
 * Copyright (c) 2008-2009 Marcos Jos� Sant'Anna Magalh�es
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
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Xml.Linq;

namespace MusiC.Extensions
{
	/// <summary>
	/// 
	/// </summary>
	internal class ExtensionCache : MusiCObject
	{
		// Attributes
		private Configurator _objConfig;
		
		#region Static Attributes

		static readonly
		private XDocument _doc;

		static readonly
		private XElement _root;

		static readonly
		private LinkedList<ExtensionInfo> _infoList = new LinkedList<ExtensionInfo>(); 
		
		#endregion
		
		//::::::::::::::::::::::::::::::::::::::://
		
		static
		ExtensionCache()
		{
			_doc = new XDocument(
				new XDeclaration("1.0", "utf-16", "true"),
    			_root = new XElement("MusiC.Extensions.Cache"));
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		#region Extension Handling
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="extensionsDir">
		/// A <see cref="String"/>
		/// </param>
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
		
		//::::::::::::::::::::::::::::::::::::::://

		private XElement BuildEntry(ExtensionInfo info)
		{
			XElement entry = new XElement("MusiC.Extension");
			entry.Add(new XAttribute("Index", _infoList.Count() + 1));
			entry.Add(new XElement("Class",info.Class.FullName));
			entry.Add(new XElement("Kind", info.Kind.ToString()));

			return entry;
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		public void Add(Type extensionType)
		{
			
			if(ExtensionInfo.Identify(extensionType) == ExtensionKind.Error)
			{
				Message(extensionType.ToString() + " ... [REJECTED]");
				return;
			}
			
			ExtensionInfo info = new ExtensionInfo(extensionType);
			
			_root.Add(BuildEntry(info));
			_infoList.AddLast(info);
			
			Message(extensionType.ToString() + " ... [ADDED]");
		}
		
		#endregion
		
		//::::::::::::::::::::::::::::::::::::::://
		
		#region FileHandlers Handling
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="file">
		/// A <see cref="String"/>
		/// </param>
		/// <returns>
		/// A <see cref="Managed.Handler"/>
		/// </returns>
		public Managed.Handler GetManagedHandler(String file)
		{
//			foreach(IHandler h in _handlerList)
//			{
//				if(h.CanHandle(file) && ExtensionManagement.Managed == ExtensionInfo.IdentifyManagement(h.GetType()))
//				{
//					return h as Managed.Handler;
//				}
//			}
			
			return null;
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="file">
		/// A <see cref="String"/>
		/// </param>
		/// <returns>
		/// A <see cref="Unmanaged.Handler"/>
		/// </returns>
		public Unmanaged.Handler GetUnmanagedHandler(String file)
		{
//			foreach(IHandler h in _handlerList)
//			{
//				if(h.CanHandle(file) && ExtensionManagement.Unmanaged == ExtensionInfo.IdentifyManagement(h.GetType()))
//				{
//					return h as Unmanaged.Handler;
//				}
//			}
			
			return null;
		}
		
		#endregion
		
		//::::::::::::::::::::::::::::::::::::::://
		
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

			IEnumerable<ExtensionInfo> result =
				from ExtensionInfo info in _infoList where info.Kind == ExtensionKind.Configuration select info;
			
			if( result.Count() == 0 )
				throw new Exceptions.MissingExtensionException("MusiC doesn't know how to load a configuration extension. Seems none was provided or aproved.");
			
			_objConfig = Invoker.LoadConfig(result.ElementAt(0).Class);
			
			return _objConfig;
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="configObj">
		/// A <see cref="Configurator"/>
		/// </param>
		public void SetConfigurator(Configurator configObj)
		{
			_objConfig = configObj;
		}
		
		#endregion
		
		//::::::::::::::::::::::::::::::::::::::://
		
		#region Info Handling
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="className">
		/// A <see cref="String"/>
		/// </param>
		/// <returns>
		/// A <see cref="ExtensionInfo"/>
		/// </returns>
		static public ExtensionInfo GetInfo(String className)
		{
			IEnumerable<ExtensionInfo> result =
			from ExtensionInfo info in _infoList where info.Class.FullName == className select info;

			if( result.Count() == 0 )
				return null;

			if( result.Count() > 1 )
				return null;
			
			return result.ElementAt(0);
		}
		
		#endregion
		
		//::::::::::::::::::::::::::::::::::::::://
		
		#region Checkers
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public bool HasConfig()
		{
			IEnumerable<ExtensionInfo> result =
				from ExtensionInfo info in _infoList where info.Kind == ExtensionKind.Configuration select info;
			
			return (result.Count() > 0);
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public bool HasFileHandler()
		{
			IEnumerable<ExtensionInfo> result =
				from ExtensionInfo info in _infoList where info.Kind == ExtensionKind.FileHandler select info;
			
			return (result.Count() > 0);
		}
		
		#endregion

		//::::::::::::::::::::::::::::::::::::::://

		public void Say()
		{
			Console.WriteLine(_doc);
		}
	}
}
