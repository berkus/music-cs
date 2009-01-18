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

namespace MusiC
{
	public class ParamList : MusiCObject
	{
		Boolean _isParamInitiated = false;
		
		LinkedList<Instantiable> _paramList = new LinkedList<Instantiable>();
		
		public virtual void AddParam(String paramName, String paramClass)
		{
			AddParam(paramName, paramClass, null);
		}
		
		public virtual void AddParam(String paramName, String paramClass, String strValue)
		{
			Instantiable i = new Instantiable(paramName, paramClass);
			i.StrValue = strValue;
			_paramList.AddLast(i);
		}
		
		virtual public void Instantiate()
		{
			if(_paramList.Count != 0 && !_isParamInitiated)
			{
				foreach(Instantiable i in _paramList)
					i.Instantiate();
			}
		}
		
		public Instantiable GetParamByName(String name)
		{
			foreach(Instantiable i in _paramList)
			{
				if(i.Name == name)
					return i;
			}
			
			return null;
			
			///@todo error handling
		}
		
		public virtual Type[] GetTypes()
		{
			int paramCount = _paramList.Count;
			Type[] paramClassTypes = new Type[paramCount];
			
			int i = 0;
			foreach(Instantiable param in _paramList)
			{
				paramClassTypes[i] = param.TypeObj; 
				i++;
			}
			
			return paramClassTypes;
		}
		
		public virtual Object[] GetParamsValue()
		{
			if(_paramList.Count != 0 && !_isParamInitiated)
			{
				foreach(Instantiable i in _paramList)
					i.Instantiate();
			}
			
			Int32 paramCount = _paramList.Count;
			Object[] paramValue = new Object[paramCount];
			
			Int32 j = 0;
			foreach(Instantiable param in _paramList)
			{
				paramValue[j] = param.Value; 
				j++;
			}
			
			return paramValue;
		}
	}
}