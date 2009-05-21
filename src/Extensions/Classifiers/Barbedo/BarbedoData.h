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
#if !defined(_MUSIC_NATIVE_BARBEDODATA_H_)
#define _MUSIC_NATIVE_BARBEDODATA_H_

#include <limits>
#include <cmath>
#include <cstring>
#include <iostream>
#include <sstream>

#include <gsl/gsl_combination.h>
#include <gsl/gsl_sf.h>

#include <ExtractedData.h>

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

            CombinationData(ClassData * a)
            {
                unsigned int base = 3;

                raw = gsl_combination_calloc( (size_t) a->nFrames, 3 );
                data = a;
                frames = new FrameData * [base];
                idx = new unsigned int[base];

                reset();
            }

            ~CombinationData()
            {
                gsl_combination_free(raw);

                delete[] frames;
                delete[] idx;
            }

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

        ///
        ///
        ///
        class RefVecIndex
        {
        public:

            CombinationData * a;
            CombinationData * b;

            RefVecIndex(ClassData * class_a, ClassData * class_b)
            {
                a = new CombinationData(class_a);
                b = new CombinationData(class_b);
            }

            ~RefVecIndex()
            {
                delete a;
                delete b;
            }
        };

        class GenrePairData
        {

        public:

            int genre_a;
            FrameData ** frames_a;

            int genre_b;
            FrameData ** frames_b;

            GenrePairData()
            {
                frames_a = new FrameData * [3];
                frames_b = new FrameData * [3];
            }

            void setData( unsigned int a, unsigned int b, RefVecIndex * ref )
            {
                genre_a = a;
                genre_b = b;
                memcpy( frames_a, ref->a->frames, 3 * sizeof(size_t) );
                memcpy( frames_b, ref->b->frames, 3 * sizeof(size_t) );
            }
        };

        class BarbedoTData
        {
        public:

            GenrePairData * data;
            unsigned int nFeat;
            unsigned int genreCombinationCount;
            unsigned int genreCount;

            BarbedoTData(unsigned int genres)
            {
                genreCount = genres;
                double count = gsl_sf_choose(genres, 2);

                if( count < std::numeric_limits<int>::max() )
                {
                    genreCombinationCount = (unsigned int) count;
                    data = new GenrePairData[genreCombinationCount];
                }
                else
                {
                    genreCombinationCount = 0;
                    data = NULL;
                }
            }

            void setPair( UInt64 idx, gsl_combination * genres, RefVecIndex * ref )
            {
                data[ idx ].setData(
                    gsl_combination_get(genres, 0),
                    gsl_combination_get(genres, 1),
                    ref);
            }
        };

        class BarbedoCData
        {
            unsigned int * votes;

        public:

            BarbedoCData(unsigned int genres)
            {
                votes = new unsigned int[genres]();
            }
        };
    }
 }

#endif
