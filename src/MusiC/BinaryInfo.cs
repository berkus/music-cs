using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using MCModule.Exceptions;

namespace MCModule
{
	public class BinaryInfo
	{	
		// Private members
		
		String mLib = String.Empty;
		String mClass = String.Empty;
		String mDir = System.IO.Directory.GetCurrentDirectory();
		int mLine = -1;
				
		LinkedList<BinaryParam> mParam = new LinkedList<BinaryParam>();
		
		// Properties
		
		public String Lib
		{
			get { return mLib; }
			set { if (value != null) { mLib = value; } }
		}
	
		public String Class
		{
			get { return mClass; }
			set { if (value != null) { mClass = value; } }
		}
	
		public String Dir
		{
			get { return mDir; }
			set { if (value != null) { mDir = value; } }
		}
		
		public int Line
		{
			get { return mLine; }
			set { mLine = value; }
		}
		
		// Public Methods
		
		public Extension New()
		{
			return Invoker.LoadType(this);
		}
		
		public void Parse(XmlNode n)
		{
			XmlAttribute attr = n.Attributes["library"];
			Lib = attr != null ? attr.Value : null;
	
			attr = n.Attributes["directory"];
			Dir = attr != null ? attr.Value : null;
	
			attr = n.Attributes["class"];
			if(attr == null) throw new MissingInformationException("class", n);
			else 	Class = attr.Value;
		}
			
		public void AddParam(String tag, Type t)
		{
			mParam.AddLast(new BinaryParam(tag, t));
		}
	
		public LinkedList<BinaryParam> GetParam()
		{
			return mParam;
		}
	
		public Type[] GetTypes()
		{
			Type[] t = new Type[mParam.Count];
			uint n = 0;
	
			foreach (BinaryParam p in mParam)
			{
				t[n] = p.Type;
				n++;
			}
	
			return t;
		}
	
		public Object[] GetParamValues()
		{
			Object[] t = new Object[mParam.Count];
			uint n = 0;
	
			foreach (BinaryParam p in mParam)
			{
				t[n] = p.Value;
				n++;
			}
	
			return t;
		}
		
		public void Print()
		{
			foreach (BinaryParam p in mParam)
			{
				Console.WriteLine("* " + p.Name + ":" + p.strValue + ":" + p.Type.ToString());
			}
		}
	}
}

namespace MusiC
{
	// TODO: Make use of base class
	public class BinaryInfo : BinaryParam
	{	
		// Private members
		LinkedList<BinaryParam> mParam = new LinkedList<BinaryParam>();
		Type _tAssembly=null;
		
		// Constructor
		public BinaryInfo()
		{
		}
		
		public BinaryInfo(Type t)
		{
			_tAssembly=t;
		}
		
		// Public Methods
		public Extension New()
		{
			return Invoker.LoadType(this);
		}
		
		public void AddParam(String tag, Type t)
		{
			mParam.AddLast(new BinaryParam(tag, t));
		}
		
		public void AddParam(String tag, String t)
		{
			//mParam.AddLast(new BinaryParam(tag, t));
		}
	
		public LinkedList<BinaryParam> GetParam()
		{
			return mParam;
		}
	
		public Type[] GetTypes()
		{
			Type[] t = new Type[mParam.Count];
			uint n = 0;
	
			foreach (BinaryParam p in mParam)
			{
				//t[n] = p.Type;
				n++;
			}
	
			return t;
		}
	
		public Object[] GetParamValues()
		{
			Object[] t = new Object[mParam.Count];
			uint n = 0;
	
			foreach (BinaryParam p in mParam)
			{
				t[n] = p.Value;
				n++;
			}
	
			return t;
		}
		
		public void Print()
		{
			foreach (BinaryParam p in mParam)
			{
				Console.WriteLine("* " + p.Name + ":" + p.strValue + ":" + p.Type.ToString());
			}
		}
	}
}
