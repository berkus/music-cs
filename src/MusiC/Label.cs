// Copyright (c) 2008-2009 Marcos José Sant'Anna Magalhães
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using System.Security.Cryptography;

namespace MusiC
{
	/// <summary>
	/// 
	/// </summary>
	public class Label : MusiCObject, ILabel, IEnumerable<string>
	{
		class FileEnumerator : IEnumerator<string>
		{
			IEnumerator<FileEntry> _enumF;

			//::::::::::::::::::::::::::::::::::::::://

			/// <value>
			/// 
			/// </value>
			string IEnumerator<string>.Current
			{
				get { return _enumF.Current.File; }
			}

			object IEnumerator.Current
			{
				get { return _enumF.Current.File; }
			}

			//::::::::::::::::::::::::::::::::::::::://

			public FileEnumerator( LinkedList<FileEntry> source )
			{
				_enumF = source.GetEnumerator();
			}

			//::::::::::::::::::::::::::::::::::::::://

			/// <summary>
			/// 
			/// </summary>
			public void Reset()
			{
				_enumF.Reset();
			}

			//::::::::::::::::::::::::::::::::::::::://

			/// <summary>
			/// 
			/// </summary>
			/// <returns>
			/// A <see cref="System.Boolean"/>
			/// </returns>
			public bool MoveNext()
			{
				return _enumF.MoveNext();
			}

			//::::::::::::::::::::::::::::::::::::::://

			/// <summary>
			/// 
			/// </summary>
			public void Dispose()
			{
				if( _enumF != null )
					_enumF.Dispose();
			}
		}

		//---------------------------------------//

		/// <summary>
		/// 
		/// </summary>
		class FileEntry
		{
			FileSource _src;
			string _path;

			//::::::::::::::::::::::::::::::::::::::://

			public string File
			{
				get { return _path; }
			}

			//::::::::::::::::::::::::::::::::::::::://

			public FileSource Source
			{
				get { return _src; }
			}

			//::::::::::::::::::::::::::::::::::::::://

			public FileEntry( FileSource src, string path )
			{
				_src = src;
				_path = path;
			}
		}

		//---------------------------------------//

		enum FileSource
		{
			Dir,
			File
		}

		//---------------------------------------//

		private string _label;
		private string _outDir;

		private LinkedList<FileEntry> _inputList = new LinkedList<FileEntry>();

		//::::::::::::::::::::::::::::::::::::::://

		/// <value>
		/// 
		/// </value>
		public string Name
		{
			get { return _label; }
			set { _label = value; }
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <value>
		/// 
		/// </value>
		public string OutputDir
		{
			get { return _outDir; }
			set { _outDir = value; }
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dir"></param>
		/// <param name="recursive"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public bool AddInputDir( string dir, bool recursive, string filter )
		{
			bool ret = false;

			if( ret = Directory.Exists( dir ) )
			{
				SearchOption opt = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
				filter = ( filter == null ) ? "*" : filter;

				foreach( string file in Directory.GetFiles( dir, filter, opt ) )
				{
					_inputList.AddLast( new FileEntry( FileSource.Dir, file ) );
				}
			}

			return ret;
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public bool AddInputFile( string file )
		{
			bool ret = false;
			if( ret = File.Exists( file ) )
				_inputList.AddLast( new FileEntry( FileSource.File, file ) );

			return ret;
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public bool Validate()
		{
			return true;
		}

		//::::::::::::::::::::::::::::::::::::::://

		IEnumerator<string> IEnumerable<string>.GetEnumerator()
		{
			return new FileEnumerator( this._inputList );
		}

		//::::::::::::::::::::::::::::::::::::::://

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new FileEnumerator( this._inputList );
		}
	}
}