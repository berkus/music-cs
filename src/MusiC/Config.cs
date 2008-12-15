using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.IO;

using MusiC.Exceptions;
using MusiC.Extensions;

namespace MusiC
{
	abstract public class Config : MusiCObject
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
