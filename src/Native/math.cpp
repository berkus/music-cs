#if !defined(__HEADER_MATH_CPP__)
#define __HEADER_MATH_CPP__

#include <cstdlib>
#include <cmath>
#include <cstring>

#include "alloc.cpp"
#define PI 3.14f

using namespace std;

// Changed interface to float.
class mcMath
{
	MemoryManager mgr;
	
	float * _sintbl;
	float * cplx;
	int maxfftsize;
	
	int checkm(int m)
	{
		int k;

		for (k = 4; k <= m; k <<= 1)
		{
			if (k == m)
				return (0);
		}

		return (-1);
	}
	
	public:
	
	int fft(float* x, float* y, int m)
	{
		int j, lmx, li;
		float * xp, * yp;
		float * sinp, * cosp;
		int lf, lix, tblsize;
		int mv2, mm1;
		float t1, t2;
		float arg;

		//***************
		//* RADIX-2 FFT *
		//***************

		if (checkm(m) == -1)
			return (-1);

		//************************
		//* SIN table generation *
		//************************

		if ((_sintbl == NULL) || (maxfftsize < m))
		{
			tblsize = m - m / 4 + 1;
			arg = PI / m * 2;

			if (_sintbl != NULL)
				mgr.Dealloc(_sintbl);

			_sintbl = sinp = mgr.Allocator<float>(tblsize);
			*sinp++ = 0;

			for (j = 1; j < tblsize; j++)
				*sinp++ = sin(arg * (float)j);

			_sintbl[m / 2] = 0;
			maxfftsize = m;
		}

		lf = maxfftsize / m;
		lmx = m;

		for ( ; ; )
		{
			lix = lmx;
			lmx /= 2;
			if (lmx <= 1)
				break;
			sinp = _sintbl;
			cosp = _sintbl + maxfftsize / 4;
			for (j = 0; j < lmx; j++)
			{
				xp = &x[j];
				yp = &y[j];
				for (li = lix; li <= m; li += lix)
				{
					t1 = *(xp) - *(xp + lmx);
					t2 = *(yp) - *(yp + lmx);
					*(xp) += *(xp + lmx);
					*(yp) += *(yp + lmx);
					*(xp + lmx) = *cosp * t1 + *sinp * t2;
					*(yp + lmx) = *cosp * t2 - *sinp * t1;
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

		for (li=m/2; li--; xp+=2,yp+=2) 
		//for (li = m / 2; li > 0; xp += 2, yp += 2, li--)
		{
			t1 = *(xp) - *(xp + 1);
			t2 = *(yp) - *(yp + 1);
			*(xp) += *(xp + 1);
			*(yp) += *(yp + 1);
			*(xp + 1) = t1;
			*(yp + 1) = t2;
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
			if ((li = lmx - j) < 0)
			{
				t1 = *(xp);
				t2 = *(yp);
				*(xp) = *(xp + li);
				*(yp) = *(yp + li);
				*(xp + li) = t1;
				*(yp + li) = t2;
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
	
	int fftr(float* x, int m)
	{
		if (m > maxfftsize || cplx == NULL)
		{
			if (cplx != NULL)
				mgr.Dealloc(cplx);

			cplx = mgr.Allocator<float>(m);
		}
		
		memset(cplx, '\0', m);

		return fft(x, cplx, m);
	}
	
	int fftr_mag(float * x, int m)
	{
		int r = fftr(x, m);
		
		if(!r)
			return r;
		
		for (int i = 0; i < m; i++)
			*(x + i) = sqrt(*(x + i) * *(x + i) + *(cplx + i) * *(cplx + i));
			
		return r;
	}
	
	~mcMath()
	{
		mgr.Dealloc(_sintbl);
		mgr.Dealloc(cplx);
	}
};

#endif

// END OF FILE
