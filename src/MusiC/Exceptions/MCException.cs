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

namespace MusiC.Exceptions
{
	public class MCException : ApplicationException
	{
		class MCExceptionReporter : MusiCObject
		{
			public void ReportException(MCException e)
			{
				Error(e);
			}
		}
		
		int msgCounter = 0;
		System.Collections.Generic.Queue<String> m_message = new System.Collections.Generic.Queue<String>();
		MCExceptionReporter _reporter = new MCExceptionReporter();
		
//		public MCExceptionReporter Reporter
//		{
//			get {return _reporter;}
//		}
		
		public System.Collections.Generic.IEnumerable<String> MessageList {
			get { return m_message; }
		}
		
		public MCException(String message) : base(message)
		{
			//m_message.Enqueue("[MusiC]:");
			m_message.Enqueue((++msgCounter).ToString() + ". " + message);
		}
		
		public MCException(System.Exception e, String message) : base(e.Message, e)
		{	
			m_message.Enqueue("[SYSTEM]:" + e.Message);
			m_message.Enqueue("");
			//m_message.Enqueue("[MusiC]:");
			m_message.Enqueue((++msgCounter).ToString() + ". " + message);
		}
		
		public void AddMessage(string message)
		{
			m_message.Enqueue((++msgCounter).ToString() + ". " + message);
		}
		
		public String GetMyName()
		{
			return null;
		}
		
		public void Report()
		{
			_reporter.ReportException(this);
		}
	}
}
