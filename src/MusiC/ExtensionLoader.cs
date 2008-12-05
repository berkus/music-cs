using System;
using System.Reflection;
using System.IO;

using MusiC.Exceptions;

namespace MusiC.Extensions
{
	public class ExtensionLoader : MusiCObject, IGlobal
	{
		String _basePath=null;
		public String BasePath
		{
			get {return _basePath;}
			set {_basePath=value;}
		}
		
		public ExtensionLoader()
		{
		}
		
		public void Initialize()
		{
			if(_basePath==null)
				// Get exec path if user don't provide one.
				_basePath=Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			
			ExtensionCache cache = Global<ExtensionCache>.GetInstance();
			String extensionDir=_basePath + "/Extensions";
			
			if(!Directory.Exists(extensionDir))
				throw new MCException("Wasn't able to find extension folder. Add an 'Extension' folder at " + _basePath + ".");
			
			foreach(String ext in Directory.GetFiles(extensionDir))
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
