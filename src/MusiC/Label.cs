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
	class Label : MusiCObject, ILabel, IEnumerable<string>
	{
		/// <value>
		/// 
		/// </value>
		class DirEntry
		{
			private string _dir;
			private LinkedList<FileEntry> _fileList = new LinkedList<FileEntry>();
			
			//::::::::::::::::::::::::::::::::::::::://
			
			/// <summary>
			/// Recursive search.
			/// </summary>
			/// <param name="dir">
			/// A <see cref="System.String"/>
			/// </param>
			/// <returns>
			/// A <see cref="System.Boolean"/>
			/// </returns>
			public bool SetDir(string dir)
			{
				bool ret;

				if( _dir != null )
					_fileList.Clear();
				
				if( ret = Directory.Exists(dir) )
				{
					_dir = dir;

					foreach( string file in Directory.GetFiles(dir, "*.wav", SearchOption.AllDirectories) )
					{
						_fileList.AddLast(new FileEntry(file));
					}
				}

				return ret;
			}

			public IEnumerator<FileEntry> GetFileEnumerator()
			{
				return _fileList.GetEnumerator();
			}
		}
		
		//---------------------------------------//
		
		/// <value>
		/// 
		/// </value>
		class FileEntry
		{
			private string _file;
			//private string _md5;

			//::::::::::::::::::::::::::::::::::::::://

			/// <value>
			/// 
			/// </value>
			public string File
			{
				get { return _file; }
			}
			
			//::::::::::::::::::::::::::::::::::::::://
			
			public FileEntry(string file)
			{
				_file = file;
				//_md5 = Hash(_file);
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
			static
			public string Hash(string file)
			{
				Byte[] encodedBytes;
				MD5 md5;
				
				StreamReader st = new StreamReader( new FileStream( file, FileMode.Open ) );
				
				md5 = new MD5CryptoServiceProvider();
				encodedBytes = md5.ComputeHash(st.BaseStream);

				st.Close();
				
				//Convert encoded bytes back to a 'readable' string
				return BitConverter.ToString(encodedBytes);
			}
		}
		
		//---------------------------------------//

		class FileEnumerator : IEnumerator<string>
		{
			IEnumerator<DirEntry> _enumD;
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
			
			public FileEnumerator( LinkedList<DirEntry> source )
			{
				_enumD = source.GetEnumerator();
				_enumD.MoveNext();
				_enumF = _enumD.Current.GetFileEnumerator();
			}
			
			//::::::::::::::::::::::::::::::::::::::://
			
			/// <summary>
			/// 
			/// </summary>
			 public void Reset()
			{
				_enumD.Reset();
				_enumF = _enumD.Current.GetFileEnumerator();
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
				bool ret;
				
				if( _enumF != null )
				{
					ret = _enumF.MoveNext();
					if( ret )
						return true;
				}

				ret = _enumD.MoveNext();
				if( ret )
				{
					_enumF = _enumD.Current.GetFileEnumerator();
				}

				return ret;
			}
			
			//::::::::::::::::::::::::::::::::::::::://
			
			/// <summary>
			/// 
			/// </summary>
			public void Dispose()
			{
				if( _enumD != null )
					_enumD.Dispose();

				if( _enumF != null )
					_enumF.Dispose();
			}
		}
		
		//---------------------------------------//
		
		private string _label;
		private string _outDir;

		private LinkedList<DirEntry> _inputList = new LinkedList<DirEntry>();
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <value>
		/// 
		/// </value>
		public string Name {
			get { return _label; }
			set { _label = value; }
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <value>
		/// 
		/// </value>
		public string OutputDir {
			get { return _outDir; }
			set { _outDir = value; }
		}

		//::::::::::::::::::::::::::::::::::::::://
		
//		override
//		public String ToString()
//		{
//			String obj=PrintHeader();
//			
//			obj= obj +
//			"[Label]:"+ _label + "\n" +
//			"[Input Dir]:"+_inDir + "\n"+
//			"[Output Dir]:"+_outDir + "\n";
//			
//			obj = obj + PrintFooter();
//			
//			return obj;
//		}
		
		//::::::::::::::::::::::::::::::::::::::://

		public bool AddInputDir(string dir)
		{
			DirEntry d = new DirEntry();

			bool ret;
			ret = d.SetDir(dir);

			if( ret )
				_inputList.AddLast(d);

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
			if (!Directory.Exists(_outDir)) 
			{
				/// @todo change to a default place
				Message("Label:" + _label + " - Invalid directory - " + _outDir);
				return false;
			}
			
			Print();
			return true;
		}

		//::::::::::::::::::::::::::::::::::::::://

		IEnumerator<string> IEnumerable<string>.GetEnumerator()
		{
			return new FileEnumerator(this._inputList);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new FileEnumerator(this._inputList);
		}
	}
}