/*
 * The MIT License
 * Copyright (c) 2008-2009 Marcos Jos� Sant'Anna Magalh�es
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
 */

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
			public Data.Managed.DataCollection Extract(Window wnd, Feature f)
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
			public float * data;
			public bool extracted;
		}
		
		//---------------------------------------//
		
		/// <summary>
		/// 
		/// </summary>
		class Extractor
		{
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
			public void Extract(Window wnd, IEnumerable<Feature> featList, Data.Unmanaged.FileData * dataStg)
			{
				int fIdx;
                float * windowBuffer;
				
				BaseHandler bdb = wnd.HandlerInterface as BaseHandler;
				DBHandler db = bdb.GetDBHandler();
				
				LinkedList<FeatureHelper> parsedList = new LinkedList<FeatureHelper>();
				
				// Files might have different features extracted or not. So we need to ask
				// each one if the feature is available.
				foreach( Feature f in featList )
				{
					FeatureHelper fh = new FeatureHelper();
					fh.feat = f;
									
					// fh.data is null if the window/feature combination is not available
					db.GetFeature(wnd.GetID(), f.GetID(), fh.data);
					
					if( fh.data == null )
					{
						fh.extracted = false;
						fh.data = NativeMethods.Pointer.fgetmem(wnd.WindowCount);
					}
					
					parsedList.AddLast(fh);
				}

				for( int i = 0; i < wnd.WindowCount; i++ )
				{
					windowBuffer = wnd.GetWindow(i);
					
					if (windowBuffer == null)
                    {
						continue;
					}
					
					Data.Unmanaged.FrameData* frame = 
						Data.Unmanaged.DataHandler.BuildFrameData(dataStg);
					
                    fIdx = 0;
                    foreach( FeatureHelper fh in parsedList )
                    {
						if( fh.extracted )
						{
							// If the data has been extracted already get the value correspondent
							// to the current window.
							*(frame->pData + fIdx) = *(fh.data + i);
						}
						else
						{
							// if it hasn't been extracted yet then extract it and prepare to store it.
							/// @todo Shield this method against any changes in the data structure.

							// This code breaks mono compiler. It generates invalid IL instructions.
							//*(fh.data + i) = *(frame->pData + fIdx) = fh.feat.Extract(windowBuffer, wnd.WindowSize);
							
							float val = fh.feat.Extract(windowBuffer, wnd.WindowSize);
							*(frame->pData + fIdx) = val;
							*(fh.data + i) = val;
						}

						fIdx++;
                    }
				}
				
				// storing unextracted data.
				foreach( FeatureHelper fh in parsedList )
				{
					if( !fh.extracted )
						db.AddFeature(wnd.GetID(), fh.feat.GetID(), fh.data, wnd.WindowCount);
				}
			}
		}
	}
}