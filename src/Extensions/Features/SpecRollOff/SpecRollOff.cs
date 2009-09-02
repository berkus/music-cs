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

[assembly: CLSCompliant( true )]
[assembly: ComVisible( false )]
[assembly: AssemblyTitle( "MusiC - Feature - SpecRollOff" )]
[assembly: AssemblyVersionAttribute( "0.0.*" )]

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
	[CLSCompliant( false )]
	unsafe
	public class SpecRollOffU : Unmanaged.Feature
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="wndData">
		/// A <see cref="Single"/>
		/// </param>
		/// <param name="wndSize">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// A <see cref="Single"/>
		/// </returns>
		override unsafe
		public Single Extract( Unmanaged.Frame frame )
		{
			double sum = 0, cum = 0;
			int sro = 0;

			int wndSize = frame.Size;
			float* spectrum = frame.RequestFFTMagnitude();

			for( int i = 0; i < wndSize / 2; i++ )
				sum += spectrum[ i ] * spectrum[ i ];

			cum = sum;

			for( sro = ( wndSize / 2 ) - 1; sro > 0; sro-- )
			{
				if( cum < ( 0.95 * sum ) )
					break;

				cum -= spectrum[ sro ] * spectrum[ sro ];
			}

			return sro;
		}
	}

	//---------------------------------------//

	public class SpecRollOffM : Managed.Feature
	{
		override public Single[] Extract( Managed.Window window )
		{
			return null;
		}
	}
}