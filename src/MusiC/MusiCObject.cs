using System;
using System.Reflection;

using System.Collections.Generic;

namespace MusiC
{
	public enum ReportLevel
	{
		NOTSET,
		ERROR,
		WARNING,
		MESSAGE,
		DEBUG,
	}
	
	
	
	public class MusiCObject
	{
		static int _level=0;
		static int _globalReportLevel=(int)ReportLevel.DEBUG;
		static String _indent = String.Empty;
		
		private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		
		int _localReportLevel=(int)ReportLevel.NOTSET;
		
		override public String ToString()
		{
			return base.ToString();
		}
		
		private void ToScreen(FieldInfo f)
		{
			Object o = f.GetValue(this);
			
			if(o == null)
				return;
			
			Type tObject = o.GetType();
			
			if(tObject.IsSubclassOf(typeof(MusiCObject)))
			{
				(o as MusiCObject).Print();
				return;
			}
			
			if(tObject.IsGenericType)
			{
				Message(tObject.ToString() + " [" + f.Name + "]: [NOT NULL]");
				// TODO: If it is a collection print elements
				
				return;
			}
			
			Message(tObject.ToString() + " [" + f.Name + "]: " + o.GetType().ToString() + " [" +	o.ToString() + "]");
		}
		
		public void Print()
		{	
			Message("+------- {" + this.GetType().ToString() + "} --------");
			Type t = this.GetType();
			
			ReportIndent();
			while(t != typeof(MusiCObject))
			{
				FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
	
				for(int i = 0; i < fields.Length; i++)
				{
					ToScreen(fields[i]);
				}
				
				t=t.BaseType;
			}
			ReportUnindent(false);
			Message("+-------------------------------------------------------------");
			Report("","");
		}
		
		public void Error(String msg)
		{
			int currentLvl = (_localReportLevel==(int)ReportLevel.NOTSET) ? _globalReportLevel : _localReportLevel;
			
			if(currentLvl >= (int)ReportLevel.ERROR)
				Report(msg, "ERROR");
		}
		
		public void Warning(String msg)
		{
			int currentLvl = (_localReportLevel==(int)ReportLevel.NOTSET) ? _globalReportLevel : _localReportLevel;
			
			if(currentLvl >= (int)ReportLevel.WARNING)
				Report(msg, "WARNING");
		}
		
		public void Message(String msg)
		{
			int currentLvl = (_localReportLevel==(int)ReportLevel.NOTSET) ? _globalReportLevel : _localReportLevel;
			
			if(currentLvl >= (int)ReportLevel.MESSAGE)
				Report(msg, "MESSAGE");
		}
		
		public void Debug(String msg)
		{
			int currentLvl = (_localReportLevel==(int)ReportLevel.NOTSET) ? _globalReportLevel : _localReportLevel;
			
			if(currentLvl >= (int)ReportLevel.DEBUG)
				Report(msg, "DEBUG");
		}
				
		void Report(String msg, String msgLvl)
		{
			String logLine = _indent + "[" + this.GetType().Name + "] [" + msgLvl + "]:" + msg;
			
			switch (msgLvl)
			{
				case "ERROR":
					_log.Error(logLine);
					break;
				case "WARNING":
					_log.Warn(logLine);
					break;
				case "DEBUG":
					_log.Debug(logLine);
					break;
				case "MESSAGE":
					_log.Info(logLine);
					break;
				default:
					_log.Error(""); //add new line
					break;
			}
		}
		
		public void ReportIndent()
		{
			int currentLvl = (_localReportLevel==(int)ReportLevel.NOTSET) ? _globalReportLevel : _localReportLevel;
			
			if(currentLvl != (int)ReportLevel.NOTSET)
			{
				//Console.WriteLine
				_log.Info(_indent+"{");
				_level++;
				_indent=_indent+".   ";
			}
		}
		
		public void ReportUnindent()
		{
			ReportUnindent(true);
		}
		
		public void ReportUnindent(bool extraNewLine)
		{
			int currentLvl = (_localReportLevel==(int)ReportLevel.NOTSET) ? _globalReportLevel : _localReportLevel;
			
			if(currentLvl != (int)ReportLevel.NOTSET)
			{
				_indent=_indent.Remove(0,4);
				_level--;
				//Console.WriteLine
				_log.Info(_indent + "}");
				
				if(extraNewLine)
					_log.Error("");
			}
		}
	}
}
