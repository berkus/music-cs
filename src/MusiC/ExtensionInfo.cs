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
using System.Reflection;

namespace MusiC
{
	enum ExtensionKind
	{
		Classifier,
		Feature,
		Window,
		Configuration,
		FileHandler,
		NotSet,
		Error
	}
	
	enum ExtensionManagement
	{
		Managed,
		Unmanaged,
		NotSet,
		Error
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// @todo Inherit from ParamList
	class ExtensionInfo : MusiCObject
	{
		Type _class=null;
		ExtensionKind _kind = ExtensionKind.NotSet;
		ExtensionManagement _managed = ExtensionManagement.NotSet;
		
		public Type Class
		{
			get { return _class; }
		}
		
		public ExtensionKind Kind
		{
			get { return _kind; }
		}
		
		public ExtensionManagement Manager
		{
			get { return _managed; }
		}
		
		public ExtensionInfo(Type t)
		{
			_class = t;
			_kind = Identify(_class);
			_managed = IdentifyManagement(_class);
		}
		
		/// <summary>
		/// Identify the category of an extension.
		/// </summary>
		/// <returns>
		/// The kind of the extension.
		/// </returns>
		static public ExtensionKind Identify(Type t)
		{
			// This one should be the most frequent call.
			if(typeof(BaseFeature).IsAssignableFrom(t))
				return ExtensionKind.Feature;
			
			if(typeof(Configurator).IsAssignableFrom(t))
				return ExtensionKind.Configuration;
			
			if(typeof(IClassifier).IsAssignableFrom(t))
				return ExtensionKind.Classifier;
			
			if(typeof(IHandler).IsAssignableFrom(t))
				return ExtensionKind.FileHandler;
			
			if(typeof(IWindow).IsAssignableFrom(t))
				return ExtensionKind.Window;
			
			return ExtensionKind.Error;
		}
		
		static public ExtensionManagement IdentifyManagement(Type t)
		{
			// Managed
			if(
				typeof(Managed.Feature).IsAssignableFrom(t) ||
				typeof(Managed.Classifier).IsAssignableFrom(t) ||
				typeof(Managed.Handler).IsAssignableFrom(t) ||
				typeof(Managed.Window).IsAssignableFrom(t) ||
				typeof(Configurator).IsAssignableFrom(t)
			)
				return ExtensionManagement.Managed;
			
			// Unmanaged
			if(
				typeof(Unmanaged.Feature).IsAssignableFrom(t) ||
				typeof(Unmanaged.Classifier).IsAssignableFrom(t) ||
				typeof(Unmanaged.Handler).IsAssignableFrom(t) ||
				typeof(Unmanaged.Window).IsAssignableFrom(t)
			)
				return ExtensionManagement.Unmanaged;
					
			return ExtensionManagement.Error;
		}
		
		/// <summary>
		/// Creates an instance of the represented extension.
		/// </summary>
		/// <param name="pList"></param>
		/// <returns></returns>
		/// @todo Use Invoker
		public Extension Instantiate(ParamList pList)
		{
			Type[] paramTypes;
			Object[] paramValues;
			
			if(pList == null)
			{
				paramTypes = new Type[0]{};
				paramValues = null;
			}
			else
			{
				paramTypes = pList.GetTypes();
				paramValues = pList.GetParamsValue();
			}
			
			ConstructorInfo ctor = _class.GetConstructor(paramTypes);
			
			Extension e = null;
			
			if(ctor != null)
			{
				e = ctor.Invoke(paramValues) as Extension;
			}
			
			return e;
		}
	}
}