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
	public class SpectralFlux : Unmanaged.Feature
	{
		float * lastframe, aux;
		bool first = true;

		~SpectralFlux()
		{
			NativeMethods.Pointer.free( lastframe );
		}
		
		override unsafe
		public float Extract( float * wndData, int wndSize )
		{
			if( lastframe == null )
			{
				lastframe = NativeMethods.Pointer.fgetmem( wndSize );
				aux = NativeMethods.Pointer.fgetmem( wndSize );
			}

			if( first )
			{
				first = false;
				NativeMethods.Math.FFTMagnitude( wndData, lastframe, wndSize );
				return 0.0f;
			}
			
			float ret = 0.0f, dif = 0.0f;
			NativeMethods.Math.FFTMagnitude( wndData, aux, wndSize );
			
			for( int idx = 0; idx < wndSize; idx++ )
			{
				dif = (float) ( Math.Log10( (double) aux[ idx ] ) - Math.Log10( ( double ) lastframe[ idx ] ) );
				ret += dif * dif;
				
				lastframe[ idx ] = aux[ idx ];
			}
			
			return ret;
		}

		override
		public void Clear()
		{
			first = true;
		}
	}
}