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
using System.Xml;
using System.Collections.Generic;

using MCModule.Exceptions;

namespace MusiC
{
	public class ParamList : Parametrized
	{
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