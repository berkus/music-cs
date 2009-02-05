/*
 * The MIT License
 * Copyright (c) 2008-2009 Marcos José Sant'Anna Magalhães
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
using System.IO;

namespace MusiC
{
	//---------------------------------------//

	/// <summary>
	/// 
	/// </summary>
	/// @todo Make sure base Attach and Detach are executed.
	abstract
	public class BaseHandler : Extension, IHandler
	{
		private string _file;

		//::::::::::::::::::::::::::::::::::::::://

		//// <value>
		/// 
		/// </value>
		public string CurrentFile
		{
			get { return _file; }
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		virtual
		public bool CanHandle(string file)
		{
			return false;
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		virtual
		public void Attach(string file)
		{
			if(System.IO.File.Exists(file))
				_file = file;
			else
				Detach();
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		public DBHandler GetDBHandler()
		{
			DBHandler hnd = null;

			if(_file != null)
				hnd = new DBHandler(_file);

			return hnd;
		}
		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		virtual
		public void Detach()
		{
			_file = null;
		}

		abstract
		public int GetStreamSize();
	}
	
	//---------------------------------------//
	
	namespace Managed
	{
		/// <summary>
		/// 
		/// </summary>
		abstract
		public class Handler : BaseHandler
		{
		}
	}

	//---------------------------------------//
	
	namespace Unmanaged
	{
		/// <summary>
		/// 
		/// </summary>
		[CLSCompliant(false)]
		abstract
		public class Handler : BaseHandler
		{
			abstract unsafe
			public Single * Read(long windowPos, int windowSize);
		}
	}
}