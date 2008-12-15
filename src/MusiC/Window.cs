using System;
using System.Runtime.InteropServices;

using MCModule.UnmanagedInterface;

namespace MusiC
{
	abstract unsafe public class Window : Extension
	{
		/// @todo this shouldnt be protected. Add methods(or properties) for access.
		protected int m_size;
		int m_nWnd = -1;
		int m_overlap;
		int m_step;
		
		/// Window Values
		double* m_wndData = null;
		
		/// Windowed Sound Data
		double* m_dataStream = null;
	
		string m_name;
		public string Name
		{
			get { return m_name; }
		}
	
		public int WindowCount
		{
			get { return m_nWnd; }
		}
	
		public int WindowSize
		{
			get { return m_size; }
		}
		
		int m_bytesAllocated = 0;
	
		public Window(string name, int size, int overlap)
		{
		}
		
		private void Load()
		{
		}
		
		virtual public void Attach()
		{
		}
		
		virtual public void Detach()
		{
		}
		
		virtual public bool IsAttached()
		{
			return false;
		}
		
		/// @brief Returns a pointer to the first element representing the window.
		/// @details User must be sure to keep inside the window to avoid stack corruption.
		unsafe public double* GetWindow(int n)
		{
			return m_wndData;
		}
		
		virtual public void Dispose()
		{
		}
		
		abstract public double Factory(int n);
	}
}

//namespace MCModule
//{
///// @brief Basic window type.
//	/// @details To implement a new window you must extended this class.
//	abstract unsafe public class Window : Extension, IDisposable
//	{
//		/// @todo this shouldnt be protected. Add methods(or properties) for access.
//		protected int m_size;
//		int m_nWnd = -1;
//		int m_overlap;
//		int m_step;
//		
//		Handler mHandler = null;
//		
//		/// Window Values
//		double* m_wndData = null;
//		
//		/// Windowed Sound Data
//		double* m_dataStream = null;
//	
//		string m_name;
//		public string Name
//		{
//			get { return m_name; }
//		}
//	
//		public int WindowCount
//		{
//			get 
//			{ 
//				if(m_nWnd == -1 & mHandler != null) 
//				{
//					Load();
//					return m_nWnd;
//				}
//				else
//				{
//					if(mHandler == null)
//						throw new System.Exception("No handler is attached to this window");
//					else
//						return m_nWnd;
//				}
//			}
//		}
//	
//		public int WindowSize
//		{
//			get { return m_size; }
//		}
//		
//		public Handler FileHandler
//		{
//			get { return mHandler; }
//		}
//		
//		int m_bytesAllocated = 0;
//	
//		public Window(string name, int size, int overlap)
//		{
//			m_size = size;
//			m_step = size - overlap;
//			m_overlap = overlap;
//				
//			m_wndData = UnsafePtr.dgetmem(m_size);
//	
//			m_name = name;
//				
//			for (int i = 0; i < m_size; i++)
//				m_wndData[i] = Factory(i);
//	
//			//Console.WriteLine(name + ":" + size + ":" + overlap);
//		}
//		
//		private void Load()
//		{
//			m_nWnd = (Int32)Math.Floor((Convert.ToDouble(mHandler.Size - m_overlap) / Convert.ToDouble(m_size - m_overlap)));
//			
//			Print();
//			
//			m_bytesAllocated = m_size * m_nWnd * sizeof(double);
//			
//			//Console.WriteLine("Window Allocated: " + m_bytesAllocated + "B");			
//			//@todo check if it is already allocated
//			
//			double * p = m_dataStream = UnsafePtr.dgetmem(m_bytesAllocated);			
//			
//			int n = 0;
//			double * soundData = mHandler.GetData();
//			
//			for(int i = 0; i < m_nWnd; i++)
//			{				
//				for(int j = 0; j < 0; j++)
//				{
//					// Assign and move
//					*p++ = *(m_wndData + j) * *(soundData + n);
//					n++;
//				}
//				
//				// Go back to begin next window
//				n += m_step - m_size + 1;
//			}
//			
//			//UnsafePtr.free(soundData);
//		}
//		
//		virtual public void Attach(Handler h)
//		{
//			mHandler = h;
//		}
//		
//		virtual public void Detach()
//		{
//			mHandler.Detach();
//			mHandler = null;
//			m_nWnd = -1;
//		}
//		
//		virtual public bool IsAttached()
//		{
//			if(mHandler != null)
//				return true;
//			
//			return false;
//		}
//		
//		/// @brief Returns a pointer to the first element representing the window.
//		/// @details User must be sure to keep inside the window to avoid stack corruption.
//		unsafe public double* GetWindow(int n)
//		{	
//			if(mHandler == null)
//				return null;
//			else
//			{
//				if(m_dataStream == null)
//					Load();
//				
//				return (m_dataStream + n * m_size);
//			}
//		}
//		
//		void Print()
//		{
//			Console.WriteLine("Window Size: {0}", m_size);
//			Console.WriteLine("Overlap: {0}", m_overlap);
//			if(IsAttached())
//			{
//				Console.WriteLine("Attached to: {0}", mHandler.WorkingFile );
//				Console.WriteLine("Numeber of Windows: {0}", m_nWnd);
//			}
//		}
//		
//		virtual public void Dispose()
//		{
//			UnsafePtr.free(m_wndData);
//			UnsafePtr.free(m_dataStream);
//		}
//		
//		abstract public double Factory(int n);
//	}
//}