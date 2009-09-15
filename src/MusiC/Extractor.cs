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
using System.Text;

using MusiC.Data;

namespace MusiC
{
	/// <summary>
	/// 
	/// </summary>
	internal class BaseExtractor : MusiCObject
	{
	}

	//---------------------------------------//

	namespace Managed
	{
		/// <summary>
		/// 
		/// </summary>
		internal class Extractor
		{
			static
			public Data.Managed.DataCollection Extract( Window wnd, Feature f )
			{
				return null;
			}
		}
	}

	//---------------------------------------//

	namespace Unmanaged
	{
		/// <summary>
		/// 
		/// </summary>
		unsafe
		internal class FeatureHelper
		{
			public Feature feat;
			public float* data;
			public bool extracted;
		}

		//---------------------------------------//

		/// <summary>
		/// 
		/// </summary>
		static
		class Extractor
		{
			private class ExtractorReporter : Reporter
			{
			}

			//---------------------------------------//

			static
			private ExtractorReporter reporter = new ExtractorReporter();

			//::::::::::::::::::::::::::::::::::::::://

			/// <summary>
			/// 
			/// </summary>
			/// <param name="wnd">
			/// A <see cref="Window"/>
			/// </param>
			/// <param name="featList">
			/// A <see cref="IEnumerable<Feature>"/>
			/// </param>
			/// <param name="dataStg">
			/// A <see cref="Data.Unmanaged.FileData"/>
			/// </param>
			static unsafe
			public Data.Unmanaged.FileData* Extract( Window wnd, IEnumerable<Feature> featList, Config cfg )
			{
				int fIdx;
				Frame dataFrame;
				int featCount = 0;

				BaseHandler bdb = wnd.HandlerInterface as BaseHandler;
				DBHandler db = bdb.GetDBHandler();

				// Options
				bool ok, save, force_extract;

				ok = cfg.GetBoolOption( Config.Option.ALLOW_SAVE, out save );
				if( !ok ) save = true;

				ok = cfg.GetBoolOption( Config.Option.FORCE_EXTRACT, out force_extract );
				if( !ok ) force_extract = false;

				uint frameCount = 2944;

				uint samples = ( uint ) ( frameCount * ( wnd.WindowSize - wnd.WindowOverlap ) + wnd.WindowOverlap );
				// Centering Samples
				uint first_sample = ( uint ) Math.Floor( ( float ) ( wnd.HandlerInterface.GetStreamSize() - ( samples ) ) / 2 );
				//uint last_sample = first_sample + frameCount - 1;

				if( frameCount > wnd.WindowCount )
					return null;

				LinkedList<FeatureHelper> parsedList = new LinkedList<FeatureHelper>();

				// Files might have different features extracted. So we need to ask
				// each one if the feature is available.
				foreach( Feature f in featList )
				{
					FeatureHelper fh = new FeatureHelper();
					fh.feat = f;
					fh.feat.Clear(); // restarts feature
					fh.data = NativeMethods.Pointer.fgetmem( wnd.WindowCount );

					// fh.data is null if the window/feature combination is not available
					reporter.AddMessage( "Searching File DB" );
					int read = db.GetFeature( wnd.GetID(), f.GetID(), fh.data );

					if( read == 0 )
					{
						reporter.AddMessage( "Feature Not Found" );
						fh.extracted = false;
					}
					else
					{
						reporter.AddMessage( "Feature Found - Bytes Read: " + read );
						fh.extracted = true;
					}

					featCount++;
					parsedList.AddLast( fh );
				}

				//reporter.AddMessage( "Number of Frames: " + wnd.WindowCount );

				Data.Unmanaged.FileData* dataStg = Data.Unmanaged.DataHandler.BuildFileData();
				
				float value;
				for( uint i = 0; i < frameCount; i++ )
				{
					Data.Unmanaged.FrameData* frame =
						Data.Unmanaged.DataHandler.BuildFrameData( featCount );

					Data.Unmanaged.DataHandler.AddFrameData( frame, dataStg );

					fIdx = 0;
					foreach( FeatureHelper fh in parsedList )
					{
						if( !fh.extracted || force_extract )
						{
							dataFrame = wnd.GetWindow( i, first_sample );

							if( !dataFrame.IsValid() )
								break;

							// if it hasn't been extracted yet then extract it and prepare to store it.
							value = fh.feat.Extract( dataFrame );
							
							if( float.IsInfinity( value ) || float.IsNaN( value ) )
								break;

							*( fh.data + i ) = value;
						}

						*( frame->pData + fIdx ) = *( fh.data + i );

						fIdx++;
					}
				}

				if( save )
				{
					// storing unextracted data.
					foreach( FeatureHelper fh in parsedList )
					{
						if( !fh.extracted || force_extract )
							db.AddFeature( wnd.GetID(), fh.feat.GetID(), fh.data, wnd.WindowCount );

						NativeMethods.Pointer.free( fh.data );
					}

					db.Terminate();
				}

				return dataStg;
			}
		}
	}
}