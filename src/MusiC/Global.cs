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
