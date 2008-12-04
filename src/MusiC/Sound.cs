using System;
using System.IO;

using MCModule.UnmanagedInterface;
	
namespace MCModule
{
	public unsafe class Sound : IDisposable
	{
		String m_filename;
		public String FileName
		{
			get { return m_filename; }
		}
	
		SoundInfo m_info;
	
		double* m_data;
		
		void ReadHeader(String file)
		{
			if (Path.GetExtension(file) == ".mp3")
			{
				///	@todo add code for mp3 files
			}
	
			if (Path.GetExtension(file) == ".wav")
			{
				WaveRead_Header(file);
			}
		}
		
		void Read(String file)
		{
			if (Path.GetExtension(file) == ".mp3")
			{
				///	@todo add code for mp3 files
			}
	
			if (Path.GetExtension(file) == ".wav")
			{
				WaveRead(file);
			}
		}
		
#region WAVE SUPPORT
		///	@note http://ccrma.stanford.edu/courses/422/projects/WaveFormat/
		void WaveRead_Header(string file)
		{
			BinaryReader rd = new BinaryReader(new FileStream(file, FileMode.Open));
	
			// RIFF
			rd.ReadChars(4);
			rd.ReadInt32();
	
			// WAVE
			rd.ReadChars(4);
	
			// 'fmt	'
			rd.ReadChars(4);
			rd.ReadInt32();
	
			m_info.Compression = rd.ReadInt16();
			m_info.Channels = rd.ReadInt16();
			m_info.SampleRate = rd.ReadInt32();
			m_info.ByteRate = rd.ReadInt32();
			m_info.BlockSize = rd.ReadInt16();
			m_info.Depth = rd.ReadInt16();
	
			///	@todo add support to compressed	wave
	
			// DATA
			rd.ReadChars(4);
			Int32 dataSz;
			dataSz = m_info.DataSize = rd.ReadInt32();
			m_info.Samples = dataSz / m_info.BlockSize;
	
			int bytesInUse = m_info.DepthInBytes = Convert.ToInt16(m_info.Depth / 8);
			m_info.SamplesPerChannel = Convert.ToInt32(dataSz / (m_info.Channels * bytesInUse));
			rd.Close();
		}
		
		/// @todo Avoid code repetition using WaveRead_Header
		void WaveRead(String file)
		{
			BinaryReader rd = new BinaryReader(new FileStream(file, FileMode.Open));
	
			// RIFF
			rd.ReadChars(4);
			rd.ReadInt32();
	
			// WAVE
			rd.ReadChars(4);
	
			// 'fmt	'
			rd.ReadChars(4);
			rd.ReadInt32();
	
			m_info.Compression = rd.ReadInt16();
			m_info.Channels = rd.ReadInt16();
			m_info.SampleRate = rd.ReadInt32();
			m_info.ByteRate = rd.ReadInt32();
			m_info.BlockSize = rd.ReadInt16();
			m_info.Depth = rd.ReadInt16();
	
			///	@todo add support to compressed	wave
	
			// DATA
			rd.ReadChars(4);
			Int32 dataSz;
			dataSz = m_info.DataSize = rd.ReadInt32();
			// m_info.Samples = dataSz / m_info.BlockSize;
	
			int bytesInUse = m_info.DepthInBytes = Convert.ToInt16(m_info.Depth / 8);
			Int32 samplesPerChannel = m_info.SamplesPerChannel = Convert.ToInt32(dataSz / (m_info.Channels * bytesInUse));
	
			Byte[] raw_data = rd.ReadBytes(dataSz);
	
			if (m_data != null)
				UnsafePtr.free(m_data);
			
			m_data = UnsafePtr.dgetmem(m_info.SamplesPerChannel);
	
			short i = 0;
			long count = 0;
			short c;
	
			unsafe
			{
				double* pData = m_data;
				Int64 temp;
	
				fixed (Byte* pB = raw_data)
				{
					Byte* bitPt = pB; //can't assign to pB
	
					for (; count < samplesPerChannel; count++)
					{
						*(pData) = 0;
	
						for (c = 0; c < m_info.Channels; c++)
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
							*pData += temp / m_info.Channels;
						}
	
						pData++;
					}
				}
			}
	
			rd.Close();
		}
#endregion
	
		public Int64 Size
		{
			get 
			{ //return m_info.Samples;
				return m_info.SamplesPerChannel;
			}
		}
	
		public Sound(String file)
		{
			m_filename = file;
			m_info = new SoundInfo();
			ReadHeader(file);
		}
		
		public void SetSound(String file)
		{
			m_filename = file;
			PurgeData();
			ReadHeader(file);
		}
		
		public unsafe double* GetPointerTo(long n)
		{
			if (m_data == null)
				Read(m_filename);
			
			return (m_data + n);
		}
		
		public void PurgeData()
		{
			if(m_data != null)
			{
				UnsafePtr.free(m_data);
				m_data = null;
			}
		}
	
		public void Dispose()
		{
			PurgeData();
		}
	}
	
	public class SoundInfo
	{
		Int16 m_compression = -1;
		public Int16 Compression
		{
			get { return m_compression; }
			set { m_compression = value; }
		}
	
		Int16 m_channels = -1;
		public Int16 Channels
		{
			get { return m_channels; }
			set { m_channels = value; }
		}
	
		Int32 m_sampleRate = -1;
		public Int32 SampleRate
		{
			get { return m_sampleRate; }
			set { m_sampleRate = value; }
		}
	
		Int32 m_byteRate = -1;
		public Int32 ByteRate
		{
			get { return m_byteRate; }
			set { m_byteRate = value; }
		}
	
		Int16 m_blockSize = -1;
		public Int16 BlockSize
		{
			get { return m_blockSize; }
			set { m_blockSize = value; }
		}
	
		Int16 m_depth = -1;
		public Int16 Depth
		{
			get { return m_depth; }
			set { m_depth = value; }
		}
	
		Int32 m_dataSize = -1;
		public Int32 DataSize
		{
			get { return m_dataSize; }
			set { m_dataSize = value; }
		}
	
		// Non Standard	wave info
		Int16 m_bytes = -1;
		public Int16 DepthInBytes
		{
			get { return m_bytes; }
			set { m_bytes = value; }
		}
	
		Int32 m_samples = -1;
		public Int32 Samples
		{
			get { return m_samples; }
			set { m_samples = value; }
		}
	
		Int32 m_samplesPerChannel = -1;
		public Int32 SamplesPerChannel
		{
			get { return m_samplesPerChannel; }
			set { m_samplesPerChannel = value; }
		}
	}
}
