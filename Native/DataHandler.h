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

#if !defined(_MUSIC_NATIVE_DATAHANDLER_H_)
#define _MUSIC_NATIVE_DATAHANDLER_H_

#include <cstdlib>

#include "ExtractedData.h"

namespace MusiC
{
	namespace Native
	{
		class DataHandler
		{
		
		private:
		
			DataCollection * _data;
			ClassData * _curClass;
			FileData * _curFile;
			FrameData * _curFrame;
			
		public:
		
			DataHandler();
			
			void Attach( DataCollection * );
			
			unsigned int GetNumClasses()
			{
				return _data->nClasses;
			}
			
			unsigned int GetNumFeatures()
			{
				return _data->nFeatures;
			}

			ClassData * GetClass( unsigned int idx );
			ClassData * GetNextClass();

			FileData * GetNextLocalFile();
			FrameData * GetNextLocalFrame();

			FileData * GetNextGlobalFile();
			FrameData * GetNextGlobalFrame();

			static DataCollection * BuildDataCollection( unsigned int nfeat )
			{
				DataCollection * d = new DataCollection();

				d->nClasses = 0;
				d->nFeatures = nfeat;

				d->pFirstClass = d->pLastClass = NULL;

				return d;
			}

			static ClassData * BuildClassData( DataCollection * dataCol )
			{
				ClassData * d = new ClassData();

				d->nFiles = 0;
				d->nFrames = 0;
				d->pCollection = dataCol;
				d->pFirstFile = NULL;
				d->pLastFile = NULL;
				d->pNextClass = NULL;

				if( dataCol->pLastClass )
					dataCol->pLastClass->pNextClass = d;

				if( !dataCol->pFirstClass )
					dataCol->pFirstClass = d;

				dataCol->pLastClass = d;
				dataCol->nClasses++;

				return d;
			}

			static FileData * BuildFileData( ClassData * class_data )
			{
				FileData * d = new FileData();

				d->nFrames = 0;
				d->pClass = class_data;
				d->pFirstFrame = d->pFirstFrame = NULL;
				d->pNextFile = d->pPrevFile = NULL;
				d->pFiltered = NULL;

				if( !class_data->pFirstFile )
					class_data->pFirstFile = d;

				if( class_data->pLastFile )
					class_data->pLastFile->pNextFile = d;

				d->pPrevFile = class_data->pLastFile;
				class_data->pLastFile = d;

				class_data->nFiles++;

				return d;
			}

			static FrameData * BuildFrameData( FileData * currentFile )
			{
				FrameData * newFrame = new FrameData();
				newFrame->pData = new float [ currentFile->pClass->pCollection->nFeatures ];

				currentFile->nFrames++;

				if( currentFile->pClass != NULL )
					currentFile->pClass->nFrames++;

				if( currentFile->pLastFrame != NULL )
				{
					currentFile->pLastFrame->pNextFrame = newFrame;
					currentFile->pLastFrame = newFrame;
				}
				else
				{
					// First frame of the file links the last frame of the previous file.
					currentFile->pFirstFrame = newFrame;
					currentFile->pLastFrame = newFrame;

					if( currentFile->pNextFile != NULL )
						newFrame->pNextFrame = currentFile->pNextFile->pFirstFrame;

					if( currentFile->pPrevFile != NULL )
						currentFile->pPrevFile->pLastFrame->pNextFrame = newFrame;
				}

				return newFrame;
			}
		};
	}
}

#endif
