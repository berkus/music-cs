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
        int _size = 0;
		float * lastframe;
		bool first = true;

		~SpectralFlux()
		{
			NativeMethods.Pointer.free( lastframe );
		}
		
		override unsafe
		public float Extract( Unmanaged.Frame frame )
		{
            int wndSize = frame.Size;

			if( lastframe == null || wndSize > _size )
			{
                if( lastframe != null )
                    NativeMethods.Pointer.free( lastframe );

				lastframe = NativeMethods.Pointer.fgetmem( wndSize );
                _size = wndSize;
			}

            float * spectrum;
			if( first )
			{
				first = false;
                spectrum = frame.RequestFFTMagnitude();

                // Copy data from fft buffer before next frame overwrites.
                for (int idx = 0; idx < wndSize / 2; idx++)
                    lastframe[ idx ] = spectrum[ idx ];

				return 0.0f;
			}
			
			float ret = 0.0f, dif = 0.0f;
            spectrum = frame.RequestFFTMagnitude();
			
			for( int idx = 0; idx < wndSize / 2; idx++ )
			{
				dif = (float) ( Math.Log10( spectrum[ idx ] ) - Math.Log10( lastframe[ idx ] ) );
				ret += dif * dif;

                lastframe[ idx ] = spectrum[ idx ];
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