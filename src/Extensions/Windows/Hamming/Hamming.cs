using System;
using System.Collections.Generic;
using System.Text;

using MusiC;

//namespace MCModule.Windows
//{
//	public class Hamming : MCModule.Window
//	{
//		public Hamming(int size, int overlap) : base("Hamming", size, overlap)
//		{
//		}
//		
//		override public double Factory(int n)
//		{
//			//return 0.53836 - 0.46164 * (Math.Cos(2 * Math.PI * n / (m_size - 1)));
//			return 0.54 - 0.46 * (Math.Cos(2 * Math.PI * n / (m_size - 1)));
//		}
//	}
//}

namespace MusiC.Extensions.Windows
{
	public class Hamming : Window
	{
		public Hamming(int size, int overlap) : base("Hamming", size, overlap)
		{
		}
		
		override public double Factory(int n)
		{
			//return 0.53836 - 0.46164 * (Math.Cos(2 * Math.PI * n / (m_size - 1)));
			return 0.54 - 0.46 * (Math.Cos(2 * Math.PI * n / (m_size - 1)));
		}
	}
}