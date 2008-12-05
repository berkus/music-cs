using System;

using MusiC.Exceptions;

namespace MusiC
{
	public interface IGlobal
	{
		void Initialize();
	}
	
	public class Global<T> : MusiCObject
	where T : class, IGlobal, new()
	{
		static T _instance=null;
		
		static public T GetInstance()
		{
			if(_instance==null)
			{
				try {
					// TODO: Check if class has abstract CreateInstance or a public Constructor and call the one
					// TODO: available
					_instance = new T();
					_instance.Initialize();
				}
				catch (MCException e) {
					Type t = typeof(T);
					String msg = "Global wrapper could not create an instance of class " + t.ToString();
					e.AddMessage(msg);
					throw;
				}
				catch (System.Exception e) {
					Type t = typeof(T);
					String msg = "Global wrapper could not create an instance of class " + t.ToString();
					throw new MCException(e, msg);
				}
			}
			
			return _instance;
		}
		
		static public bool SetInstance(T instance)
		{
			if(_instance != null)
				return false;
			
			_instance=instance;
			return true;
		}
		
		static public bool HasInstance()
		{
			return !(_instance==null);
		}
	}
}
