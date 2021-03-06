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
using System.Reflection;

namespace MusiC
{
	/// <summary>
	/// 
	/// </summary>
	public
	class Instantiable : ParamList
	{
		private string _classname;
		private string _strValue;

		private Type _class;
		private object _value;

		private bool _isInitiated = false;

		//::::::::::::::::::::::::::::::::::::::://

		//// <value>
		/// 
		/// </value>
		public Type TypeObj
		{
			get { return _class; }
			set { _class = value; _classname = value.FullName; }
		}

		//::::::::::::::::::::::::::::::::::::::://

		//// <value>
		/// 
		/// </value>
		public string Class
		{
			get { return _classname; }
			set { _class = Type.GetType( value, false, false ); _classname = value; }
		}

		//::::::::::::::::::::::::::::::::::::::://

		//// <value>
		/// 
		/// </value>
		public string StrValue
		{
			get { return _strValue; }
			set { _strValue = value; }
		}

		//::::::::::::::::::::::::::::::::::::::://

		//// <value>
		/// 
		/// </value>
		public object Value
		{
			get { return _value; }
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="paramName">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="paramClass">
		/// A <see cref="System.String"/>
		/// </param>
		public Instantiable( string paramClass )
		{
			_classname = paramClass;
			_class = Type.GetType( paramClass, false, false );
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// Creates a new instance of a class
		/// 
		/// @anchor constructor_param_order
		/// 
		/// @warning
		/// The parameters added by the Configurator must be in the same order of the target-type constructor declaration.
		/// This function uses the System.Type.GetConstructor(Type[]) method to select the constructor that
		/// should be invoked.
		///  
		/// </summary>
		override
		public void Instantiate()
		{
			if( _value != null || _isInitiated )
				return;

			///@todo throw exception
			if( _class == null || _classname == null )
				return;

			base.Instantiate();

			if( _strValue != null )
			{
				MethodInfo parse = _class.GetMethod( "Parse", new Type[] { typeof( String ) } );

				if( parse == null )
				{
					Warning( _classname + ".Parse wasn't found. This must be a static method. Using declared constructor." );
				}
				else
				{
					_value = parse.Invoke( null, new Object[] { StrValue } );
					return;
				}
			}

			// Find the constructor matching the types contained by the list. Order IS relevant.
			ConstructorInfo defaultCtor = _class.GetConstructor( GetTypes() );

			// Invoke the constructor.
			defaultCtor.Invoke( GetParamsValue() );
		}
	}
}