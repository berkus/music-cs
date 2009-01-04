//g++ -fPIC -shared -lgsl -lgslcblas -o ../bin/lin64/uBarbedo.dll barbedo.cpp

#include <iostream>
#include <fstream>
#include <math.h>
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

tRefVecIndex * mc_tRefVecsIndex_alloc(long szA, long szB)
{
	tRefVecIndex * r = (tRefVecIndex *) malloc(sizeof(tRefVecIndex));

	r->a = gsl_combination_calloc(szA, 3);
	r->b = gsl_combination_calloc(szB, 3);

	//cout << "szA:"<< szA << " szB:" << szB << endl;

	return r;
}

//gsl_matrix * filterCandidates(gsl_matrix *m)
ClassData * filterCandidates(ClassData *m)
{
	return m;
}

int combine(tRefVecIndex * ref)
{
	if(gsl_combination_next(ref->b) == GSL_FAILURE)
	{
		gsl_combination_next(ref->a);
		gsl_combination_init_first(ref->b);
	}

	return GSL_SUCCESS;
}

double dist(float * src, float * ref, int size)
{
	// column order
	double ret = 0;
	double cum;

	if(src == NULL)
		cout << "src is NULL !" << endl;

	if(ref == NULL)
		cout << "ref is NULL !" << endl;

	//cout << "Distance:Step ... " << size << endl;

	for(int idx=0; idx < size; idx++)
	{
		double a = *src;
		//cout << "a" << endl;

		double b = *ref;
		//cout << "b" << endl;

		cum = a - b;
		//cout << cum << endl;

		ret += cum * cum;
	}

	//cout << "Distance:" << ret << endl;

	return sqrt(ret);
}

float * mc_random(ClassData * c, Int64 idx)
{
	if(c == NULL)
		cout << "mcRandom:ClassData is NULL" << endl;

	//long counter = 0;
	FileData * fi = c->pFirstFile;
	//cout << "mcRandom: Start" << endl;

	while(idx >= fi->nFrames)
	{
		//cout << "idx=" << idx << " Section Size:" << m->nVectors << endl;
		idx -= fi->nFrames;
		fi = fi->pNextFile;
	}

	//cout << "mcRandom: memory Section Found" << endl;

	FrameData * fr = fi->pFirstFrame;

	while(idx > 0)
	{
		//cout << "idx=" << idx << " Section Size:" << m->nVectors << endl;
		idx--;
		fr = fr->pNextFrame;
	}

	float * ret = fr->pData;

	//for(int i = 0; i < idx; i++)
	//{
	//	ret += c->pCollection->nFeatures;
	//}

	//cout << "mcRandom: Index Found" << endl;
	//cout << "mcRandom: End" << endl;

	return ret;
}

float * nextVector(FileData * vec)
{
	if (vec->next == vec->nFrames)
	{
		vec->next = 0;
		vec++;
	}

	//return (vec->pData + vec->next++);
    return NULL;
}

