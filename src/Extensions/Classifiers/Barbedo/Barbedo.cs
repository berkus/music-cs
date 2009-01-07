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
using System.Runtime.InteropServices;

using MusiC;
using MusiC.Data.Unmanaged;

namespace MusiC.Extensions.Classifiers
{
	public class Barbedo : Unmanaged.Classifier
	{
		public Barbedo()// : base("Barbedo")
		{
		}

        override unsafe public void Train(DataCollection* dtCol)
		{
			uTrain(ref (*dtCol));
		}

        override unsafe public void Filter(DataCollection* dataIn)
		{
			uFilter(ref *dataIn);
		}
		
		
		[DllImport("./MusiC.Extensions.Classifiers.uBarbedo.dll", EntryPoint="Barbedo_Train")]
		static extern public void uTrain(ref DataCollection dtCol);
		
		[DllImport("./MusiC.Extensions.Classifiers.uBarbedo.dll", EntryPoint="Barbedo_Filter")]
		static extern unsafe public DataCollection * uFilter(ref DataCollection dtCol);
    }
}

//namespace MCModule.Classifiers
//{
//	public class Barbedo : MCModule.Classifier
//	{
//		public Barbedo() : base("Barbedo")
//		{
//		}
//		
//		override unsafe public void Train(MCDataCollection * dtCol)
//		{
//			IntPtr ptr = new IntPtr(dtCol);
//			Console.WriteLine("Address:"+ptr.ToInt64());
//			//uTrain(ref (*dtCol));
//		}
//		
//		override unsafe public MCDataCollection * Filter(MCDataCollection * dataIn)
//		{
//			return uFilter(ref *dataIn);
//		}
//		
//		override public void Classify()
//		{
//		}
//		
//		override public void TryLoadingParameters()
//		{
//		}
//		
//		override public void Dispose()
//		{
//		}
//		
//		[DllImport("./musiC-uMng.dll", EntryPoint="Barbedo_Train")]
//		static extern public void uTrain(ref MCDataCollection dtCol);
//		
//		[DllImport("./musiC-uMng.dll", EntryPoint="Barbedo_Filter")]
//		static extern unsafe public MCDataCollection * uFilter(ref MCDataCollection dtCol);
//	}
//}