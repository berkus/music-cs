using System;
using System.Xml;
using System.Collections.Generic;

using MusiC;
using MusiC.Extensions;
using MusiC.Exceptions;

namespace MusiC.Configs
{
	public class XMLConfig : Config
	{
		Dictionary<String, ExtensionInfo> _tagCache = new Dictionary<String, ExtensionInfo>();
		
		public XMLConfig()
		{
		}
		
		public override void Load(String cfgPath)
		{
			XmlDocument cfgFile = new XmlDocument();
			cfgFile.Load(cfgPath);
			
			#region Parsing Declares
			BuildTagCache(cfgFile);
			#endregion
			
			#region Parsing Handlers
			BuildHandlerList(cfgFile);
			#endregion
			
			#region Parsing Data
			ParseData(cfgFile);
			#endregion
			
			#region Parsing algorithms
			BuildAlgorithmList(cfgFile);
			#endregion
		}
		
		String XmlSafeAttribute(XmlNode n, String attName, bool isOptional)
		{
			if(n==null)
				throw new MCException("XmlNode submited is null");
			
			XmlAttribute att = n.Attributes[attName];

			if (att == null)
			{
				if (!isOptional)
					throw new MissingAttributeException(attName);
				return null;
			}
			
			return att.Value;
		}
		
		String XmlSafeAttribute(XmlNode n, String attName)
		{
			return XmlSafeAttribute(n, attName, false);
		}
		
		void BuildHandlerList(XmlDocument cfgFile)
		{
			foreach (XmlNode n in cfgFile.GetElementsByTagName("Handler"))
			{
				HandlerInfo hInfo = new HandlerInfo();
				
				XmlSafeAttribute(n, "class");
				XmlSafeAttribute(n, "pattern");
				
				/// @todo 2 Parse constructor info
				XmlNodeList nList = n.ChildNodes;
				foreach (XmlNode param in nList)
				{
					if(param.Name == "Param")
					{
						hInfo.AddParam(XmlSafeAttribute(n, "name"), XmlSafeAttribute(n, "class"));
					}
				}
				
				AddHandler(hInfo);
			}
		}
		
		void BuildTagCache(XmlDocument cfgFile)
		{
			foreach (XmlNode n in cfgFile.GetElementsByTagName("Classifier"))
			{
				ExtensionInfo info = new ExtensionInfo();
				info.Class = XmlSafeAttribute(n, "class");
				info.Name = XmlSafeAttribute(n, "name");
				info.Type = ExtensionType.Classifier;
				
				XmlNodeList nList = n.ChildNodes;
				foreach (XmlNode param in nList)
				{
					if(param.Name == "Param")
					{
						info.AddParam(XmlSafeAttribute(param, "name"), XmlSafeAttribute(param, "class"));
					}
				}
				
				_tagCache.Add(info.Name, info);
			}
			
			foreach (XmlNode n in cfgFile.GetElementsByTagName("Window"))
			{
				ExtensionInfo info = new ExtensionInfo();
				info.Class = XmlSafeAttribute(n, "class");
				info.Name = XmlSafeAttribute(n, "name");
				info.Type = ExtensionType.Window;
				
				XmlNodeList nList = n.ChildNodes;
				foreach (XmlNode param in nList)
				{
					if(param.Name == "Param")
					{
						info.AddParam(XmlSafeAttribute(param, "name"), XmlSafeAttribute(param, "class"));
					}
				}
				
				_tagCache.Add(info.Name, info);
			}
			
			foreach (XmlNode n in cfgFile.GetElementsByTagName("Feature"))
			{
				ExtensionInfo info = new ExtensionInfo();
				info.Class = XmlSafeAttribute(n, "class");
				info.Name = XmlSafeAttribute(n, "name");
				info.Type = ExtensionType.Feature;
				
				XmlNodeList nList = n.ChildNodes;
				foreach (XmlNode param in nList)
				{
					if(param.Name == "Param")
					{
						info.AddParam(XmlSafeAttribute(param, "name"), XmlSafeAttribute(param, "class"));
					}
				}
				
				_tagCache.Add(info.Name, info);
			}
		}
		
		void ParseData(XmlDocument cfgFile)
		{
			foreach(XmlNode train in cfgFile.GetElementsByTagName("Train"))
			{
				String baseDir = XmlSafeAttribute(train, "dir", true);
				
				XmlNodeList nList = train.ChildNodes;
				foreach (XmlNode xmlLabelNode in nList)
				{
					if(xmlLabelNode.Name == "Label")
					{
						TrainLabel l = new TrainLabel();
						l.Label=XmlSafeAttribute(xmlLabelNode,"name");
						
						/// @todo 2 Add support to multiple input dirs
						l.InputDir=XmlSafeAttribute(xmlLabelNode,"input", true);
						if (l.InputDir==null)
						{
							/// @todo Check baseDir availability
							/// @todo Check "/"
							l.InputDir = baseDir + "/" + l.Label;
						}
						
						/// @todo 2 Make it optional. If missing equals to input
						l.OutputDir=XmlSafeAttribute(xmlLabelNode,"output", true);
						
						if(l.OutputDir == null)
							l.OutputDir=l.InputDir;
						
						AddTrainLabel(l);
					}
				}
			}
			
			// Adding Train/Classify support
			foreach(XmlNode classifyNode in cfgFile.GetElementsByTagName("Classify"))
			{
				/// @todo Add support to allow/deny algorithms to use this folder
				
				String dir = XmlSafeAttribute(classifyNode,"dir");
				
				if(!AddDir(dir))
					Warning("A non-existent directory ("+ dir +") wasn't added to the processing queue.");
			}
		}
		
		public void BuildAlgorithmList(XmlDocument cfgFile)
		{
			foreach (XmlNode n in cfgFile.GetElementsByTagName("Algorithm"))
			{
				Algorithm algorithm = new Algorithm();

				foreach (XmlNode child in n.ChildNodes)
				{
					ExtensionInfo i;
					
					if(_tagCache.TryGetValue(child.Name, out i))
						algorithm.Add(i);
					else
						Warning("Cant find extension "+child.Name);
				}
				
				AddAlgorithm(algorithm);
			}
		}
	}
}