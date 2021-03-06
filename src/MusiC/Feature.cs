// Copyright (c) 2008-2009 Marcos José Sant'Anna Magalhães
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;

using MusiC;

namespace MusiC
{
	/// <summary>
	/// 
	/// </summary>
	abstract
	public class BaseFeature : Extension
	{
	}
	
	//---------------------------------------//

	namespace Managed
	{
		abstract public class Feature : BaseFeature
		{
			virtual
			protected Single[] OuterExtract( Managed.Window window )
			{
				return Extract(window);
			}
			
			//::::::::::::::::::::::::::::::::::::::://
			
			virtual
			public Single[] Extract( Managed.Window window )
			{
				return null;
			}
		}
	}
	
	//---------------------------------------//
	
	namespace Unmanaged
	{
		[CLSCompliant(false)]
		abstract unsafe
		public class Feature : BaseFeature
		{
			/// <summary>
			/// 
			/// </summary>
			virtual
			public void Clear()
			{
			}
			
			//::::::::::::::::::::::::::::::::::::::://
			
			/// <summary>
			/// 
			/// </summary>
			/// <param name="wndData">
			/// A <see cref="Single"/>
			/// </param>
			/// <param name="wndSize">
			/// A <see cref="Int32"/>
			/// </param>
			/// <returns>
			/// A <see cref="Single"/>
			/// </returns>
			/// <remarks>All frame data must be consecutive.</remarks>
			abstract
			public Single Extract( Frame dataFrame );
		}
	}
}