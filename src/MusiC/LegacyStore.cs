using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace MCModule
{
	
	public class LegacyStore
	{
		public static void Main()
		{
			//DynamicDllFunctionInvoke(Directory.GetCurrentDirectory() + "/legacy.so", "f");
		}
	
		static public object DynamicDllFunctionInvoke(string DllPath, string EntryPoint)
		{
			//Define return type of your dll function.
			Type returnType = typeof(int);
	
			//out or in parameters of your function.
			Type[] parameterTypes = { };
			object[] parameterValues = { };
	
			// Create a dynamic assembly and a dynamic module
			AssemblyName asmName = new AssemblyName("legacy.so");
	
			AssemblyBuilder legacyLib =
			AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
	
			ModuleBuilder dynamicMod = legacyLib.DefineDynamicModule("legacyMod");
	
			// Dynamically construct a global PInvoke signature 
			// using the input information
	
			dynamicMod.DefinePInvokeMethod(EntryPoint, DllPath,	MethodAttributes.Static | MethodAttributes.Public | MethodAttributes.PinvokeImpl,
			CallingConventions.Standard, returnType, parameterTypes, CallingConvention.StdCall,	CharSet.Ansi);
			
			// This global method is now complete
			dynamicMod.CreateGlobalFunctions();
	
			// Get a MethodInfo for the PInvoke method
			MethodInfo mi = dynamicMod.GetMethod(EntryPoint);
	
			// Invoke the static method and return whatever it returns
			object retval = mi.Invoke(null, parameterValues);
	
			return retval;
		}
	}	
}