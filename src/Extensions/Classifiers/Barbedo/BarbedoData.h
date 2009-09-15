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

#if !defined(_MUSIC_NATIVE_BARBEDODATA_H_)
#define _MUSIC_NATIVE_BARBEDODATA_H_

#include <limits>
#include <cmath>
#include <cstring>
#include <iostream>
#include <sstream>

#include <vector>

#include <ExtractedData.h>

#include "gsl.h"

namespace MusiC
{
	namespace Native
	{
		class CombinationData
		{
		public:

			gsl_combination * raw;
			ClassData * data;
			unsigned int * idx;
			FrameData ** frames;

			//::::::::::::::::::::::::::::::::::::::://

			CombinationData(ClassData * a)
			{
				unsigned int base = 3;

				raw = gsl_combination_calloc( (size_t) a->nFrames, base );
				data = a;
				frames = new FrameData * [base];
				idx = new unsigned int[base];

				reset();
			}

			//::::::::::::::::::::::::::::::::::::::://

			~CombinationData()
			{
				gsl_combination_free(raw);

				delete[] frames;
				delete[] idx;
			}
			
			//::::::::::::::::::::::::::::::::::::::://

			void reset()
			{
				unsigned int base = 3;

				FrameData * ta = data->pFirstFile->pFirstFrame;
				for( unsigned int i = 0; i < base; i++ )
				{
					frames[ i ] = ta; ta = ta->pNextFrame;
					idx[ i ] = i;
				}
			}
			
			//::::::::::::::::::::::::::::::::::::::://

			void update()
			{
				unsigned int base = 3;

				for( unsigned int i = 0; i < base; i++)
				{
					if( raw->data[ i ] == idx[ i ] )
						continue;

					// tipical operation. move next.
					if( raw->data[ i ] == idx[ i ] + 1 )
					{
						frames[ i ] = frames[ i ]->pNextFrame;
						idx[ i ]++;
						continue;
					}

					// tipical operation. move next to neighbour.
					if( i > 0 )
					{
						if( raw->data[ i ] == raw->data[ i - 1 ] + 1 )
						{
							frames[ i ] = frames[ i - 1]->pNextFrame;
							idx[ i ] = idx[ i - 1 ] + 1;
							continue;
						}
					}

					std::cout << "reached worse case" << std::endl;

					std::ostringstream a1;
					std::ostringstream a2;

					a1 << "[ ";
					a2 << "[ ";

					for ( unsigned int elemIdx = 0; elemIdx < 3; elemIdx++ )
					{
						a1 << gsl_combination_get( raw, elemIdx ) << " ";
						a2 << idx[ elemIdx ] << " ";
					}

					a1 << "]";
					a2 << "]";

					std::cout << a1.str() << " " << a2.str() << std::endl;

					//worst case ... move counting
					int offset = raw->data[ i ] - idx[ i ];
					while ( offset != 0 )
					{
						offset--;
						frames[ i ] = frames[ i ]->pNextFrame;
						idx[ i ]++;
					}
				}
			}
		};
		
		//---------------------------------------//

		class VectorCombinationData
		{
		public:

			CombinationData * a;
			CombinationData * b;
			
			//::::::::::::::::::::::::::::::::::::::://

			VectorCombinationData(ClassData * class_a, ClassData * class_b)
			{
				a = new CombinationData(class_a);
				b = new CombinationData(class_b);
			}
			
			//::::::::::::::::::::::::::::::::::::::://

			~VectorCombinationData()
			{
				delete a;
				delete b;
			}
		};
		
		//---------------------------------------//

		class GenreCombinationData
		{
			FrameData * _frames_a;
			FrameData * _frames_b;

			unsigned int _size;

		public:

			int genre_a;
			FrameData ** frames_a;

			int genre_b;
			FrameData ** frames_b;

			//::::::::::::::::::::::::::::::::::::::://

			GenreCombinationData( unsigned int size )
			{
				_frames_a = new FrameData [3];
				_frames_b = new FrameData [3];

				frames_a = new FrameData * [3];
				frames_b = new FrameData * [3];

				_size = size;

				for( unsigned int idx = 0; idx < 3; idx++)
				{
					frames_a[ idx ] = &_frames_a[ idx ];
					_frames_a[ idx ].pData = new float[ size ];

					frames_b[ idx ] = &_frames_b[ idx ];
					_frames_b[ idx ].pData = new float[ size ];
				}
			}

			//::::::::::::::::::::::::::::::::::::::://

			void SetData( unsigned int a, unsigned int b, VectorCombinationData * ref )
			{
				genre_a = a;
				genre_b = b;

				for( unsigned int idx = 0; idx < 3; idx++)
				{
					memcpy( _frames_a[ idx ].pData, ref->a->frames[ idx ]->pData, _size * sizeof(float) );
					memcpy( _frames_b[ idx ].pData, ref->b->frames[ idx ]->pData, _size * sizeof(float) );
				}
			}

			//::::::::::::::::::::::::::::::::::::::://

			void SetVector( unsigned int genre, unsigned int vector, float * data, unsigned int size )
			{
				FrameData * genreData = (genre) ? _frames_b : _frames_a;
				memcpy( genreData[ vector ].pData, data, size * sizeof(float) );
			}

			//::::::::::::::::::::::::::::::::::::::://

			void SetGenrePair( unsigned int a, unsigned int b )
			{
				genre_a = a;
				genre_b = b;
			}

			//::::::::::::::::::::::::::::::::::::::://

			float * GetData( unsigned int genre, unsigned int vector )
			{
				FrameData * genreData = (genre) ? _frames_b : _frames_a;
				return (genreData + vector)->pData;
			}
		};

		//---------------------------------------//

		class BarbedoTData
		{
			unsigned int _genreCombinationCount;
			unsigned int _genreCount;

		public:

			GenreCombinationData ** data;
			unsigned int nFeat;
			
			//::::::::::::::::::::::::::::::::::::::://

			BarbedoTData( unsigned int genreCount, unsigned int nfeat )
			{
				_genreCount = genreCount;
				nFeat = nfeat;

				double count = gsl_sf_choose( genreCount, 2 );

				if( count < std::numeric_limits<int>::max() )
				{
					_genreCombinationCount = ( unsigned int ) count;
					data = new GenreCombinationData * [ _genreCombinationCount ];

					for( unsigned int i = 0; i < _genreCombinationCount; i++ )
						data[ i ] = new GenreCombinationData( nfeat );
				}
				else
				{
					_genreCombinationCount = 0;
					data = NULL;
				}
			}
			
			//::::::::::::::::::::::::::::::::::::::://

			void SetPair( unsigned int idx, gsl_combination * genres, VectorCombinationData * ref )
			{
				data[ idx ]->SetData(
					gsl_combination_get(genres, 0),
					gsl_combination_get(genres, 1),
					ref);
			}
			
			//::::::::::::::::::::::::::::::::::::::://

			float * GetPair( unsigned int idx, unsigned int genre, unsigned int vector )
			{
				return data[ idx ]->GetData( genre, vector );
			}
			
			//::::::::::::::::::::::::::::::::::::::://

			unsigned int GetGenreCount()
			{
				return _genreCount;
			}

			//::::::::::::::::::::::::::::::::::::::://

			unsigned int GetGenreCombinationCount()
			{
				return _genreCombinationCount;
			}
		};

		//---------------------------------------//

		class BarbedoCData
		{
			unsigned int * votes;
			
			//::::::::::::::::::::::::::::::::::::::://

		public:

			BarbedoCData( unsigned int genreCount )
			{
				votes = new unsigned int[ genreCount ]();
			}
		};
	}
}

#endif
