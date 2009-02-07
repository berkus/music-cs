/*
 * The MIT License
 * Copyright (c) 2008-2009 Marcos Jos� Sant'Anna Magalh�es
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

#if !defined(_MUSIC_NATIVE_EXTRACTEDDATA_H_)
#define _MUSIC_NATIVE_EXTRACTEDDATA_H_

#if defined(_MSC_VER)
	typedef __int64 Int64;
#else
	typedef long long Int64;
#endif

namespace MusiC
{
	namespace Native
	{
		struct FrameData
		{
			float * pData;

			FrameData * pNextFrame;
		};

		struct ClassData;

		struct FileData
		{
			Int64 nFrames;

			FileData * pNextFile;
			FileData * pPrevFile;

			FrameData * pFirstFrame;
			FrameData * pLastFrame;

			FrameData * pFiltered;

			ClassData * pClass;
		};

		struct DataCollection;

		struct ClassData
		{
			Int64 nFiles;
			Int64 nFrames;

			ClassData * pNextClass;

			FileData * pFirstFile;
			FileData * pLastFile;

			DataCollection * pCollection;
		};

		struct DataCollection
		{
			int nClasses;
			int nFeatures;

			ClassData * pFirstClass;
			ClassData * pLastClass;
		};
	}
}

#endif

