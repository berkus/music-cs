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
using System.Reflection;
using System.Xml;
using System.IO;

using MusiC.Exceptions;
using MusiC.Extensions;

namespace MusiC
{
	/// <summary>
	/// Base Configurator class.
	/// 
	/// Configurator Tasks:
	/// Assemble the algorithms
	/// Sets input paths and labels
	/// 
	/// </summary>
	/// <see cref="MusiC.Algorithms"/>
	abstract public class Configurator : Extension
	{
		private LinkedList<IAlgorithm> _algList = new LinkedList<IAlgorithm>();
		private LinkedList<ILabel> _trainList = new LinkedList<ILabel>();
		private LinkedList<String> _classifyList = new LinkedList<String>();
		
		static readonly 
		protected Intantiator New = new Intantiator();
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <value>
		/// 
		/// </value>
		protected class Intantiator
		{
			/// <value>
			/// 
			/// </value>
			public IAlgorithm Algorithm()
			{
				return new Algorithm();
			}
			
			/// <value>
			/// 
			/// </value>
			public ILabel Label()
			{
				return new TrainLabel();
			}
			
			/// <value>
			/// 
			/// </value>
			public IParamList ParamList()
			{
				return new ParamList();
			}
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <value>
		/// 
		/// </value>
		public LinkedList<IAlgorithm> AlgorithmList
		{
			get { return _algList; }
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <value>
		/// 
		/// </value>
		public LinkedList<ILabel> LabelList
		{
			get { return _trainList; }
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		protected Configurator()
		{
			Message(this.GetType().FullName + " ... [LOADED]");
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="algorithm">
		/// A <see cref="IAlgorithm"/>
		/// </param>
		protected void AddAlgorithm(IAlgorithm algorithm)
		{
			_algList.AddLast(algorithm);
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="label">
		/// A <see cref="ILabel"/>
		/// </param>
		protected void AddTrainLabel(ILabel label)
		{
			if(label.Validate())
				_trainList.AddLast(label);
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dir">
		/// A <see cref="String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		protected bool AddDir(String dir)
		{
			bool returnValue;
			
			if(returnValue=Directory.Exists(dir))
				_classifyList.AddLast(dir);
			
			return returnValue;
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="file">
		/// A <see cref="String"/>
		/// </param>
		abstract 
		public void Load(String file);
	}
}
