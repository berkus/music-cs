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

//::::::::::::::::::::::::::::::::::::::://

using namespace std;

using namespace MusiC::Native;

//::::::::::::::::::::::::::::::::::::::://

const int MusiC::Native::Barbedo::MAX_CANDIDATES = 20;
const int MusiC::Native::Barbedo::CLUSTER_SIZE = 92;
const int MusiC::Native::Barbedo::CLUSTER_COUNT = 32;
const float MusiC::Native::Barbedo::RADIUS = 0.6f;
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
VectorCombinationData * Barbedo::CreateCombination( ClassData * classA, ClassData * classB )
{
	LOG_IN();

	if( classA->nFrames < 3 || classB->nFrames < 3 )
	{
		log << "ERROR: A class must have at least 3 vectors" << endl;

		LOG_OUT();
		return NULL;
	}

	VectorCombinationData * r = new VectorCombinationData(classA, classB);

	LOG_OUT();
	return r;
}

//::::::::::::::::::::::::::::::::::::::://

///
///
///
/// <param name="cl">
/// A <see cref="MusiC::Native::ClassData"/>
/// </param>
DataCollection * Barbedo::FilterCandidates( DataCollection * dtCol )
{
	LOG_IN();

	log << "Target Frame Count: " << MAX_CANDIDATES << endl << endl;
	log << "Std. Dev. Radius for Approval: " << RADIUS << endl << endl;

	// feat idx and feat count
	register int idx = 0;
	register int nFeat = dtCol->nFeatures;

	// Initialize statistics vector
	float * mean = new float[ nFeat ];
	float * var = new float[ nFeat ];
	float * high_bound = new float[ nFeat ];
	float * low_bound = new float[ nFeat ];

	float * currentFrame;

	DataHandler data; data.Attach( dtCol );

	DataCollection * fdtCol = DataHandler::BuildDataCollection( data.GetNumFeatures() );
	DataHandler fdata; fdata.Attach( fdtCol );

	ClassData * cl = data.GetNextClass();

	while( cl )
	{
		ClassData * ncl = DataHandler::BuildClassData( fdtCol );
		FileData * nfl = DataHandler::BuildFileData( ncl );

		// loop counter
		int candidates_counter = 0;

		// Clear statistics
		for( idx = 0; idx < nFeat; idx++ )
		{
			mean[ idx ] = 0.0f;
			var[ idx ] = 0.0f;
			high_bound[ idx ] = 0.0f;
			low_bound[ idx ] = 0.0f;
		}

		FrameData * frameDt = cl->pFirstFile->pFirstFrame;
		int nframes_before = ( int ) cl->nFrames;

		// Calculating mean, var of all frames.
		// var = E( x^2 ) - E( x )^2
		// http://en.wikipedia.org/wiki/Computational_formula_for_the_variance
		while ( frameDt )
		{
			currentFrame = frameDt->pData;

			for( idx = 0; idx < nFeat; idx++ )
			{
				mean[ idx ] += currentFrame[ idx ] / cl->nFrames;
				var[ idx ] += currentFrame[ idx ] * currentFrame[ idx ] / cl->nFrames;
			}

			frameDt = frameDt->pNextFrame;
		}

		// Calculating limits of acceptable vectors
		for( idx = 0; idx < nFeat; idx++ )
		{
			var[ idx ] = abs( var[ idx ] - mean[ idx ] * mean[ idx ] );

			// 1% of the standard deviation
			if( var[ idx ] < 0.0001f * mean[ idx ] )
				var[ idx ] = 0.0001f * mean[ idx ];

			high_bound[ idx ] = mean[ idx ] + RADIUS * sqrt( var[ idx ] );
			low_bound[ idx ] = mean[ idx ] - RADIUS * sqrt( var[ idx ] );

			float std_dev = sqrt( var[ idx ] );
			log << "" << idx << ": " << mean[ idx ] << " | " << var[ idx ] << " | " << std_dev << " (" << 100*std_dev / mean[ idx ];
			log << "%) | " << high_bound[ idx ] << " | " << low_bound[ idx ] << endl;
		}

		// Go back to first frame
		frameDt = cl->pFirstFile->pFirstFrame;

		bool keep;

		while ( frameDt )
		{
			keep = true;
			currentFrame = frameDt->pData;

			// @todo: Add comparison to force include.

			// Avaliate if the frame should be kept.
			for( idx = 0; idx < nFeat; idx++ )
			{
				if(
					currentFrame[ idx ] >= high_bound[ idx ] ||
					currentFrame[ idx ] <= low_bound[ idx ]
				)
				{
					keep = false;
					break;
				}
			}

			// Is it a surplus ?
			if( candidates_counter >= MAX_CANDIDATES && keep )
			{
				keep = false;

				// make sure we know we are loosing good frames.
				candidates_counter++;
			}

			if( !keep )
			{	
				frameDt = frameDt->pNextFrame;
				continue;
			}

			candidates_counter++;
			FrameData * nfr = DataHandler::BuildFrameData( nfl );
			memcpy( nfr->pData, frameDt->pData, nFeat * sizeof(float) );

			frameDt = frameDt->pNextFrame;
		}

		log.put('\n');
		log << "- Frame Count -" << endl;
		log << "Initial: " << nframes_before << " Aproved: " << candidates_counter << " (" << 100*candidates_counter/nframes_before << "%) ";
		log << " Selected: " << ncl->nFrames << endl << endl;


		cl = cl->pNextClass;
	}

	delete mean;
	delete var;
	delete low_bound;
	delete high_bound;

	LOG_OUT();
	return fdtCol;
}

