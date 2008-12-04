using System;
using System.Collections.Generic;
using System.Text;

namespace MCModule
{
	public class Algorithm : IDisposable
	{
		LinkedList<Feature> m_FeatList = new LinkedList<Feature>();
		public ICollection<Feature> FeatList
		{
			get { return m_FeatList; }
		}
	
		public void AddFeature(Feature f)
		{
			m_FeatList.AddLast(f);
		}
	
		Window m_Wnd = null;
		public Window STFTWindow
		{
			get { return m_Wnd; }
			set { if (m_Wnd != null) { throw new System.Exception("A Window was already set"); } m_Wnd = value; }
		}
	
		Classifier m_Class = null;
		public Classifier AlgClassifier
		{
			get { return m_Class; }
			set { if (m_Class != null) { throw new System.Exception("A Classifier was already set"); } m_Class = value; }
		}
	
		///@todo Dispose other elements
		public void Dispose()
		{
			m_Wnd.Dispose();
		}
	}
}
