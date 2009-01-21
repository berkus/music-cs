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
using System.Diagnostics;
using System.IO;
using System.Reflection;

using MusiC.Extensions;
using MusiC.Exceptions;

[assembly: CLSCompliant(true)]
namespace MusiC
{
	public class MusiC : MusiCObject
	{		
		UnhandledExceptionEventHandler _UnhandledExceptionHandler;
		String _configFile;
		String _extensionsDir;
		
		ExtensionCache _cache = new ExtensionCache();
		ExtensionLoader _loader = new ExtensionLoader();
		Configurator _cfg;
		
		public String ExtensionsDir
		{
			get {return _extensionsDir;}
			set {
				if(!Directory.Exists(value))
					throw new MissingFileOrDirectoryException("Wasn't able to find extensions dir (" + _extensionsDir + ").");				
				_extensionsDir=value;
			}
		}
		
		public String ConfigFile
		{
			get {return _configFile;}
			set {
				if(!File.Exists(value))
					throw new MissingFileOrDirectoryException("Wasn't able to find configuration file (" + _configFile + ").");
				
				_configFile=value;
			}
		}
		
		public void Load()
		{	
			Message("Registering Unhandled Exception Event");
			_UnhandledExceptionHandler = new UnhandledExceptionEventHandler(MusiCObject.UnhandledException);
			AppDomain.CurrentDomain.UnhandledException += _UnhandledExceptionHandler;
			
			try
			{
				// Grab an ExtensionLoader instance
				Message("Starting Extension Loading");ReportIndent();
				_loader.Load(_extensionsDir, _cache);
				ReportUnindent();
				
				//@todo Uncomment protection
				//if( !loader.HasConfig() || !loader.HasFileHandler() )
				//	throw new MissingExtensionException("MusiC needs at least a Config and a Handler extension.");
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
				
				// Get exec path if user don't provide one.
				if(_configFile == null)
					_configFile=Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "config.xml");
				
				_cfg.Load(_configFile);
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
		
		public void Unload()
		{
			AppDomain.CurrentDomain.UnhandledException -= _UnhandledExceptionHandler;
		}
	}
}
