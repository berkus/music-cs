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
			
			foreach(String ext in Directory.GetFiles(extensionsDir))
			{
				Message("Loading " + ext);ReportIndent();
				
				try {
					Assembly l = Assembly.LoadFrom(ext);
					
					
					foreach(Type t in l.GetExportedTypes())
					{
						// TODO: try { } catch { }
						Message(t.ToString() + " ... [FOUND]");
						cache.Add(t);
					}
				}
				catch {
					// Probably trying to load an unmanaged assembly.
					Warning("Wasnt able to load. Non managed assembly ?");
				}
				finally {
					ReportUnindent();
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
