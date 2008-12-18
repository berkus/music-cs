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

using MusiC;

namespace MusiC.Extensions
{
	public class ExtensionCache : MusiCObject, IGlobal
	{
		Config _objConfig;
		Type _tConfig;
		
		//Dictionary<String, Type> _tHandlerCache = new Dictionary<String, Type>();
		Dictionary<String, ExtensionInfo> _extensionList = new Dictionary<String, ExtensionInfo>();
		
		public void Initialize()
		{
		}
		
		public void Add(Type extensionType)
		{
			if(!typeof(Extension).IsAssignableFrom(extensionType))
			{
				Message(extensionType.ToString() + " ... [REJECTED]");
				return;
			}
			
			ExtensionInfo info = new ExtensionInfo(extensionType);
			_extensionList.Add(extensionType.FullName, info);
			
			if(info.Kind == ExtensionKind.Configuration)
				_tConfig = extensionType;
			
			Message(extensionType.ToString() + " ... [ADDED]");
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <exception cref="MusiC.Exceptions.MissingExtensionException"></exception>
		/// <returns></returns>
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
