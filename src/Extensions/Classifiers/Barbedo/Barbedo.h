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

#if !defined(_MUSIC_NATIVE_BARBEDO_)
#define _MUSIC_NATIVE_BARBEDO_

#include <iostream>
#include <fstream>
#include <cmath>

#include "gsl.h"

#include <MusiC-Native.h>

#include "BarbedoData.h"

//::::::::::::::::::::::::::::::::::::::://

namespace MusiC
{
	namespace Native
	{
		class Barbedo
		{

		private:

			// FilterCandidates constants.
			// 25C3 * 25C3 = 5,300,000. Doesn't take soooo long.
			static const int MAX_CANDIDATES;

			// Filter Constants.
			static const int CLUSTER_SIZE; // frames to make a second
			static const int CLUSTER_COUNT; // how many seconds
			static const int FRAME_COUNT;

			LogHandler log;

			//::::::::::::::::::::::::::::::::::::::://

		private:

			VectorCombinationData * CreateCombination( ClassData * classA, ClassData * classB );
			DataCollection * FilterCandidates( DataCollection * dtCol );
			int Combine( VectorCombinationData * ref );
			float EuclideanDistance( float * src, float * ref, int size );
			unsigned int Evaluate( FrameData * pCurrent,  FrameData ** a, FrameData ** b, unsigned int nFeat );

			//::::::::::::::::::::::::::::::::::::::://

		public:

			Barbedo()
			{
				log.open( "Barbedo.txt", std::ios_base::app );
			}

			~Barbedo()
			{
				log.close();
			}

			//::::::::::::::::::::::::::::::::::::::://

		public:

			DataCollection * Filter( DataCollection * extractedData );
			FileData * Filter( FileData * file, unsigned int nFeat );
			void * Train( DataCollection * extractedData );
			int Classify( FrameData * pCurrent, FrameData ** a, FrameData ** b, unsigned int nFeat );
		};
	}
}

#endif
