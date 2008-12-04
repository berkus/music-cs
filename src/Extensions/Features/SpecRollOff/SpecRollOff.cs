using System;

namespace MCModule.Features
{
	public class SpecRollOff : MCModule.Feature
	{
		public SpecRollOff() : base("SpecRollOff")
		{
		}
		
		override unsafe public double * Extract(Window wnd)
		{
			double sum = 0, cum = 0;
			int sro = 0;
			
			for(int w = 0; w < wnd.WindowCount; w++)
			{
				sum = 0; cum = 0;
				
				MCMath.FFTMagnitude(wnd.GetWindow(w), m_temp, wnd.WindowSize);
						
				for (int i = 0; i < wnd.WindowSize; i++)
					sum += *(m_temp + i) * *(m_temp + i);
		
				cum = sum;
		
				for (sro = wnd.WindowSize - 1; sro >= 0; sro--)
				{
					cum -= *(m_temp + sro);
					if (cum < 0.95 * sum)
						break;
				}
				
				*(m_data + w) = sro;
			}
			
			return m_data;
		}
		
		override public int FeatureSize(Window wnd)
		{
			return wnd.WindowCount;
		}
		
		override public void Dispose()
		{
		}
	}
}