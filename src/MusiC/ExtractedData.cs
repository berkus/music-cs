using System;
using System.Runtime.InteropServices;

namespace MusiC.Data
{
	[StructLayout(LayoutKind.Sequential,Pack = 1)]
	unsafe public struct MCFeatVector
	{
		public double * pData;
		public long nVectors;
		// Unmanaged side only
		public long next;
	}	
	
	[StructLayout(LayoutKind.Sequential,Pack = 1)]
	unsafe public struct MCClassData
	{
		public long nVectorListAlloc;
		public long nVectorList;
		public long nVectors;
		public MCFeatVector * pVectorList;
		public MCDataCollection * pCollection;
	}
	
	[StructLayout(LayoutKind.Sequential,Pack = 1)]
	unsafe public struct MCDataCollection
	{
		public MCClassData * pClassData;
		public long nClasses;
		public long nFeatures;
	}
		
	unsafe public class MCDataHandler
	{
		public static MCDataCollection * BuildCollection(int nClasses, int nFeatures)
		{
			MCDataCollection * data = (MCDataCollection *)System.Runtime.InteropServices.Marshal.AllocHGlobal(sizeof(MCDataCollection)).ToPointer();
			MCClassData * newEntry = data->pClassData = (MCClassData *)System.Runtime.InteropServices.Marshal.AllocHGlobal(sizeof(MCClassData) * nClasses).ToPointer();
			
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
