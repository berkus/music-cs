using System;
using System.Collections.Generic;
using System.Text;

using MCModule.UnmanagedInterface;

namespace MCModule
{
	public unsafe class Extractor
	{	
		public struct BindInfo
		{
			public string file;
			public Handler fileHandler;
		}
		
		public static LinkedList<BindInfo> GetExtractableFiles(string dir)
		{
			LinkedList<BindInfo> fileList = new LinkedList<BindInfo>();
			string[] files = System.IO.Directory.GetFiles(dir);
			
			foreach(HandlerInfo info in Config.GetHandlerList())
			{			
				foreach(string file in files)
				{
					if(info.MatchPattern(file))
					{
						///@todo Calling Handler.CanHandle()
						BindInfo i = new BindInfo();
						i.file = file;
						i.fileHandler = info.New() as Handler;
						fileList.AddLast(i);
					}
				}
			}
			
			return fileList;
		}

		public static MCDataCollection * BatchExtract(ICollection<Feature> FeatList, Window wnd, ICollection<String> DirList)
		{
			MCDataCollection * dtCol = MCDataHandler.BuildCollection(DirList.Count, FeatList.Count);
			
			Console.WriteLine("-- Reporting Extraction --");
			Console.WriteLine("Classes Found: " + dtCol->nClasses);
			Console.WriteLine("Features Found: " + dtCol->nFeatures);
			
			MCClassData * wClass = dtCol->pClassData;
			IntPtr ptr = new IntPtr(wClass);
			//Console.WriteLine("Address:"+ptr.ToInt64());
			
			/// @note Extracts features from all files in the folder !
			foreach(string dir in DirList)
			{
				LinkedList<BindInfo> fileList = GetExtractableFiles(dir);	
				
				MCDataHandler.BuildVectorList(wClass, fileList.Count);
				
				foreach(BindInfo info in fileList)
				{
					Console.WriteLine("Working File:" + info.file);
					
					//Handler h = Config.GetHandler(file);
					info.fileHandler.Attach(info.file);
					wnd.Attach(info.fileHandler);
					
					Console.WriteLine("Extracting ...");
					Extract(FeatList, wnd, wClass);
					
					wnd.Detach();
				}
				
				wClass++;
				IntPtr ptr2 = new IntPtr(wClass);
				Console.WriteLine("Address:"+ptr2.ToInt64());
				//wClass = wClass->next;
			}

			Console.WriteLine("Size Of MCClassData:"+sizeof(MCClassData));
			
			Console.WriteLine("Vector Count:");
			wClass = dtCol->pClassData;
			for(int i = 0; i < dtCol->nClasses; i++)
			{
				Console.WriteLine("Class " + (i+1) + ":" + wClass->nVectors);
				//wClass = wClass->next;
				wClass++;
			}
			
			return dtCol;
		}
		
		public static void Extract(ICollection<Feature> FeatList, Window wnd, MCClassData * wClass)
		{
			// dbHandler dbH = new dbHandler();
			// dbH.Attach(wnd.FileHandler.WorkingFile);
			// 
			// int fIdx = 0;
			// double * p;
			// double * fData = null;
			// double * soundData = null;
			// 
			// // Check feature size
			// int sz1 = -1;
			// int sz2 = -1;
			// foreach (Feature f in FeatList)
			// {			
				// if((sz2 = dbH.GetFeature(wnd.Name, f.Name, fData)) == 0)
				// {
					// fData = f.OutterExtract(wnd);
					// sz2 = f.FeatureSize(wnd);
					// dbH.AddFeature(wnd.Name, f.Name, fData, sz2);
				// }
				// 
				// if(sz1 == -1)
				// {
					// sz1 = sz2;
					// if(soundData == null)
						// soundData = UnsafePtr.dgetmem(sizeof(double) * sz2 * FeatList.Count);
				// }
				// else
				// {
					// if(sz1 != sz2)
					// {
						// UnsafePtr.free(soundData);
						// throw new System.Exception("Feature Size do not Agree");
					// }
				// }
				// 
				// p = fData;
				// 
				// for(long idx = fIdx; idx < sz1 * FeatList.Count; idx += sz1)
				// {
					// *(soundData + idx) = *(p++);
				// }
				// 
				// fIdx += 1;
			// }
			// 
			// MCDataHandler.AddVectorList(wClass, soundData, sz2);
// 
			// dbH.Detach();
			// dbH.Dispose();
			// dbH = null;

			return;
		}
	}
}