//::::::::::::::::::::::::::::::::::::::://

///
///
///
/// <param name="ref">
/// A <see cref="::RefVecIndex"/>
/// </param>
int Barbedo::Combine( VectorCombinationData * ref )
{
	LOG_IN();

	if( gsl_combination_next( ref->b->raw ) == GSL_FAILURE )
	{
		if( gsl_combination_next( ref->a->raw ) == GSL_FAILURE )
		{
			LOG_OUT();
			return GSL_FAILURE;
		}
		else
			ref->a->update();

		gsl_combination_init_first( ref->b->raw );
		ref->b->reset();
	}
	else
	{ // update b
		ref->b->update();
	}

	LOG_OUT();
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
float Barbedo::EuclideanDistance( float * src, float * ref, int size )
{
	LOG_IN();

	// column order
	double ret = 0;
	double cum = 0;

	for( int idx = 0; idx < size; idx++ )
	{
		cum = src[ idx ] - ref[ idx ];
		ret += cum * cum;
	}

	LOG_OUT();
	return (float) sqrt( ret );
}

//::::::::::::::::::::::::::::::::::::::://

FileData * Barbedo::Filter( FileData * fileDt, unsigned int nFeat )
{
	LOG_IN();

	// loop counters
	//unsigned int frame_counter = 0;
	unsigned int cluster_sz_counter = 0;
	unsigned int cluster_counter = 0;

	register unsigned int idx = 0;
	register unsigned int nfeat = nFeat;

	float * mean = new float[ nFeat ];
	float * var = new float[ nFeat ];
	float * max = new float[ nFeat ];

	for( idx = 0; idx < nfeat; idx++ )
	{
		mean[ idx ] = 0.0f;
		var[ idx ] = 0.0f;
		max[ idx ] = -INFINITY;
	}

	unsigned int firstFrameIdx = ( unsigned int ) ceil( (float) ( fileDt->nFrames - 1 ) / 2 ) - floor( (float) FRAME_COUNT / 2 );

	if( firstFrameIdx < 0 )
	{
		log << "File too short" << endl;

		LOG_OUT();
		return NULL;
	}

	//log << "FrameCount: " << fileDt->nFrames << " Begin: " << firstFrameIdx;
	//log << " End: " << firstFrameIdx + FRAME_COUNT - 1 << endl << endl;

	FileData * nfl = new FileData();
	nfl->pFirstFrame = NULL;
	nfl->nFrames = 0;

	FrameData * frameDt = fileDt->pFirstFrame;

	float * currentFrame;

	/*for( frame_counter = 0; frame_counter < firstFrameIdx; frame_counter++ )
	{
		frameDt = frameDt->pNextFrame;
	}*/

	for( cluster_counter = 0; cluster_counter < CLUSTER_COUNT; cluster_counter++ )
	{
		FrameData * nfd = new FrameData( );
		nfd->pData = new float[ nFeat * 3 ];
		nfd->pNextFrame = NULL;

		for( idx = 0; idx < nfeat; idx++ )
		{
			mean[ idx ] = 0.0f;
			var[ idx ] = 0.0f;
			max[ idx ] = -INFINITY;
		}

		unsigned int cluster_size = 0;
		for( cluster_sz_counter = 0; cluster_sz_counter < CLUSTER_SIZE; cluster_sz_counter++ )
		{
			currentFrame = frameDt->pData;

			for( idx = 0; idx < nfeat; idx++ )
			{
				// NaN Test
				if( currentFrame[ idx ] != currentFrame[ idx ] )
					continue;

				// Inf Test
				if( currentFrame[ idx ] > FLT_MAX || currentFrame[ idx ] < -FLT_MAX )
					continue;

				cluster_size++;

				mean[ idx ] += currentFrame[ idx ] / CLUSTER_SIZE;
				var[ idx ] += currentFrame[ idx ] * currentFrame[ idx ] / CLUSTER_SIZE;

				if( currentFrame[ idx ] > max[ idx ] ) max[ idx ] = currentFrame[ idx ];
			}

			frameDt = frameDt->pNextFrame;
		}

		log << "cluster: " << cluster_counter << " - frames:" << cluster_size / nfeat << endl;
		for( idx = 0; idx < nfeat; idx++ )
		{
			var[ idx ] = abs( var[ idx ] - mean[ idx ] * mean[ idx ] );

			// 1% of the standard deviation
			if( var[ idx ] < 0.0001f * mean[ idx ] )
				var[ idx ] = 0.0001f * mean[ idx ];

			nfd->pData[ 3 * idx ] = mean[ idx ];
			nfd->pData[ 3 * idx + 1 ] = var[ idx ];
			nfd->pData[ 3 * idx + 2 ] = max[ idx ] / mean[ idx ];

			// NaN Test
			if( nfd->pData[ 3 * idx + 2 ] != nfd->pData[ 3 * idx + 2 ] )
				nfd->pData[ 3 * idx + 2 ] = 1.0f;

			float std_dev = sqrt( var[ idx ] );
			log << "" << idx << ": " << mean[ idx ] << " | " << var[ idx ] << " | " << std_dev;
			log <<" (" << 100 * std_dev/mean[ idx ] << "%) | " << max[ idx ] / mean[ idx ] << endl;
		}

		log.put('\n');

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
	}

	delete max;
	delete mean;
	delete var;

	LOG_OUT();
	return nfl;
}

//::::::::::::::::::::::::::::::::::::::://

///
///
///
DataCollection * Barbedo::Filter( DataCollection * extractedData )
{
	LOG_IN();

	DataHandler hnd;
	hnd.Attach( extractedData );

	log << "============ Barbedo::Filter ============" << endl;
	//log << "Address (target):" << reinterpret_cast<size_t>( extractedData ) << endl;
	//log << "Address( pointer ):" << reinterpret_cast<size_t>( &extractedData ) << endl;
	log << "Received " << hnd.GetNumClasses( ) << " Classes" << endl;
	log << "Received " << hnd.GetNumFeatures( ) << " Features" << endl;
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
	for( unsigned int class_counter = 0; class_counter < dtCol->nClasses; class_counter++ )
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

		for( int file_counter = 0; file_counter < cl->nFiles; file_counter++ )
		{
			FileData * nfl = Filter( fileDt, hnd.GetNumFeatures() );

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

			//log << endl;
			fileDt = fileDt->pNextFile;
		}

		//log << "Output Summary frames: " << ncl->nFrames << endl << endl;
		cl = cl->pNextClass;
	}

	LOG_OUT();
	return dtCol;
}

