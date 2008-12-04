using System;
using System.Diagnostics;

using MusiC.Exceptions;

namespace MusiC
{
	public class Report
	{
		static public void Error(MCException ex)
		{
			Console.WriteLine();
			Console.WriteLine("--------- [ERROR] ---------");
			Console.WriteLine();
			
			foreach(string message in ex.MessageList)
				Console.WriteLine(message);
			
			StackFrame entry;
			StackTrace st = new StackTrace(ex, true);
			
			Console.WriteLine();
			Console.WriteLine("--------- [STACK TRACE] ---------");
			Console.WriteLine();
			
			for(int idx=0; idx < st.FrameCount; idx++)
			{
				entry = st.GetFrame(idx);
				Console.WriteLine("[" + entry.GetMethod().Name+"] - " +
					System.IO.Path.GetFileName(entry.GetFileName()) +
					" (L" + entry.GetFileLineNumber().ToString()+
					", C"+entry.GetFileColumnNumber()+")");
			}
		}
		
		static public void Error(Exception ex)
		{
			Console.WriteLine();
			Console.WriteLine("--------- [ERROR] ---------");
			Console.WriteLine();
			
			Console.WriteLine("[SYSTEM MESSAGE]: " + ex.Message);
			
			StackFrame entry;
			StackTrace st = new StackTrace(ex, true);
			
			Console.WriteLine();
			Console.WriteLine("--------- [STACK TRACE] ---------");
			Console.WriteLine();
			
			for(int idx=0; idx < st.FrameCount; idx++)
			{
				entry = st.GetFrame(idx);
				Console.WriteLine("[" + entry.GetMethod().Name+"] - " +
					System.IO.Path.GetFileName(entry.GetFileName()) +
					" (L" + entry.GetFileLineNumber().ToString()+
					", C"+entry.GetFileColumnNumber()+")");
			}
		}
		
		static public void UnhandledException(Object sender, UnhandledExceptionEventArgs args)
		{
			MCException ex = args.ExceptionObject as MCException;
			
			if(ex==null)
				Error(args.ExceptionObject as Exception);
			
			Error(ex);
		}
	}
}
