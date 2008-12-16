using System;
using System.Reflection;
using System.IO;

using MusiC.Exceptions;

namespace MusiC.Extensions
{/*
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
