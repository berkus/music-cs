using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.IO;

using MusiC.Exceptions;

namespace MCModule
{
	public class Config
	{		
		LinkedList<Algorithm> mAlgFifo = new LinkedList<Algorithm>();
		LinkedList<ActionNode> m_trainList = new LinkedList<ActionNode>();
		LinkedList<ActionNode> m_classifyList = new LinkedList<ActionNode>();
		static LinkedList<HandlerInfo> mHandlerInfoList = new LinkedList<HandlerInfo>();
		
		Dictionary<string, BinaryInfo> mTagCache = new Dictionary<string, BinaryInfo>();
	
		public Config(string file)
		{
			Console.WriteLine(file);
			XmlDocument cfgFile = new XmlDocument();
			cfgFile.Load(file);
	
			#region Parsing Declares
			BuildTagCache(cfgFile);
			#endregion
			
			#region Parsing Handlers
			BuildHandlerCache(cfgFile);
			#endregion
	
			#region Parsing Data
			foreach (XmlNode dir in cfgFile.GetElementsByTagName("Data"))
			{
				// Add data dir
				//mDirs.AddLast(dir.Attributes["dir"].Value);
				
				// Adding Train/Classify support
				foreach(XmlNode action in cfgFile.GetElementsByTagName("Train", dir.NamespaceURI))
				{
					///@todo add possibility to change rootdir
					m_trainList.AddLast(new ActionNode(action.Attributes["name"].Value, dir.Attributes["dir"].Value));
				}
				
				foreach(XmlNode action in cfgFile.GetElementsByTagName("Classify", dir.NamespaceURI))
				{
					m_classifyList.AddLast(new ActionNode(action.Attributes["dir"].Value, dir.Attributes["dir"].Value));
				}
			}
			#endregion
	
			#region Parsing algorithms
			BuildAlgorithmList(cfgFile);
			#endregion
		}
		
		public void BuildHandlerCache(XmlDocument cfgFile)
		{
			foreach (XmlNode n in cfgFile.GetElementsByTagName("Handler"))
			{
				HandlerInfo info = new HandlerInfo();
				info.Parse(n);
				
				mHandlerInfoList.AddLast(info);
			}
		}
	
		public void BuildTagCache(XmlDocument cfgFile)
		{
			foreach (XmlNode n in cfgFile.GetElementsByTagName("Declare"))
			{
				ExtensionInfo info = new ExtensionInfo();
				
				info.Parse(n);
	
				mTagCache.Add(info.Tag, info);
			}
		}
		
		public void ParseData(XmlDocument cfgFile)
		{
		}
		
		public void BuildAlgorithmList(XmlDocument cfgFile)
		{
			foreach (XmlNode n in cfgFile.GetElementsByTagName("Algorithm"))
			{
				Algorithm algorithm = new Algorithm();
	
				// Searching for defined tags
				foreach (String key in mTagCache.Keys)
				{
					//Loading declared tags inside the algorithm node
					/// @todo Look in child nodes !
					XmlNodeList nList = cfgFile.GetElementsByTagName(key, n.NamespaceURI);

					if (nList.Count == 0)
						continue;
	
					foreach (XmlNode tag in nList)
					{
						BinaryInfo info;
						mTagCache.TryGetValue(tag.Name, out info);
	
						// Loading parameters
						foreach (BinaryParam param in info.GetParam())
						{
							try
							{
								XmlNodeList paramList = cfgFile.GetElementsByTagName(param.Name, tag.NamespaceURI);
								XmlAttribute attr = paramList.Item(0).Attributes["value"];// Parameters with same tag name are not accepted.
								param.strValue = attr.Value;
							}
							catch (Exception e)
							{
								Console.WriteLine(" --- Error while loading parameter value ---");
								Console.WriteLine("[*]" + e.Message);
							}
						} //Closing the foreach loading the parameter before loading the library
						//Support multiple parameters
	
						Extension i = Invoker.LoadType(info);
						Type _base = i.GetType().BaseType;
						
						//Console.WriteLine("TypeInfo:" + i.GetType().ToString() + ":" + _base.ToString());
						
						if (_base == typeof(Window) || i.GetType() == typeof(Window))
						{
							try
							{
								algorithm.STFTWindow = (i as Window);
							}
							catch (Exception ex)
							{
								throw ex;
							}

							continue;
						}

						if (_base == typeof(Feature) || i.GetType() == typeof(Feature))
						{
							algorithm.AddFeature((i as Feature));
							continue;
						}

						if (_base == typeof(Classifier) || i.GetType() == typeof(Classifier))
						{
							try
							{
								algorithm.AlgClassifier = (i as Classifier);
							}
							catch (Exception ex)
							{
								throw ex;
							}
						}
					}
				}
				mAlgFifo.AddLast(algorithm);
			}
		}
		
		public IEnumerable<Algorithm> GetAlgorithms()
		{
			return mAlgFifo;
		}
	
		public IEnumerable<String> GetDirs()
		{
			//return mDirs;
			return null;
		}
		
		public ICollection<ActionNode> GetTrainingData()
		{
			return m_trainList;
		}
		
		public ICollection<ActionNode> GetClassificationData()
		{
			return m_classifyList;
		}
		
		static public ICollection<HandlerInfo> GetHandlerList()
		{
			return mHandlerInfoList;
		}
		
		static public Handler GetHandler(string file)
		{
			Handler handler = null;
						
			foreach(HandlerInfo hi in mHandlerInfoList)
			{
				if(hi.MatchPattern(file))
				{
					handler = (hi.New() as Handler);
					
					if(handler.CanHandle(file))
						break;
					else
						handler = null;
				}
			}
			
			return handler;
		}
		
		/// @brief Creates a new Config object.
		/// @param file The xml condiguration file.
		/// @returns A new Config instance.
		static public Config LoadConfig(string file)
		{
			return new Config(file);
		}
	}
}

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
		
		abstract public void Load(String file);
	}
}
