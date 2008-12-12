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

namespace MusiC
{
	public class Algorithm
	{
		bool _isInitialized=false;
		ExtensionInfo _classifierInfo = null;
		ExtensionInfo _windowInfo = null;
		LinkedList<ExtensionInfo> _featureListInfo = new LinkedList<ExtensionInfo>();
		
		Window _window = null;
		Classifier _classifier = null;
		LinkedList<Feature> _featureList = new LinkedList<Feature>();
		
		public void Add(ExtensionInfo info)
		{
			switch(info.Type)
			{
				case ExtensionType.Classifier:
					_classifierInfo=info;
					break;
				case ExtensionType.Window:
					_windowInfo=info;
					break;
				case ExtensionType.Feature:
					_featureListInfo.AddLast(info);
					break;
				default:
					break;
			}
		}
		
		public void Initialize()
		{
			if(_isInitialized)
				return;
			
			//@todo Throw exceptions while we dont have defaults
			//@todo Add default componenets
			
			if(_windowInfo == null)
				return;
			
			// classifier is not necessary ... maybe just extraction.
			//if(_classifierInfo == null)
			//	return;
			
			if(_featureListInfo.Count != 0)
				return;
			
			_window = Invoker.LoadType(_windowInfo) as Window;
			
			Feature f;
			foreach(ExtensionInfo i in _featureListInfo)
			{
				f = Invoker.LoadType(i) as Feature;
				
				if (f != null)
					_featureList.AddLast(f);
			}
			
			if(_classifierInfo != null)
				_classifier = Invoker.LoadType(_classifierInfo) as Classifier;
		}
	}
}