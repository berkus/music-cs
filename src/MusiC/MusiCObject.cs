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
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

using MusiC.Exceptions;

namespace MusiC
{
	public enum ReportLevel
	{
		NotSet,
		Error,
		Warning,
		Message,
		Debug,
	}
	/// <summary>
	/// Base class to ALL other classes.
	/// 
	/// This class provides useful methods (notably logging) to other classes.
	/// </summary>
	public class MusiCObject
	{
		static int _level=0;
		static int _globalReportLevel=(int)ReportLevel.Debug;
		static String _indent = String.Empty;
		
		 static readonly private log4net.ILog _log;
		
		int _localReportLevel=(int)ReportLevel.NotSet;
		
		static MusiCObject()
		{
			log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("music.log.xml"));
			_log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		}
		
		override public String ToString()
		{
			return base.ToString();
		}
		
#region Not-In-Use
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
#endregion
		
		private void Report(String msg, String msgLvl)
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
		
		public void Print()
		{	
			BeginReportSection("+------- {" + this.GetType().ToString() + "} --------");
			Type t = this.GetType();
			
			while(t != typeof(MusiCObject))
			{
				FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
	
				for(int i = 0; i < fields.Length; i++)
				{
					ToScreen(fields[i]);
				}
				
				t=t.BaseType;
			}
			EndReportSection(true);
		}
		
		protected void Error(String msg)
		{
			int currentLvl = (_localReportLevel==(int)ReportLevel.NotSet) ? _globalReportLevel : _localReportLevel;
			
			if(currentLvl >= (int)ReportLevel.Error)
				Report(msg, "ERROR");
		}
		
		protected void Error(MCException ex)
		{
			Error("[ MCException ]");
			
			foreach(string message in ex.MessageList)
				Error(message);
			
			StackFrame entry;
			StackTrace st = new StackTrace(ex, true);
			
			Error("[ MCException: STACK TRACE ]");
			
			for(int idx=0; idx < st.FrameCount; idx++)
			{
				entry = st.GetFrame(idx);
				Error("[" + entry.GetMethod().Name+"] - " +
					System.IO.Path.GetFileName(entry.GetFileName()) +
					" (L" + entry.GetFileLineNumber().ToString()+
					", C"+entry.GetFileColumnNumber()+")");
			}
		}
		
		protected void Error(Exception ex)
		{
			Error("[ System Exception ]");
			
			Error(ex.Message);
			
			StackFrame entry;
			StackTrace st = new StackTrace(ex, true);
			
			Error("[ StackTrace ]");
			
			for(int idx=0; idx < st.FrameCount; idx++)
			{
				entry = st.GetFrame(idx);
				Error("[" + entry.GetMethod().Name+"] - " +
					System.IO.Path.GetFileName(entry.GetFileName()) +
					" (L" + entry.GetFileLineNumber().ToString()+
					", C"+entry.GetFileColumnNumber()+")");
			}
		}
		
		protected void Warning(String msg)
		{
			int currentLvl = (_localReportLevel==(int)ReportLevel.NotSet) ? _globalReportLevel : _localReportLevel;
			
			if(currentLvl >= (int)ReportLevel.Warning)
				Report(msg, "WARNING");
		}
		
		protected void Message(String msg)
		{
			int currentLvl = (_localReportLevel==(int)ReportLevel.NotSet) ? _globalReportLevel : _localReportLevel;
			
			if(currentLvl >= (int)ReportLevel.Message)
				Report(msg, "MESSAGE");
		}
		
		protected void Debug(String msg)
		{
			int currentLvl = (_localReportLevel==(int)ReportLevel.NotSet) ? _globalReportLevel : _localReportLevel;
			
			if(currentLvl >= (int)ReportLevel.Debug)
				Report(msg, "DEBUG");
		}
		
		protected void BeginReportSection(String sectionName)
		{
			int currentLvl = (_localReportLevel==(int)ReportLevel.NotSet) ? _globalReportLevel : _localReportLevel;
			
			if(currentLvl != (int)ReportLevel.NotSet)
			{
				_log.Info(_indent+sectionName);
				_log.Info(_indent+"{");
				_level++;
				_indent=_indent+".   ";
			}
		}
		
		protected void EndReportSection(bool addNewLine)
		{
			int currentLvl = (_localReportLevel==(int)ReportLevel.NotSet) ? _globalReportLevel : _localReportLevel;
			
			if(currentLvl != (int)ReportLevel.NotSet)
			{
				_indent=_indent.Remove(0,4);
				_level--;
				_log.Info(_indent + "}");
			}
			
			if(addNewLine)
				AddNewLine();
		}
		
		protected void AddNewLine()
		{
			_log.Info("");
		}
	
		static public void UnhandledException(Object sender, UnhandledExceptionEventArgs args)
		{
			MCException ex = args.ExceptionObject as MCException;
			
			if(ex != null)
			{
				//ex.Reporter.Error("This library or one of its components generated an unhandled exception.");
				//ex.Reporter.Error("Please report this event at our site, http://sites.google.com/site/music-cs");
				ex.Report();
			}
		}
	}
}
