using System;
using System.Runtime.InteropServices;

namespace MCModule
{
	/// @todo implement in cpp
	public unsafe class MCMath
	{		
		[DllImport("./musiC-uMng.dll", EntryPoint="fftr_mag2")]
		public extern static int FFTMagnitude(double * sequence, double * magnitude, int size);
	}
}

namespace MCModule.UnmanagedInterface
{
	unsafe public class UnsafePtr
	{
		public static double* dgetmem(int size)
		{
			IntPtr ptr = Marshal.AllocHGlobal(size * sizeof(double));
			return (double*)ptr.ToPointer();
		}
		
		public static void free(void* p)
		{
			if (p != null)
			{
				IntPtr ptr = new IntPtr(p);
				Marshal.FreeHGlobal(ptr);
				p = null;
			}
		}
	}
}