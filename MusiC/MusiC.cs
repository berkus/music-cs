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
		private UnhandledExceptionEventHandler _UnhandledExceptionHandler;
		
		private String _configFile = "config.xml";
		private String _extensionsDir = ".";
		
		private ExtensionCache _cache = new ExtensionCache();
		#endregion

		//::::::::::::::::::::::::::::::::::::::://
		
		#region Properties
		/// <summary>
		/// The directory where we are looking for extensions.
		/// 
		/// If the assigned path doesn't exists it keeps unchanged.
		/// </summary>
		public String ExtensionsDir
		{
			get { return _extensionsDir; }

			set
			{
				if(Directory.Exists(value))
				{
                    _extensionsDir = value;
                    //_extensionsDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				}
				
			}
		}

		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// Library config file.
		/// 
		/// If the assigned path doesn't exists it keeps unchanged.
		/// </summary>
		public String ConfigFile
		{
			get {return _configFile;}
			set 
            {
				if(File.Exists(value))
				{
                    _configFile = value;
					//_configFile = Path.Combine(ExtensionsDir, "config.xml");
				}	
			}
		}
		#endregion

		//::::::::::::::::::::::::::::::::::::::://
		
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
				_cache.Say();
				
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
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		public void Stop()
		{
			AppDomain.CurrentDomain.UnhandledException -= _UnhandledExceptionHandler;
		}
		
		#endregion
		
		//::::::::::::::::::::::::::::::::::::::://
		
		#region Extensions Handling
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="path">
		/// A <see cref="System.String"/>
		/// </param>
		public void LoadFrom(String path)
		{
			_cache.Load(path);
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="classname">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public bool IsExtensionLoaded(String classname)
		{
			return !(ExtensionCache.GetInfo(classname) == null);
		}
		
		#endregion
		
		//::::::::::::::::::::::::::::::::::::::://
		
		#region Algorithm Handling
		#endregion
		
		//::::::::::::::::::::::::::::::::::::::://
		
		#region Algorithm Execution

        /// <summary>
        /// 
        /// </summary>
        public void Run()
        {
            try
            {
                Configurator cfg = _cache.GetConfigurator(ConfigFile);
                Config conf = cfg.LoadConfig(ConfigFile);

                Message(_configFile + " ... [LOADED]");

                foreach (Algorithm a in conf.AlgorithmList)
                {
                    a.Execute(conf);
                }
            }
            catch (MCException mce)
            {
                mce.Report();
            }
            catch (Exception e)
            {
                Error(e);
            }
        }
		
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
/// Copyright (c) 2008-2009 Marcos José Sant'Anna Magalhães
///
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
///  
/// The above copyright notice and this permission notice shall be included in
/// all copies or substantial portions of the Software.
///
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
/// THE SOFTWARE.
///
/// Please refer to the @ref application_notes for relevant notes about the system.
///
/// @page application_notes Notes
///
/// @section internal_algorithm Internal Algorithm
/// 
/// A few notes about the internals.
/// 
/// File Handlers:
/// 
/// Windows expect data in raw format so file handlers are responsible for any decompression eventually needed.
/// 
/// Windows also need the file to be broken in small size fix chunks so the handler must handle this. Of course it is possible to pass all
/// available data to the window but the internal data representation would be 3 times (file data, window data, the product filedata * windowdata)
/// the size of the file.
/// 
/// Windows:
/// 
/// First thing to be noted is that window data is MULTIPLIED by the file data. I intend to give the user the possibility to overload the "interaction"
/// function enabling those who want to use the library to filter something.
///
/// @section imported_classes_parameters Extensions Parameters
/// 
/// Requisites to extensions parameters:
/// @li void Parse(String) - This method must be available IF a non-default value is needed.
/// 
/// This is true for System.Int32 (int), System.Single (float) and System.Double (double) and possibly for other base types.
/// 
/// This also means that you may pass a class as a parameter, if it implements the method, or if the default value is always used. 
/// 
/// Check this(@ref constructor_param_order "About the order of the parameter declaration") note about the order the parameters are declared. 
///
/// @subsection obs_about_types Expected Types
/// In C# the keywords int, float and double are wrappers for standard classes.
/// When declaring an extension the parameters MUST be declared as one of the underlying types which can be found
/// running the following code:
///
/// @verbatim
/// using System;
///
/// class abc
/// {
///		static void Main()
///		{
///             // This list can also include: bool, string, char, short, long and possibly others.
///				Console.WriteLine("int:"+typeof(int).ToString()); // System.Int32
///				Console.WriteLine("float:"+typeof(float).ToString()); // System.Single
///				Console.WriteLine("double:"+typeof(double).ToString()); // System.Double
///		}
///	}
/// @endverbatim
///