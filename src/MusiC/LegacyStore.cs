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