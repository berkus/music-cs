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
using System.Collections.Generic;
using System.Text;

using MusiC;

//namespace MCModule.Windows
//{
//	public class Hamming : MCModule.Window
//	{
//		public Hamming(int size, int overlap) : base("Hamming", size, overlap)
//		{
//		}
//		
//		override public double Factory(int n)
//		{
//			//return 0.53836 - 0.46164 * (Math.Cos(2 * Math.PI * n / (m_size - 1)));
//			return 0.54 - 0.46 * (Math.Cos(2 * Math.PI * n / (m_size - 1)));
//		}
//	}
//}

namespace MusiC.Extensions.Windows
{
	public class Hamming : Window
	{
		public Hamming(int size, int overlap) : base("Hamming", size, overlap)
		{
		}
		
		override public Single Factory(int n)
		{
			//return 0.53836 - 0.46164 * (Math.Cos(2 * Math.PI * n / (WindowSize - 1)));
			return (Single) (0.54f - 0.46f * (Math.Cos(2.0f * Math.PI * n / (WindowSize - 1))));
		}
	}
}