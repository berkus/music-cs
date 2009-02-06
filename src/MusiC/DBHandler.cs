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
 */

using System;
using System.Runtime.InteropServices;

namespace MusiC
{
	/// <summary>
	/// 
	/// </summary>
	public class DBHandler : MusiCObject
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="file">
		/// A <see cref="System.String"/>
		/// </param>
		public DBHandler(string file)
		{
			uInitialize(GetDBName(file));
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		public void Terminate ()
		{
			uTerminate();
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="file">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/>
		/// </returns>
		public string GetDBName(string file)
		{
			return file + ".db";
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="wndName">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="featName">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="data">
		/// A <see cref="System.Single"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// </returns>
		[CLSCompliant(false)]
		unsafe
		public int GetFeature(string wndName, string featName, float * data)
		{
			return uGetFeature(wndName, featName, data);
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="wndName">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="featName">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="data">
		/// A <see cref="System.Single"/>
		/// </param>
		/// <param name="size">
		/// A <see cref="System.Int32"/>
		/// </param>
		[CLSCompliant(false)]
		unsafe
		public void AddFeature(string wndName, string featName, float * data, int size)
		{
			uAddFeature(wndName, featName, data, size);
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dbName">
		/// A <see cref="System.String"/>
		/// </param>
		[DllImport("MusiC.Native.Core.dll", EntryPoint="Initialize", CharSet=CharSet.Ansi)]
		extern static unsafe
		private void uInitialize(string dbName);
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		[DllImport("MusiC.Native.Core.dll", EntryPoint="Terminate")]
		extern static unsafe
		private void uTerminate();
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="wndName">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="featName">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="dt">
		/// A <see cref="System.Single"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// </returns>
		[DllImport("MusiC.Native.Core.dll", EntryPoint="GetFeature", CharSet=CharSet.Ansi)]
		extern static unsafe
		private int uGetFeature(string wndName, string featName, float * data);
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="wndName">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="featName">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="data">
		/// A <see cref="System.Single"/>
		/// </param>
		/// <param name="size">
		/// A <see cref="System.Int32"/>
		/// </param>
		[DllImport("MusiC.Native.Core.dll", EntryPoint="AddFeature", CharSet=CharSet.Ansi)]
		extern static unsafe
		private void uAddFeature(string wndName, string featName, float * data, int size);

	}
}