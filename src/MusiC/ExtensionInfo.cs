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
	public enum ExtensionKind
	{
		Classifier,
		Feature,
		Window,
		Configuration,
		FileHandler,
		NotSet,
		Error
	}
	
	public enum ExtensionManagement
	{
		Managed,
		Unmanaged,
		NotSet,
		Error
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <todo>Inherit from ParamList</todo>
	public class ExtensionInfo : MusiCObject
	{
		Type _class=null;
		ExtensionKind _kind = ExtensionKind.NotSet;
		ExtensionManagement _managed = ExtensionManagement.NotSet;
		
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
			_kind = Identify();
			_managed = IdentifyManagement();
		}
		
		/// <summary>
		/// Identify the category of an extension.
		/// </summary>
		/// <returns>
		/// The kind of the extension.
		/// </returns>
		ExtensionKind Identify()
		{
			// This one should be the most frequent call.
			if(typeof(Feature).IsAssignableFrom(_class))
				return ExtensionKind.Feature;
			
			if(typeof(Config).IsAssignableFrom(_class))
				return ExtensionKind.Configuration;
			
			if(typeof(Classifier).IsAssignableFrom(_class))
				return ExtensionKind.Classifier;
			
			if(typeof(Handler).IsAssignableFrom(_class))
				return ExtensionKind.FileHandler;
			
			if(typeof(Window).IsAssignableFrom(_class))
				return ExtensionKind.Window;
			
			return ExtensionKind.Error;
		}
		
		ExtensionManagement IdentifyManagement()
		{
			switch(_kind)
			{
				case ExtensionKind.Classifier:
					return ExtensionManagement.Unmanaged;
				
				case ExtensionKind.Configuration:
					return ExtensionManagement.Managed;
					
				case ExtensionKind.Feature:
					if(typeof(Feature.UnmanagedImplementation).IsAssignableFrom(_class))
						return ExtensionManagement.Unmanaged;
					
					if(typeof(Feature.ManagedImplementation).IsAssignableFrom(_class))
						return ExtensionManagement.Managed;
					
					return ExtensionManagement.Error;
				
				case ExtensionKind.FileHandler:
					return ExtensionManagement.Managed;
					
				case ExtensionKind.Window:
					if(typeof(Window.UnmanagedImplementation).IsAssignableFrom(_class))
						return ExtensionManagement.Unmanaged;
					
					if(typeof(Window.ManagedImplementation).IsAssignableFrom(_class))
						return ExtensionManagement.Managed;
					
					return ExtensionManagement.Error;
					
				default:
					return ExtensionManagement.Error;
			}
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