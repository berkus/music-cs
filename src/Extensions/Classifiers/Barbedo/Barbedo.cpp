/*
* The MIT License
* Copyright( c ) 2008-2009 Marcos José Sant'Anna Magalhães
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files( the "Software" ), to deal
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

#include "Barbedo.h"

//::::::::::::::::::::::::::::::::::::::://

using namespace std;

using namespace MusiC::Native;

//::::::::::::::::::::::::::::::::::::::://

const int MusiC::Native::Barbedo::MAX_CANDIDATES = 20;
const int MusiC::Native::Barbedo::CLUSTER_SIZE = 92;
const int MusiC::Native::Barbedo::CLUSTER_COUNT = 32;
const int MusiC::Native::Barbedo::FRAME_COUNT =
	MusiC::Native::Barbedo::CLUSTER_SIZE *
	MusiC::Native::Barbedo::CLUSTER_COUNT;

//::::::::::::::::::::::::::::::::::::::://

///
///
///
/// <param name="szA">
/// A <see cref="std::int"/>
/// </param>
///
/// <param name="szB">
/// A <see cref="std::int"/>
/// </param>
RefVecIndex * Barbedo::createCombination( ClassData * classA, ClassData * classB )
{
    if( classA->nFrames < 3 || classB->nFrames < 3 )
    {
        log << "ERROR: A class must have at least 3 vectors" << endl;
        return NULL;
    }

	RefVecIndex * r = new RefVecIndex(classA, classB);

	return r;
}

//::::::::::::::::::::::::::::::::::::::://

///
///
///
/// <param name="cl">
/// A <see cref="MusiC::Native::ClassData"/>
/// </param>
ClassData * Barbedo::filterCandidates( ClassData * cl )
{
	log << "Target Frame Count: " << MAX_CANDIDATES << endl;

	// loop counter
	int candidates_counter = 0;

	// feat idx and feat count
	register int idx = 0;
	register int nFeat = cl->pCollection->nFeatures;

	float * mean = new float[ nFeat ];
	float * var = new float[ nFeat ];
	float * high_bound = new float[ nFeat ];
	float * low_bound = new float[ nFeat ];

	for ( idx = 0; idx < nFeat; idx++ )
	{
		mean[ idx ] = 0.0f;
		var[ idx ] = 0.0f;
		high_bound[ idx ] = 0.0f;
		low_bound[ idx ] = 0.0f;
	}

	float * currentFrame;

	FileData * fileDt = cl->pFirstFile;
	FrameData * frameDt = fileDt->pFirstFrame;

	unsigned int nframes_before = cl->nFrames;

	// Calculating mean, var of all frames.
	// var = E( x^2 ) - E( x )^2
	// http://en.wikipedia.org/wiki/Computational_formula_for_the_variance

	while ( frameDt )
	{
		currentFrame = frameDt->pData;

		for ( idx = 0; idx < nFeat; idx++ )
		{
			mean[ idx ] += currentFrame[ idx ] / cl->nFrames;
			var[ idx ] += currentFrame[ idx ] * currentFrame[ idx ] / cl->nFrames;
		}

		frameDt = frameDt->pNextFrame;
	}

	for ( idx = 0; idx < nFeat; idx++ )
	{
		var[ idx ] = abs( var[ idx ] - mean[ idx ] * mean[ idx ] );

        // 1% of the standard deviation
		if ( var[ idx ] < 0.0001 * mean[ idx ] )
			var[ idx ] = 0.0001 * mean[ idx ];

		high_bound[ idx ] = mean[ idx ] + 1.5 * sqrt( var[ idx ] );
		low_bound[ idx ] = mean[ idx ] - 1.5 * sqrt( var[ idx ] );

		log << "feature: " << idx <<
		" mean: " << mean[ idx ] <<
		" var: " << var[ idx ] <<
		" std_dev.: +-" << sqrt( var[ idx ] ) << endl;

		log << "hi: " << high_bound[ idx ] << "   lo:" << low_bound[ idx ] << endl;
	}

	frameDt = fileDt->pFirstFrame;

	FrameData * lastkept = NULL;

	bool keep;

	while ( frameDt )
	{
		keep = true;
		currentFrame = frameDt->pData;

		// Avaliate if the frame should be kept.

		for ( idx = 0; idx < nFeat; idx++ )
		{
			if (
                currentFrame[ idx ] >= high_bound[ idx ] ||
                currentFrame[ idx ] <= low_bound[ idx ]
                )
			{
				keep = false;
				break;
			}
		}

		// Is it a surplus ?
		if ( candidates_counter >= MAX_CANDIDATES && keep )
		{
			keep = false;

			// make sure we know we are loosing good frames.
			candidates_counter++;
		}

		if ( !keep )
		{
			if ( lastkept )
                lastkept->pNextFrame = frameDt->pNextFrame;

			fileDt->nFrames--;
			cl->nFrames--;

			delete frameDt->pData;
			delete frameDt;

			frameDt = frameDt->pNextFrame;

			continue;
		}

		candidates_counter++;

		if ( !lastkept )
            fileDt->pFirstFrame = frameDt;

		lastkept = frameDt;

		frameDt = frameDt->pNextFrame;
		if( frameDt == fileDt->pLastFrame )
            fileDt = fileDt->pNextFile;
	}

	if ( lastkept ) lastkept->pNextFrame = NULL;

	log << "- Frame Count -" << endl;
	log << "after selection: " << cl->nFrames << endl;
	log << "total aproved: " << candidates_counter << endl;
	log << "before selection: " << nframes_before << endl;

	delete mean;
	delete var;
	delete low_bound;
	delete high_bound;

	return cl;
}

//::::::::::::::::::::::::::::::::::::::://

///
///
///
/// <param name="ref">
/// A <see cref="::RefVecIndex"/>
/// </param>
int Barbedo::combine( RefVecIndex * ref )
{
	if ( gsl_combination_next( ref->b->raw ) == GSL_FAILURE )
	{
		if ( gsl_combination_next( ref->a->raw ) == GSL_FAILURE )
			return GSL_FAILURE;
        else
            ref->a->update();

		gsl_combination_init_first( ref->b->raw );
		ref->b->reset();
	}
	else
	{ // update b
	    ref->b->update();
	}

	return GSL_SUCCESS;
}

//::::::::::::::::::::::::::::::::::::::://

///
///
///
/// <param name="src">
/// A <see cref="std::float"/>
/// </param>
/// <param name="ref">
/// A <see cref="std::float"/>
/// </param>
/// <param name="size">
/// A <see cref="std::int"/>
/// </param>
float Barbedo::dist( float * src, float * ref, int size )
{
	// column order
	double ret = 0;
	double cum = 0;

	for ( int idx = 0; idx < size; idx++ )
	{
		cum = src[ idx ] - ref[ idx ];
		ret += cum * cum;
	}

	return sqrt( ret );
}

//::::::::::::::::::::::::::::::::::::::://

///
///
///
float * Barbedo::seq_access( ClassData * cl, Int64 idx )
{
	FrameData * fr = cl->pFirstFile->pFirstFrame;

	while ( idx > 0 )
	{
		idx--;
		fr = fr->pNextFrame;
	}

	return fr->pData;
}
FileData * Barbedo::Filter( FileData * fileDt, unsigned int nFeat )
{
    // loop counters
	unsigned int frame_counter = 0;
	unsigned int cluster_sz_counter = 0;
	unsigned int cluster_counter = 0;

	register int idx = 0;
	register unsigned int nfeat = nFeat;

    float * mean = new float[ nFeat ];
	float * var = new float[ nFeat ];
	float * max = new float[ nFeat ];

	for ( idx = 0; idx < nfeat; idx++ )
	{
		mean[ idx ] = 0.0f;
		var[ idx ] = 0.0f;
		max[ idx ] = -INFINITY;
	}

	int firstFrameIdx = ( int ) ceil( (float) ( fileDt->nFrames - 1 ) / 2 ) - floor( (float) FRAME_COUNT / 2 );

    if ( firstFrameIdx < 0 )
    {
        log << "File too short" << endl;
        return NULL;
    }

    log << "frame Count: " << fileDt->nFrames << endl;
    log << "first selected frame: " << firstFrameIdx << endl;
    log << "last selected frame: " << firstFrameIdx + FRAME_COUNT - 1 << endl;
    log << endl;

    FileData * nfl = new FileData();
    nfl->pFirstFrame = NULL;
    nfl->nFrames = 0;

    FrameData * frameDt = fileDt->pFirstFrame;

    float * currentFrame;

    for ( frame_counter = 0; frame_counter < firstFrameIdx; frame_counter++ )
    {
        frameDt = frameDt->pNextFrame;
    }

    for ( cluster_counter = 0; cluster_counter < CLUSTER_COUNT; cluster_counter++ )
    {
        FrameData * nfd = new FrameData( );
        nfd->pData = new float[ nFeat * 3 ];
        nfd->pNextFrame = NULL;

        currentFrame = frameDt->pData;

        for ( idx = 0; idx < nfeat; idx++ )
        {
            mean[ idx ] = 0.0f;
            var[ idx ] = 0.0f;
            max[ idx ] = -INFINITY;
        }

        for ( cluster_sz_counter = 0; cluster_sz_counter < CLUSTER_SIZE; cluster_sz_counter++ )
        {
            for ( idx = 0; idx < nfeat; idx++ )
            {
                mean[ idx ] += currentFrame[ idx ] / CLUSTER_SIZE;
                var[ idx ] += currentFrame[ idx ] * currentFrame[ idx ] / CLUSTER_SIZE;

                if ( currentFrame[ idx ] > max[ idx ] ) max[ idx ] = currentFrame[ idx ];
            }
        }

        for ( idx = 0; idx < nfeat; idx++ )
        {
            var[ idx ] = abs( var[ idx ] - mean[ idx ] * mean[ idx ] );

            nfd->pData[ 3 * idx ] = mean[ idx ];
            nfd->pData[ 3 * idx + 1 ] = var[ idx ];
            nfd->pData[ 3 * idx + 2 ] = max[ idx ] / mean[ idx ];

            log << "cluster: " << cluster_counter <<
            " feature: " << idx <<
            " mean: " << mean[ idx ] <<
            " var: " << var[ idx ] <<
            " ppp: " << max[ idx ] / mean[ idx ] << endl;
        }

        nfl->nFrames++;

        if( nfl->pFirstFrame == NULL )
        {
            nfl->pFirstFrame = nfd;
            nfl->pLastFrame = nfd;
        }
        else
        {
            nfl->pLastFrame->pNextFrame = nfd;
            nfl->pLastFrame = nfd;
        }

        frameDt = frameDt->pNextFrame;
    }

//    if( fileDt->pPrevFile )
//    {
//        fileDt->pPrevFile->pNextFile = nfl;
//
//        if( fileDt->pPrevFile->pLastFrame )
//            fileDt->pPrevFile->pLastFrame->pNextFrame = nfl->pFirstFrame;
//    }
//
//    if( fileDt->pNextFile )
//    {
//        fileDt->pNextFile->pPrevFile = nfl;
//
//        if( fileDt->pNextFile->pFirstFrame )
//            nfl->pLastFrame->pNextFrame = fileDt->pNextFile->pFirstFrame;
//    }

    log << endl;

	delete max;
	delete mean;
	delete var;

    return nfl;
}

//::::::::::::::::::::::::::::::::::::::://

///
///
///
DataCollection * Barbedo::Filter( DataCollection * extractedData )
{
	DataHandler hnd;
	hnd.Attach( extractedData );

	log << "============ Barbedo::Filter ============" << endl;
	//log << "Address( target ):" << reinterpret_cast<size_t>( extractedData ) << endl;
	//log << "Address( pointer ):" << reinterpret_cast<size_t>( &extractedData ) << endl;
	log << "Received " << hnd.getNumClasses( ) << " Classes" << endl;
	log << "Received " << hnd.getNumFeatures( ) << " Features" << endl;
	log << "========== Algorithm Constants ==========" << endl;
	log << "Frames per cluster: " << CLUSTER_SIZE << endl;
	log << "Clusters per file: " << CLUSTER_COUNT << endl;
	log << "=========================================" << endl;

	DataCollection * dtCol = new DataCollection( );

	dtCol->nFeatures = 3 * extractedData->nFeatures;
	dtCol->nClasses = extractedData->nClasses;

	dtCol->pFirstClass = NULL;
	dtCol->pLastClass = NULL;

	ClassData * cl = extractedData->pFirstClass;
	for ( int class_counter = 0; class_counter < dtCol->nClasses; class_counter++ )
	{
		// New Class
		ClassData * ncl = new ClassData( );
		ncl->pNextClass = NULL;
		ncl->pFirstFile = NULL;
		ncl->pLastFile = NULL;
		ncl->pCollection = dtCol;

		ncl->nFrames = 0;
		ncl->nFiles = 0;

		if( !dtCol->pFirstClass )
            dtCol->pFirstClass = ncl;

        if( dtCol->pLastClass )
            dtCol->pLastClass->pNextClass = ncl;

        dtCol->pLastClass = ncl;

        ////////////////////////

		FileData * fileDt = cl->pFirstFile;

		log << "class: " << class_counter << endl;
		log << "#files: " << cl->nFiles << "   #frames: " << cl->nFrames << endl << endl;

		for ( int file_counter = 0; file_counter < cl->nFiles; file_counter++ )
		{
			FileData * nfl = Filter( fileDt, hnd.getNumFeatures() );

			nfl->pNextFile = NULL;
            nfl->pPrevFile = NULL;
            nfl->pClass = ncl;

            ncl->nFiles++;
            ncl->nFrames += nfl->nFrames;

            if( ncl->pFirstFile == NULL )
            {
                ncl->pFirstFile = nfl;
                ncl->pLastFile = nfl;
            }
            else
            {
                ncl->pLastFile->pLastFrame->pNextFrame = nfl->pFirstFrame;
                ncl->pLastFile->pNextFile = nfl;
                ncl->pLastFile = nfl;
            }

			fileDt = fileDt->pNextFile;
		}

		log << "Output Summary frames: " << ncl->nFrames << endl << endl;
		cl = cl->pNextClass;
	}

	log << "End Filter" << endl << endl;

	return dtCol;
}

//::::::::::::::::::::::::::::::::::::::://

///
///
///
void * Barbedo::Train( DataCollection * extractedData )
{
	DataHandler data;
	data.Attach( extractedData );

	log << "Address:" << reinterpret_cast<size_t>( extractedData ) << endl;
	log << "Size of MCClassData:" << sizeof( ClassData ) << endl;
	log << "Size of MCClassData *:" << sizeof( ClassData* ) << endl;
	log << "Size of Int64:" << sizeof( Int64 ) << " long:" << sizeof( long ) << endl;
	log << "Received " << data.getNumClasses( ) << " Classes" << endl;
	log << "Received " << data.getNumFeatures( ) << " Features" << endl;

	double genreCombinationIndex = 0;
	double genreCombinationCount = gsl_sf_choose( extractedData->nClasses, 2 );
	log << "Algorithm Rounds: " << genreCombinationCount << endl;
	log << "========================" << endl << endl;

    BarbedoTData * tdata = new BarbedoTData( data.getNumClasses() );
    tdata->nFeat = data.getNumFeatures();

	// Genre Combination Loop
	gsl_combination * selectedGenres = gsl_combination_calloc( data.getNumClasses(), 2 );

	for ( ;genreCombinationIndex < genreCombinationCount; genreCombinationIndex++ )
	{
		log << "========================" << endl;
		log << "   Round " << genreCombinationIndex << endl;
		log << "========================" << endl;

		// Get Classes to Compare
		int a = gsl_combination_get( selectedGenres, 0 );
		int b = gsl_combination_get( selectedGenres, 1 );

		log << "Class A bound to index " << a << endl;
		log << "Class B bound to index " << b << endl;

		ClassData * classA = data.getClass( a );
		ClassData * classB = data.getClass( b );

		// Filter Classes
		log << "Filtering Best Candidates" << endl << endl;

		log << "Class A #Vectors( before selection ): " << classA->nFrames << endl;
		ClassData * fClassA = filterCandidates( classA );
		log << "Class A #Vectors( after selection ): " << fClassA->nFrames << endl << endl;

		log << "Class B #Vectors( before selection ): " << classB->nFrames << endl;
		ClassData * fClassB = filterCandidates( classB );
		log << "Class B #Vectors( after selection ):" << fClassB->nFrames << endl << endl;

		log << "============" << endl;
		log.flush( );

		// ----------- Frames Combintation -----------//
		long score_a, score_b, score_max, winner;
		long potentialsCombinationIndex = 0;
		score_max = winner = 0;

		// Create combination control structure
		RefVecIndex * refVecsIndex = createCombination( fClassA, fClassB );

		if ( refVecsIndex == NULL )
		{
			log << "Combination structure out of memory" << endl;
			return NULL;
		}

		// Frames Combination Loop
		do
		{
			score_a = 0;
			score_b = 0;

			// Dist 1 - Class A
			FrameData * refVec = classA->pFirstFile->pFirstFrame;

			for( Int64 idx = 0; idx < fClassA->nFrames; idx++ )
			{
				if( Classify(refVec, refVecsIndex->a->frames, refVecsIndex->b->frames, tdata->nFeat) == 0 )
					score_a++;

                // Get Next Frame
				refVec = refVec->pNextFrame;
			}

			// Dist 2 - Class B
			refVec = classB->pFirstFile->pFirstFrame;

			for( Int64 idx = 0; idx < fClassB->nFrames; idx++ )
			{
				if( Classify(refVec, refVecsIndex->a->frames, refVecsIndex->b->frames, tdata->nFeat) == 1 )
					score_b++;

                refVec = refVec->pNextFrame;
			} // Frame Comparison

			// Scoring System
			if ( score_a + score_b > score_max )
			{
				score_max = score_a + score_b;
				winner = potentialsCombinationIndex;
				tdata->setPair(genreCombinationIndex, selectedGenres, refVecsIndex);

				/// @todo Grab vectors.s

				ostringstream aElem;
				ostringstream bElem;

				aElem << "[ ";
				bElem << "[ ";

				for ( int elemIdx = 0; elemIdx < 3; elemIdx++ )
				{
					aElem << gsl_combination_get( refVecsIndex->a->raw, elemIdx ) << " ";
					bElem << gsl_combination_get( refVecsIndex->b->raw, elemIdx ) << " ";
				}

				aElem << "]";
				bElem << "]";

				log << "New Winner: " << winner << " / " <<
				( Int64 ) ( gsl_sf_choose( fClassA->nFrames, 3 ) *
							gsl_sf_choose( fClassA->nFrames, 3 ) ) <<
                " - score: A(" << score_a << "/" << fClassA->nFrames << ")" <<
                " B(" << score_b << "/" << fClassB->nFrames << ")" <<
				" - " << aElem.str( ) << " & " << bElem.str( ) << endl;
			}

			potentialsCombinationIndex++;
		}
		while( combine( refVecsIndex ) == GSL_SUCCESS ); // Stop Condition: Class Comparison

		delete refVecsIndex;

		if ( gsl_combination_next( selectedGenres ) == GSL_FAILURE ) // Stop Condition: Genre Comparison
			break;
	}

	return tdata;
}

//::::::::::::::::::::::::::::::::::::::://

int Barbedo::Classify( FrameData * pCurrent,  FrameData ** a, FrameData ** b, unsigned int nFeat )
{
    float dist_loop_min = INFINITY;
	float tmp_dist;
    unsigned int loop_min;

    for ( int vec = 0; vec < 3; vec++ )
    {
        float * pTarget = a[ vec ]->pData;
        tmp_dist = dist( pCurrent->pData, pTarget, nFeat );

        if ( tmp_dist < dist_loop_min )
        {
            dist_loop_min = tmp_dist;
            loop_min = vec;
        }
    }

    for ( int vec = 0; vec < 3; vec++ )
    {
        float * pTarget = b[ vec ]->pData;
        tmp_dist = dist( pCurrent->pData, pTarget, nFeat );

        if ( tmp_dist < dist_loop_min )
        {
            dist_loop_min = tmp_dist;
            loop_min = vec + 3;
        }
    }

    if ( loop_min < 3 )
        return 0;
    else
        return 1;
}

#if defined( MUSIC_TEST )

int main( )
{
	int featCount, nFeat = 2;
	int classCount, nClasses = 2;
	int fileCount, nFiles = 3;
	int frame_count, nFrames = 3000;

	DataCollection * dtCol = new DataCollection( );
	dtCol->nFeatures = nFeat;
	dtCol->pFirstClass = NULL;
	dtCol->nClasses = 0;

	for ( classCount = 0; classCount < nClasses; classCount++ )
	{
		ClassData * newclass = new ClassData( );
		newclass->nFiles = 0;
		newclass->nFrames = 0;
		newclass->pFirstFile = NULL;
		newclass->pCollection = dtCol;

		newclass->pNextClass = dtCol->pFirstClass;
		dtCol->pFirstClass = newclass;
		dtCol->nClasses++;

		for ( fileCount = 0; fileCount < nFiles; fileCount++ )
		{
			FileData * newfile = new FileData( );

			newfile->nFrames = 0;
			newfile->pFirstFrame = NULL;
			newfile->pClass = newclass;

			newfile->pNextFile = newclass->pFirstFile;
			newclass->pFirstFile = newfile;
			newclass->nFiles++;

			for ( frame_count = 0; frame_count < nFrames; frame_count++ )
			{
				FrameData * newFrame = new FrameData( );

				newFrame->pData = new float[ nFeat ];

				for ( int idx = 0; idx < nFeat; idx++ )
					newFrame->pData[ idx ] = random( );

				newFrame->pNextFrame = newfile->pFirstFrame;

				newfile->pFirstFrame = newFrame;

				newfile->nFrames++;

				newclass->nFrames++;
			}
		}
	}

	DataCollection * col = Barbedo_Filter( dtCol );

	Barbedo_Train( col );

	return 0;
}

#endif
