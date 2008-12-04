using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;

namespace MCModule
{
	public unsafe class DataFile : IDisposable
	{
		string m_file;
			
		public DataFile(string file)
		{
			m_file = file + ".db";
			uInitialize(m_file);
		}
		
		public void Dispose()
		{
			uTerminate();
		}
		
		public double * GetFeature(string wName, string fName)
		{
			return uGetFeature(wName, fName);
		}
		
		public void AddFeature(string wName, string fName, double * data, long sz)
		{
			uAddFeature(wName, fName, data, sz);
		}
		
		#region Imported Functions
		[DllImport("./mcFileHandler.dll", EntryPoint="Initialize", CharSet=System.Runtime.InteropServices.CharSet.Ansi)]
		extern static void uInitialize(string database);
		
		[DllImport("./mcFileHandler.dll", EntryPoint="Terminate")]
		extern static void uTerminate();
		
		[DllImport("./mcFileHandler.dll", EntryPoint="AddFeature", CharSet=System.Runtime.InteropServices.CharSet.Ansi)]
		extern public static void uAddFeature(string windowName, string featureName, double * data, long dataSize);
		
		[DllImport("./mcFileHandler.dll", EntryPoint="GetFeature", CharSet=System.Runtime.InteropServices.CharSet.Ansi)]
		extern static double * uGetFeature(string windowName, string featureName);
		#endregion
	}
}
