/*
 * The MIT License
 * Copyright (c) 2008-2009 Marcos Jos� Sant'Anna Magalh�es
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

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace MusiC.Data.Unmanaged
{
	[CLSCompliant(false)]
	[StructLayout(LayoutKind.Sequential,Pack = 1)]
	unsafe public struct FrameData
	{
		public float * pData;
		public FrameData * pNextFrame;
	}	
	
	[CLSCompliant(false)]
	[StructLayout(LayoutKind.Sequential,Pack = 1)]
	unsafe public struct FileData
	{
		public long nFrames;
		
		public FileData * pNextFile;
		
		public FrameData * pFirstFrame;
		
		public FrameData * pFiltered;
		
		public ClassData * pClass;
	}	
	
	[CLSCompliant(false)]
	[StructLayout(LayoutKind.Sequential,Pack = 1)]
	unsafe public struct ClassData
	{
		public long nFiles;
		public long nFrames;
		
		public ClassData * pNextClass;
		
		public FileData * pFirstFile;
		
		public DataCollection * pCollection;
	}
	
	[CLSCompliant(false)]
	[StructLayout(LayoutKind.Sequential,Pack = 1)]
	unsafe public struct DataCollection
	{
		public int nClasses;
		public int nFeatures;
		
		public ClassData * pFirstClass;
	}
	
	[CLSCompliant(false)]
	unsafe public class DataHandler
	{
		public static DataCollection * BuildCollection(int nFeatures)
		{
			DataCollection * data = (DataCollection *) Marshal.AllocHGlobal(sizeof(DataCollection)).ToPointer();

			data->nClasses = 0;
			data->nFeatures = nFeatures;

			data->pFirstClass = null;
			
			return data;
		}
		
		public static ClassData * BuildClassData(DataCollection * dtCol)
		{
			ClassData * newClass = (ClassData *) Marshal.AllocHGlobal(sizeof(ClassData)).ToPointer();
			
			newClass->pNextClass = null;
			
			newClass->pFirstFile = null;
				
			newClass->nFrames = 0;
			newClass->nFiles = 0;
			newClass->pCollection = dtCol;
			
			dtCol->nClasses++;
			
			// first class
			if(dtCol->pFirstClass == null)
			{
				dtCol->pFirstClass = newClass;
				
				return newClass;
			}
			
			newClass->pNextClass = dtCol->pFirstClass;
			dtCol->pFirstClass = newClass;
			
			return newClass;
		}
		
		public static FileData * BuildFileData(ClassData * currentClass)
		{
			FileData * newFile = (FileData *) Marshal.AllocHGlobal(sizeof(FileData)).ToPointer();
			
			newFile->pNextFile = null;
			newFile->pFirstFrame = null;
			newFile->pFiltered = null;
			
			newFile->nFrames = 0;
			newFile->pClass = currentClass;
			
			currentClass->nFiles++;
			
			newFile->pNextFile = currentClass->pFirstFile;
			currentClass->pFirstFile = newFile;
			
			return newFile;
		}
		
		public static FrameData * BuildFrameData(FileData * currentFile)
		{
			FrameData * newFrame = (FrameData *) Marshal.AllocHGlobal(sizeof(FrameData)).ToPointer();
			
			newFrame->pData = (Single *) Marshal.AllocHGlobal( (currentFile->pClass->pCollection->nFeatures) * sizeof(float)).ToPointer();
			
			currentFile->nFrames++;
			currentFile->pClass->nFrames++;
			
			if( currentFile->pFirstFrame != null )
			{
				newFrame->pNextFrame = currentFile->pFirstFrame;
			}
			else
			{
				// First frame of the file links the last frame of the previous file.
				if(currentFile->pNextFile != null)
					newFrame->pNextFrame = currentFile->pNextFile->pFirstFrame;
				else
					// First frame of the class.
					newFrame->pNextFrame = null;
			}
			
			currentFile->pFirstFrame = newFrame;
			return newFrame;
		}
	}
}

namespace MusiC.Data.Managed
{
	public class MCFeatVector
	{
		public double [] pData;
		public long nVectors = 0;
		// Unmanaged side only - historical
		public long next = 0;
	}	
	
	public class MCClassData
	{
		public long nVectorListAlloc = 0;
		public long nVectorList = 0;
		public long nVectors = 0;
		
		public LinkedList<MCFeatVector> pVectorList = new LinkedList<MCFeatVector>();
		public MCDataCollection pCollection;
	}
	
	public class MCDataCollection
	{
		public MCClassData[] pClassData;
		public long nClasses;
		public long nFeatures;
	}
	
	public class MCDataHandler
	{
		public static MCDataCollection BuildCollection(int nClasses, int nFeatures)
		{
			MCDataCollection data = new MCDataCollection();
			data.pClassData = new MCClassData[nClasses];
			
			for(int i = 0; i < nClasses; i++)
			{
				data.pClassData[i].pCollection = data;
			}
			
			data.nClasses = nClasses;
			data.nFeatures = nFeatures;
			
			return data;
		}
		
		public static void BuildVectorList(MCClassData wClass, Int32 nLists)
		{
			//wClass.pVectorList = ;
			wClass.nVectorListAlloc = nLists;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <remarks>Each list is a new data unit (song, picture, ...) that has been extracted. </remarks>
		/// <param name="wClass"></param>
		/// <param name="data"></param>
		/// <param name="nWindows"></param>
		public static void AddVectorList(MCClassData wClass, Double[] data, Int64 nWindows)
		{
			if(wClass == null )
				Console.WriteLine("MCClassData is NULL");
			
			MCFeatVector vec = new MCFeatVector();
			vec.pData = data;
			vec.nVectors = nWindows;
				
			wClass.pVectorList.AddLast(vec);
			wClass.nVectors += nWindows;
			wClass.nVectorList++;			
		}
	}
}