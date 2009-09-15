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
	abstract
	public class Configurator : Extension
	{
		private Config _currentConf;
		
		//::::::::::::::::::::::::::::::::::::::://

		public Config CurrentConfig
		{
			get { return _currentConf; }
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		protected Configurator()
		{
			Message( this.GetType().FullName + " ... [LOADED]" );
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="algorithm"></param>
		/// <returns></returns>
		protected bool AddAlgorithm( IAlgorithm algorithm )
		{
			return _currentConf.AddAlgorithm( algorithm );
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		protected bool AddTrainLabel( ILabel label )
		{
			return _currentConf.AddTrainLabel( label as Label );
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dir"></param>
		/// <param name="recursive"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		protected bool AddClassificationDir( string dir, bool recursive, string filter )
		{
			bool ret;

			if( !(ret = _currentConf.AddClassificationDir( dir, recursive, filter )) )
			{
				Warning( "A non-existent directory (" + dir + ") wasn't added to the processing queue." );
			}

			return ret;
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		protected bool AddClassificationFile( string file )
		{
			bool ret;

			if( !(ret = _currentConf.AddClassificationFile( file )) )
			{
				Warning( "A non-existent file (" + file + ") wasn't added to the processing queue." );
			}

			return ret;
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="file">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="Config"/>
		/// </returns>
		public Config LoadConfig( string file )
		{
			Config conf = new Config();
			_currentConf = conf;

			Load( file );

			_currentConf = null;

			return conf;
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected int GetAlgorithmCount()
		{
			return _currentConf.AlgorithmList.Count;
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="file">
		/// A <see cref="String"/>
		/// </param>
		abstract
		protected void Load( string file );

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="file">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		abstract
		public bool CanHandle( string file );
	}
}
