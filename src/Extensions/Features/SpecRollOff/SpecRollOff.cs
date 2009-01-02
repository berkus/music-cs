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
//using System;
//using System.Diagnostics.CodeAnalysis;
//using System.Reflection;
//using System.Runtime.CompilerServices;
//using System.Runtime.InteropServices;
//using System.Resources;
//using System.Security.Permissions;

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;

[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]
[assembly: AssemblyTitle("MusiC - Feature - SpecRollOff")]
[assembly: AssemblyVersionAttribute("0.0.*")]

#region Security
//[assembly: PermissionSetAttribute(SecurityAction.RequestMinimum, Name="Nothing")]
//[assembly: StrongNameIdentityPermission(SecurityAction.RequestMinimum, PublicKey="{public-key}")]

//[assembly: EnvironmentPermission(SecurityAction.RequestRefuse)]
//[assembly: FileDialogPermission(SecurityAction.RequestRefuse)]
//[assembly: FileIOPermission(SecurityAction.RequestRefuse)]
//[assembly: IsolatedStorageFilePermission(SecurityAction.RequestRefuse)]
//[assembly: PublisherIdentityPermission(SecurityAction.RequestRefuse)]
//[assembly: ReflectionPermission(SecurityAction.RequestRefuse)]
//[assembly: RegistryPermission(SecurityAction.RequestRefuse)]
//[assembly: SecurityPermission(SecurityAction.RequestRefuse, UnmanagedCode=true)]
//[assembly: SiteIdentityPermission(SecurityAction.RequestRefuse)]
//[assembly: UIPermission(SecurityAction.RequestRefuse)]
//[assembly: ZoneIdentityPermission(SecurityAction.RequestRefuse)]
#endregion 

namespace MusiC.Extensions.Features
{
	[CLSCompliant(false)]
	unsafe public class SpecRollOffU : Unmanaged.Feature
	{	
		float * _temp;

		public SpecRollOffU() : base("SpecRolloff - Unmanaged")
		{
		}
		
		override unsafe public Single Extract(Single * wndData, int wndSize)
		{
			double sum = 0, cum = 0;
			int sro = 0;
			
			_temp = GetBuffer(wndSize);
			NativeMethods.Math.FFTMagnitude(wndData, _temp, wndSize);
					
			for (int i = 0; i < wndSize; i++)
				sum += *(_temp) * *(_temp++);
	
			cum = sum;
	
			for (sro = wndSize - 1; sro >= 0; sro--)
			{
				cum -= *(_temp--);
				
				if (cum < 0.95 * sum)
					break;
			}
			
			return sro;
		}
	}
	
	public class SpecRollOffM : Managed.Feature
	{	
		public SpecRollOffM() : base("SpecRolloff - Managed")
		{
		}
		
		override public Single[] Extract(Managed.Window window)
		{
			return null;
		}
	}
}

//namespace MCModule.Features
//{
//	public class SpecRollOff : MCModule.Feature
//	{
//		public SpecRollOff() : base("SpecRollOff")
//		{
//		}
//		
//		override unsafe public double * Extract(Window wnd)
//		{
//			double sum = 0, cum = 0;
//			int sro = 0;
//			
//			for(int w = 0; w < wnd.WindowCount; w++)
//			{
//				sum = 0; cum = 0;
//				
//				MCMath.FFTMagnitude(wnd.GetWindow(w), m_temp, wnd.WindowSize);
//						
//				for (int i = 0; i < wnd.WindowSize; i++)
//					sum += *(m_temp + i) * *(m_temp + i);
//		
//				cum = sum;
//		
//				for (sro = wnd.WindowSize - 1; sro >= 0; sro--)
//				{
//					cum -= *(m_temp + sro);
//					if (cum < 0.95 * sum)
//						break;
//				}
//				
//				*(m_data + w) = sro;
//			}
//			
//			return m_data;
//		}
//		
//		override public int FeatureSize(Window wnd)
//		{
//			return wnd.WindowCount;
//		}
//	}
//}