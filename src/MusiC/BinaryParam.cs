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

namespace MusiC
{
	public class BinaryParam
	{
		String m_name=null;
		public String Name
		{
			get { return m_name; }
		}

		String m_type=null;
		public String Type
		{
			get { return m_type; }
			set { m_type=value; }
		}

		Object m_value=null;
		public Object Value
		{
			get { return m_value; }
		}

		String m_strValue;
		public String strValue
		{
			get { return m_strValue; }
			set
			{
				// m_strValue = value;
				// 
				// if(m_type != typeof(String))
				// {	
					// // TODO: Change to TryParse ?
					// // TODO: Catch exception
					// System.Reflection.MethodInfo parse = m_type.GetMethod("Parse", new Type[] { typeof(String) });
					// m_value = parse.Invoke(null, new object[] { m_strValue });
				// }
				// else
				// {
					// m_value = value;
				// }
			}
		}
		
		public BinaryParam()
		{
		}
		
		public BinaryParam(String name, Type type)
		{
			m_name = name;
			//m_type = type;
		}
	}
}