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
using System.Collections.Generic;

using MusiC.Data;

namespace MusiC
{
	/// <summary>
	/// Base class of Windows extensions implementation.
	/// </summary>
	/// @todo Allow different types.
	abstract public class Window : Extension
	{
		[CLSCompliant(false)]
		abstract unsafe public class UnmanagedImplementation : Window
		{
			/// Window Values
			Single * _wndData = null;
		
			/// Windowed Data
			Single * _dataStream = null;
			
			protected UnmanagedImplementation(String name, Int32 size, Int32 overlap) : base(name, size, overlap)
			{
				_wndData = NativeMethods.Pointer.dgetmem(size);
				Initialize();
			}
			
			void Initialize()
			{
				for (Int32 i = 0; i < _size; i++)
					_wndData[i] = Factory(i);
			}
			
			~UnmanagedImplementation()
			{
				if(_wndData != null)
					NativeMethods.Pointer.free(_wndData);
			}
			
			unsafe public Single * GetWindow(Int32 windowPos)
			{
				return _wndData;
			}
		}
		
		abstract public class ManagedImplementation : Window
		{
			/// Window Values
			Single[] _wndData;
		
			/// Windowed Data
			Single[] _dataStream;
			
			protected ManagedImplementation(String name, Int32 size, Int32 overlap) : base(name, size, overlap)
			{
				_wndData = new Single[size];
				
				Int16 i = 0;
				for(; i < size; i++)
					_wndData[i] = Factory(i);
			}
			
			void Initialize()
			{
				for (Int32 i = 0; i < _size; i++)
					_wndData[i] = Factory(i);
			}
			
			public Single[] GetWindow(Int32 windowPos)
			{
				return _dataStream;
			}
		}
		
		Int32 _size;
		Int32 _nWnd = -1;
		Int32 _overlap;
		Int32 _step;
		
		String _name;
	
		public String Name
		{
			get { return _name; }
		}
	
		public Int32 WindowCount
		{
			get { return _nWnd; }
		}
	
		public Int32 WindowSize
		{
			get { return _size; }
		}
	
		protected Window(String name, Int32 size, Int32 overlap)
		{
			_name = name;
			_size = size;
			_overlap = overlap;
			
			_step = size - overlap;
		}
		
		virtual public void Attach(Handler file)
		{
		}
		
		virtual public void Detach()
		{
		}
		
		virtual public Boolean IsAttached()
		{
			return false;
		}
		
		abstract public Single Factory(Int32 windowPos);
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
