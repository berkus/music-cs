using System;
using System.Xml;
using System.Collections.Generic;

using MusiC;
using MusiC.Extensions;
using MusiC.Exceptions;

namespace MusiC.Configs
{
	/// <summary>
	/// A XML parser that knows how to configure the library.
	/// </summary>
	/// @todo There is no need to have "Declare" tags any more.
	/// @todo Algorithm should have a name to refer errors.
	public class XMLConfigurator : Config
	{
		/// store class alias. Tag <=> classname.
		Dictionary<String, String> _tagCache = new Dictionary<String, String>();
		
		public override void Load(String cfgPath)
		{
			XmlDocument cfgFile = new XmlDocument();
			cfgFile.Load(cfgPath);
			
			BuildTagCache(cfgFile);
			
			BuildHandlerList(cfgFile);
			
			ParseData(cfgFile);
			
			BuildAlgorithmList(cfgFile);
		}
		
		
		/// <summary> Get attributes from a xml node. </summary>
		/// <description> 
		/// Safely retrieves an attribute named attName from the node n which may be forced
		/// to have it. In case the attribute is missing returns null if it is optional or throw an
		/// MusiC.MissingAttributeException in case it is mandatory.
		/// </description>
		/// <param name="n">The System.XmlNode here the attribute might be</param>
		/// <param name="attName">The System.Attribute name</param>
		/// <param name="isOptional">Is it optional ?</param>
		/// <returns>The attribute value</returns>
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
		
		/// <summary>
		/// Get attributes from a xml node. 
		/// </summary>
		/// <param name="n">The System.XmlNode here the attribute might be</param>
		/// <param name="attName">The System.Attribute name</param>
		/// <returns>The attribute value</returns>
		/// @details This form sets the isOptional to false.
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
				_tagCache.Add(XmlSafeAttribute(n, "name"), XmlSafeAttribute(n, "class"));
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
					String className;
					ParamList paramList = new ParamList();
					
					if(_tagCache.TryGetValue(child.Name, out className))
					{
						foreach(XmlNode param in child.ChildNodes)
						{
							if(param.Name != "Param")
								continue;
							
							paramList.AddParam(XmlSafeAttribute(param, "name"), XmlSafeAttribute(param, "class"), XmlSafeAttribute(param, "value", true));
						}
						
						try
						{
							algorithm.Add(className, paramList);
						} catch (MissingExtensionException e)
						{
							Error("Error while loading an algorithm .... Skipping");
							Error(e);
							break;
						}
					}
					else
						Warning("Cant find tag "+child.Name);
				}
				
				AddAlgorithm(algorithm);
			}
		}
	}
}