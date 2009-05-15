/*
 * The MIT License
 * Copyright (c) 2008-2009 Marcos José Sant'Anna Magalhães
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

namespace MusiC.Extensions.Features
{
	unsafe
	public class Loudness : Unmanaged.Feature
	{
		// this will be replaced later.
		float d = 0.046875f;
		uint sz = 0;
		float * _w, _x;

		~Loudness()
		{
		}
		
		override unsafe
		public float Extract (float * wndData, int wndSize)
		{
			if( sz != wndSize )
			{	
				// Local buffer managed by the managed base class.
				// As long as the size doesn't change it may be used to store values 
				// between calls.

				if( sz < wndSize )
				{
					sz = (uint) wndSize;
					
					NativeMethods.Pointer.free( _w );
					NativeMethods.Pointer.free( _x );
					
					_w = NativeMethods.Pointer.fgetmem( wndSize );
					_x = NativeMethods.Pointer.fgetmem( wndSize );
				}
				
				for( int idx = 0; idx < wndSize; idx++ )
				{
					float f = idx * d;
					_w[ idx ] = (float) (-0.6 * 3.64 * Math.Pow(f, -0.8) - 6.5 * Math.Pow(Math.E, (f-3.3) * (f-3.3) ) + 0.001 * Math.Pow(f, 3.6));
				}
			}
			
			float ld = 0.0f;
			
			NativeMethods.Math.FFTMagnitude( wndData, _x, wndSize );
			for( int idx = 0; idx < wndSize; idx++ )
			{
				ld += (float) ( _x[ idx ] * _x[ idx ] * Math.Pow(10, _w[ idx ] / 20 ) );
			}
			
			return ld;
		}

	}
}