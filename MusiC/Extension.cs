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

namespace MusiC
{
	/// <summary>
	/// 
	/// </summary>
	abstract 
	public class Extension : MusiCObject
	{
        string _id;
        
        //::::::::::::::::::::::::::::::::::::::://

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        /// <remarks>Any aditional parameter the feature takes should be added here. This string is used
        /// to know if the stored feature is the same as the one we want to extract.</remarks>
        virtual
        public string GetID()
        {
            return _id;
        }
        
        //::::::::::::::::::::::::::::::::::::::://
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pList"></param>
        public void BuildID(ParamList pList)
        {
            _id = this.GetType().FullName;

            foreach (Instantiable i in pList.GetParamList())
            {
                _id += ":" + i.StrValue;
            }
        }
	}
}

