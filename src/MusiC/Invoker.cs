using System;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;

namespace MCModule
{
	public class Invoker
	{
		public Invoker(Config cfg) { }

		public static Extension LoadType(BinaryInfo info)
		{
			Extension instance = null;
			
			//Console.WriteLine("** Printing Binary Info --");
			//info.Print();
			//Console.WriteLine("***");
			
			if(info.Lib == String.Empty)
			{
				//String typeName = (info.Type == String.Empty) ? info.Tag : info.Type;
				String typeName = info.Class; 
				
				try
				{
					Type toLoad = Type.GetType(info.Class);
					Console.WriteLine("Attempting to find type {0} in known types.",typeName);
					instance = toLoad.GetConstructor(info.GetTypes()).Invoke(info.GetParamValues()) as Extension;
				}
				catch(Exception e)
				{
					//Console.WriteLine(e.Message);
					///@todo Change displayed information to include the xml code.
					Console.WriteLine("This type isn't known yet. Try adding a Library(Attribute 'library') to the line " + info.Line + " tag.");
					//instance = LoadLib(info);
				}
			}
			else
			{
				instance = LoadLib(info);
			}
			
			return instance;
		}
		
		static Extension LoadLib(BinaryInfo info)
		{
			//string typeName = (info.Class == String.Empty) ? info.Tag : info.Type;
			string typeName = info.Class;
			string libName = (info.Lib == String.Empty) ? typeName + ".dll" : info.Lib;
			
			//Console.WriteLine("info.Dir = {0} info.Lib = {1}", info.Dir, info.Lib);

			string assemblyName = (info.Dir == String.Empty ? String.Empty : info.Dir + System.IO.Path.DirectorySeparatorChar) + libName;
			
			//if(info.Dir == null) Console.WriteLine("NULL");
			//if(info.Dir == System.IO.Directory.GetCurrentDirectory()) Console.WriteLine("CURDIR");
			//if(info.Dir == String.Empty) Console.WriteLine("EMPTY");
			//string assemblyName = libName;
			
			/// @todo evaluate assembly type (MCCL or MCLL)
			return LoadClassLib(assemblyName, typeName, info.GetTypes(), info.GetParamValues());
		}
	
		static Extension LoadClassLib(string assemblyName, string typeName, Type[] types, Object[] param)
		{
			Assembly lib = Assembly.LoadFrom(assemblyName);
	
			Type toImport = lib.GetType(typeName);
			Extension instance = toImport.GetConstructor(types).Invoke(param) as Extension;
	
			return instance;
		}
	
		public void LoadLegacyLib() { }
	}
}

namespace MusiC
{
	public class Extension
	{
	}
	
	class Invoker
	{
		public static Extension LoadType()
		{
			return null;
		}
		
		public static Extension LoadType(BinaryInfo t)
		{
			return null;
		}
		
		public static Config LoadConfig(Type tConfig)
		{
			// TODO: Check if this type match expected config.
			ConstructorInfo configCtor = tConfig.GetConstructor(new Type[0]);
			if(configCtor!=null)
				return configCtor.Invoke(null) as Config;
			// TODO: Implement MusiC.Invoker.LoadConfig())
			return null;
		}
	}
}