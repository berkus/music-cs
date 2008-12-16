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

using MusiC;

using MCModule.UnmanagedInterface;

namespace MCModule
{
	/// @brief Basic feature type.
	/// @details To implement a new feature you must extended this class.
	/// @todo Implement IDispose interface
	abstract unsafe public class Feature : Extension
	{
		/// @todo this shouldnt be protected. Add methods(or properties) for access.
		/// @todo change m_data for another name
		protected double* m_data;
		/// @todo change m_temp for another name
		protected double* m_temp;

		int m_wndSize;
		int m_wndCount;
		
	
		string m_name;
		public string Name
		{
			get { return m_name; }
		}
	
		public Feature(string name)
		{
			m_name = name;
			Console.WriteLine(name);
		}
	
		public double * OutterExtract(Window wnd)
		{
			if(m_temp == null)
			{
				m_temp = UnsafePtr.dgetmem(wnd.WindowSize);
				m_wndSize = wnd.WindowSize;
			}
			
			if(m_data == null)
			{ 
				m_data = UnsafePtr.dgetmem(wnd.WindowCount);
				m_wndCount = wnd.WindowCount;
			}
			
			if(wnd.WindowSize > m_wndSize)
			{
				UnsafePtr.free(m_temp);
				m_temp = UnsafePtr.dgetmem(wnd.WindowSize);
			}
			
			if(wnd.WindowCount > m_wndCount)
			{
				UnsafePtr.free(m_data);
				m_data = UnsafePtr.dgetmem(wnd.WindowCount);
			}
					
			return Extract(wnd);
		}
		
		abstract public int FeatureSize(Window wnd);
		
		/// All frame data must be consecutive
		abstract public double * Extract(Window wnd);
		
		abstract public void Dispose();
	} 
}

namespace MusiC
{
	abstract unsafe public class Feature : Extension
	{
		public Feature()
		{
			Message("Feature ok");
		}
		
		/// @todo this shouldnt be protected. Add methods(or properties) for access.
		/// @todo change m_data for another name
		protected double* m_data;
		/// @todo change m_temp for another name
		protected double* m_temp;

		int m_wndSize;
		int m_wndCount;
		
		string m_name;
		public string Name
		{
			get { return m_name; }
		}
	
		public Feature(string name)
		{
			m_name = name;
			Message(name);
		}
	
		public double * OutterExtract(Window wnd)
		{	
			return Extract(wnd);
		}
		
		abstract public int FeatureSize(Window wnd);
		
		/// All frame data must be consecutive
		abstract public double * Extract(Window wnd);
		
		virtual public void Dispose()
		{
		}
	}
}