using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Xml;

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
