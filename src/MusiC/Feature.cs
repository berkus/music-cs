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

namespace MusiC
{
	/// @brief Basic feature type.
	/// @details To implement a new feature you must extended this class.
	/// @todo Implement IDispose interface
	abstract unsafe public class Feature : Extension
	{
		[CLSCompliant(false)]
		abstract unsafe public class UnmanagedImplementation : Feature
		{
			Single * _data;
			Single * _temp;
		
			protected UnmanagedImplementation(String name) : base(name)
			{
			}
			
			protected Single * OuterExtract(Window.UnmanagedImplementation window)
			{
				if(_temp == null)
				{
					_temp = NativeMethods.Pointer.dgetmem(window.WindowSize);
					_wndSize = window.WindowSize;
				}
				
				if(_data == null)
				{ 
					_data = NativeMethods.Pointer.dgetmem(window.WindowCount);
					_wndCount = window.WindowCount;
				}
				
				if(window.WindowSize > _wndSize)
				{
					NativeMethods.Pointer.free(_temp);
					_temp = NativeMethods.Pointer.dgetmem(window.WindowSize);
				}
				
				if(window.WindowCount > _wndCount)
				{
					NativeMethods.Pointer.free(_data);
					_data = NativeMethods.Pointer.dgetmem(window.WindowCount);
				}
						
				return Extract(window);
			}
			
			/// All frame data must be consecutive
			abstract public Single * Extract(Window.UnmanagedImplementation window);
		}
		
		abstract public class ManagedImplementation : Feature
		{
			protected ManagedImplementation(String name) : base(name)
			{
			}
			
			virtual protected Single[] OuterExtract(Window.ManagedImplementation window)
			{
				return Extract(window);
			}
			
			virtual public Single[] Extract(Window.ManagedImplementation window)
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
	
		protected Feature(String name)
		{
			_name = name;
			Console.WriteLine(name);
		}
		
		virtual public Int32 FeatureSize(Window window)
		{
			return window.WindowCount;
		}
	} 
}