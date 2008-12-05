using System;

namespace MusiC.Exceptions
{
	public class MCException : ApplicationException
	{
		public class MCExceptionReporter : MusiCObject
		{
		}
		
		int msgCounter = 0;
		System.Collections.Generic.Queue<String> m_message = new System.Collections.Generic.Queue<String>();
		MCExceptionReporter _reporter = new MCExceptionReporter();
		
		public MCExceptionReporter Reporter
		{
			get {return _reporter;}
		}
		
		public System.Collections.Generic.IEnumerable<String> MessageList {
			get { return m_message; }
		}
		
		public MCException(String message) : base(message)
		{
			//m_message.Enqueue("[MusiC Messages]:");
			m_message.Enqueue((++msgCounter).ToString() + ". " + message);
		}
		
		public MCException(System.Exception e, String message) : base(e.Message, e)
		{	
			m_message.Enqueue("[SYSTEM]:" + e.Message);
			m_message.Enqueue("");
			//m_message.Enqueue("[MusiC Messages]:");
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
			_reporter.Error(this);
		}
	}
}
