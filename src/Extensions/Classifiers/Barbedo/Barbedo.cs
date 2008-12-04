using System;
using System.Runtime.InteropServices;

using MCModule.UnmanagedInterface;

namespace MCModule.Classifiers
{
	public class Barbedo : MCModule.Classifier
	{
		public Barbedo() : base("Barbedo")
		{
		}
		
		override unsafe public void Train(MCDataCollection * dtCol)
		{
			IntPtr ptr = new IntPtr(dtCol);
			Console.WriteLine("Address:"+ptr.ToInt64());
			//uTrain(ref (*dtCol));
		}
		
		override unsafe public MCDataCollection * Filter(MCDataCollection * dataIn)
		{
			return uFilter(ref *dataIn);
		}
		
		override public void Classify()
		{
		}
		
		override public void TryLoadingParameters()
		{
		}
		
		override public void Dispose()
		{
		}
		
		[DllImport("./musiC-uMng.dll", EntryPoint="Barbedo_Train")]
		static extern public void uTrain(ref MCDataCollection dtCol);
		
		[DllImport("./musiC-uMng.dll", EntryPoint="Barbedo_Filter")]
		static extern unsafe public MCDataCollection * uFilter(ref MCDataCollection dtCol);
	}
}