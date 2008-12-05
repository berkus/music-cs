using System;
using System.Diagnostics;

using MusiC.Extensions;
using MusiC.Exceptions;

namespace MusiC
{
	public class MusiC : MusiCObject, IGlobal
	{
		String _configFile="data/config.xml";
		
		UnhandledExceptionEventHandler _UnhandledExceptionHandler;
//		static MusiC _this=null;
		
		public MusiC()
		{
		}
		
		public void Initialize()
		{
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
			log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("music.log.xml"));
			
			Message("Registering Unhandled Exception Event");
			_UnhandledExceptionHandler = new UnhandledExceptionEventHandler(MusiCObject.UnhandledException);
			AppDomain.CurrentDomain.UnhandledException += _UnhandledExceptionHandler;
			
			try
			{
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
			}
			catch(MCException mce)
			{
				mce.Report();
			}
			
			//cache.Print();
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
