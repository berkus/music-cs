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
using System.Runtime.InteropServices;

using MusiC.Data;

namespace MusiC
{
	abstract public class Classifier : Extension
	{
		LinkedList<String> m_classList = new LinkedList<String>();
		LinkedList<String> m_dirs = new LinkedList<String>();
		
		public Classifier(string type)
		{
		}
		
		public unsafe void Execute(System.Collections.Generic.ICollection<Feature> FeatList, Window wnd)
		{
		}
		
		virtual unsafe public MCDataCollection * Filter(MCDataCollection * dataIn)
		{
			return dataIn;
		}
		
		abstract unsafe public void Train(MCDataCollection * dtCol);
		abstract public void Classify();		
		abstract public void TryLoadingParameters();
		abstract public void Dispose();
	}
}

//namespace MCModule
//{
//	/// @brief Basic classifier type.
//	/// @details To implement a new classifier you must extended this class.
//	/// @todo implement IDispose interface
//	abstract public class Classifier : Extension
//	{
//		ICollection<ActionNode> m_trainList;
//		ICollection<ActionNode> m_classifyList;
//		
//		LinkedList<String> m_classList = new LinkedList<String>();
//		LinkedList<String> m_dirs = new LinkedList<String>();
//		
//		public ICollection<ActionNode> TrainList
//		{
//			set{ m_trainList = value; }
//			get{ return m_trainList; }
//		}
//		
//		public ICollection<ActionNode> ClassifyList
//		{
//			set{ m_classifyList = value; }
//			get{ return m_classifyList; }
//		}
//		
//		public Classifier(string type)
//		{
//			//Console.WriteLine(type);
//		}
//		
//		public unsafe void Execute(System.Collections.Generic.ICollection<Feature> FeatList, Window wnd)
//		{
//			if(m_trainList.Count > 0)
//			{
//				foreach(ActionNode ac in m_trainList)
//				{
//					//Liat of existing classes
//					m_classList.AddLast(ac.Name);
//					m_dirs.AddLast(ac.Dir);
//				}
//				
//				Console.WriteLine("Extracting ...");
//				MCDataCollection * dtCol = Extractor.BatchExtract(FeatList, wnd, m_dirs);
//				
//				Console.WriteLine("Training ...");
//				Train(dtCol);
//			}
//			else
//			{
//				TryLoadingParameters();
//			}
//			
//			m_dirs.Clear();
//			
//			if(m_classifyList.Count > 0)
//			{
//				foreach(ActionNode ac in m_classifyList)
//				{
//					m_dirs.AddLast(ac.Dir);
//				}
//				
//				Console.WriteLine("Classifying ...");
//				Classify();
//			}
//		}
//		
//		LinkedList<String> GetDirs()
//		{
//			return m_dirs;
//		}
//		
//		virtual unsafe public MCDataCollection * Filter(MCDataCollection * dataIn)
//		{
//			return dataIn;
//		}
//		
//		abstract unsafe public void Train(MCDataCollection * dtCol);
//		abstract public void Classify();		
//		abstract public void TryLoadingParameters();
//		
//		
//#region IDisposable
//		abstract public void Dispose();
//#endregion
//	}
//}