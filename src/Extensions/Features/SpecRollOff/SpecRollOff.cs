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

//namespace MCModule.Features
//{
//	public class SpecRollOff : MCModule.Feature
//	{
//		public SpecRollOff() : base("SpecRollOff")
//		{
//		}
//		
//		override unsafe public double * Extract(Window wnd)
//		{
//			double sum = 0, cum = 0;
//			int sro = 0;
//			
//			for(int w = 0; w < wnd.WindowCount; w++)
//			{
//				sum = 0; cum = 0;
//				
//				MCMath.FFTMagnitude(wnd.GetWindow(w), m_temp, wnd.WindowSize);
//						
//				for (int i = 0; i < wnd.WindowSize; i++)
//					sum += *(m_temp + i) * *(m_temp + i);
//		
//				cum = sum;
//		
//				for (sro = wnd.WindowSize - 1; sro >= 0; sro--)
//				{
//					cum -= *(m_temp + sro);
//					if (cum < 0.95 * sum)
//						break;
//				}
//				
//				*(m_data + w) = sro;
//			}
//			
//			return m_data;
//		}
//		
//		override public int FeatureSize(Window wnd)
//		{
//			return wnd.WindowCount;
//		}
//	}
//}

namespace MusiC.Extensions.Features
{
	public class SpecRollOffU : Feature.UnmanagedImpl
	{	
		public SpecRollOffU() : base("SpecRolloff")
		{
		}
		
		override unsafe public Single * Extract(Window.UnmanagedImpl wnd)
		{
			return null;
		}
	}
	
	public class SpecRollOffM : Feature.ManagedImpl
	{	
		public SpecRollOffM() : base("SpecRolloff")
		{
		}
		
		override public Single[] Extract(Window.ManagedImpl wnd)
		{
			return null;
		}
	}
}