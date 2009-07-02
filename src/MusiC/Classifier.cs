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
		bool NeedTraining( Config cfg );
		void LoadTraining();
		void SaveTraining();
	}

	//---------------------------------------//

	/// <summary>
	/// 
	/// </summary>
	abstract
	public class BaseClassifier : Extension, IClassifier
	{
		virtual
		public bool NeedTraining( Config cfg )
		{
			return true;
		}

		abstract
		public void LoadTraining();

		abstract
		public void SaveTraining();
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
			public int Classify()
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
		[CLSCompliant( false )]
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
			public Data.Unmanaged.DataCollection* TrainingFilter( Data.Unmanaged.DataCollection* dtCol )
			{
				return null;
			}

			//::::::::::::::::::::::::::::::::::::::://

			virtual unsafe
			public Data.Unmanaged.FileData* ClassificationFilter( Data.Unmanaged.FileData* fileDt, uint nfeat )
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
			public void Train( Data.Unmanaged.DataCollection* dtCol );

			//::::::::::::::::::::::::::::::::::::::://

			abstract unsafe
			public int Classify( Data.Unmanaged.FileData* fd );
		}
	}
}