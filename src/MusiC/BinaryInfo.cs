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
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace MusiC
{
	// TODO: Make use of base class
	public class BinaryInfo : BinaryParam
	{	
		// Private members
		LinkedList<BinaryParam> mParam = new LinkedList<BinaryParam>();
		Type _tAssembly=null;
		
		// Constructor
		public BinaryInfo()
		{
		}
		
		public BinaryInfo(Type t)
		{
			_tAssembly=t;
		}
		
		// Public Methods
		public Extension New()
		{
			return Invoker.LoadType(this);
		}
		
		public void AddParam(String tag, Type t)
		{
			mParam.AddLast(new BinaryParam(tag, t));
		}
		
		public void AddParam(String tag, String t)
		{
			//mParam.AddLast(new BinaryParam(tag, t));
		}
	
		public LinkedList<BinaryParam> GetParam()
		{
			return mParam;
		}
	
		public Type[] GetTypes()
		{
			Type[] t = new Type[mParam.Count];
			uint n = 0;
	
			foreach (BinaryParam p in mParam)
			{
				//t[n] = p.Type;
				n++;
			}
	
			return t;
		}
	
		public Object[] GetParamValues()
		{
			Object[] t = new Object[mParam.Count];
			uint n = 0;
	
			foreach (BinaryParam p in mParam)
			{
				t[n] = p.Value;
				n++;
			}
	
			return t;
		}
		
		public void Print()
		{
			foreach (BinaryParam p in mParam)
			{
				Console.WriteLine("* " + p.Name + ":" + p.strValue + ":" + p.Type.ToString());
			}
		}
	}
}
