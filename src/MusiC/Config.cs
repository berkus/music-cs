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
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.IO;

using MusiC.Exceptions;
using MusiC.Extensions;

namespace MusiC
{
	abstract public class Config : Extension
	{
		LinkedList<Algorithm> _algList = new LinkedList<Algorithm>();
		LinkedList<TrainLabel> _trainList = new LinkedList<TrainLabel>();
		LinkedList<String> _classifyList = new LinkedList<String>();
		LinkedList<HandlerInfo> _handlerList = new LinkedList<HandlerInfo>();
		
		public Config()
		{
			Message(this.GetType().FullName + " ... [LOADED]");
		}
		
		protected void AddAlgorithm(Algorithm algorithm)
		{
			_algList.AddLast(algorithm);
		}
		
		protected void AddTrainLabel(TrainLabel label)
		{
			label.Validate();
			_trainList.AddLast(label);
		}
		
		protected bool AddDir(String dir)
		{
			bool returnValue;
			
			if(returnValue=Directory.Exists(dir))
				_classifyList.AddLast(dir);
			
			return returnValue;
		}
		
		protected void AddHandler(HandlerInfo hInfo)
		{
			_handlerList.AddLast(hInfo);
		}
		
		protected ExtensionInfo GetExtensionInfo(String className)
		{
			return Global<ExtensionCache>.GetInstance().GetInfo(className);
		}
		
		public IEnumerable<Algorithm> GetAlgorithmList()
		{
			return _algList;
		}
		
		abstract public void Load(String file);
	}
}
