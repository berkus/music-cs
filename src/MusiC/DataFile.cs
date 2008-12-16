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
