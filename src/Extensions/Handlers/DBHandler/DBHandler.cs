using System;
using System.Runtime.InteropServices;

namespace MCModule.FileHandlers
{
	public class dbHandler : Handler
	{
		public string GetDBName(string file)
		{
			return file + ".db";
		}

		
		unsafe public int GetFeature(string wndName, string featName, double * data)
		{
			return uGetFeature(wndName, featName, data);
		}
		
		unsafe public void AddFeature(string wndName, string featName, double * data, int size)
		{
			uAddFeature(wndName, featName, data, size);
		}
		
		override public void Dispose ()
		{
			///@todo Implement
		}
		
		override unsafe protected void InnerAttach(string file)
		{

			uInitialize(GetDBName(file));
		}
		
		override unsafe protected void InnerDetach()
		{
			uTerminate();
		}

		
		override protected void Load()
		{
		}
		
		[DllImport("musiC-uMng.dll", EntryPoint="Initialize", CharSet=CharSet.Ansi)]
		extern static unsafe double * uInitialize(string dbName);
		
		[DllImport("musiC-uMng.dll", EntryPoint="Terminate")]
		extern static unsafe double * uTerminate();
		
		[DllImport("musiC-uMng.dll", EntryPoint="GetFeature", CharSet=CharSet.Ansi)]
		extern static unsafe int uGetFeature(string wndName, string featName, double * dt);
		
		[DllImport("musiC-uMng.dll", EntryPoint="AddFeature", CharSet=CharSet.Ansi)]
		extern static unsafe double * uAddFeature(string wndName, string featName, double * data, int size);

	}
}