//::::::::::::::::::::::::::::::::::::::://

///
///
///
void * Barbedo::Train( DataCollection * extractedData )
{
	LOG_IN();

	DataHandler data;
	data.Attach( extractedData );

	//log << "DataCollection: " << sizeof(DataCollection) << endl;
	//log << "ClassData: " << sizeof(ClassData) << endl;
	//log << "FileData: " << sizeof(FileData) << endl;
	//log << "FrameData: " << sizeof(FrameData) << endl;

	//log << "Architecture:  size_t: " << sizeof(size_t) << "  pointer: " << sizeof(int *) << endl;

	//log << "Address:" << reinterpret_cast<size_t>( extractedData ) << endl;
	//log << "Received " << data.GetNumClasses( ) << " Classes" << endl;
	//log << "Received " << data.GetNumFeatures( ) << " Features" << endl;

	BarbedoTData * tdata = new BarbedoTData( data.GetNumClasses(), data.GetNumFeatures() );
	
	unsigned int genreCombinationIndex = 0;
	log << "========================" << endl;
	unsigned int genreCombinationCount = tdata->GetGenreCombinationCount();
	log << "Algorithm Rounds: " << genreCombinationCount << endl;
	log << "========================" << endl << endl;

	DataCollection * filteredData = FilterCandidates( extractedData );

	DataHandler fdata;
	fdata.Attach( filteredData );

	// Genre Combination Loop
	gsl_combination * selectedGenres = gsl_combination_calloc( data.GetNumClasses(), 2 );

	for( ;genreCombinationIndex < genreCombinationCount; genreCombinationIndex++ )
	{
		log << "========================" << endl;
		log << "   Round " << genreCombinationIndex << endl;
		log << "========================" << endl << endl;

		// Get Classes to Compare
		int a = gsl_combination_get( selectedGenres, 0 );
		int b = gsl_combination_get( selectedGenres, 1 );

		log << "Comparing Genres " << a << " and " << b << endl;

		ClassData * fClassA = fdata.GetClass( a );
		ClassData * fClassB = fdata.GetClass( b );

		ClassData * classA = data.GetClass( a );
		ClassData * classB = data.GetClass( b );

		log << "Frame Count: " << fClassA->nFrames << "/" << classA->nFrames << endl;
		log << "Frame Count: " << fClassB->nFrames << "/" << classB->nFrames << endl << endl;

		float genre_weight =  ((float) classA->nFrames / (float) classB->nFrames);

		unsigned int nclusters_a = classA->nFrames;
		unsigned int nclusters_b = genre_weight * classB->nFrames;

		// ----------- Frames Combintation -----------//
		long score_a, score_b, score_max, winner;
		long potentialsCombinationIndex = 0;
		score_max = winner = 0;

		// Create combination control structure
		VectorCombinationData * refVecsIndex = CreateCombination( fClassA, fClassB );

		if( refVecsIndex == NULL )
		{
			log << "Combination structure out of memory" << endl;

			LOG_OUT();
			return NULL;
		}

		// Frames Combination Loop
		do
		{
			score_a = 0;
			score_b = 0;

			// Dist 1 - Class A
			FrameData * refVec = classA->pFirstFile->pFirstFrame;

			for( UInt64 idx = 0; idx < classA->nFrames; idx++ )
			{
				if( Evaluate(refVec, refVecsIndex->a->frames, refVecsIndex->b->frames, tdata->nFeat) == 0 )
					score_a++;

				// Get Next Frame
				refVec = refVec->pNextFrame;
			}

			// Dist 2 - Class B
			refVec = classB->pFirstFile->pFirstFrame;

			for( UInt64 idx = 0; idx < classB->nFrames; idx++ )
			{
				if( Evaluate(refVec, refVecsIndex->a->frames, refVecsIndex->b->frames, tdata->nFeat) == 1 )
					score_b++;

				refVec = refVec->pNextFrame;
			} // Frame Comparison

			unsigned int wscore_b = (genre_weight * score_b);

			// Scoring System
			if( score_a + wscore_b > score_max )
			{
				score_max = score_a + wscore_b;
				winner = potentialsCombinationIndex;
				tdata->SetPair( genreCombinationIndex, selectedGenres, refVecsIndex );

				ostringstream aElem;
				ostringstream bElem;

				aElem << "[ ";
				bElem << "[ ";

				for( int elemIdx = 0; elemIdx < 3; elemIdx++ )
				{
					aElem << gsl_combination_get( refVecsIndex->a->raw, elemIdx ) << " ";
					bElem << gsl_combination_get( refVecsIndex->b->raw, elemIdx ) << " ";
				}

				aElem << "]";
				bElem << "]";

				log << "New Winner: " << winner << " / " <<
					( unsigned int ) ( gsl_sf_choose( fClassA->nFrames, 3 ) *
					gsl_sf_choose( fClassB->nFrames, 3 ) ) <<
					" - score: A(" << score_a << "/" << nclusters_a << " - " << 100 * score_a / nclusters_a <<
					"%) B(" << wscore_b << "/" << nclusters_b << " - " << 100 * wscore_b / nclusters_b <<
					"%) - Total: " << score_max << "/" << (nclusters_a + nclusters_b) << " " <<
					100 * score_max / (nclusters_a + nclusters_b) << "% - Vectors: " << aElem.str( ) << " & " << bElem.str( ) << endl;
			}

			if( !(potentialsCombinationIndex % 100000) )
			{
				log << "GenreComb " << genreCombinationIndex << " VectorComb " << potentialsCombinationIndex << std::endl;
			}

			potentialsCombinationIndex++;
		}
		while( Combine( refVecsIndex ) == GSL_SUCCESS ); // Stop Condition: Class Comparison

		delete refVecsIndex;

		if( gsl_combination_next( selectedGenres ) == GSL_FAILURE ) // Stop Condition: Genre Comparison
			break;
	}

	LOG_OUT();
	return tdata;
}

