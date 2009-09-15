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
    MUSIC_EXPORT void Barbedo_GetTrainingData( void * tData, float * data, int idx, int genre, int vec );
    MUSIC_EXPORT unsigned int Barbedo_GetFeatureCount( void* tData );
    MUSIC_EXPORT unsigned int Barbedo_GetGenreCombinationCount( void* tData );
    MUSIC_EXPORT unsigned int Barbedo_GetGenreCount( void* tData );
    MUSIC_EXPORT unsigned int Barbedo_GetFirstGenreIdx( void* tData, unsigned int idx );
    MUSIC_EXPORT unsigned int Barbedo_GetSecondGenreIdx( void* tData, unsigned int idx );
    MUSIC_EXPORT void * Barbedo_CreateTrainingData( unsigned int genreCount, unsigned int nfeat );
	MUSIC_EXPORT void Barbedo_SetGenreIndex( void * data, unsigned int index, unsigned int genreA, unsigned int genreB );
	MUSIC_EXPORT void Barbedo_SetVector( void * data, unsigned int index, unsigned int genre, unsigned int vector, float * vec_data, unsigned int size );
}

MUSIC_EXPORT void Barbedo_SetVector( void * data, unsigned int index, unsigned int genre, unsigned int vector, float * vec_data, unsigned int size )
{
	BarbedoTData * tdata = reinterpret_cast< BarbedoTData * >( data );
	tdata->data[ index ]->SetVector( genre, vector, vec_data, size );
}

MUSIC_EXPORT void Barbedo_SetGenreIndex( void * data, unsigned int index, unsigned int genreA, unsigned int genreB )
{
	BarbedoTData * tdata = reinterpret_cast< BarbedoTData * >( data );
	tdata->data[ index ]->SetGenrePair( genreA, genreB );
}

MUSIC_EXPORT void * Barbedo_CreateTrainingData( unsigned int genreCount, unsigned int nfeat )
{
	BarbedoTData * data = new BarbedoTData( genreCount, nfeat );
	return data;
}

MUSIC_EXPORT unsigned int Barbedo_GetFirstGenreIdx( void* tData, unsigned int idx )
{
	BarbedoTData * tdata = reinterpret_cast< BarbedoTData * >( tData );
	return tdata->data[ idx ]->genre_a;
}

MUSIC_EXPORT unsigned int Barbedo_GetSecondGenreIdx( void* tData, unsigned int idx )
{
	BarbedoTData * tdata = reinterpret_cast< BarbedoTData * >( tData );
	return tdata->data[ idx ]->genre_b;
}

MUSIC_EXPORT unsigned int Barbedo_GetGenreCount( void* tData )
{
	BarbedoTData * tdata = reinterpret_cast< BarbedoTData * >( tData );
	return tdata->GetGenreCount();
}

MUSIC_EXPORT unsigned int Barbedo_GetGenreCombinationCount( void* tData )
{
	BarbedoTData * tdata = reinterpret_cast< BarbedoTData * >( tData );
	return tdata->GetGenreCombinationCount();
}

MUSIC_EXPORT unsigned int Barbedo_GetFeatureCount( void* tData )
{
	BarbedoTData * tdata = reinterpret_cast< BarbedoTData * >( tData );
	return tdata->nFeat;
}

MUSIC_EXPORT void Barbedo_GetTrainingData( void * tData, float * data, int idx, int genre, int vec )
{
	BarbedoTData * tdata = reinterpret_cast< BarbedoTData * >( tData );
	memcpy( data, tdata->GetPair( idx, genre, vec ), tdata->nFeat * sizeof( float ) );
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

    unsigned int * total_votes = new unsigned int[ tdata->GetGenreCount() ];
	unsigned int * frame_votes = new unsigned int[ tdata->GetGenreCount() ];
	unsigned int * music_votes = new unsigned int[ tdata->GetGenreCount() ];
	unsigned int invalid_votes = 0;

    for ( unsigned int idx = 0; idx < tdata->GetGenreCount(); idx++)
	{
		total_votes[ idx ] = 0;
        frame_votes[ idx ] = 0;
		music_votes[ idx ] = 0;
	}

	Barbedo b;
	unsigned int winner, score, is_invalid = tdata->GetGenreCount() + 1;

	FrameData * frame = fd->pFirstFrame;
    while( frame )
    {
        for( unsigned int idx = 0; idx < tdata->GetGenreCombinationCount(); idx++ )
        {
            // returns 0 or 1 representing class a or b.
            int res = b.Classify( frame, tdata->data[ idx ]->frames_a, tdata->data[ idx ]->frames_b, tdata->nFeat );
            if( res == 0 )
            {
                frame_votes[ tdata->data[ idx ]->genre_a ]++;
            }
            else
            {
                frame_votes[ tdata->data[ idx ]->genre_b ]++;
            }
        }

		score = 0; winner = is_invalid;
		for( unsigned int idx = 0; idx < tdata->GetGenreCount(); idx++ )
		{
			if( frame_votes[ idx ] > score )
			{
				score = frame_votes[ idx ];
				winner = idx;
			}
			else
			{
				if( frame_votes[ idx ] == score && winner != is_invalid )	
					winner = is_invalid;
			}

			total_votes[ idx ]+= frame_votes[ idx ];
			frame_votes[ idx ] = 0;
		}

		if( winner != is_invalid )
			music_votes[ winner ]++;
		else
			invalid_votes++;

        frame = frame->pNextFrame;
    }

	std::ofstream log;
	log.open( "class.txt", std::ios_base::app );
	
	unsigned int count = 0;
	score = 0; winner = is_invalid;
    for( unsigned int idx = 0; idx < tdata->GetGenreCount(); idx++ )
    {
		log << music_votes[ idx ] << ", ";
		count += music_votes[ idx ];

        if( music_votes[ idx ] > score )
        {
            score = music_votes[ idx ];
            winner = idx;
        }
		else
		{
			if( frame_votes[ idx ] == score && winner != is_invalid )	
				winner = is_invalid;
		}
    }

	log << count << " (" << invalid_votes << ")" << endl;
	log.close();

	if( winner == is_invalid )
	{
		for( unsigned int idx = 0; idx < tdata->GetGenreCount(); idx++ )
		{
			if( total_votes[ idx ] > score )
			{
				score = total_votes[ idx ];
				winner = idx;
			}
			else
			{
				if( total_votes[ idx ] == score && winner != is_invalid )
					winner = is_invalid;
			}
		}
	}

	if ( winner == is_invalid )
		cout << "EMPATE" << endl;

	delete[] music_votes;
	delete[] total_votes;
	delete[] frame_votes;

    return winner;
}
