//g++ -fPIC -shared -lgsl -lgslcblas -o ../bin/lin64/uBarbedo.dll barbedo.cpp

#include <iostream>
#include <fstream>
#include <math.h>
#include <gsl/gsl_combination.h>
#include <gsl/gsl_sf.h>

using namespace std;

typedef long long int64;

// struct 1 - Dados dos G�neros
struct MCFeatVector
{
	double * pData;
	int64 nVectors;
	///////////////
	int64 next;
};

struct MCDataCollection;

struct MCClassData
{
	int64 nVectorListAlloc;
	int64 nVectorList;
	int64 nVectors;
	MCFeatVector * pVectorList;
	MCDataCollection * pCollection;
};

struct MCDataCollection
{
	MCClassData * pClassData;
	int64 nClasses;
	int64 nFeatures;
};

// struct 2 - tRefVecIndex

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
MCClassData * filterCandidates(MCClassData *m)
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

double dist(double * src, double * ref, int64 &size)
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

double * mc_random(MCClassData * c, long idx)
{
	if(c == NULL)
		cout << "mcRandom:MCClassData is NULL" << endl;
	
	//long counter = 0;
	MCFeatVector * m = c->pVectorList;
	//cout << "mcRandom: Start" << endl;

	while(idx >= m->nVectors)
	{
		//cout << "idx=" << idx << " Section Size:" << m->nVectors << endl;
		idx -= m->nVectors;
		m++;
	}

	//cout << "mcRandom: memory Section Found" << endl;

	double * ret = m->pData;

	for(int i = 0; i < idx; i++)
	{
		ret += c->pCollection->nFeatures;
	}

	//cout << "mcRandom: Index Found" << endl;
	//cout << "mcRandom: End" << endl;

	return ret;
}

double * nextVector(MCFeatVector * vec)
{
	if (vec->next == vec->nVectors)
	{
		vec->next = 0;
		vec++;
	}

	return (vec->pData + vec->next++);
}

