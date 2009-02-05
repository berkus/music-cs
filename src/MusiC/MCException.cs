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

namespace MusiC.Exceptions
{
	/// <summary>
	/// 
	/// </summary>
	public class MCException : ApplicationException
	{
		//// <value>
		/// 
		/// </value>
		class MCExceptionReporter : MusiCObject
		{
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e">
			/// A <see cref="MCException"/>
			/// </param>
			public void ReportException(MCException e)
			{
				Error(e);
			}
		}

		//::::::::::::::::::::::::::::::::::::::://
		
		private int msgCounter = 0;
		private Queue<string> m_message = new Queue<string>();
		private MCExceptionReporter _reporter = new MCExceptionReporter();

		//::::::::::::::::::::::::::::::::::::::://
		
//		public MCExceptionReporter Reporter
//		{
//			get {return _reporter;}
//		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		public System.Collections.Generic.IEnumerable<String> MessageList {
			get { return m_message; }
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		public MCException(String message) : base(message)
		{
			//m_message.Enqueue("[MusiC]:");
			m_message.Enqueue((++msgCounter).ToString() + ". " + message);
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e">
		/// A <see cref="System.Exception"/>
		/// </param>
		/// <param name="message">
		/// A <see cref="System.String"/>
		/// </param>
		public MCException(System.Exception e, String message) : base(e.Message, e)
		{	
			m_message.Enqueue("[SYSTEM]:" + e.Message);
			m_message.Enqueue("");
			//m_message.Enqueue("[MusiC]:");
			m_message.Enqueue((++msgCounter).ToString() + ". " + message);
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message">
		/// A <see cref="System.String"/>
		/// </param>
		public void AddMessage(string message)
		{
			m_message.Enqueue((++msgCounter).ToString() + ". " + message);
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/>
		/// </returns>
		public String GetMyName()
		{
			return null;
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		public void Report()
		{
			_reporter.ReportException(this);
		}
	}
}
