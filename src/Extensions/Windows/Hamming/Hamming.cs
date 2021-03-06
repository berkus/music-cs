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

namespace MusiC.Extensions.Windows
{
	class Hamming
	{
		static
		public Single Factory(Int32 n, Int32 wndSize)
		{
			//return 0.53836 - 0.46164 * (Math.Cos(2 * Math.PI * n / (wndSize - 1)));
			return (float) (0.54f - 0.46f * (Math.Cos(2.0f * Math.PI * n / (wndSize - 1))));
		}
	}
	
	//----------------------------------------//
	
	public class HammingU : Unmanaged.Window
	{
		public HammingU(int size, int overlap) : base(size, overlap)
		{
		}

		//::::::::::::::::::::::::::::::::::::::://
		
		override
		public Single Factory(int n)
		{
			return Hamming.Factory(n, WindowSize);
		}
	}
	
	//----------------------------------------//
	
	public class HammingM : Managed.Window
	{
		public HammingM(int size, int overlap) : base(size, overlap)
		{
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		override
		public Single Factory(int n)
		{
			return Hamming.Factory(n, WindowSize);
		}
	}
}
