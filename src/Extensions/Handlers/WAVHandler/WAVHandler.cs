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

using MusiC.Exceptions;

namespace MusiC.Extensions.Handlers
{
	/// <summary>
	/// 
	/// </summary>
	unsafe
	public class WavHandler : Unmanaged.Handler
	{
		private float* _data = null;

		// total audio data, loaded audio data, bytes per sample, channels
		private int _streamSz, _dataSz, _bytesInUse, _channels, _samplesPerChannel;
		private long _offset;
		private BinaryReader rd;

		//::::::::::::::::::::::::::::::::::::::://

		~WavHandler()
		{
			NativeMethods.Pointer.free( _data );
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="file">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		override
		public bool CanHandle( string file )
		{
			///@todo This should check if the file is uncompressed
			return Path.GetExtension( file ).ToUpper() == ".WAV";
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// Attaches the handler to an existing file.
		/// </summary>
		/// <param name="file"></param>
		override
		public void Attach( string file )
		{
			base.Attach( file );
			Load();
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// Dettaches the handler from a file.
		/// </summary>
		override
		public void Detach()
		{
			rd.Close();
			rd = null;

			base.Detach();
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// </returns>
		override
		public int GetStreamSize()
		{
			return _samplesPerChannel;
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="firstSample">
		/// A <see cref="System.Int64"/>
		/// </param>d
		/// <param name="windowSize">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Single"/>
		/// </returns>
		override unsafe
		public System.Single* Read( long firstSample, int windowSize )
		{
			long firstByte = _offset + ( firstSample * _bytesInUse * _channels );

			rd.BaseStream.Seek( firstByte, SeekOrigin.Begin );
			Byte[] raw_data = rd.ReadBytes( windowSize * _bytesInUse * _channels );

			if( _dataSz < windowSize )
			{
				if( _data != null )
					NativeMethods.Pointer.free( _data );

				_data = NativeMethods.Pointer.fgetmem( windowSize );
				_dataSz = windowSize;
			}

			if( raw_data.Length < windowSize )
				return null;

			short i = 0;
			long count = 0;
			short c;

			float norm_factor = ( float ) Math.Pow( 2, ( 8 * _bytesInUse ) - 1 );
			//float norm_factor = 1;
			unsafe
			{
				float* pData = _data;
				long temp;

				fixed( byte* pB = raw_data )
				{
					byte* bytePt = pB; //can't assign to pB
					byte* sampleBytePtr = ( byte* ) &temp;

					for( ; count < windowSize; count++ )
					{
						*( pData ) = 0;

						for( c = 0; c < _channels; c++ )
						{
							// if it is a negative sample set temp to -1
							// to make the value correct.
							// if MSB > 128 ---> [[LSB]...[MSB]], ..., [[LSB]...[MSB]]
							//              bitPt ^
							temp = ( *( bytePt + _bytesInUse - 1 ) > 128 ) ? -1 : 0;

							// temp = current sample
							for( i = 0; i < _bytesInUse; i++ )
								*( sampleBytePtr + i ) = *( bytePt + i );

							// next sample
							bytePt += _bytesInUse;

							// Increases the number of divisions but avoid overflow problems
							// Makes data mono
							// long / int = int .... need to cast to float.
							*pData += ( float ) temp / _channels;
						}
						*pData /= norm_factor;
						pData++;
					}
				}
			}

			return _data;
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		protected void Load()
		{
			rd = new BinaryReader( new FileStream( CurrentFile, FileMode.Open ) );

			// RIFF
			rd.ReadChars( 4 );
			rd.ReadInt32();

			// WAVE
			rd.ReadChars( 4 );

			// 'fmt	'
			rd.ReadChars( 4 );
			rd.ReadInt32();

			//m_info.Compression = rd.ReadInt16();
			rd.ReadInt16();

			//Channels
			rd.ReadInt16();

			//m_info.SampleRate = rd.ReadInt32();
			rd.ReadInt32();

			//m_info.ByteRate = rd.ReadInt32();
			rd.ReadInt32();

			//m_info.BlockSize = rd.ReadInt16();
			int blockSize = rd.ReadInt16();

			//m_info.Depth = rd.ReadInt16();
			int sampleSize = rd.ReadInt16();

			///	@todo Handle compressed wave

			// DATA
			rd.ReadChars( 4 );

			_streamSz = rd.ReadInt32();

			// m_info.Samples = dataSz / m_info.BlockSize;
			//int bytesInUse = m_info.DepthInBytes = Convert.ToInt16(m_info.Depth / 8);

			_bytesInUse = sampleSize / 8;
			_channels = blockSize / _bytesInUse;

			//int samplesPerChannel = Convert.ToInt32(dataSz / (m_info.Channels * bytesInUse));
			_samplesPerChannel = Convert.ToInt32( _streamSz / blockSize );

			_offset = rd.BaseStream.Position;

			//Message(CurrentFile);
			Message( _channels + " channels with " + _samplesPerChannel + " samples." );
			Message( "Each sample has " + sampleSize + " bits making " + _streamSz + " bytes" );

			if( _samplesPerChannel <= 0 )
				throw new MCException("File stream is null");
		}
	}
}