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

#if !defined(_MUSIC_NATIVE_MATH_H_)
#define _MUSIC_NATIVE_MATH_H_

#include <limits>

#ifdef INFINITY
#undef INFINITY 
#endif

#define INFINITY numeric_limits<float>::infinity();

#include "MemoryManager.h"

namespace MusiC
{
	namespace Native
	{
		class Math
		{
			private:

				MusiC::Native::MemoryManager mgr;

				float * _sintbl;
				float * _cplx;
				int _maxfftsize;

				int checkm (int m);

			public:

                Math() : _sintbl(NULL), _cplx(NULL), _maxfftsize(0)
                {}

				int fft (float *x, float *y, const int m);
				int fftr (float* x, int m);
				int fftr_mag (float * x, int m);

				~Math();
		};
	}
}

#endif // _MUSIC_NATIVE_MATH_H_
