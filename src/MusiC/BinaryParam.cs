using System;

namespace MCModule
{
	public class BinaryParam
	{
		String m_name=null;
		public String Name
		{
			get { return m_name; }
		}

		Type m_type=null;
		public Type Type
		{
			get { return m_type; }
		}

		Object m_value=null;
		public Object Value
		{
			get { return m_value; }
		}

		String m_strValue;
		public String strValue
		{
			get { return m_strValue; }
			set
			{
				m_strValue = value;
				
				if(m_type != typeof(String))
				{	
					// TODO: Change to TryParse ?
					// TODO: Catch exception
					System.Reflection.MethodInfo parse = m_type.GetMethod("Parse", new Type[] { typeof(String) });
					m_value = parse.Invoke(null, new object[] { m_strValue });
				}
				else
				{
					m_value = value;
				}
			}
		}
		
		public BinaryParam()
		{
		}
	
		public BinaryParam(String name, Type type)
		{
			m_name = name;
			m_type = type;
		}
	}
}

namespace MusiC
{
	public class BinaryParam
	{
		String m_name=null;
		public String Name
		{
			get { return m_name; }
		}

		String m_type=null;
		public String Type
		{
			get { return m_type; }
			set { m_type=value; }
		}

		Object m_value=null;
		public Object Value
		{
			get { return m_value; }
		}

		String m_strValue;
		public String strValue
		{
			get { return m_strValue; }
			set
			{
				// m_strValue = value;
				// 
				// if(m_type != typeof(String))
				// {	
					// // TODO: Change to TryParse ?
					// // TODO: Catch exception
					// System.Reflection.MethodInfo parse = m_type.GetMethod("Parse", new Type[] { typeof(String) });
					// m_value = parse.Invoke(null, new object[] { m_strValue });
				// }
				// else
				// {
					// m_value = value;
				// }
			}
		}
		
		public BinaryParam()
		{
		}
		
		public BinaryParam(String name, Type type)
		{
			m_name = name;
			//m_type = type;
		}
	}
}