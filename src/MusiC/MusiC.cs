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
using System.IO;
using System.Reflection;

using MusiC.Exceptions;
using MusiC.Extensions;

[assembly: CLSCompliant(true)]

namespace MusiC
{
	/// <summary>
	/// Main API.
	/// 
	/// It is through this class that all library functionalities may be accessed. If you are creating a new
	/// extension you probably don't need to create an instance of this class.
	/// </summary>
	/// <see cref="MusiC.Extension"/>
	public class MusiC : MusiCObject
	{
		#region Attributes
		UnhandledExceptionEventHandler _UnhandledExceptionHandler;
		
		String _configFile;
		String _extensionsDir;
		
		ExtensionCache _cache = new ExtensionCache();
		Configurator _cfg;
		#endregion
		
		#region Properties
		/// <summary>
		/// The directory where we are looking for extensions.
		/// 
		/// If the assigned path doesn't exists it keeps unchanged.
		/// In case it is not null the path of the executable that called the library is assigned.
		/// </summary>
		public String ExtensionsDir
		{
			get {return _extensionsDir;}
			set {
				if(!Directory.Exists(value))
				{
					if(_extensionsDir == null)
						_extensionsDir=Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
					
					return;
				}
				_extensionsDir=value;
			}
		}
		
		/// <summary>
		/// Library config file.
		/// 
		/// If the assigned path doesn't exists it keeps unchanged.
		/// In case it is not null ExtensionsDir\config.xml is assigned.
		/// </summary>
		public String ConfigFile
		{
			get {return _configFile;}
			set {
				if(!File.Exists(value))
				{
					if(_configFile == null)
						_configFile = Path.Combine(ExtensionsDir, "config.xml");
					
					return;
				}
				
				_configFile=value;
			}
		}
		#endregion
		
		#region Library Startup/Finishing Functions
		/// <summary>
		/// Basic library startup code.
		/// </summary>
		public void Start()
		{
			_UnhandledExceptionHandler = new UnhandledExceptionEventHandler(MusiCObject.UnhandledException);
			AppDomain.CurrentDomain.UnhandledException += _UnhandledExceptionHandler;
			
			try
			{
				BeginReportSection("Starting Extension Loading");
				
				_cache.Load(ExtensionsDir);
				
				EndReportSection(true);
			}
			catch(MCException mce)
			{
				mce.Report();
			}
			catch(Exception e)
			{
				Error(e);
			}
		}
		
		public void Run()
		{
			try
			{
				_cfg = _cache.GetConfigurator();				
				_cfg.Load(ConfigFile);
				
				Message(_configFile + " ... [LOADED]");
				
				foreach(Algorithm a in _cfg.AlgorithmList)
				{
					a.Execute(_cache);
				}
			}
			catch(MCException mce)
			{
				mce.Report();
			}
			catch(Exception e)
			{
				Error(e);
			}
		}
		
		public void Stop()
		{
			AppDomain.CurrentDomain.UnhandledException -= _UnhandledExceptionHandler;
		}
		
		#endregion
		
		#region Extensions Handling
		public void LoadFrom(String path)
		{
			_cache.Load(path);
		}
		
		public bool IsExtensionLoaded(String classname)
		{
			return !(ExtensionCache.GetInfo(classname) == null);
		}
		#endregion
		
		#region Algorithm Handling
		#endregion
		
		#region Algorithm Execution
		#endregion
	}
}

/// @mainpage
/// System Basic Information:
///
/// @author Marcos José Sant'Anna Magalhães
/// @version 0.9.1
/// @date 21.01.09
///
/// Please refer to the @ref application_notes
/// for relevant notes about the system.
///
/// @page Roadmap
/// @todo Use some xml structure verifier like DTD or Scheme
///
/// @page application_notes Notes
///
/// @section About parameters to imported classes
/// Requisites to instatiable types are: \n
/// @li Type must have default constructor
/// @li void Parse(String) method must be available
///
/// @subsection Expected Types
/// In C# the keywords int, float and double are wrappers for standard classes.
/// The parameters must have those underlying type which can be found running the following code
///
/// @verbatim
/// using System;
///
/// class abc
/// {
///		static void Main()
///		{
///				Console.WriteLine("int:"+typeof(int).ToString());
///				Console.WriteLine("float:"+typeof(float).ToString());
///				Console.WriteLine("double:"+typeof(double).ToString());
///		}
///	}
/// @endverbatim