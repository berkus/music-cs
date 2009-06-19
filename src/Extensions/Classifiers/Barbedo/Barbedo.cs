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
using System.Runtime.InteropServices;

using MusiC;
using MusiC.Data.Unmanaged;

namespace MusiC.Extensions.Classifiers
{
	/// <summary>
	/// 
	/// </summary>
	public unsafe class Barbedo : Unmanaged.Classifier
	{
		void* data;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dtCol">
		/// A <see cref="DataCollection"/>
		/// </param>
		override unsafe
		public void Train( DataCollection* dtCol )
		{
			data = uTrain( ref ( *dtCol ) );
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dataIn">
		/// A <see cref="DataCollection"/>
		/// </param>
		/// <returns>
		/// A <see cref="Data.Unmanaged.DataCollection"/>
		/// </returns>
		override unsafe
		public Data.Unmanaged.FileData* ClassificationFilter( FileData* dataIn, uint nfeat )
		{
			return uCFilter( ref *dataIn, nfeat );
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dtCol">
		/// A <see cref="DataCollection"/>
		/// </param>
		/// <returns>
		/// A <see cref="DataCollection"/>
		/// </returns>
		override unsafe
		public DataCollection* ExtractionFilter( DataCollection* dtCol )
		{
			return uEFilter( ref *dtCol );
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dataIn">
		/// A <see cref="System.Void"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// </returns>
		override unsafe
			//public int Classify( FileData * f, void * dataIn )
		public int Classify( FileData* f )
		{
			return uClassify( ref *f, data );
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="f">
		/// A <see cref="FileData"/>
		/// </param>
		/// <param name="data">
		/// A <see cref="System.Void"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// </returns>
		[DllImport( "MusiC.Extensions.Classifiers.uBarbedo.dll", EntryPoint = "Barbedo_Classify" )]
		static extern unsafe
		public int uClassify( ref FileData f, void* data );

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dtCol">
		/// A <see cref="DataCollection"/>
		/// </param>
		[DllImport( "MusiC.Extensions.Classifiers.uBarbedo.dll", EntryPoint = "Barbedo_Train" )]
		static extern unsafe
		public void* uTrain( ref DataCollection dtCol );

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="f">
		/// A <see cref="FileData"/>
		/// </param>
		/// <param name="nfeat">
		/// A <see cref="System.UInt32"/>
		/// </param>
		/// <returns>
		/// A <see cref="FileData"/>
		/// </returns>
		[DllImport( "MusiC.Extensions.Classifiers.uBarbedo.dll", EntryPoint = "Barbedo_CFilter" )]
		static extern unsafe
		public FileData* uCFilter( ref FileData f, uint nfeat );

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="DataCollection">
		/// A <see cref="DataCollection"/>
		/// </param>
		/// <returns>
		/// A <see cref="DataCollection"/>
		/// </returns>
		[DllImport( "MusiC.Extensions.Classifiers.uBarbedo.dll", EntryPoint = "Barbedo_EFilter" )]
		static extern unsafe
		public DataCollection* uEFilter( ref DataCollection dtCol );
	}
}