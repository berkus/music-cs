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

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace MusiC.Data.Unmanaged
{
	[CLSCompliant(false)]
	[StructLayout(LayoutKind.Sequential,Pack = 1)]
	unsafe 
	public struct FrameData
	{
		public float * pData;
		public FrameData * pNextFrame;
	}
	
	//---------------------------------------//
	
	[CLSCompliant(false)]
	[StructLayout(LayoutKind.Sequential,Pack = 1)]
	unsafe 
	public struct FileData
	{
		public ulong nFrames;
		
		public FileData * pNextFile;
		public FileData * pPrevFile;
		
		public FrameData * pFirstFrame;
		public FrameData * pLastFrame;
		
		public FrameData * pFiltered;
		
		public ClassData * pClass;
	}
	
	//---------------------------------------//
	
	[CLSCompliant(false)]
	[StructLayout(LayoutKind.Sequential,Pack = 1)]
	unsafe 
	public struct ClassData
	{
		public ulong nFiles;
		public ulong nFrames;
		
		public ClassData * pNextClass;
		
		public FileData * pFirstFile;
		public FileData * pLastFile;
		
		public DataCollection * pCollection;
	}
	
	//---------------------------------------//
	
	[CLSCompliant(false)]
	[StructLayout(LayoutKind.Sequential,Pack = 1)]
	unsafe 
	public struct DataCollection
	{
		public uint nClasses;
		public uint nFeatures;
		
		public ClassData * pFirstClass;
		public ClassData * pLastClass;
	}
	
	//---------------------------------------//
	
	/// <summary>
	/// 
	/// </summary>
	[CLSCompliant(false)]
	unsafe 
	public class DataHandler
	{
		private class ExtractedDataReporter : Reporter
		{
		}

		//---------------------------------------//

		static
		private ExtractedDataReporter reporter;

		//::::::::::::::::::::::::::::::::::::::://

		static
		DataHandler()
		{
			reporter = new ExtractedDataReporter();

            reporter.AddMessage("DataCollection: " + sizeof(DataCollection) + " bytes");
            reporter.AddMessage("ClassData: " + sizeof(ClassData) + " bytes");
            reporter.AddMessage("FileData: " + sizeof(FileData) + " bytes");
            reporter.AddMessage("FrameData: " + sizeof(FrameData) + " bytes");
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="nFeatures">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// A <see cref="DataCollection"/>
		/// </returns>
		static
		public DataCollection * BuildCollection(uint nFeatures)
		{
			DataCollection * data = 
				(DataCollection *) Marshal.AllocHGlobal(sizeof(DataCollection)).ToPointer();

			data->nClasses = 0;
			data->nFeatures = nFeatures;

			data->pFirstClass = null;
			
			return data;
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dtCol">
		/// A <see cref="DataCollection"/>
		/// </param>
		/// <returns>
		/// A <see cref="ClassData"/>
		/// </returns>
		static
		public ClassData * BuildClassData(DataCollection * dtCol)
		{
			ClassData * newClass = 
				(ClassData *) Marshal.AllocHGlobal(sizeof(ClassData)).ToPointer();
			
			newClass->pNextClass = null;
			
			newClass->pFirstFile = null;
			newClass->pLastFile = null;
				
			newClass->nFrames = 0;
			newClass->nFiles = 0;
			newClass->pCollection = dtCol;
			
			dtCol->nClasses++;
			
			// first class
			if(dtCol->pFirstClass == null)
			{
				dtCol->pFirstClass = newClass;
				dtCol->pLastClass = newClass;
				
				return newClass;
			}
			
			// newClass->pNextClass = dtCol->pFirstClass;
			dtCol->pLastClass->pNextClass = newClass;
			dtCol->pLastClass = newClass;
			
			return newClass;
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="currentClass">
		/// A <see cref="ClassData"/>
		/// </param>
		/// <returns>
		/// A <see cref="FileData"/>
		/// </returns>
		static
		public FileData * BuildFileData(ClassData * currentClass)
		{	
			FileData * newFile = BuildFileData();
			AddFileData(newFile, currentClass);
			
			return newFile;
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		/// A <see cref="FileData"/>
		/// </returns>
		static
		public FileData * BuildFileData()
		{
			FileData * newFile = (FileData *) Marshal.AllocHGlobal(sizeof(FileData)).ToPointer();
			
			newFile->pNextFile = null;
			newFile->pPrevFile = null;
			
			newFile->pFirstFrame = null;
			newFile->pLastFrame = null;
			
			newFile->pFiltered = null;
			newFile->pClass = null;
			
			newFile->nFrames = 0;

			return newFile;
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="newFile">
		/// A <see cref="FileData"/>
		/// </param>
		/// <param name="currentClass">
		/// A <see cref="ClassData"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		static
		public bool AddFileData(FileData * newFile, ClassData * currentClass)
		{
			if( ( newFile == null ) || ( currentClass == null ) )
			{
				reporter.AddWarning("The file or the current class is NULL");
				return false;
			}
			
			newFile->pClass = currentClass;
			
			newFile->pPrevFile = currentClass->pLastFile;

			currentClass->nFiles++;
			currentClass->nFrames += newFile->nFrames;

			if( currentClass->pLastFile != null )
				currentClass->pLastFile->pNextFile = newFile;
			
			currentClass->pLastFile = newFile;
			
			if( currentClass->pFirstFile == null )
				currentClass->pFirstFile = newFile;

			return true;
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="featCount">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// A <see cref="FrameData"/>
		/// </returns>
		static
		public FrameData * BuildFrameData(int featCount)
		{
			FrameData * newFrame = (FrameData *) Marshal.AllocHGlobal(sizeof(FrameData)).ToPointer();
			
			newFrame->pData = (Single *) Marshal.AllocHGlobal( featCount * sizeof( float ) ).ToPointer();
		 	newFrame->pNextFrame = null;
			
			return newFrame;
		}

		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="newFrame">
		/// A <see cref="FrameData"/>
		/// </param>
		/// <param name="currentFile">
		/// A <see cref="FileData"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		static
		public bool AddFrameData(FrameData * newFrame, FileData * currentFile)
		{
			if( ( newFrame == null ) || ( currentFile == null ) )
			{
				reporter.AddWarning("The frame or the current file is NULL");
				return false;
			}
			
			currentFile->nFrames++;
			
			if( currentFile->pClass != null )
				currentFile->pClass->nFrames++;
			
			if( currentFile->pLastFrame != null )
			{
				currentFile->pLastFrame->pNextFrame = newFrame;
				currentFile->pLastFrame = newFrame;
			}
			else
			{
				// First frame of the file links the last frame of the previous file.
				currentFile->pFirstFrame = newFrame;
				currentFile->pLastFrame = newFrame;

				if( currentFile->pNextFile != null )
					newFrame->pNextFrame = currentFile->pNextFile->pFirstFrame;
				
				if( currentFile->pPrevFile != null )
					currentFile->pPrevFile->pLastFrame->pNextFrame = newFrame;

			}

			return true;
		}

        //::::::::::::::::::::::::::::::::::::::://

        static
        public void DestroyCollection(DataCollection * dtCol)
        {
            uint idxClass = 0; ClassData * pClass = null;
            uint idxFile = 0; FileData * pFile = null;
            uint idxFrame = 0; FrameData* pFrame = null;
            FrameData* pNextFrame = null;

            while (idxClass < dtCol->nClasses)
            {
                pClass = dtCol->pFirstClass;

                while (idxFile < pClass->nFiles)
                {
                    pFile = pClass->pFirstFile;

                    while (idxFrame < pFile->nFrames)
                    {
                        pFrame = pFile->pFirstFrame;
                        pFile->pFirstFrame = pFrame->pNextFrame;

                        NativeMethods.Pointer.free(pFrame->pData);
                        NativeMethods.Pointer.free(pFrame);

                        idxFrame++;
                    }

                    pClass->pFirstFile = pFile->pNextFile;

                    NativeMethods.Pointer.free(pFile);

                    idxFile++;
                }

                dtCol->pFirstClass = pClass->pNextClass;

                NativeMethods.Pointer.free(pClass);

                idxClass++;
            }

            NativeMethods.Pointer.free(dtCol);
        }
    }
}

//---------------------------------------//

namespace MusiC.Data.Managed
{
	public class FrameData
	{
		public double [] pData;
		public long nVectors = 0;
		// Unmanaged side only - historical
		public long next = 0;
	}
	
	//---------------------------------------//
	
	public class ClassData
	{
		public long nVectorListAlloc = 0;
		public long nVectorList = 0;
		public long nVectors = 0;
		
		public LinkedList<FrameData> pVectorList = new LinkedList<FrameData>();
		public DataCollection pCollection;
	}
	
	//---------------------------------------//
	
	public class DataCollection
	{
		public ClassData[] pClassData;
		public long nClasses;
		public long nFeatures;
	}
	
	//---------------------------------------//
	
	public class DataHandler
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="nClasses">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="nFeatures">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// A <see cref="DataCollection"/>
		/// </returns>
		static
		public DataCollection BuildCollection(int nClasses, int nFeatures)
		{
			DataCollection data = new DataCollection();
			data.pClassData = new ClassData[nClasses];
			
			for(int i = 0; i < nClasses; i++)
			{
				data.pClassData[i].pCollection = data;
			}
			
			data.nClasses = nClasses;
			data.nFeatures = nFeatures;
			
			return data;
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="wClass">
		/// A <see cref="ClassData"/>
		/// </param>
		/// <param name="nLists">
		/// A <see cref="Int32"/>
		/// </param>
		static
		public void BuildVectorList(ClassData wClass, Int32 nLists)
		{
			//wClass.pVectorList = ;
			wClass.nVectorListAlloc = nLists;
		}

		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <remarks>Each list is a new data unit (song, picture, ...) that has been extracted. </remarks>
		/// <param name="wClass"></param>
		/// <param name="data"></param>
		/// <param name="nWindows"></param>
		public static void AddVectorList(ClassData wClass, Double[] data, Int64 nWindows)
		{
			if(wClass == null )
				Console.WriteLine("ClassData is NULL");
			
			FrameData vec = new FrameData();
			vec.pData = data;
			vec.nVectors = nWindows;
				
			wClass.pVectorList.AddLast(vec);
			wClass.nVectors += nWindows;
			wClass.nVectorList++;			
		}
	}
}