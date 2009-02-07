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
using System.Runtime.InteropServices;

using MusiC.Data;

namespace MusiC
{
	/// <summary>
	/// 
	/// </summary>
	public interface IClassifier
	{
	}
	
	//---------------------------------------//
	
	/// <summary>
	/// 
	/// </summary>
	public class BaseClassifier : Extension, IClassifier
	{
	}
	
	//---------------------------------------//
	
	namespace Managed
	{
		/// <summary>
		/// 
		/// </summary>
		abstract
		public class Classifier : BaseClassifier
		{
			/// <summary>
			/// 
			/// </summary>
			virtual
			public void Execute()
			{
				throw new NotImplementedException();
			}
			
			//::::::::::::::::::::::::::::::::::::::://
			
			/// <summary>
			/// 
			/// </summary>
			virtual
			public void Classify()
			{
				throw new NotImplementedException();
			}
			
			//::::::::::::::::::::::::::::::::::::::://
			
			/// <summary>
			/// 
			/// </summary>
			virtual
			public void TryLoadingParameters()
			{
				throw new NotImplementedException();
			}
			
			//::::::::::::::::::::::::::::::::::::::://
			
			/// <summary>
			/// 
			/// </summary>
			virtual
			public void Dispose()
			{
				throw new NotImplementedException();
			}
		}
	}
	
	//---------------------------------------//
	
	namespace Unmanaged
	{
		/// <summary>
		/// 
		/// </summary>
		[CLSCompliant(false)]
		abstract
		public class Classifier : BaseClassifier
		{
			/// <summary>
			/// 
			/// </summary>
			/// <param name="dataIn">
			/// A <see cref="Data.Unmanaged.DataCollection"/>
			/// </param>
			/// <returns>
			/// A <see cref="Data.Unmanaged.DataCollection"/>
			/// </returns>
			virtual unsafe
			public Data.Unmanaged.DataCollection * Filter(Data.Unmanaged.DataCollection * dataIn)
			{
				return null;
			}
			
			//::::::::::::::::::::::::::::::::::::::://
			
			/// <summary>
			/// 
			/// </summary>
			/// <param name="dtCol">
			/// A <see cref="Data.Unmanaged.DataCollection"/>
			/// </param>
			abstract unsafe
			public void Train(Data.Unmanaged.DataCollection* dtCol);

			//::::::::::::::::::::::::::::::::::::::://

			//abstract
			//public int Classify();
		}
	}
}