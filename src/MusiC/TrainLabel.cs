/*
 * The MIT License
 * Copyright (c) 2008 Marcos Jos� Sant'Anna Magalh�es
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

namespace MusiC
{
	public class TrainLabel : MusiCObject
	{
		String _label;
		String _inDir;
		String _outDir;
		
		public String Label
		{
			get {return _label;}
			set {_label=value;}
		}
		
		public String OutputDir
		{
			get {return _outDir;}
			set {_outDir=value;}
		}
		
		public String InputDir
		{
			get {return _inDir;}
			set {_inDir=value;}
		}
		
		/* override public String ToString()
		{
			String obj=PrintHeader();
			
			obj= obj +
			"[Label]:"+ _label + "\n" +
			"[Input Dir]:"+_inDir + "\n"+
			"[Output Dir]:"+_outDir + "\n";
			
			obj = obj + PrintFooter();
			
			return obj;
		} */
		
		public void Validate()
		{
			if(!Directory.Exists(_outDir))
			{
				/// @todo Throw an Exception
				Console.WriteLine("The specified output directory isnt available");
			}
			
			if(!Directory.Exists(_inDir))
				/// @todo Throw an Exception
				Console.WriteLine("The specified input directory isnt available.");
				
			Print();
		}
	}
}
