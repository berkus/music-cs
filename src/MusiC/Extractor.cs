/*
 * The MIT License
 * Copyright (c) 2008-2009 Marcos José Sant'Anna Magalhães
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
	public class Extractor
	{
	}
	
	namespace Managed
	{
		public class Extractor
		{
			static public Data.Managed.MCDataCollection Extract(Window wnd, Feature f)
			{
				return null;
			}
		}
	}
	
	namespace Unmanaged
	{
		[CLSCompliant(false)]
		public class Extractor
		{
			static unsafe public void Extract(Window wnd, IEnumerable<Feature> featList, Data.Unmanaged.FileData * dataStg)
			{
				int fIdx;
                float * windowBuffer;

				for(int i = 0; i < wnd.WindowCount; i++)
				{
                    fIdx = 0;
                    foreach (Feature f in featList)
                    {
                        windowBuffer = wnd.GetWindow(i);

                        if (windowBuffer != null)
                        {
                        	Data.Unmanaged.FrameData* frame = Data.Unmanaged.DataHandler.BuildFrameData(dataStg);
                            *(frame->pData + fIdx) = f.Extract(windowBuffer, wnd.WindowSize);
                        }
                    }
				}
			}
		}
	}
}