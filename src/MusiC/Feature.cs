/*
 * The MIT License
 * Copyright (c) 2008-2009 Marcos Jos� Sant'Anna Magalh�es
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
 */

using System;

using MusiC;

namespace MusiC
{
	/// @brief Basic feature type.
	/// @details To implement a new feature you must extended this class.
	/// @todo Implement IDispose interface
	abstract
	public class BaseFeature : Extension
	{
	} 

	namespace Managed
	{
		abstract public class Feature : BaseFeature
		{
			virtual
			protected Single[] OuterExtract(Managed.Window window)
			{
				return Extract(window);
			}
			
			virtual
			public Single[] Extract(Managed.Window window)
			{
				return null;
			}
		}
	}
	
	namespace Unmanaged
	{
		[CLSCompliant(false)]
		abstract unsafe
		public class Feature : BaseFeature
		{
			float * _buf;
			int _sz;
			
			protected Single * GetBuffer(int sz)
			{
				if(sz <= _sz)
				{
					return _buf;
				}
				
				if(_buf == null)
					NativeMethods.Pointer.free(_buf);
				
				_buf = NativeMethods.Pointer.fgetmem(sz);
				_sz = sz;
				
				return _buf;
			}
			
			/// All frame data must be consecutive
			abstract
			public Single Extract(Single * wndData, Int32 wndSize);
		}
	}
}