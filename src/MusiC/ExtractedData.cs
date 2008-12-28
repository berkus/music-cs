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

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace MusiC.Data.Unmanaged
{
	[CLSCompliant(false)]
	[StructLayout(LayoutKind.Sequential,Pack = 1)]
	unsafe public struct MCFeatVector
	{
		public double * pData;
		public long nVectors;
		// Unmanaged side only
		public long next;
	}	
	
	[CLSCompliant(false)]
	[StructLayout(LayoutKind.Sequential,Pack = 1)]
	unsafe public struct MCClassData
	{
		public long nVectorListAlloc;
		public long nVectorList;
		public long nVectors;
		public MCFeatVector * pVectorList;
		public MCDataCollection * pCollection;
	}
	
	[CLSCompliant(false)]
	[StructLayout(LayoutKind.Sequential,Pack = 1)]
	unsafe public struct MCDataCollection
	{
		public MCClassData * pClassData;
		public long nClasses;
		public long nFeatures;
	}
	
	[CLSCompliant(false)]
	unsafe public class MCDataHandler
	{
		public static MCDataCollection * BuildCollection(int nClasses, int nFeatures)
		{
			MCDataCollection * data = (MCDataCollection *) Marshal.AllocHGlobal(sizeof(MCDataCollection)).ToPointer();
			MCClassData * newEntry = data->pClassData = (MCClassData *) Marshal.AllocHGlobal(sizeof(MCClassData) * nClasses).ToPointer();
			
			for(int i = 0; i < nClasses; i++)
			{
				(newEntry)->pCollection = data;
				(newEntry)->pVectorList = null;
				(newEntry)->nVectors = 0;
				(newEntry)->nVectorList = 0;
				(newEntry)->nVectorListAlloc = 0;
				newEntry++;
			}
			
			data->nClasses = nClasses;
			data->nFeatures = nFeatures;
			
			return data;
		}
		
		public static void BuildVectorList(MCClassData * wClass, int nLists)
		{
			wClass->pVectorList = (MCFeatVector *)System.Runtime.InteropServices.Marshal.AllocHGlobal(sizeof(MCFeatVector) * nLists).ToPointer();
			wClass->nVectorListAlloc = nLists;
		}
		
		public static void AddVectorList(MCClassData * wClass, double * data, long nWindows)
		{
			if(wClass == null )
				Console.WriteLine("MCClassData is NULL");
			
			(wClass->pVectorList + wClass->nVectorList)->pData = data;
			(wClass->pVectorList + wClass->nVectorList)->nVectors = nWindows;
			(wClass->pVectorList + wClass->nVectorList)->next = 0;
			wClass->nVectors += nWindows;
			wClass->nVectorList++;			
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