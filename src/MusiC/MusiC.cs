using System;
using System.Diagnostics;

using MusiC.Extensions;
using MusiC.Exceptions;

namespace MusiC
{
	public class MusiC : MusiCObject
	{
		String _configFile="data/config.xml";
		
		UnhandledExceptionEventHandler _UnhandledExceptionHandler;
		static MusiC _this=null;
		
		public MusiC()
		{
		}
		
		public static MusiC CreateInstance()
		{
			if(_this == null)
				_this = new MusiC();
			
			return _this;
		}
		
		public String BaseExtensionDir
		{
			get {return Global<ExtensionLoader>.GetInstance().BasePath;}
			set {Global<ExtensionLoader>.GetInstance().BasePath=value;}
		}
		
		public String ConfigFile
		{
			get {return _configFile;}
			set {_configFile=value;}
		}
		
		public void Load()
		{
			log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("bin/music.log.config"));
			
			Message("Registering Unhandled Exception Event");
			_UnhandledExceptionHandler = new UnhandledExceptionEventHandler(Report.UnhandledException);
			AppDomain.CurrentDomain.UnhandledException += _UnhandledExceptionHandler;
			
			// Grab an ExtensionLoader instance
			Message("Starting Extension Loading");ReportIndent();
			ExtensionLoader loader = Global<ExtensionLoader>.GetInstance();
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
			
			Message("Retrieving Config Handler");ReportIndent();
			cfg.Load(_configFile);
			ReportUnindent();
			
			cache.Print();
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
