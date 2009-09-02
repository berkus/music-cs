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
using MusiC.Exceptions;

namespace MusiC
{

	/// <summary>
	/// 
	/// </summary>
	public class Config : MusiCObject
	{
		private LinkedList<IAlgorithm> _algList = new LinkedList<IAlgorithm>();
		private LinkedList<Label> _trainList = new LinkedList<Label>();

		private Label _classify = new Label();

		private Dictionary<Option, string> _opt = new Dictionary<Option, string>();

		//::::::::::::::::::::::::::::::::::::::://

		public enum Option
		{
			ALLOW_SAVE,
			FORCE_EXTRACT,
			FORCE_TRAINING
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
		/// <param name="algorithm"></param>
		/// <returns></returns>
		public bool AddAlgorithm( IAlgorithm algorithm )
		{
			bool ret = true;

			try
			{
				_algList.AddLast( algorithm );
			}
			catch( Exception e )
			{
				Error( e );
				ret = false;
			}

			return ret;
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public bool AddTrainLabel( Label label )
		{
			bool ret;

			if( ret = label.Validate() )
				_trainList.AddLast( label );

			return ret;
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

		/// <summary>
		/// Makes an option available.
		/// </summary>
		/// <param name="id"> A name to identify the option </param>
		/// <param name="value"> Option value </param>
		/// <returns> Success flag </returns>
		public bool AddOption( Option id, string value )
		{
			if( value == null )
				return false;

			bool ret = true;

			try
			{
				_opt.Add( id, value.ToUpper() );
			}
			catch( Exception e )
			{
				Error( e );
				ret = false;
			}

			return ret;
		}

		//::::::::::::::::::::::::::::::::::::::://

		public bool AddOption( string id, string value )
		{
			if( id == null || value == null )
				return false;

			bool ret = true;

			try
			{
				Option opt = ( Option ) Enum.Parse( typeof( Option ), id.ToUpper() );
				ret = AddOption( opt, value.ToUpper() );
			}
			catch
			{
				MCException ex = new MCException("Unrecognized option name: " + id );
			}

			return ret;
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// Try to get an option as a string. All options, if available, succeed to this call.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool GetOption( Option id, out string value )
		{
			return _opt.TryGetValue( id, out value );
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// Try to get an option as a boolean. Depends on successful convertion.
		/// </summary>
		/// <param name="id"> The option name </param>
		/// <param name="value"> [OUT] The boolean returned </param>
		/// <returns> Success flag </returns>
		public bool GetBoolOption( Option id, out bool value )
		{
			string val;

			if( !GetOption( id, out val ) )
			{
				value = false;
				return false;
			}

			return Boolean.TryParse( val, out value );
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// Try to get an option as an integer. Depends on successful convertion.
		/// </summary>
		/// <param name="id"> The option name </param>
		/// <param name="value"> [OUT] The integer returned </param>
		/// <returns> Success flag </returns>
		public bool GetIntOption( Option id, out int value )
		{
			string val;

			if( !GetOption( id, out val ) )
			{
				value = 0;
				return false;
			}

			return Int32.TryParse( val, out value );
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="n"></param>
		/// <returns></returns>
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
