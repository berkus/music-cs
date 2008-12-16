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
using System.IO;

using MCModule.UnmanagedInterface;

namespace MCModule.FileHandlers
{
	unsafe public class WavHandler : Handler
	{	
		//double * mData = null;
		
		override protected void InnerAttach(string file)
		{
		}
		
		override protected void InnerDetach()
		{
		}
		
		override protected void Load()
		{
			BinaryReader rd = new BinaryReader(new FileStream(WorkingFile, FileMode.Open));
	
			// RIFF
			rd.ReadChars(4);
			rd.ReadInt32();
	
			// WAVE
			rd.ReadChars(4);
	
			// 'fmt	'
			rd.ReadChars(4);
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
	
			///	@todo add support to compressed	wave
	
			// DATA
			rd.ReadChars(4);
			Int32 dataSz;
			//dataSz = m_info.DataSize = rd.ReadInt32();
			dataSz = rd.ReadInt32();
			Console.WriteLine("Has {0} cbytes in total.", dataSz);
			// m_info.Samples = dataSz / m_info.BlockSize;
	
			//int bytesInUse = m_info.DepthInBytes = Convert.ToInt16(m_info.Depth / 8);
			int bytesInUse = sampleSize / 8;
			Console.WriteLine("Each sample has {0} bits.", sampleSize);
			Console.WriteLine("Each sample has {0} bytes.", bytesInUse);
			
			int channels = blockSize / bytesInUse;
			Console.WriteLine("Has {0} channels.", channels);
			//int samplesPerChannel = m_info.SamplesPerChannel = Convert.ToInt32(dataSz / (m_info.Channels * bytesInUse));
			Int32 samplesPerChannel = Convert.ToInt32(dataSz / blockSize);
			Byte[] raw_data = rd.ReadBytes(dataSz);
			
			///@todo Add size check
			///@todo migrate this check to the base
			//if (mData != null)
			//	UnsafePtr.free(mData);
			
			///this function needs to give the data size
			SetSize(samplesPerChannel);
			double * mData = UnsafePtr.dgetmem(samplesPerChannel);
			SetDataArea(mData);
			
			short i = 0;
			long count = 0;
			short c;
	
			unsafe
			{
				double* pData = mData;
				Int64 temp;
	
				fixed (Byte* pB = raw_data)
				{
					Byte* bitPt = pB; //can't assign to pB
	
					for (; count < samplesPerChannel; count++)
					{
						*(pData) = 0;
	
						for (c = 0; c < channels; c++)
						{
							byte* m = (byte*)&temp;
							// if MSB > 128
							temp = (*(bitPt + bytesInUse - 1) > 128) ? -1 : 0;
	
							for (i = 0; i < bytesInUse; i++)
							{
								*(m + i) = *(bitPt + i);
							}
	
							bitPt += bytesInUse;
							// Increases the number of divisions but avoid overflow problems
							// Makes data mono
							*pData += temp / channels;
						}
	
						pData++;
					}
				}
			}
	
			rd.Close();
		}

		
		override public void Dispose ()
		{
			throw new NotImplementedException ();
		}

	}
}
