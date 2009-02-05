/*
 * The MIT License
 * Copyright (c) 2008-2009 Marcos José Sant'Anna Magalhães
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

#include "Barbedo.h"

//::::::::::::::::::::::::::::::::::::::://

using namespace std;
using namespace MusiC::Native;

//::::::::::::::::::::::::::::::::::::::://

const int MusiC::Native::Barbedo::MAX_CANDIDATES = 20;
const int MusiC::Native::Barbedo::CLUSTER_SIZE = 92;
const int MusiC::Native::Barbedo::CLUSTER_COUNT = 32;
const int MusiC::Native::Barbedo::FRAME_COUNT = MusiC::Native::Barbedo::CLUSTER_SIZE * MusiC::Native::Barbedo::CLUSTER_COUNT;

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
Barbedo::RefVecIndex * Barbedo::createCombination (int szA, int szB)
{
    RefVecIndex * r = new RefVecIndex();

    r->a = gsl_combination_calloc (szA, 3);
    r->b = gsl_combination_calloc (szB, 3);

    return r;
}

//::::::::::::::::::::::::::::::::::::::://

///
///
///
/// <param name="cl">
/// A <see cref="MusiC::Native::ClassData"/>
/// </param>
ClassData * Barbedo::filterCandidates (ClassData * cl)
{
    // loop counter
    int candidates_counter = 0;

    // feat idx and feat count
    register int idx = 0;
    register int nFeat = cl->pCollection->nFeatures;

    float mean[nFeat];
    float var[nFeat];
    float high_bound[nFeat];
    float low_bound[nFeat];

    for (idx = 0; idx < nFeat; idx++)
    {
        mean[idx]= 0.0f;
        var[idx]= 0.0f;
        high_bound[idx]= 0.0f;
        low_bound[idx]= 0.0f;
    }

    float * currentFrame;

    FileData * fileDt = cl->pFirstFile;
    FrameData * frameDt = fileDt->pFirstFrame;

    cout << "Starting Mean/Var Calulation" << endl;

	// Calculating mean, var of all frames.
	// var = E(x^2) - E(x)^2
	// http://en.wikipedia.org/wiki/Computational_formula_for_the_variance
    while (frameDt)
    {
        currentFrame = frameDt->pData;

        for (idx = 0; idx < nFeat; idx++)
        {
            mean[idx] += currentFrame[idx] / cl->nFrames;
            var[idx] += currentFrame[idx] * currentFrame[idx] / cl->nFrames;
        }

        frameDt = frameDt->pNextFrame;
    }

	
    for (idx = 0; idx < nFeat; idx++)
    {
        var[idx] -= mean[idx] * mean[idx];
        high_bound[idx] = mean[idx] + 1 * sqrt(var[idx]);
        low_bound[idx] = mean[idx] - 1 * sqrt(var[idx]);
    }

//    for (idx = 0; idx < nFeat; idx++)
//        cout << " " << mean[idx];
//
//    cout << endl;
//
//    for (idx = 0; idx < nFeat; idx++)
//        cout << " " << sqrt(var[idx]);
//
//    cout << endl;

    frameDt = fileDt->pFirstFrame;
    FrameData * lastkept = NULL;

    bool keep;

    while (frameDt)
    {
        keep = true;
        currentFrame = frameDt->pData;
		
		// Avaliate if the frame should be kept.
        for (idx = 0; idx < nFeat; idx++)
        {
            if (currentFrame[idx] >= high_bound[idx] || currentFrame[idx] <= low_bound[idx])
            {
                keep = false;
                break;
            }
        }
		
		// Is it a surplus ?
        if (candidates_counter >= MAX_CANDIDATES && keep)
        {
            keep = false;
			
			// make sure we know we are loosing good frames.
            candidates_counter++;
        }

        if (!keep)
        {
            if (lastkept) lastkept->pNextFrame = frameDt->pNextFrame;

            fileDt->nFrames--;

            delete frameDt->pData;
            delete frameDt;

            frameDt = frameDt->pNextFrame;

            continue;
        }

        candidates_counter++;

        if(!lastkept) fileDt->pFirstFrame = frameDt;
        lastkept = frameDt;

        frameDt = frameDt->pNextFrame;
    }

    if (lastkept) lastkept->pNextFrame = NULL;

	cout << "Frame Count: After Filter/Total Aproved/Before Filter" << endl;
    cout << fileDt->nFrames << "/" << candidates_counter << "/" << cl->nFrames << endl;
	
    cl->nFrames = fileDt->nFrames;

    return cl;
}

//::::::::::::::::::::::::::::::::::::::://

///
///
///
/// <param name="ref">
/// A <see cref="::RefVecIndex"/>
/// </param>
int Barbedo::combine (Barbedo::RefVecIndex * ref)
{
    if (gsl_combination_next (ref->b) == GSL_FAILURE)
    {
        if (gsl_combination_next (ref->a) == GSL_FAILURE)
            return GSL_FAILURE;

        gsl_combination_init_first (ref->b);
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
float Barbedo::dist (float * src, float * ref, int size)
{
    // column order
    double ret = 0;
    double cum = 0;

    for (int idx = 0; idx < size; idx++)
    {
        cum = src[idx] - ref[idx];
        ret += cum * cum;
    }

    return sqrt (ret);
}

//::::::::::::::::::::::::::::::::::::::://

///
///
///
float * Barbedo::seq_access (ClassData * cl, Int64 idx)
{
    FrameData * fr = cl->pFirstFile->pFirstFrame;

    while (idx > 0)
    {
        idx--;
        fr = fr->pNextFrame;
    }

    return fr->pData;
}

//::::::::::::::::::::::::::::::::::::::://

///
///
///
DataCollection * Barbedo::Filter (DataCollection * extractedData)
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

    float * currentFrame;
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

        FileData * fileDt = cl->pFirstFile;
        FrameData * frameDt;

        for ( int file_counter = 0; file_counter < cl->nFiles; file_counter++ )
        {
            firstFrameIdx = (int) ceil ( (fileDt->nFrames - 1) / 2) - (int) floor (FRAME_COUNT / 2);
            if (firstFrameIdx < 0)
            {
                log << "File too short" << endl;
                continue;
            }

            log << "Frame Count: " << fileDt->nFrames << endl;
            log << "First Frame: " << firstFrameIdx << endl;
            log << "Last Frame: " << firstFrameIdx + FRAME_COUNT - 1 << endl;
            log << endl;

            nlog << "NewFile" << endl << endl;

            frameDt = fileDt->pFirstFrame;

            for ( frame_counter = 0; frame_counter < firstFrameIdx; frame_counter++)
            {
                //nlog << frameDt->pData[0] << endl;
                frameDt = frameDt->pNextFrame;
            }

            for ( cluster_counter = 0; cluster_counter < CLUSTER_COUNT; cluster_counter++)
            {
                FrameData * nfd = new FrameData();
                nfd->pData = new float[extractedData->nFeatures * 3];

                currentFrame = frameDt->pData;
                nlog << "Cluster " << cluster_counter << endl << endl;

                for (idx = 0; idx < nFeat; idx++)
                {
                    mean[idx] = 0;
                    var[idx] = 0;
                    max[idx] = -INFINITY;
                }

                for ( cluster_sz_counter = 0; cluster_sz_counter < CLUSTER_SIZE; cluster_sz_counter++ )
                {
                    nlog << frameDt->pData[0] << endl;

                    for (idx = 0; idx < nFeat; idx++)
                    {
                        mean[idx] += currentFrame[idx]/CLUSTER_SIZE;
                        var[idx] += currentFrame[idx] * currentFrame[idx] / CLUSTER_SIZE;
                        if (currentFrame[idx] > max[idx]) max[idx] = currentFrame[idx];
                    }
                }

                for (idx = 0; idx < nFeat; idx++)
                {
                    log << "mean: " << (nfd->pData[ 3 * idx ] = mean[ idx ]) << endl;

                    var[idx] -= mean[idx] * mean[idx];
                    log << "var: " << (nfd->pData[ 3 * idx + 1 ] = var[ idx ]) << endl;

                    log << "max: " << max[ idx ] << endl;
                    log << "ppp:" << (nfd->pData[ 3 * idx + 2 ] = max[ idx ] / mean[ idx ]) << endl;
                    log << endl;
                }

                nfd->pNextFrame = nfl->pFirstFrame;
                nfl->pFirstFrame = nfd;

                nfl->nFrames++;
                frameDt = frameDt->pNextFrame;
            }

            fileDt = fileDt->pNextFile;
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

//::::::::::::::::::::::::::::::::::::::://

///
///
///
void Barbedo::Train (DataCollection * extractedData)
{
    ofstream log;
    double genreCombinationIndex = 0;

    DataHandler data;
    data.Attach (extractedData);

    log.open ("Barbedo_Train.txt", ios_base::out);
    log << "Address:" << reinterpret_cast<size_t> (extractedData) << endl;
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
        log << "Filtering Best Candidates" << endl;

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
        RefVecIndex * refVecsIndex = createCombination (fClassA->nFrames, fClassB->nFrames);

        if (refVecsIndex == NULL)
        {
            log << "Combination structure out of memory" << endl;
            return;
        }

        // Frames Combination Loop
        do
        {
            score = 0;

            // Dist 1 - Class A
            FrameData * refVec = classA->pFirstFile->pFirstFrame;
            for (Int64 idx = 0; idx < fClassA->nFrames; idx++)
            {
                float dist_loop_min = INFINITY;

                // dist A - A
                // Each Vector from class A in the 6-vector combination
                for (int vec = 0; vec < 3; vec++)
                {
                    size_t idx = gsl_combination_get (refVecsIndex->a, vec);

                    float * pTarget = seq_access (fClassA, idx);
                    float * pCurrent = refVec->pData;

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

        gsl_combination_free(refVecsIndex->a);
        gsl_combination_free(refVecsIndex->b);

        delete refVecsIndex;

        if ( gsl_combination_next (selectedGenres) == GSL_FAILURE ) // Stop Condition: Genre Comparison
            break;
    }

    log.close();
}

//::::::::::::::::::::::::::::::::::::::://

#if defined(MUSIC_TEST)

int main()
{
    int featCount, nFeat = 2;
    int classCount, nClasses = 2;
    int fileCount, nFiles = 3;
    int FRAME_COUNT, nFrames = 3000;

    DataCollection * dtCol = new DataCollection();
    dtCol->nFeatures = nFeat;
    dtCol->pFirstClass = NULL;
    dtCol->nClasses = 0;

    for (classCount = 0; classCount < nClasses; classCount++)
    {
        ClassData * newclass = new ClassData();
        newclass->nFiles = 0;
        newclass->nFrames = 0;
        newclass->pFirstFile = NULL;
        newclass->pCollection = dtCol;

        newclass->pNextClass = dtCol->pFirstClass;
        dtCol->pFirstClass = newclass;
        dtCol->nClasses++;

        for (fileCount = 0; fileCount < nFiles; fileCount++)
        {
            FileData * newfile = new FileData();

            newfile->nFrames = 0;
            newfile->pFirstFrame = NULL;
            newfile->pClass = newclass;

            newfile->pNextFile = newclass->pFirstFile;
            newclass->pFirstFile = newfile;
            newclass->nFiles++;

            for (FRAME_COUNT = 0; FRAME_COUNT < nFrames; FRAME_COUNT++)
            {
                FrameData * newFrame = new FrameData();

                newFrame->pData = new float[nFeat];

                for(int idx = 0; idx < nFeat; idx++)
                    newFrame->pData[idx] = random();

                newFrame->pNextFrame = newfile->pFirstFrame;
                newfile->pFirstFrame = newFrame;

                newfile->nFrames++;
                newclass->nFrames++;
            }
        }
    }

    DataCollection * col = Barbedo_Filter(dtCol);
    Barbedo_Train(col);

    return 0;
}

#endif