//::::::::::::::::::::::::::::::::::::::://

int Barbedo::Classify( FrameData * pCurrent, FrameData ** a, FrameData ** b, unsigned int nFeat )
{
	return Evaluate(pCurrent, a, b, nFeat);
}

//::::::::::::::::::::::::::::::::::::::://

unsigned int Barbedo::Evaluate( FrameData * pCurrent, FrameData ** a, FrameData ** b, unsigned int nFeat )
{
	LOG_IN();

	float dist_loop_min = INFINITY;
	float tmp_dist;
	unsigned int loop_min;

	for( unsigned int vec = 0; vec < 3; vec++ )
	{
		float * pTarget = a[ vec ]->pData;
		tmp_dist = EuclideanDistance( pCurrent->pData, pTarget, nFeat );

		if( tmp_dist < dist_loop_min )
		{
			dist_loop_min = tmp_dist;
			loop_min = vec;
		}
	}

	for( unsigned int vec = 0; vec < 3; vec++ )
	{
		float * pTarget = b[ vec ]->pData;
		tmp_dist = EuclideanDistance( pCurrent->pData, pTarget, nFeat );

		if( tmp_dist < dist_loop_min )
		{
			dist_loop_min = tmp_dist;
			loop_min = vec + 3;
		}
	}

	if( loop_min < 3 )
	{
		LOG_OUT();
		return 0;
	}

	LOG_OUT();
	return 1;
}



