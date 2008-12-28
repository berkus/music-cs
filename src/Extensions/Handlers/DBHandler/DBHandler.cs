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

namespace MusiC
{
	public class DBHandler : Handler
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		/// <todo>Check if the it can handle the music-db file</todo>
		override public bool CanHandle(String file)
		{
			return true;
		}
	}
}

//namespace MCModule.FileHandlers
//{
//	public class dbHandler : Handler
//	{
//		public string GetDBName(string file)
//		{
//			return file + ".db";
//		}
//
//		
//		unsafe public int GetFeature(string wndName, string featName, double * data)
//		{
//			return uGetFeature(wndName, featName, data);
//		}
//		
//		unsafe public void AddFeature(string wndName, string featName, double * data, int size)
//		{
//			uAddFeature(wndName, featName, data, size);
//		}
//		
//		override public void Dispose ()
//		{
//			///@todo Implement
//		}
//		
//		override unsafe protected void InnerAttach(string file)
//		{
//
//			uInitialize(GetDBName(file));
//		}
//		
//		override unsafe protected void InnerDetach()
//		{
//			uTerminate();
//		}
//
//		
//		override protected void Load()
//		{
//		}
//		
//		[DllImport("musiC-uMng.dll", EntryPoint="Initialize", CharSet=CharSet.Ansi)]
//		extern static unsafe double * uInitialize(string dbName);
//		
//		[DllImport("musiC-uMng.dll", EntryPoint="Terminate")]
//		extern static unsafe double * uTerminate();
//		
//		[DllImport("musiC-uMng.dll", EntryPoint="GetFeature", CharSet=CharSet.Ansi)]
//		extern static unsafe int uGetFeature(string wndName, string featName, double * dt);
//		
//		[DllImport("musiC-uMng.dll", EntryPoint="AddFeature", CharSet=CharSet.Ansi)]
//		extern static unsafe double * uAddFeature(string wndName, string featName, double * data, int size);
//
//	}
//}