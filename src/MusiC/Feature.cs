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

using MusiC;
using MCModule.UnmanagedInterface;

namespace MusiC
{
	/// @brief Basic feature type.
	/// @details To implement a new feature you must extended this class.
	/// @todo Implement IDispose interface
	abstract unsafe public class Feature : Extension
	{
		abstract unsafe public class UnmanagedImpl : Feature
		{
			Single * _data;
			Single * _temp;
		
			public UnmanagedImpl(String name) : base(name)
			{
			}
			
			protected Single * OutterExtract(Window.UnmanagedImpl wnd)
			{
				if(_temp == null)
				{
					_temp = UnsafePtr.dgetmem(wnd.WindowSize);
					_wndSize = wnd.WindowSize;
				}
				
				if(_data == null)
				{ 
					_data = UnsafePtr.dgetmem(wnd.WindowCount);
					_wndCount = wnd.WindowCount;
				}
				
				if(wnd.WindowSize > _wndSize)
				{
					UnsafePtr.free(_temp);
					_temp = UnsafePtr.dgetmem(wnd.WindowSize);
				}
				
				if(wnd.WindowCount > _wndCount)
				{
					UnsafePtr.free(_data);
					_data = UnsafePtr.dgetmem(wnd.WindowCount);
				}
						
				return Extract(wnd);
			}
			
			/// All frame data must be consecutive
			abstract public Single * Extract(Window.UnmanagedImpl wnd);
		}
		
		abstract public class ManagedImpl : Feature
		{
			public ManagedImpl(String name) : base(name)
			{
			}
			
			virtual protected Single[] OutterExtract(Window.ManagedImpl wnd)
			{
				return Extract(wnd);
			}
			
			virtual public Single[] Extract(Window.ManagedImpl wnd)
			{
				return null;
			}
		}

		Int32 _wndSize;
		Int32 _wndCount;
		String _name;
		
		public String Name
		{
			get { return _name; }
		}
	
		public Feature(String name)
		{
			_name = name;
			Console.WriteLine(name);
		}
		
		virtual public Int32 FeatureSize(Window wnd)
		{
			return wnd.WindowCount;
		}
	} 
}