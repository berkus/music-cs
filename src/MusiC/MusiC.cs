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

namespace MusiC
{
	public class MusiC : MusiCObject, IGlobal
	{		
		UnhandledExceptionEventHandler _UnhandledExceptionHandler;
		String _configFile;
		String _extensionsDir;
		
//		static MusiC _this=null;
		
		public MusiC()
		{
		}
		
		public void Initialize()
		{
		}
		
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
			log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("music.log.xml"));
			
			Message("Registering Unhandled Exception Event");
			_UnhandledExceptionHandler = new UnhandledExceptionEventHandler(MusiCObject.UnhandledException);
			AppDomain.CurrentDomain.UnhandledException += _UnhandledExceptionHandler;
			
			try
			{
				// Grab an ExtensionLoader instance
				Message("Starting Extension Loading");ReportIndent();
				ExtensionLoader loader = Global<ExtensionLoader>.GetInstance();
				loader.Load(_extensionsDir);
				ReportUnindent();
				
				// TODO: Uncomment protection
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
				ExtensionCache cache = Global<ExtensionCache>.GetInstance();
				Config cfg = cache.GetConfig();
				
				// Get exec path if user don't provide one.
				if(_configFile==null)
					_configFile=Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "config.xml");
				
				cfg.Load(_configFile);
				Message(_configFile + " ... [LOADED]");
				
				foreach(Algorithm a in cfg.GetAlgorithmList())
				{
					a.Execute();
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
