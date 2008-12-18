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
		NotSet
	}
	
	public class ExtensionInfo : MusiCObject
	{
		Type _class=null;
		ExtensionKind _kind = ExtensionKind.NotSet;
		
		public ExtensionKind Kind
		{
			get { return _kind; }
		}
		
		public ExtensionInfo(Type t)
		{
			_class = t;
			_kind = Identify();
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
			
			//if(typeof(Window).IsAssignableFrom(t))
			//	return ExtensionType.FileHandler;
			
			if(typeof(Window).IsAssignableFrom(_class))
				return ExtensionKind.Window;
			
			return ExtensionKind.NotSet;
		}
		
		/// <summary>
		/// Creates an instance of the represented extension.
		/// </summary>
		/// <param name="pList"></param>
		/// <returns></returns>
		/// @todo Use Invoker
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