using System;

namespace MCModule
{	
	public class ActionNode
	{
		String m_dir;
		String m_name;
		
		public String Dir
		{
			get { return m_dir; }
		}
		
		public String Name
		{
			get { return m_name; }
		}
			
		public ActionNode(String actionName, String rootDir)
		{
			m_dir = rootDir + ((actionName != String.Empty) ? System.IO.Path.DirectorySeparatorChar + actionName : String.Empty);
			m_name = actionName;
		}
	}
}
