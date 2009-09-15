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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Xml.Linq;

namespace MusiC.Extensions
{
	/// <summary>
	/// 
	/// </summary>
	public class ExtensionCache : MusiCObject
	{
		// Attributes
		private Configurator _objConfig;

		#region Static Attributes

		static readonly
		private XDocument _doc;

		static readonly
		private XElement _root;

		static readonly
		private LinkedList<ExtensionInfo> _infoList = new LinkedList<ExtensionInfo>();

		static readonly
		private LinkedList<IHandler> _hndPool = new LinkedList<IHandler>();

		#endregion

		//::::::::::::::::::::::::::::::::::::::://

		static
		ExtensionCache()
		{
			_doc = new XDocument(
				new XDeclaration( "1.0", "utf-16", "true" ),
				_root = new XElement( "MusiC.Extensions.Cache" ) );
		}

		//::::::::::::::::::::::::::::::::::::::://

		#region Extension Handling

		/// <summary>
		/// 
		/// </summary>
		/// <param name="extensionsDir">
		/// A <see cref="String"/>
		/// </param>
		public void LoadDir( String extensionsDir )
		{
			foreach( String ext in Directory.GetFiles( extensionsDir, "*.dll" ) )
			{
				// FIX: When MusiC.dll is loaded the Type.IsSubClassOf() fails.
				// FIX: No need to load log4net classes.
				if(
					Path.GetFileName( ext ) == "MusiC.dll" ||
					Path.GetFileName( ext ) == "log4net.dll"
					)

					continue;

				LoadExtension( ext );
			}
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// Load an extension file from the indicated path. May throw exceptions.
		/// </summary>
		/// <param name="ext"></param>
		public void LoadExtension( String ext )
		{
			if( !File.Exists( ext ) )
				Error( "Extensions file " + ext + " doesn't exists." );

			try
			{
				Assembly l = Assembly.LoadFrom( ext );
				BeginReportSection( ext + " ... [LOADED]" );

				foreach( Type t in l.GetExportedTypes() )
				{
					Add( t );
				}

				EndReportSection( true );
			}
			catch( BadImageFormatException )
			{
				// Probably trying to load an unmanaged assembly.
				Warning( ext + " ... [FAILED - UNMANAGED]" );
			}
			catch( Exception ex )
			{
				Error( ex );
			}
		}

		//::::::::::::::::::::::::::::::::::::::://

		private XElement BuildEntry( ExtensionInfo info )
		{
			XElement entry = new XElement( "MusiC.Extension" );
			entry.Add( new XAttribute( "Index", _infoList.Count() ) );
			entry.Add( new XElement( "Class", info.Class.FullName ) );
			entry.Add( new XElement( "Kind", info.Kind.ToString() ) );
			entry.Add( new XElement( "Model", info.Model.ToString() ) );

			if( info.Kind == ExtensionKind.FileHandler )
				entry.Add( new XElement( "Handler_Index", _hndPool.Count() ) );

			return entry;
		}

		//::::::::::::::::::::::::::::::::::::::://

		public void Add( Type extensionType )
		{

			if( ExtensionInfo.Identify( extensionType ) == ExtensionKind.Error )
			{
				Message( extensionType.ToString() + " ... [REJECTED]" );
				return;
			}

			ExtensionInfo info = new ExtensionInfo( extensionType );

			// Check for duplicated entries.
			IEnumerable<ExtensionInfo> result =
			from ExtensionInfo _info in _infoList where _info.Class.FullName == extensionType.FullName select _info;

			if( result.Count() != 0 )
			{
				Message( extensionType.ToString() + " ... [REJECTED - DUPLICATED]" );
				return;
			}

			_root.Add( BuildEntry( info ) );
			_infoList.AddLast( info );

			if( info.Kind == ExtensionKind.FileHandler )
				_hndPool.AddLast( info.Instantiate( null ) as IHandler );

			Message( extensionType.ToString() + " ... [ADDED]" );
		}

		#endregion

		//::::::::::::::::::::::::::::::::::::::://

		#region FileHandlers Handling

		/// <summary>
		/// 
		/// </summary>
		/// <param name="file">
		/// A <see cref="String"/>
		/// </param>
		/// <returns>
		/// A <see cref="Managed.Handler"/>
		/// </returns>
		static
		public Managed.Handler GetManagedHandler( String file )
		{
			//			foreach(IHandler h in _handlerList)
			//			{
			//				if(h.CanHandle(file) && ExtensionManagement.Managed == ExtensionInfo.IdentifyManagement(h.GetType()))
			//				{
			//					return h as Managed.Handler;
			//				}
			//			}

			return null;
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="file">
		/// A <see cref="String"/>
		/// </param>
		/// <returns>
		/// A <see cref="Unmanaged.Handler"/>
		/// </returns>
		[CLSCompliant( false )]
		static
		public Unmanaged.Handler GetUnmanagedHandler( String file )
		{
			IHandler hnd = null;
			// Get possible extensions
			IEnumerable<IHandler> result =
				from element in _root.Elements( "MusiC.Extension" )
				where element.Descendants( "Kind" ).ElementAt( 0 ).Value == "FileHandler"
				where (
					   element.Descendants( "Model" ).ElementAt( 0 ).Value == "Unmanaged" ||
					   element.Descendants( "Model" ).ElementAt( 0 ).Value == "Both"
					   )
				where ( hnd = _hndPool.ElementAt( int.Parse( element.Elements( "Handler_Index" ).ElementAt( 0 ).Value ) ) ).CanHandle( file )
				select hnd;

			//Message("GetUnmanagedHandler - Handlers Found: " + result.Count());

			if( result.Count() > 0 )
				return result.ElementAt( 0 ) as Unmanaged.Handler;
			else
				return null;
		}

		#endregion

		//::::::::::::::::::::::::::::::::::::::://

		#region Configurator Handling

		/// <summary>
		/// 
		/// </summary>
		/// <param name="file">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="Configurator"/>
		/// </returns>
		/// <exception cref="MusiC.Exceptions.MissingExtensionException"></exception>
		public Configurator GetConfigurator( string file )
		{
			if( _objConfig != null )
				return _objConfig;

			IEnumerable<ExtensionInfo> result =
				from ExtensionInfo info in _infoList where info.Kind == ExtensionKind.Configuration select info;

			if( result.Count() == 0 )
				throw new Exceptions.MissingExtensionException( "MusiC doesn't know how to load a configuration extension. Seems none was provided or aproved." );

			_objConfig = Invoker.LoadConfig( result.ElementAt( 0 ).Class );

			return _objConfig;
		}

		#endregion

		//::::::::::::::::::::::::::::::::::::::://

		#region Info Handling

		/// <summary>
		/// 
		/// </summary>
		/// <param name="className">
		/// A <see cref="String"/>
		/// </param>
		/// <returns>
		/// A <see cref="ExtensionInfo"/>
		/// </returns>
		static public ExtensionInfo GetInfo( String className )
		{
			IEnumerable<ExtensionInfo> result =
			from ExtensionInfo _info in _infoList where _info.Class.FullName == className select _info;

			return result.ElementAt( 0 );
		}

		#endregion

		//::::::::::::::::::::::::::::::::::::::://

		#region Checkers

		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public bool HasConfig()
		{
			IEnumerable<ExtensionInfo> result =
				from ExtensionInfo info in _infoList where info.Kind == ExtensionKind.Configuration select info;

			return ( result.Count() > 0 );
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public bool HasFileHandler()
		{
			IEnumerable<ExtensionInfo> result =
				from ExtensionInfo info in _infoList where info.Kind == ExtensionKind.FileHandler select info;

			return ( result.Count() > 0 );
		}

		#endregion

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		public void Say()
		{
			Message( _doc.ToString() );
		}
	}
}
