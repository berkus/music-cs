/*
 * The MIT License
 * Copyright (c) 2008 Marcos José Sant'Anna Magalhães
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
 *
 */

#include <iostream>
#include <fstream>
#include <cmath>
#include <gsl/gsl_combination.h>
#include <gsl/gsl_sf.h>

#include <MusiC-Native.h>

using namespace std;
using namespace MusiC::Native;

struct tRefVecIndex
{
	char * name;
	gsl_combination * a;
	gsl_combination * b;
};

// Functions

tRefVecIndex * mc_tRefVecsIndex_alloc (long szA, long szB)
{
	tRefVecIndex * r = (tRefVecIndex *) malloc (sizeof (tRefVecIndex) );

	r->a = gsl_combination_calloc (szA, 3);
	r->b = gsl_combination_calloc (szB, 3);

	//cout << "szA:"<< szA << " szB:" << szB << endl;

	return r;
}

ClassData * filterCandidates (ClassData * cl)
{
	// Algorithm constants.
	// 25C3 * 25C3 = 5,300,000. Doesn't take soooo long.
	const int target_frame_count = 20;

	// loop counters
	int frame_counter = 0;

	register int idx = 0;
	register int nFeat = cl->pCollection->nFeatures;

	float mean[nFeat];
	memset (mean, 0, nFeat * sizeof(float));

	float var[nFeat];
	memset (var, 0, nFeat * sizeof(float));

	float high_bound[nFeat];
	memset (high_bound, 0, nFeat * sizeof(float));

	float low_bound[nFeat];
	memset (low_bound, 0, nFeat * sizeof(float));

	float * x;

	FileData * fl = cl->pFirstFile;
	FrameData * fd = fl->pFirstFrame;

	cout << "Starting Mean/Var Calulation" << endl;

	while (fd)
	{
	    x = fd->pData;

		for (idx = 0; idx < nFeat; idx++)
		{
			mean[idx] += x[idx] / cl->nFrames;
		}

		for (idx = 0; idx < nFeat; idx++)
		{
			var[idx] = (x[idx] - mean[idx]) * (x[idx] - mean[idx]) / cl->nFrames;
			high_bound[idx] = mean[idx] + 2 * var[idx];
			low_bound[idx] = mean[idx] - 2 * var [idx];
		}

		fd = fd->pNextFrame;
	}

	for (idx = 0; idx < nFeat; idx++)
		cout << " " << mean[idx];

	cout << endl;

	for (idx = 0; idx < nFeat; idx++)
		cout << " " << var[idx];

    cout << endl;

	fd = fl->pFirstFrame;
	FrameData * lastkept = NULL;

	bool keep;

	while (fd)
	{
		keep = true;
		x = fd->pData;

		for (idx = 0; idx < nFeat; idx++)
		{
			if (x[idx] >= high_bound[idx] || x[idx] <= low_bound[idx])
			{
				keep = false;
				break;
			}
		}

		if (frame_counter >= target_frame_count && keep)
		{
			keep = false;
			frame_counter++;
		}

		if (!keep)
		{
			lastkept->pNextFrame = fd->pNextFrame;
			fl->nFrames--;

			delete fd->pData;
			delete fd;

			fd = lastkept->pNextFrame;

			continue;
		}

		frame_counter++;
		lastkept = fd;
		fd = fd->pNextFrame;
	}

	cout << fl->nFrames << "/" << frame_counter << endl;

	cl->nFrames = fl->nFrames;

	return cl;
}

int combine (tRefVecIndex * ref)
{
	if (gsl_combination_next (ref->b) == GSL_FAILURE)
	{
		if (gsl_combination_next (ref->a) == GSL_FAILURE)
			return GSL_FAILURE;

		gsl_combination_init_first (ref->b);
	}

	return GSL_SUCCESS;
}

double dist (float * src, float * ref, int size)
{
	// column order
	double ret = 0;
	double cum;

	for (int idx = 0; idx < size; idx++)
	{
		double a = *src;
		double b = *ref;

		cum = a - b;

		ret += cum * cum;
	}

	return sqrt (ret);
}

