using System;
using System.Reflection;
using System.IO;

using MusiC.Exceptions;

namespace MusiC.Extensions
{
	public class ExtensionLoader : MusiCObject, IGlobal
	{		
		public ExtensionLoader()
		{
		}
		
		public void Initialize()
		{
		}
		
		public void Load(String extensionsDir)
		{
			// Get exec path if user don't provide one.
			if(extensionsDir==null)
				extensionsDir=Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Extensions");
			
			ExtensionCache cache = Global<ExtensionCache>.GetInstance();
			
			foreach(String ext in Directory.GetFiles(extensionsDir, "*.dll"))
			{
				// FIX: When MusiC.dll is loaded the Type.IsSubClassOf() fails.
				if(Path.GetFileName(ext) == "MusiC.dll")
					continue;
					
				try {
					Assembly l = Assembly.LoadFrom(ext);
					Message(ext+" ... [LOADED]");ReportIndent();
					
					foreach(Type t in l.GetExportedTypes())
					{
						cache.Add(t);
					}
					
					ReportUnindent();
				}
				catch {
					// Probably trying to load an unmanaged assembly.
					Warning(ext+" ... [LOADING FAILED]");
				}
			}
		}
		public void BuildCache()
		{
		}
		
		public bool HasConfig()
		{
			return false;
		}
		
		public bool HasFileHandler()
		{
			return false;
		}
	}
}
