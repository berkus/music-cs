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
			foreach (XmlNode n in cfgFile.GetElementsByTagName("Declare"))
			{
				ExtensionInfo info = new ExtensionInfo();
				info.Type = XmlSafeAttribute(n, "class");
				
				XmlNodeList nList = n.ChildNodes;
				foreach (XmlNode param in nList)
				{
					if(param.Name == "Param")
					{
						info.AddParam(XmlSafeAttribute(param, "name"), XmlSafeAttribute(param, "class"));
					}
				}
				
				try {
					_tagCache.Add(XmlSafeAttribute(n, "name"), info);
				} catch {
					// TODO: Consume and throw MCException
					Console.WriteLine("Error while creating dictionary. Check for duplicated entries.");
				}
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
				AddDir(XmlSafeAttribute(classifyNode,"dir"));
			}
		}
		
		public void BuildAlgorithmList(XmlDocument cfgFile)
		{
			foreach (XmlNode n in cfgFile.GetElementsByTagName("Algorithm"))
			{
				MCModule.Algorithm algorithm = new MCModule.Algorithm();
	
				// Searching for defined tags
				foreach (String key in _tagCache.Keys)
				{
					//Loading declared tags inside the algorithm node
					// TODO: Check with multiple algorithms
					XmlNodeList nList = cfgFile.GetElementsByTagName(key, n.NamespaceURI);

					if (nList.Count == 0)
						continue;
	
					foreach (XmlNode tag in nList)
					{
						ExtensionInfo info;
						_tagCache.TryGetValue(tag.Name, out info);
	
						// Loading parameters
						foreach (BinaryParam param in info.GetParam())
						{
							try
							{
								// TODO: Prevent erros
								XmlNodeList paramList = cfgFile.GetElementsByTagName(param.Name, tag.NamespaceURI);
								XmlAttribute attr = paramList.Item(0).Attributes["value"];// Parameters with same tag name are not accepted.
								param.strValue = attr.Value;
							}
							catch (System.Exception e)
							{
								Console.WriteLine(" --- Error while loading parameter value ---");
								Console.WriteLine("[*]" + e.Message);
							}
						} //Closing the foreach loading the parameter before loading the library
						//Support multiple parameters
						
						// TODO: Finish algorithm object adding window, features and classifier to it.
					}
				}
				AddAlgorithm(algorithm);
			}
		}
	}
}
