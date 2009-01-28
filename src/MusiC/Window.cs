/*
 * The MIT License
 * Copyright (c) 2008-2009 Marcos Jos� Sant'Anna Magalh�es
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
using System.Collections.Generic;

using MusiC.Data;

namespace MusiC
{
	public interface IWindow
	{
		void Attach(IHandler hnd);
		void Detach();
	}
	
	/// <summary>
	/// Base class of Windows extensions implementation.
	/// </summary>
	/// @todo Allow different types.
	abstract
	public class BaseWindow : Extension, IWindow
	{
		private int _nWnd = -1;
		private int _size;
		private int _overlap;
		
		private IHandler _hnd;
	
		public int WindowCount
		{
			get { return _nWnd; }
		}
	
		public int WindowSize
		{
			get { return _size; }
		}

		public int WindowOverlap
		{
			get { return _overlap; }
		}
		
		public IHandler HandlerInterface
		{
			get { return _hnd; }
		}
	
		protected BaseWindow(int size, int overlap)
		{
			_size = size;
			_overlap = overlap;
		}
		
		virtual
		public void Attach(IHandler file)
		{
			_hnd = file;
			_nWnd = (int) Math.Floor( (double) (_hnd.GetStreamSize() / _size) );
		}
		
		virtual
		public void Detach()
		{
			_hnd = null;
			_nWnd = -1;
		}
		
		abstract
		public Single Factory(int windowPos);
	}
	
	namespace Unmanaged
	{
		[CLSCompliant(false)]
		abstract unsafe
		public class Window : BaseWindow
		{
			/// Window Values
			Single * _wndData = null;
		
			/// File Handler Buffer
			Single * _rawStream = null;

            /// Windowed Data
            Single* _dataStream = null;
			
			protected Handler FileHandler
			{
				get { return HandlerInterface as Unmanaged.Handler; }
			}
			
			protected Window(int size, int overlap) : base(size, overlap)
			{
				_wndData = NativeMethods.Pointer.fgetmem(size);
                _dataStream = NativeMethods.Pointer.fgetmem(size);

				for (int i = 0; i < WindowSize; i++)
					_wndData[i] = Factory(i);
			}
			
			/// <summary>
			/// Calculates interaction between window data and file data.
			/// 
			/// By default it implements the multiplication of the vectors.
			/// 
			/// @warning
			/// It is allowed to modify the the value of the pointer (wndData++) but the value it contains(*wndData = 0) shouldn't be modified.
			/// This same pointer might be used again later. This is true for both wndData and fileData. I am sure you will want to change the value that
			/// result holds.
			///  
			/// </summary>
			/// <param name="wndData">
			/// A float-pointer to the window data. 
			/// </param>
			/// <param name="fileData">
			/// A float-pointer to the current file chunk data.
			/// </param>
			/// <param name="result">
			/// A float-pointer to hold the result.
			/// </param>
			virtual protected void Calculate(Single * wndData, Single * fileData, Single * result)
			{
				for(int i = 0; i < WindowSize; i++)
					*(result++) = *(fileData++) * *(wndData++);
			}
			
			~Window()
			{
				if(_wndData != null)
					NativeMethods.Pointer.free(_wndData);

                if (_dataStream != null)
                    NativeMethods.Pointer.free(_dataStream);
			}
			
			unsafe
			public Single * GetWindow(int windowPos)
			{
				_rawStream = FileHandler.Read(windowPos * (WindowSize - WindowOverlap), WindowSize);

                if (_rawStream == null)
                    return _rawStream;

				Single * ptrRawStream = _rawStream;
                Single * ptrDataStream = _dataStream;
				Single * ptrWnd = _wndData;
				
				Calculate(ptrWnd, ptrRawStream, ptrDataStream);
				
				return _dataStream;
			}
		}
	}
	
	namespace Managed
	{
		abstract
		public class Window : BaseWindow
		{
			/// Window Coeficients
			Single[] _wndData;
		
			/// Windowed Data
			//Single[] _dataStream;
			
			protected Window(int size, int overlap) : base(size, overlap)
			{
				_wndData = new Single[size];
				
				for (int i = 0; i < WindowSize; i++)
					_wndData[i] = Factory(i);
			}
			
			public Single[] GetWindow(int windowPos)
			{
				//return _dataStream;
				return null;
			}
		}
	}
}