extern "C"
{
	MCDataCollection * Barbedo_Filter(MCDataCollection * extractedData)
	{
		//log.open("mcGenreClassifier-Barbedo.log", ios_base::out);
		cout << "============ Barbedo_Filter ============" << endl;
		cout << "Address:" << reinterpret_cast<long>(extractedData) << endl;
		cout << "Received " << extractedData->nClasses << " Classes" << endl;
		cout << "Received " << extractedData->nFeatures << " Features" << endl;
		cout << "====================================" << endl;
		return extractedData;
	}
	
	void Barbedo_Train(MCDataCollection * extractedData)
	{
		ofstream log;
		double genreCombinationIndex = 0;

		log.open("mcGenreClassifier-Barbedo.log", ios_base::out);
		cout << "Address:" << reinterpret_cast<long>(extractedData) << endl;
		cout << "Size of MCClassData:" << sizeof(MCClassData) << endl;
		cout << "Size of MCClassData *:" << sizeof(MCClassData*) << endl;
		cout << "Size of int64:" << sizeof(int64) << endl;
		log << "Received " << extractedData->nClasses << " Classes" << endl;
		log << "Received " << extractedData->nFeatures << " Features" << endl;
		
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
			
			// Extração
			MCClassData * vecsA = &extractedData->pClassData[gsl_combination_get(selectedGenres, 0)];
			MCClassData * vecsB = &extractedData->pClassData[gsl_combination_get(selectedGenres, 1)];


			// Filtragem
			//gsl_matrix * listOfPotsA = filterCandidates(mA);
			//gsl_matrix * listOfPotsB = filterCandidates(mB);

			MCClassData * listOfPotsA = filterCandidates(vecsA);
			log << "Class A #Vectors: " << listOfPotsA->nVectors << endl;
			MCClassData * listOfPotsB = filterCandidates(vecsB);
			log << "Class B #Vectors: " << listOfPotsB->nVectors << endl;

			log << "============" << endl;

			// Loop das Combinações
			long score, score_max, winner;
			long potentialsCombinationIndex = 0;
			score_max = winner = 0;
			
			double tmp_dist = 0;
			int loop_min = 0;

			tRefVecIndex * refVecsIndex = mc_tRefVecsIndex_alloc(listOfPotsA->nVectors, listOfPotsB->nVectors);
			
			if(refVecsIndex == NULL)
			{
				printf("FATAL: Couldnt Alloc enough memory for the vectors combination\n");
				exit(-1);
			}

			// 6-Vectors Combination Loop
			do
			{
				score = 0;

				// Dist 1
				// Class A Loop
				MCFeatVector * refVec = vecsA->pVectorList;
				for(int64 idx = 0; idx < listOfPotsA->nVectors; idx++)
				{
					double dist_loop_min = 1e300;
					//log << "Idx:" << idx << endl;
					//log << "AA:" << endl;

					// dist A - A
					// Each Vector from class A in the 6-vector combination
					for(int vec = 0; vec < 3; vec++)
					{
						//log << "Vec:" << vec << endl;
						
						size_t idx = gsl_combination_get(refVecsIndex->a, vec);
						//log << "Index:" << idx << endl;
						
						double * pTarget = mc_random(listOfPotsA, idx);
// 						log << "Target Selected" << endl;
// 
// 						if(refVec == NULL)
// 							log << "Ref Vec is NULL" << endl;
							
						double *pCurrent = refVec->pData;
// 						log << "Current Vector Available" << endl;
// 
// 						if(pCurrent == NULL)
// 							log << "Current is NULL" << endl;

						tmp_dist = dist(pCurrent, pTarget, extractedData->nFeatures);
						//log << "Distance Measured ! .... " << tmp_dist << endl;

						if(tmp_dist < dist_loop_min)
						{
							dist_loop_min = tmp_dist; loop_min = vec;
							//log << loop_min << endl;
						}
					}

					//log << "AB:" << endl;

					// dist A - B
					for(int vec = 0; vec < 3; vec++)
					{
						//log << "Vec:" << vec + 3 << endl;
						
						size_t idx = gsl_combination_get(refVecsIndex->b, vec);
						//log << "Index:" << idx << endl;
						
						double * pTarget = mc_random(listOfPotsB, idx);
						//log << "Target Selected" << endl;

// 						if(refVec == NULL)
// 							log << "Ref Vec is NULL" << endl;
// 							else
// 							log << "refVec is OK" << endl;
// 
// 						if(pTarget == NULL)
// 							log << "Target is NULL" << endl;
// 							else
// 							log << "Target is Ok" << endl;

						double *pCurrent = refVec->pData;
						//log << "Current Vector Available" << endl;

// 						if(pCurrent == NULL)
// 							log << "Current is NULL" << endl;
// 							else
// 							log << "Current is Ok" << endl;
						
						tmp_dist = dist(pCurrent, pTarget, extractedData->nFeatures);
						//log << "Distance Measured ! .... " << tmp_dist << endl;

						if(tmp_dist < dist_loop_min)
						{
							dist_loop_min = tmp_dist; loop_min = vec + 3;
						}
					}

					//refVec = refVec->next;
					//refVec = refVec->prev;

					if(loop_min < 3)
						score++;
				}

				// Dist 2
				refVec = vecsB->pVectorList;
				for(int64 idx = 0; idx < listOfPotsB->nVectors; idx++)
				{
					double dist_loop_min = 1e300;
					//log << "Idx:" << idx << endl;
					//log << "BA:" << endl;
					
					for(int vec = 0; vec < 3; vec++)
					{
						//log << "Vec:" << vec << endl;
						
						size_t idx = gsl_combination_get(refVecsIndex->a, vec);
						//log << "Index:" << idx << endl;
						
						double * pTarget = mc_random(listOfPotsA, idx);
// 						log << "Target Selected" << endl;
// 
// 						if(refVec == NULL)
// 							log << "Ref Vec is NULL" << endl;
							
						double *pCurrent = refVec->pData;
// 						log << "Current Vector Available" << endl;
// 
// 						if(pCurrent == NULL)
// 							log << "Current is NULL" << endl;

						tmp_dist = dist(pCurrent, pTarget, extractedData->nFeatures);
						//log << "Distance Measured ! .... " << tmp_dist << endl;

						if(tmp_dist < dist_loop_min)
						{
							dist_loop_min = tmp_dist; loop_min = vec;
							//log << loop_min << endl;
						}
					}

					//log << "BB:" << endl;

					for(int vec = 0; vec < 3; vec++)
					{
						//log << "Vec:" << vec + 3 << endl;
						
						size_t idx = gsl_combination_get(refVecsIndex->b, vec);
						//log << "Index:" << idx << endl;
						
						double * pTarget = mc_random(listOfPotsB, idx);
						//log << "Target Selected" << endl;

// 						if(refVec == NULL)
// 							log << "Ref Vec is NULL" << endl;
// 							else
// 							log << "refVec is OK" << endl;
// 
// 						if(pTarget == NULL)
// 							log << "Target is NULL" << endl;
// 							else
// 							log << "Target is Ok" << endl;

						double *pCurrent = refVec->pData;
						//log << "Current Vector Available" << endl;

// 						if(pCurrent == NULL)
// 							log << "Current is NULL" << endl;
// 							else
// 							log << "Current is Ok" << endl;
						
						tmp_dist = dist(pCurrent, pTarget, extractedData->nFeatures);
						//log << "Distance Measured ! .... " << tmp_dist << endl;

						if(tmp_dist < dist_loop_min)
						{
							dist_loop_min = tmp_dist; loop_min = vec + 3;
						}
					}

					//refVec = refVec->prev;
					nextVector(refVec);
					if(loop_min >= 3)
						score++;
				}
	
				// Avaliação
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
			
			}while(combine(refVecsIndex) == GSL_SUCCESS);
			
			// C only
			if( gsl_combination_next(selectedGenres) == GSL_FAILURE )
				break;
		}

		log.close();
	}
}