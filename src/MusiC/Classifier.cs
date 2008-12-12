using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using MCModule.UnmanagedInterface;
	
namespace MCModule
{
	/// @brief Basic classifier type.
	/// @details To implement a new classifier you must extended this class.
	/// @todo implement IDispose interface
	abstract public class Classifier : Extension
	{
		ICollection<ActionNode> m_trainList;
		ICollection<ActionNode> m_classifyList;
		
		LinkedList<String> m_classList = new LinkedList<String>();
		LinkedList<String> m_dirs = new LinkedList<String>();
		
		public ICollection<ActionNode> TrainList
		{
			set{ m_trainList = value; }
			get{ return m_trainList; }
		}
		
		public ICollection<ActionNode> ClassifyList
		{
			set{ m_classifyList = value; }
			get{ return m_classifyList; }
		}
		
		public Classifier(string type)
		{
			//Console.WriteLine(type);
		}
		
		public unsafe void Execute(System.Collections.Generic.ICollection<Feature> FeatList, Window wnd)
		{
			if(m_trainList.Count > 0)
			{
				foreach(ActionNode ac in m_trainList)
				{
					//Liat of existing classes
					m_classList.AddLast(ac.Name);
					m_dirs.AddLast(ac.Dir);
				}
				
				Console.WriteLine("Extracting ...");
				MCDataCollection * dtCol = Extractor.BatchExtract(FeatList, wnd, m_dirs);
				
				Console.WriteLine("Training ...");
				Train(dtCol);
			}
			else
			{
				TryLoadingParameters();
			}
			
			m_dirs.Clear();
			
			if(m_classifyList.Count > 0)
			{
				foreach(ActionNode ac in m_classifyList)
				{
					m_dirs.AddLast(ac.Dir);
				}
				
				Console.WriteLine("Classifying ...");
				Classify();
			}
		}
		
		LinkedList<String> GetDirs()
		{
			return m_dirs;
		}
		
		virtual unsafe public MCDataCollection * Filter(MCDataCollection * dataIn)
		{
			return dataIn;
		}
		
		abstract unsafe public void Train(MCDataCollection * dtCol);
		abstract public void Classify();		
		abstract public void TryLoadingParameters();
		
		
#region IDisposable
		abstract public void Dispose();
#endregion
	}
}

namespace MusiC
{
	abstract public class Classifier : Extension
	{
	}
}