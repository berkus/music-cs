// Copyright (c) 2008-2009 Marcos José Sant'Anna Magalhães
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.IO;
using System.Collections.Generic;

namespace MusiC
{

	/// <summary>
	/// 
	/// </summary>
	public class Config
	{
		private LinkedList<IAlgorithm> _algList = new LinkedList<IAlgorithm>();
		private LinkedList<Label> _trainList = new LinkedList<Label>();
		private Label _classify = new Label();

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
		public LinkedList<Label> LabelList
		{
			get { return _trainList; }
		}
		//::::::::::::::::::::::::::::::::::::::://

		//// <value>
		/// 
		/// </value>
		public Label Classify
		{
			get { return _classify; }
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="algorithm">
		/// A <see cref="IAlgorithm"/>
		/// </param>
		public void AddAlgorithm( IAlgorithm algorithm )
		{
			_algList.AddLast( algorithm );
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="label">
		/// A <see cref="ILabel"/>
		/// </param>
		public void AddTrainLabel( Label label )
		{
			if( label.Validate() )
				_trainList.AddLast( label );
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dir"></param>
		/// <param name="recursive"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public bool AddClassificationDir( string dir, bool recursive, string filter )
		{
			return _classify.AddInputDir( dir, recursive, filter );
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public bool AddClassificationFile( string file )
		{
			return _classify.AddInputFile( file );
		}

		//::::::::::::::::::::::::::::::::::::::://

		public Label GetLabel( int n )
		{
			int count = -1;

			foreach( Label l in _trainList )
			{
				count++;

				if( count == n )
					return l;
				else
					continue;
			}

			return null;
		}
	}
}