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

namespace MusiC.Extensions.Features
{
	unsafe
	public class Bandwidth : Unmanaged.Feature
	{
		override unsafe
		public float Extract( Unmanaged.Frame frame )
		{
            int wndSize = frame.Size;
            float * spectrum = frame.RequestFFTMagnitude();

			// weighted-squared-sum
			// squared-sum
			float wss = 0.0f, ss = 0.0f, aux = 0.0f;
			
			for( uint idx = 0; idx < wndSize / 2; idx++ )
			{
                aux = spectrum[ idx ] * spectrum[ idx ];
				ss += aux;
				wss += idx * aux;
			}

			float centr = wss / ss, bw = 0.0f;

			for( uint idx = 0; idx < wndSize / 2; idx++ )
			{
                aux = (centr - idx) * spectrum[idx];
				bw += aux * aux;
			}

			bw = (float) Math.Sqrt( (float) (bw / ss) );
			
			return bw;
		}
	}
}