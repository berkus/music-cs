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

#include "DBHandler.h"
#include "MemoryManager.h"
#include "MathF.h"

#include <cstring>
using namespace std;

extern "C"
{
	MusiC::Native::MemoryManager mgr;
	MusiC::Native::Math math;
	MusiC::Native::DBHandler hnd;

	//::::::::::::::::::::::::::::::::::::::://

	int fftr_mag(float* x, float* mag, int m)
	{
		memcpy(mag, x, m * sizeof(int));
		int ret = math.fftr_mag(mag, m);

		return ret;
	}

	//::::::::::::::::::::::::::::::::::::::://

	void Initialize(const char * db)
	{
		hnd.OpenDB(db);
	}

	//::::::::::::::::::::::::::::::::::::::://

	void AddFeature(const char * sectionLabel, const char * dataLabel, float * data, int dataSz)
	{
		hnd.AddData(sectionLabel, dataLabel, data, dataSz * sizeof(float));
	}

	//::::::::::::::::::::::::::::::::::::::://

	int GetFeature(const char * wndName, const char * featName, float * data)
	{
		if(hnd.HasData(wndName, featName))
		{
			return hnd.ReadData(data);
		}

		return 0;
	}

	//::::::::::::::::::::::::::::::::::::::://

	void Terminate()
	{
		hnd.CloseDB();
	}
}

// END OF FILE
