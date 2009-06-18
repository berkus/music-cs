// Copyright (c) 2008-2009 Marcos José Sant'Anna Magalhães
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.


using System;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Collections.Generic;

using MusiC;
using MusiC.Extensions;
using MusiC.Exceptions;

namespace MusiC.Extensions.Configs
{
	/// <summary>
	/// A XML parser that knows how to configure the library.
	/// </summary>
	/// <remarks>
	/// You can add any non-specified tag to the file and the parser will ignore it. Just try to avoid tags which begins
	/// by 'MusiC-*' because those may be used later. 
	/// </remarks>
	/// <see cref="MusiC.Configurator"/>
	/// @todo Algorithm should have a name to refer errors.
	/// @todo Find MusiC node and use it instead of document.
	/// @todo Update parser to run each node instead of GetElements.
	public class XMLConfigurator : Configurator
	{
		/// store class alias. Tag(key) <=> classname(value).
		private Dictionary<String, String> _tagCache = new Dictionary<String, String>();

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cfgPath">
		/// XML file with library configuration.
		/// </param>
		override
		protected void Load( String cfgPath )
		{
			XmlDocument cfgFile = new XmlDocument();
			cfgFile.Load( cfgPath );

			XmlNodeList rootCandidates = cfgFile.GetElementsByTagName( "MusiC" );

			if( rootCandidates.Count != 1 )
			{
				new MCException( "It is allowed and required to have just 1 MusiC node." );
				return;
			}

			XmlNode root = rootCandidates[ 0 ];

			foreach( XmlNode node in root.ChildNodes )
			{
				switch( node.Name )
				{
					case "MusiC-Alias":
						_tagCache.Add( XmlSafeAttribute( node, "name" ), XmlSafeAttribute( node, "class" ) );
						break;
					case "MusiC-Train":
						HandleNode_Train( node );
						break;
					case "MusiC-Classify":
						HandleNode_Classify( node );
						break;
					case "MusiC-Algorithm":
						HandleNode_Algorithm( node );
						break;
				}
			}
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="file">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		override
		public bool CanHandle( string file )
		{
			///@todo check version.
			return Path.GetExtension( file ).ToUpper() == ".XML";
		}

		//::::::::::::::::::::::::::::::::::::::://

		#region XMLAttribute Handling
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
		private String XmlSafeAttribute( XmlNode n, String attName, bool isOptional )
		{
			if( n == null )
				throw new MCException( "XmlNode submited is null" );

			XmlAttribute att = n.Attributes[ attName ];

			if( att == null )
			{
				if( !isOptional )
					throw new MissingAttributeException( attName );
				return null;
			}

			return att.Value;
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// Get attributes from a xml node. 
		/// </summary>
		/// <param name="n">The System.XmlNode here the attribute might be</param>
		/// <param name="attName">The System.Attribute name</param>
		/// <returns>The attribute value</returns>
		/// @details This form sets the isOptional to false.
		private String XmlSafeAttribute( XmlNode n, String attName )
		{
			return XmlSafeAttribute( n, attName, false );
		}
		#endregion

		//::::::::::::::::::::::::::::::::::::::://

		#region Parsing Methods

		/// <summary>
		/// 
		/// </summary>
		/// <param name="train">
		/// A <see cref="XmlNode"/>
		/// </param>
		private void HandleNode_Train( XmlNode node_train )
		{
			string baseDir = XmlSafeAttribute( node_train, "dir", true );
			if( baseDir == null ) { baseDir = Path.GetDirectoryName( Assembly.GetEntryAssembly().Location ); }

			foreach( XmlNode node_label in node_train.ChildNodes )
			{
				string path;

				Label l = new Label();
				l.Name = XmlSafeAttribute( node_label, "name" );

				if( node_label.Name == "Label" )
				{
					foreach( XmlNode node_path in node_label )
					{
						switch( node_path.Name )
						{
							case "Dir":
								path = XmlSafeAttribute( node_path, "path", true );
								if( path == null ) { path = System.IO.Path.Combine( baseDir, l.Name ); }

								string recursive_text = XmlSafeAttribute( node_path, "recursive", true );
								string filter = XmlSafeAttribute( node_path, "filter", true );

								bool recursive = false;

								try
								{ recursive = ( recursive_text == null ) ? false : bool.Parse( recursive_text ); }
								catch( Exception e )
								{ throw new MCException( e, "recursive values: True or False (case INSENSITIVE)" ); }

								l.AddInputDir( path, recursive, filter );
								break;

							case "File":
								path = XmlSafeAttribute( node_path, "path" );
								l.AddInputFile( path );
								break;
						}
					}
				}

				AddTrainLabel( l );
			}


			//l.OutputDir = XmlSafeAttribute(xmlLabelNode,"output", true);
			//if(l.OutputDir == null)
			//	l.OutputDir=iDir;
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="classify"></param>
		private
		void HandleNode_Classify( XmlNode node_classify )
		{
			foreach( XmlNode node in node_classify.ChildNodes )
			{
				string path;

				switch( node.Name )
				{
					case "Dir":
						path = XmlSafeAttribute( node, "path" );
						string recursive_text = XmlSafeAttribute( node, "recursive", true );
						string filter = XmlSafeAttribute( node, "filter", true );

						bool recursive = false;

						try
						{ recursive = ( recursive_text == null ) ? false : bool.Parse( recursive_text ); }
						catch( Exception e )
						{ new MCException( e, "recursive values: Yes or No (case INSENSITIVE)" ); }

						AddClassificationDir( path, recursive, filter );
						break;

					case "File":
						path = XmlSafeAttribute( node, "path" );
						AddClassificationFile( path );
						break;
				}
			}
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="node_algorithm"></param>
		public void HandleNode_Algorithm( XmlNode node_algorithm )
		{
			string algName = XmlSafeAttribute( node_algorithm, "name", true );

			if( algName == null )
				algName = "Algorithm_" + GetAlgorithmCount();

			Algorithm algorithm = new Algorithm( algName );
			String className;

			foreach( XmlNode child in node_algorithm.ChildNodes )
			{
				if( child.Name == "MusiC-Extension" )
				{
					className = XmlSafeAttribute( child, "class" );
				}
				else
				{
					if( !_tagCache.TryGetValue( child.Name, out className ) )
					{
						Warning( "Cant find tag " + child.Name );
						break;
					}
				}

				ParamList paramList = new ParamList();

				foreach( XmlNode param in child.ChildNodes )
				{
					if( param.Name != "Param" )
						continue;

					paramList.AddParam(
									   XmlSafeAttribute( param, "name" ),
									   XmlSafeAttribute( param, "class" ),
									   XmlSafeAttribute( param, "value", true ) );
				}

				try
				{
					if( !algorithm.Add( className, paramList ) )
						break;
				}
				catch( MissingExtensionException e )
				{
					Error( "Error while loading an algorithm .... Skipping" );
					Error( e );
					break;
				}
			}

			AddAlgorithm( algorithm );
		}

		#endregion
	}
}