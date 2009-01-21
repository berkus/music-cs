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
	class Instantiable : ParamList
	{
		String _name;
		String _classname;
		Type _class;
		
		String _strValue;
		Object _value;
		
		Boolean _isInitiated = false;
		
		public Type TypeObj
		{
			get { return _class; }
			set { _class = value; _classname = value.FullName; }
		}
		
		public String Name
		{
			get { return _name; }
			set {_name = value; }
		}
		
		public String Class
		{
			get { return _classname; }
			set { _class = Type.GetType(value, false, false); _classname = value; }
		}
		
		public String StrValue
		{
			get { return _strValue; }
			set { _strValue=value; }
		}
		
		public Object Value
		{
			get { return _value; }
		}
		
		public Instantiable(String paramName, String paramClass)
		{
			_name=paramName;
			_classname=paramClass;
			_class = Type.GetType(paramClass, false, false);
		}
		
		override public void Instantiate()
		{
			if( _value != null || _isInitiated)
				return;
			
			///@todo throw exception
			if(_class == null || _classname == null)
				return;
			
			base.Instantiate();
			
			if(_strValue != null)
			{
				MethodInfo parse = _class.GetMethod("Parse", new Type[]{typeof(String)});
				
				if(parse == null)
				{
					Warning(_classname+".Parse wasn't found. This must be a static method. Using declared constructor.");
				}
				else
				{
					_value = parse.Invoke(null, new Object[] { StrValue } );
					return;
				}
			}
			
			ConstructorInfo defaultCtor = _class.GetConstructor(GetTypes());
			defaultCtor.Invoke(GetParamsValue());
		}
	}
}