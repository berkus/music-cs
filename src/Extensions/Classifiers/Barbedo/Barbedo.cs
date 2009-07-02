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
using System.IO;
using System.Collections.Generic;

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

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dtCol">
		/// A <see cref="DataCollection"/>
		/// </param>
		override unsafe
		public void Train( DataCollection* dtCol )
		{
			data = uTrain( ref *dtCol );
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
		public DataCollection* TrainingFilter( DataCollection* dtCol )
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
		public int Classify( FileData* f )
		{
			return uClassify( ref *f, data );
		}

		//::::::::::::::::::::::::::::::::::::::://

		override
		public bool NeedTraining( Config conf )
		{
			return !File.Exists( "barbedo_train_data" );
		}

		//::::::::::::::::::::::::::::::::::::::://

		override
		public void LoadTraining()
		{
			FileStream f = new FileStream( "barbedo_train_data", FileMode.Open );
			BinaryReader reader = new BinaryReader( f );

			// Genre Count
			uint genreCount = reader.ReadUInt32();

			// Genre Combinations Count
			uint genreCombinationCount = reader.ReadUInt32();

			// Feature Count
			uint nfeat = reader.ReadUInt32();

			data = uCreateTrainingData( genreCount, nfeat );

			// Genre Labels
			//writer.Write();

			float* vec_data = NativeMethods.Pointer.fgetmem( ( int ) nfeat );
			for( uint idx = 0; idx < genreCombinationCount; idx++ )
			{
				// FirstGenreIndex
				uint frsGenreIdx = reader.ReadUInt32();

				// Second GenreIndex
				uint sndGenreIdx = reader.ReadUInt32();

				uSetGenreIndex( data, idx, frsGenreIdx, sndGenreIdx );

				for( uint vec = 0; vec < 3; vec++ )
				{
					for( uint feat = 0; feat < nfeat; feat++ )
						vec_data[ feat ] = reader.ReadSingle();

					uSetVector( data, idx, 0, vec, vec_data, nfeat );
				}

				for( uint vec = 0; vec < 3; vec++ )
				{
					for( uint feat = 0; feat < nfeat; feat++ )
						vec_data[ feat ] = reader.ReadSingle();

					uSetVector( data, idx, 1, vec, vec_data, nfeat );
				}
			}

			reader.Close();
		}

		//::::::::::::::::::::::::::::::::::::::://

		override unsafe
		public void SaveTraining()
		{
			FileStream f = new FileStream( "barbedo_train_data", FileMode.CreateNew );
			BinaryWriter writer = new BinaryWriter( f );

			// Genre Count
			uint genreCount = uGetGenreCount( data );
			writer.Write( genreCount );

			// Genre Combinations Count
			uint combCount = uGetGenreCombinationCount( data );
			writer.Write( combCount );

			// Feature Count
			uint nfeat = uGetFeatureCount( data );
			writer.Write( nfeat );

			// Genre Labels
			//writer.Write();

			float* vec_data = NativeMethods.Pointer.fgetmem( 12 );
			for( uint idx = 0; idx < combCount; idx++ )
			{
				// FirstGenreIndex
				uint fGenreIdx = uGetFirstGenreIdx( data, idx );
				writer.Write( fGenreIdx );

				// Second GenreIndex
				uint sGenreIdx = uGetSecondGenreIdx( data, idx );
				writer.Write( sGenreIdx );

				for( uint vec = 0; vec < 3; vec++ )
				{
					uGetTrainingData( data, vec_data, idx, 0, vec );

					for( uint feat = 0; feat < nfeat; feat++ )
						writer.Write( vec_data[ feat ] );
				}

				for( uint vec = 0; vec < 3; vec++ )
				{
					uGetTrainingData( data, vec_data, idx, 1, vec );

					for( uint feat = 0; feat < nfeat; feat++ )
						writer.Write( vec_data[ feat ] );
				}
			}

			writer.Close();
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
		private int uClassify( ref FileData f, void* data );

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dtCol">
		/// A <see cref="DataCollection"/>
		/// </param>
		[DllImport( "MusiC.Extensions.Classifiers.uBarbedo.dll", EntryPoint = "Barbedo_Train" )]
		static extern unsafe
		private void* uTrain( ref DataCollection dtCol );

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
		private FileData* uCFilter( ref FileData f, uint nfeat );

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
		private DataCollection* uEFilter( ref DataCollection dtCol );

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tData"></param>
		/// <param name="vec_data"></param>
		/// <param name="index"></param>
		/// <param name="genre"></param>
		/// <param name="vec"></param>
		[DllImport( "MusiC.Extensions.Classifiers.uBarbedo.dll", EntryPoint = "Barbedo_GetTrainingData" )]
		extern static unsafe
		private void uGetTrainingData( void* tData, float* vec_data, uint index, uint genre, uint vec );

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tData"></param>
		/// <returns></returns>
		[DllImport( "MusiC.Extensions.Classifiers.uBarbedo.dll", EntryPoint = "Barbedo_GetGenreCount" )]
		extern static unsafe
		private uint uGetGenreCount( void* tData );

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tData"></param>
		/// <returns></returns>
		[DllImport( "MusiC.Extensions.Classifiers.uBarbedo.dll", EntryPoint = "Barbedo_GetGenreCombinationCount" )]
		extern static unsafe
		private uint uGetGenreCombinationCount( void* tData );

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tData"></param>
		[DllImport( "MusiC.Extensions.Classifiers.uBarbedo.dll", EntryPoint = "Barbedo_GetFeatureCount" )]
		extern static unsafe
		private uint uGetFeatureCount( void* tData );

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tData"></param>
		/// <param name="idx"></param>
		/// <returns></returns>
		[DllImport( "MusiC.Extensions.Classifiers.uBarbedo.dll", EntryPoint = "Barbedo_GetFirstGenreIdx" )]
		extern static unsafe
		private uint uGetFirstGenreIdx( void* tData, uint idx );

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tData"></param>
		/// <param name="idx"></param>
		/// <returns></returns>
		[DllImport( "MusiC.Extensions.Classifiers.uBarbedo.dll", EntryPoint = "Barbedo_GetSecondGenreIdx" )]
		extern static unsafe
		private uint uGetSecondGenreIdx( void* tData, uint idx );

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="genreCount"></param>
		/// <returns></returns>
		[DllImport( "MusiC.Extensions.Classifiers.uBarbedo.dll", EntryPoint = "Barbedo_CreateTrainingData" )]
		extern static unsafe
		private void* uCreateTrainingData( uint genreCount, uint nfeat );

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="index"></param>
		/// <param name="genreA"></param>
		/// <param name="genreB"></param>
		[DllImport( "MusiC.Extensions.Classifiers.uBarbedo.dll", EntryPoint = "Barbedo_SetGenreIndex" )]
		extern static unsafe
		private void uSetGenreIndex( void* data, uint index, uint genreA, uint genreB );

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="index"></param>
		/// <param name="genre"></param>
		/// <param name="vector"></param>
		/// <param name="vec_data"></param>
		/// <param name="size"></param>
		[DllImport( "MusiC.Extensions.Classifiers.uBarbedo.dll", EntryPoint = "Barbedo_SetVector" )]
		extern static unsafe
		private void uSetVector( void* data, uint index, uint genre, uint vector, float* vec_data, uint size );
	}
}