float * seq_access (ClassData * cl, Int64 idx)
{
	FrameData * fr = cl->pFirstFile->pFirstFrame;

	while (idx > 0)
	{
		idx--;
		fr = fr->pNextFrame;
	}

	return fr->pData;
}

/// @brief Calculates vector statistics.
///
/// @note This function is a modification from the file "vstat.c" found in the source distribution of the SPTK(Signal Processing Toolkit) project.
/// SPTK Home: http://sp-tk.sourceforge.net/
void stat (ClassData * cd, float * mean, float * var)
{
//    FrameData * fd = cd->pFirstFile->pFirstFrame;
//	while (fd)
//	{
//        for (i = 0; i < leng; i++)
//        {
//            mean[i] += x[i] / k;
//            var[i] += x[i] * x[i];
//        }
//
//        for (i = 0; i < leng; i++)
//            var[i] = var[i] / k - mean[i] * mean[i];
//
//        fd = fd->pNextFrame;
//	}
}

extern "C"
{
	DataCollection * Barbedo_Filter (DataCollection * extractedData)
	{
	    ofstream nlog;
	    nlog.open ("Barbedo_Filter_dataIn.txt", ios_base::out);

		ofstream log;
		log.open ("Barbedo_Filter.txt", ios_base::out);

		DataHandler hnd;
		hnd.Attach (extractedData);

		log << "============ Barbedo_Filter ============" << endl;
		//log << "Address (target):" << reinterpret_cast<long>(extractedData) << endl;
		//log << "Address (pointer):" << reinterpret_cast<long>(&extractedData) << endl;
		log << "Received " << hnd.getNumClasses() << " Classes" << endl;
		log << "Received " << hnd.getNumFeatures() << " Features" << endl;
		log << "====================================" << endl;

		// Algorithm constants.
		const int clusterSz = 92; // frames to make a second
		const int clusterCount = 32; // how many seconds
		const int frameCount = clusterSz * clusterCount;

		// loop counters
		int frame_counter = 0;
		int cluster_sz_counter = 0;
		int cluster_counter = 0;

		register int idx = 0;
		register int nFeat = extractedData->nFeatures;

		float mean[nFeat];
		float var[nFeat];
		float max[nFeat];

		for (idx = 0; idx < nFeat; idx++)
		{
			mean[idx] = 0;
			var[idx] = 0;
			max[idx] = -INFINITY;
		}

		float * x;
		int firstFrameIdx = 0;

		DataCollection * dtCol = new DataCollection();

		dtCol->nFeatures = 3 * extractedData->nFeatures;
		dtCol->nClasses = extractedData->nClasses;

		dtCol->pFirstClass = NULL;

		ClassData * cl = extractedData->pFirstClass;
		while (cl)
		{
		    nlog << "NewClass" << endl << endl;

			// New Class
			ClassData * ncl = new ClassData();
			ncl->pNextClass = dtCol->pFirstClass;
			dtCol->pFirstClass = ncl;

			ncl->pCollection = dtCol;

			// New File
			FileData * nfl = new FileData();
			nfl->pNextFile = NULL;
			nfl->pClass = ncl;
			nfl->pFirstFrame = NULL;

			// The only file this class will have.
			ncl->pFirstFile = nfl;
			ncl->nFiles = 1;

			FileData * fl = cl->pFirstFile;
			FrameData * fd;

			for ( int file_counter = 0; file_counter < cl->nFiles; file_counter++ )
			{
				firstFrameIdx = (int) ceil ( (fl->nFrames - 1) / 2) - (int) floor (frameCount / 2);
				log << "Frame Count: " << fl->nFrames << endl;
				log << "First Frame: " << firstFrameIdx << endl;
				log << "Last Frame: " << firstFrameIdx + frameCount - 1 << endl;
				log << endl;

				nlog << "NewFile" << endl << endl;

				fd = fl->pFirstFrame;

				for ( frame_counter = 0; frame_counter < firstFrameIdx; frame_counter++)
				{
				    //nlog << fd->pData[0] << endl;
					fd = fd->pNextFrame;
				}

				for ( cluster_counter = 0; cluster_counter < clusterCount; cluster_counter++)
				{
					FrameData * nfd = new FrameData();
					nfd->pData = new float[extractedData->nFeatures * 3];

					x = fd->pData;
					nlog << "Cluster " << cluster_counter << endl << endl;

					for (idx = 0; idx < nFeat; idx++)
                    {
                        mean[idx] = 0;
                        var[idx] = 0;
                        max[idx] = -INFINITY;
                    }

					for ( cluster_sz_counter = 0; cluster_sz_counter < clusterSz; cluster_sz_counter++ )
					{
					    nlog << fd->pData[0] << endl;

						for (idx = 0; idx < nFeat; idx++)
						{
							mean[idx] += x[idx] / clusterSz;
							if (x[idx] > max[idx]) max[idx] = x[idx];
						}

						for (idx = 0; idx < nFeat; idx++)
							var[idx] = (x[idx] - mean[idx]) * (x[idx] - mean[idx]) / clusterSz;
					}

					for (idx = 0; idx < nFeat; idx++)
					{
						log << "mean: " << (nfd->pData[ 3 * idx ] = mean[ idx ]) << endl;
						log << "var: " << (nfd->pData[ 3 * idx + 1 ] = var[ idx ]) << endl;
						log << "max: " << max[ idx ] << endl;
						log << "ppp:" << (nfd->pData[ 3 * idx + 2 ] = max[ idx ] / mean[ idx ]) << endl;
						log << endl;
					}

					nfd->pNextFrame = nfl->pFirstFrame;
					nfl->pFirstFrame = nfd;

					nfl->nFrames++;
					fd = fd->pNextFrame;
				}

				fl = fl->pNextFile;
			}

			log << "Output Summary frames: " << nfl->nFrames << endl;
			ncl->nFrames = nfl->nFrames;
			cl = cl->pNextClass;
		}

		log << "Done" << endl;
		nlog.close();
		log.close();

		return dtCol;
	}

	void Barbedo_Train (DataCollection * extractedData)
	{
		ofstream log;
		double genreCombinationIndex = 0;

		DataHandler data;
		data.Attach (extractedData);

		log.open ("Barbedo_Train.txt", ios_base::out);
		log << "Address:" << reinterpret_cast<int> (extractedData) << endl;
		log << "Size of MCClassData:" << sizeof (ClassData) << endl;
		log << "Size of MCClassData *:" << sizeof (ClassData*) << endl;
		log << "Size of Int64:" << sizeof (Int64) << " long:" << sizeof (long) << endl;
		log << "Received " << data.getNumClasses() << " Classes" << endl;
		log << "Received " << data.getNumFeatures() << " Features" << endl;

		double genreCombinationCount = gsl_sf_choose (extractedData->nClasses, 2);
		log << "Algorithm Rounds: " << genreCombinationCount << endl;
		log << "========================" << endl << endl;

		log.flush();

		// Genre Combination Loop
		gsl_combination * selectedGenres = gsl_combination_calloc (extractedData->nClasses, 2);
		for (;genreCombinationIndex < genreCombinationCount; genreCombinationIndex++)
		{
			log << "========================" << endl;
			log << "   Round " << genreCombinationIndex << endl;
			log << "========================" << endl;

			// Get Classes to Compare
			int a = gsl_combination_get (selectedGenres, 0);
			int b = gsl_combination_get (selectedGenres, 1);

			log << "Class A bound to index " << a << endl;
			log << "Class B bound to index " << b << endl;

			ClassData * classA = data.getClass (a);
			ClassData * classB = data.getClass (b);

			// Filter Classes
			ClassData * fClassA = filterCandidates (classA);
			log << "Class A #Vectors: " << fClassA->nFrames << endl;

			ClassData * fClassB = filterCandidates (classB);
			log << "Class B #Vectors: " << fClassB->nFrames << endl;

			log << "============" << endl;
			log.flush();

			// ----------- Frames Combintation -----------
			long score, score_max, winner;
			long potentialsCombinationIndex = 0;
			//long potentialsCombinationCount = gsl_sf_choose(, 2);
			score_max = winner = 0;

			double tmp_dist = 0;
			int loop_min = 0;

			// Create combination control structure
			tRefVecIndex * refVecsIndex = mc_tRefVecsIndex_alloc ( (long) fClassA->nFrames, (long) fClassB->nFrames);

			// Frames Combination Loop
			do
			{
				score = 0;

				// Dist 1 - Class A
				FrameData * refVec = classA->pFirstFile->pFirstFrame;
				for (Int64 idx = 0; idx < fClassA->nFrames; idx++)
				{
					double dist_loop_min = 1e300;

					// dist A - A
					// Each Vector from class A in the 6-vector combination
					for (int vec = 0; vec < 3; vec++)
					{
						size_t idx = gsl_combination_get (refVecsIndex->a, vec);

						float * pTarget = seq_access (fClassA, idx);
						float *pCurrent = refVec->pData;

						tmp_dist = dist (pCurrent, pTarget, data.getNumFeatures() );

						if (tmp_dist < dist_loop_min)
						{
							dist_loop_min = tmp_dist;
							loop_min = vec;
						}
					}

					// dist A - B
					for (int vec = 0; vec < 3; vec++)
					{
						size_t idx = gsl_combination_get (refVecsIndex->b, vec);

						float * pTarget = seq_access (fClassB, idx);

						float *pCurrent = refVec->pData;

						tmp_dist = dist (pCurrent, pTarget, extractedData->nFeatures);

						if (tmp_dist < dist_loop_min)
						{
							dist_loop_min = tmp_dist;
							loop_min = vec + 3;
						}
					}

					// Get Next Frame
					refVec = refVec->pNextFrame;

					if (loop_min < 3)
						score++;
				}

				// Dist 2 - Class B
				refVec = classB->pFirstFile->pFirstFrame;
				for (Int64 idx = 0; idx < fClassB->nFrames; idx++)
				{
					double dist_loop_min = 1e300;

					// Class B-A
					for (int vec = 0; vec < 3; vec++)
					{
						size_t idx = gsl_combination_get (refVecsIndex->a, vec);

						float * pTarget = seq_access (fClassA, idx);

						float *pCurrent = refVec->pData;

						tmp_dist = dist (pCurrent, pTarget, extractedData->nFeatures);

						if (tmp_dist < dist_loop_min)
						{
							dist_loop_min = tmp_dist;
							loop_min = vec;
							//log << loop_min << endl;
						}
					}

					// Class B-B
					for (int vec = 0; vec < 3; vec++)
					{
						size_t idx = gsl_combination_get (refVecsIndex->b, vec);

						float * pTarget = seq_access (classB, idx);

						float *pCurrent = refVec->pData;

						tmp_dist = dist (pCurrent, pTarget, extractedData->nFeatures);

						if (tmp_dist < dist_loop_min)
						{
							dist_loop_min = tmp_dist;
							loop_min = vec + 3;
						}
					}

					refVec = refVec->pNextFrame;

					if (loop_min >= 3)
						score++;
				} // Frame Comparison

				// Scoring System
				if (score > score_max)
				{
					score_max = score;
					winner = potentialsCombinationIndex;
					log << "New Winner: " << winner << "/" << "oo" << endl;
				}

				potentialsCombinationIndex++;

			}
			while (combine (refVecsIndex) == GSL_SUCCESS); // Stop Condition: Class Comparison

			if ( gsl_combination_next (selectedGenres) == GSL_FAILURE ) // Stop Condition: Genre Comparison
				break;
		}

		log.close();
	}
}