extern "C"
{
	void Barbedo_Filter(DataCollection * extractedData)
	{
		//log.open("mcGenreClassifier-Barbedo.log", ios_base::out);

		DataHandler hnd;
		hnd.Attach(extractedData);

		cout << "============ Barbedo_Filter ============" << endl;
		cout << "Address:" << reinterpret_cast<long>(extractedData) << endl;
		cout << "Received " << hnd.getNumClasses() << " Classes" << endl;
		cout << "Received " << hnd.getNumFeatures() << " Features" << endl;
		cout << "====================================" << endl;
	}

	void Barbedo_Train(DataCollection * extractedData)
	{
		ofstream log;
		double genreCombinationIndex = 0;

		DataHandler data;
		data.Attach(extractedData);

		log.open("GenreClassifier-Barbedo.log", ios_base::out);
		log << "Address:" << reinterpret_cast<int>(extractedData) << endl;
		log << "Size of MCClassData:" << sizeof(ClassData) << endl;
		log << "Size of MCClassData *:" << sizeof(ClassData*) << endl;
		log << "Size of Int64:" << sizeof(Int64) << " long:" << sizeof(long) << endl;
		log << "Received " << data.getNumClasses() << " Classes" << endl;
		log << "Received " << data.getNumFeatures() << " Features" << endl;

		double genreCombinationCount = gsl_sf_choose(extractedData->nClasses, 2);
		log << "Algorithm Rounds: " << genreCombinationCount << endl;
		log << "========================" << endl << endl;

		// Genre Combination Loop
		gsl_combination * selectedGenres = gsl_combination_calloc(extractedData->nClasses, 2);
		for(;genreCombinationIndex < genreCombinationCount; genreCombinationIndex++)
		{
			log << "========================" << endl;
			log << "   Round " << genreCombinationIndex << endl;
			log << "========================" << endl;

			// Get Classes to Compare
			ClassData * classA = data.getClass(gsl_combination_get(selectedGenres, 0));
			ClassData * classB = data.getClass(gsl_combination_get(selectedGenres, 1));


			// Filter Classes
			ClassData * fClassA = filterCandidates(classA);
			log << "Class A #Vectors: " << fClassA->nFrames << endl;

			ClassData * fClassB = filterCandidates(classB);
			log << "Class B #Vectors: " << fClassB->nFrames << endl;

			log << "============" << endl;

			// ----------- Frames Combintation -----------
			long score, score_max, winner;
			long potentialsCombinationIndex = 0;
			score_max = winner = 0;

			double tmp_dist = 0;
			int loop_min = 0;

            // Create combination control structure
			tRefVecIndex * refVecsIndex = mc_tRefVecsIndex_alloc(fClassA->nFrames, fClassB->nFrames);

			if(refVecsIndex == NULL)
			{
				printf("FATAL: Couldn't Alloc enough memory for the vectors combination\n");
				exit(-1);
			}

			// Frames Combination Loop
			do
			{
				score = 0;

				// Dist 1
				// Class A Loop
				FrameData * refVec = classA->pFirstFile->pFirstFrame;
				for(Int64 idx = 0; idx < fClassA->nFrames; idx++)
				{
					double dist_loop_min = 1e300;

					// dist A - A
					// Each Vector from class A in the 6-vector combination
					for(int vec = 0; vec < 3; vec++)
					{
						size_t idx = gsl_combination_get(refVecsIndex->a, vec);

						float * pTarget = mc_random(fClassA, idx);

						float *pCurrent = refVec->pData;

						tmp_dist = dist(pCurrent, pTarget, data.getNumFeatures());

						if(tmp_dist < dist_loop_min)
						{
							dist_loop_min = tmp_dist; loop_min = vec;
						}
					}

					// dist A - B
					for(int vec = 0; vec < 3; vec++)
					{
						size_t idx = gsl_combination_get(refVecsIndex->b, vec);

						float * pTarget = mc_random(fClassB, idx);

						float *pCurrent = refVec->pData;

						tmp_dist = dist(pCurrent, pTarget, extractedData->nFeatures);

						if(tmp_dist < dist_loop_min)
						{
							dist_loop_min = tmp_dist; loop_min = vec + 3;
						}
					}

                    // Get Next Frame
					refVec = refVec->pNextFrame;

					if(loop_min < 3)
						score++;
				}

				// Dist 2 - Class B
				refVec = classB->pFirstFile->pFirstFrame;
				for(Int64 idx = 0; idx < fClassB->nFrames; idx++)
				{
					double dist_loop_min = 1e300;

                    // Class B-A
					for(int vec = 0; vec < 3; vec++)
					{
						size_t idx = gsl_combination_get(refVecsIndex->a, vec);

						float * pTarget = mc_random(fClassA, idx);

						float *pCurrent = refVec->pData;

						tmp_dist = dist(pCurrent, pTarget, extractedData->nFeatures);

						if(tmp_dist < dist_loop_min)
						{
							dist_loop_min = tmp_dist; loop_min = vec;
							//log << loop_min << endl;
						}
					}

                    // Class B-B
					for(int vec = 0; vec < 3; vec++)
					{
						size_t idx = gsl_combination_get(refVecsIndex->b, vec);

						float * pTarget = mc_random(classB, idx);

						float *pCurrent = refVec->pData;

						tmp_dist = dist(pCurrent, pTarget, extractedData->nFeatures);

						if(tmp_dist < dist_loop_min)
						{
							dist_loop_min = tmp_dist; loop_min = vec + 3;
						}
					}

					refVec = refVec->pNextFrame;

					if(loop_min >= 3)
						score++;
				} // Frame Comparison

				// Scoring System
				if(score > score_max)
				{
					score_max = score;
					winner = potentialsCombinationIndex;
				}

				log << potentialsCombinationIndex << " - " <<  winner << endl;

				if(!(++potentialsCombinationIndex % 1000))
				{
				//	printf("%d - %d\n", potentialsCombinationIndex, winner);
				}

			}while(combine(refVecsIndex) == GSL_SUCCESS); // Stop Condition: Class Comparison

			if( gsl_combination_next(selectedGenres) == GSL_FAILURE ) // Stop Condition: Genre Comparison
				break;
		}

		log.close();
	}
}
