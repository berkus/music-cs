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
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;

using MusiC;

namespace MusiC.Apps
{
	class GenreC
	{
		static void Main(String[] args)
		{
			MusiC m = Global<MusiC>.GetInstance();
			m.Load();
			m.Run();
			m.Unload();
		}
	}
}
/// @mainpage
/// System Basic Information:
///
/// @author Marcos José S. Magalhães
/// @version 0.1
/// @date 22.04.08
///
/// Please refer to the @ref application_notes
/// for relevant notes about the system.
///
/// @section Assemblies
/// The following assemblies are included in this release:
/// @li pfMono.exe (or musiC.mono) refered as Application
/// @li base.lib refered as Base Library
/// @li Hamming.mccl
///
/// @section Notation
///
/// @subsection File Extension
/// We are using 2 extensions:
/// @li MONO - MONO Project Binary Format.
/// @li MCCL - Music Classifier Class Library
/// @li MCLL - Music Classifier Legacy Library
///
/// @subsection In Docs
/// Each class or global function entry has a note section which contains the following info:
/// @li The assembly name which contains this entry.

/// @page Roadmap
/// @todo Use some xml structure verifier like DTD or Scheme
/// @todo Create non-code files for general documentation .... like this page.

/// @page application_notes Notes
///
/// @section About parameters to imported classes
/// Requisites to instatiable types are: \n
/// @li Type must have default constructor
/// @li void Parse(String) method must be available
///
/// @subsection Expected Types
/// In C# the keywords int, float and double are wrappers for standard classes.
/// The parameters must have those underlying type which can be found running the following code
///
/// @verbatim
/// using System;
///
/// class abc
/// {
///		static void Main()
///		{
///				Console.WriteLine("int:"+typeof(int).ToString());
///				Console.WriteLine("float:"+typeof(float).ToString());
///				Console.WriteLine("double:"+typeof(double).ToString());
///		}
///	}
/// @endverbatim

/// @brief Project entry point.
/// @details This class contains the methods where the application starts.
/// @note Included in (Application)
