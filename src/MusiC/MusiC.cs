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
				
				Message("Initializing Extension Cache");ReportIndent();
				ExtensionCache cache = Global<ExtensionCache>.GetInstance();
				ReportUnindent();
				
				Message("Retrieving Config Handler");ReportIndent();
				Config cfg = cache.GetConfig();
				ReportUnindent();
				
				Message("Loading Config File");ReportIndent();
				// Get exec path if user don't provide one.
				if(_configFile==null)
					_configFile=Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "config.xml");
				
				cfg.Load(_configFile);
				Message(_configFile + " ... [LOADED]");
				
				ReportUnindent();
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
		}
		
		public void Unload()
		{
			AppDomain.CurrentDomain.UnhandledException -= _UnhandledExceptionHandler;
		}
	}
}
