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
	/// <remarks>
	/// You can add any non-specified tag to the file and the parser will ignore it. Just try to avoid tags which begins
	/// by 'MusiC-*' because those may be used later. 
	/// </remarks>
	/// @todo Algorithm should have a name to refer errors.
	/// @todo Find MusiC node and use it instead of document.
	/// @todo Update parser to run each node instead of GetElements.
	public class XMLConfigurator : Configurator
	{
		/// store class alias. Tag <=> classname.
		Dictionary<String, String> _tagCache = new Dictionary<String, String>();
		
		public override void Load(String cfgPath)
		{
			XmlDocument cfgFile = new XmlDocument();
			cfgFile.Load(cfgPath);
			
			BuildTagCache(cfgFile);
			
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
		
		void BuildTagCache(XmlDocument cfgFile)
		{
			foreach (XmlNode n in cfgFile.GetElementsByTagName("MusiC-Alias"))
			{				
				_tagCache.Add(XmlSafeAttribute(n, "name"), XmlSafeAttribute(n, "class"));
			}
		}
		
		void ParseData(XmlDocument cfgFile)
		{
			foreach(XmlNode train in cfgFile.GetElementsByTagName("MusiC-Train"))
			{
				String baseDir = XmlSafeAttribute(train, "dir", true);
				
				XmlNodeList nList = train.ChildNodes;
				foreach (XmlNode xmlLabelNode in nList)
				{
					if(xmlLabelNode.Name == "Label")
					{
						ILabel l = New.Label();
						l.Label=XmlSafeAttribute(xmlLabelNode,"name");
						
						/// @todo Add support to multiple input dirs
						l.InputDir=XmlSafeAttribute(xmlLabelNode,"input", true);
						if (l.InputDir==null)
						{
							/// @todo Check baseDir availability
							l.InputDir = System.IO.Path.Combine(baseDir, l.Label);
						}
						
						l.OutputDir=XmlSafeAttribute(xmlLabelNode,"output", true);
						
						if(l.OutputDir == null)
							l.OutputDir=l.InputDir;
						
						AddTrainLabel(l);
					}
				}
			}
			
			// Adding Train/Classify support
			foreach(XmlNode classifyNode in cfgFile.GetElementsByTagName("MusiC-Classify"))
			{
				/// @todo Add support to allow/deny algorithms to use this folder
				
				String dir = XmlSafeAttribute(classifyNode,"dir");
				
				if(!AddDir(dir))
					Warning("A non-existent directory ("+ dir +") wasn't added to the processing queue.");
			}
		}
		
		public void BuildAlgorithmList(XmlDocument cfgFile)
		{
			foreach (XmlNode n in cfgFile.GetElementsByTagName("MusiC-Algorithm"))
			{
				IAlgorithm algorithm = New.Algorithm();
				String className;

				foreach (XmlNode child in n.ChildNodes)
				{
					if(child.Name == "MusiC-Extension")
						className = XmlSafeAttribute(child, "name");
					else
					{
						if(!_tagCache.TryGetValue(child.Name, out className))
						{
							Warning("Cant find tag "+child.Name);
							break;
						}
					}
					
					IParamList paramList = New.ParamList();
					
					foreach(XmlNode param in child.ChildNodes)
					{
						if(param.Name != "Param")
							continue;
						
						paramList.AddParam(XmlSafeAttribute(param, "name"), XmlSafeAttribute(param, "class"), XmlSafeAttribute(param, "value", true));
					}
					
					try
					{
						if(!algorithm.Add(className, paramList))
							break;
					} catch (MissingExtensionException e)
					{
						Error("Error while loading an algorithm .... Skipping");
						Error(e);
						break;
					}
				}
				
				AddAlgorithm(algorithm);
			}
		}
	}
}