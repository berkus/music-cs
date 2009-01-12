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

#include <cstdlib>
#include <cmath>
#include <cstring>

#include "MemoryManager.h"
#include "MathF.h"

#define PI 3.14f

#include <iostream>
using namespace std;

int MusiC::Native::Math::checkm (int m)
{
	int k;

	for (k = 4; k <= m; k <<= 1)
	{
		if (k == m)
			return (0);
	}

	return (-1);
}

int MusiC::Native::Math::fft (float *x, float *y, const int m)
{
	int j, lmx, li;
	float *xp, *yp;
	float *sinp, *cosp;
	int lf, lix, tblsize;
	int mv2, mm1;
	float t1, t2;
	float  arg;

	/**************
	* RADIX-2 FFT *
	**************/

	if (checkm (m) )
		return (-1);

	/***********************
	* SIN table generation *
	***********************/

	if ( (_sintbl == 0) || (maxfftsize < m) )
	{
		tblsize = m - m / 4 + 1;
		arg = PI / m * 2;
		if (_sintbl != 0)
			free (_sintbl);
		_sintbl = sinp = mgr.Allocator<float> (tblsize);
		*sinp++ = 0;
		for (j = 1; j < tblsize; j++)
			*sinp++ = sin (arg * (float) j);
		_sintbl[m/2] = 0;
		maxfftsize = m;
	}

	lf = maxfftsize / m;
	lmx = m;

	for (;;)
	{
		lix = lmx;
		lmx /= 2;
		if (lmx <= 1) break;
		sinp = _sintbl;
		cosp = _sintbl + maxfftsize / 4;
		for (j = 0; j < lmx; j++)
		{
			xp = &x[j];
			yp = &y[j];
			for (li = lix; li <= m ; li += lix)
			{
				t1 = * (xp) - * (xp + lmx);
				t2 = * (yp) - * (yp + lmx);
				* (xp) += * (xp + lmx);
				* (yp) += * (yp + lmx);
				* (xp + lmx) = *cosp * t1 + *sinp * t2;
				* (yp + lmx) = *cosp * t2 - *sinp * t1;
				xp += lix;
				yp += lix;
			}
			sinp += lf;
			cosp += lf;
		}
		lf += lf;
	}

	xp = x;
	yp = y;
	for (li = m / 2; li--; xp += 2, yp += 2)
	{
		t1 = * (xp) - * (xp + 1);
		t2 = * (yp) - * (yp + 1);
		* (xp) += * (xp + 1);
		* (yp) += * (yp + 1);
		* (xp + 1) = t1;
		* (yp + 1) = t2;
	}

	/***************
	* bit reversal *
	***************/
	j = 0;
	xp = x;
	yp = y;
	mv2 = m / 2;
	mm1 = m - 1;
	for (lmx = 0; lmx < mm1; lmx++)
	{
		if ( (li = lmx - j) < 0)
		{
			t1 = * (xp);
			t2 = * (yp);
			* (xp) = * (xp + li);
			* (yp) = * (yp + li);
			* (xp + li) = t1;
			* (yp + li) = t2;
		}
		li = mv2;
		while (li <= j)
		{
			j -= li;
			li /= 2;
		}
		j += li;
		xp = x + j;
		yp = y + j;
	}

	return (0);
}

int MusiC::Native::Math::fftr (float* x, int m)
{
	if (m > maxfftsize || cplx == NULL)
	{
		if (cplx != NULL)
			mgr.Dealloc (cplx);

		cplx = mgr.Allocator<float> (m);
	}

	memset (cplx, '\0', m * sizeof(int));
	int ret = fft (x, cplx, m);

	return ret;
}

int MusiC::Native::Math::fftr_mag (float * x, int m)
{
	int r = fftr (x, m);

	for (int i = 0; i < m; i++)
		x[i] = sqrt ( x[i] * x[i] + cplx[i] * cplx[i] );

	return r;
}

MusiC::Native::Math::~Math()
{
	mgr.Dealloc (_sintbl);
	mgr.Dealloc (cplx);
}

// END OF FILE