#if defined( MUSIC_TEST )

int main()
{
	int featCount, nFeat = 2;
	int classCount, nClasses = 2;
	int fileCount, nFiles = 3;
	int frame_count, nFrames = 3000;

	DataCollection * dtCol = new DataCollection( );
	dtCol->nFeatures = nFeat;
	dtCol->pFirstClass = NULL;
	dtCol->nClasses = 0;

	for( classCount = 0; classCount < nClasses; classCount++ )
	{
		ClassData * newclass = new ClassData( );
		newclass->nFiles = 0;
		newclass->nFrames = 0;
		newclass->pFirstFile = NULL;
		newclass->pCollection = dtCol;

		newclass->pNextClass = dtCol->pFirstClass;
		dtCol->pFirstClass = newclass;
		dtCol->nClasses++;

		for( fileCount = 0; fileCount < nFiles; fileCount++ )
		{
			FileData * newfile = new FileData( );

			newfile->nFrames = 0;
			newfile->pFirstFrame = NULL;
			newfile->pClass = newclass;

			newfile->pNextFile = newclass->pFirstFile;
			newclass->pFirstFile = newfile;
			newclass->nFiles++;

			for( frame_count = 0; frame_count < nFrames; frame_count++ )
			{
				FrameData * newFrame = new FrameData( );

				newFrame->pData = new float[ nFeat ];

				for( int idx = 0; idx < nFeat; idx++ )
					newFrame->pData[ idx ] = 1;

				newFrame->pNextFrame = newfile->pFirstFrame;

				newfile->pFirstFrame = newFrame;

				newfile->nFrames++;

				newclass->nFrames++;
			}
		}
	}

	Barbedo b;
	DataCollection * col = b.Filter( dtCol );
	b.Train( col );

	return 0;
}

#endif