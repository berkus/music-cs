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