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

#include "Barbedo.h"

#include <iostream>

using namespace std;
using namespace MusiC::Native;

extern "C"
{
    MUSIC_EXPORT FileData * Barbedo_CFilter( FileData * data, unsigned int nfeat );
    MUSIC_EXPORT DataCollection * Barbedo_EFilter( DataCollection * dtCol );
    MUSIC_EXPORT void * Barbedo_Train( DataCollection * extractedData );
    MUSIC_EXPORT int Barbedo_Classify( FileData * f, void * data );
}

MUSIC_EXPORT FileData * Barbedo_CFilter( FileData * data, unsigned int nfeat )
{
    Barbedo b;
    return b.Filter( data, nfeat );
}

MUSIC_EXPORT DataCollection * Barbedo_EFilter( DataCollection * dtCol )
{
    Barbedo b;
    return b.Filter( dtCol );
}

MUSIC_EXPORT void * Barbedo_Train( DataCollection * extractedData )
{
    Barbedo b;
    return b.Train( extractedData );
}

MUSIC_EXPORT int Barbedo_Classify( FileData * fd, void * data )
{
    BarbedoTData * tdata = reinterpret_cast< BarbedoTData * >( data );
    Barbedo b;

    cout << "info" << endl;
    cout << tdata->nFeat << endl;
    cout << tdata->genreCount << endl;
    cout << tdata->genreCombinationCount << endl;

    FrameData * frame = fd->pFirstFrame;
    unsigned int * votes = new unsigned int[ tdata->genreCount ];

    for ( unsigned int idx = 0; idx < tdata->genreCount; idx++)
    {
        votes[ idx ] = 0;
    }

    while( frame )
    {
        for ( unsigned int idx = 0; idx < tdata->genreCombinationCount; idx++)
        {
            // returns 0 or 1 representing class a or b.
            int res = b.Classify( frame, tdata->data[ idx ].frames_a, tdata->data[ idx ].frames_a, tdata->nFeat );
            if( res == 0 )
            {
                votes[ tdata->data[ idx ].genre_a ]++;
            }
            else
            {
                votes[ tdata->data[ idx ].genre_b ]++;
            }
        }

        frame = frame->pNextFrame;
    }
    unsigned int winner, score = 0;

    for(unsigned int idx = 0; idx < tdata->genreCount; idx++)
    {
        if( votes[ idx ] > score )
        {
            score = votes[ idx ];
            winner = idx;
        }
    }

    return winner;
}
