using System;
using System.IO;

using MCModule.UnmanagedInterface;

namespace MCModule
{
	abstract unsafe public class Handler : Extension
	{
		string mFilePath;
		int mSize = -1;
		double * mData = null;
		
		protected void SetSize(int sz)
		{
			mSize = sz;
		}
		
		protected void SetDataArea(double * dt)
		{
			mData = dt;
		}
		
		public int Size
		{
			get 
			{
				if(mSize >= 0)
					return mSize;
				
				//if(mData != null & IsAttached())
				if(IsAttached())
					Load();
				else
				{
					if(mSize == -1)
						throw new System.Exception("Size hasnt been setup b the handler implementation");
					
					if(mSize < 0)
						throw new System.Exception( "Size = " + mSize + ", is invalid");
					
					throw new System.Exception("Size is unknown.");
				}
				
				return mSize;
			}
			//set { mSize = value; }
		}
		
		public string WorkingFile
		{
			get { return mFilePath; }
		}
		
		public bool IsExtracted()
		{
			///@todo A static method should be added to dbHandler to find the name of the extracted file.
			///This change requires a major change in the project tree since it cause a cyclic dependence, whic is not allowed.
			///The other way to do this is merge mcLib and mcFileHandlers, and therefore all managed code, in a single library.
			///The same should be done to the managed code.
			return File.Exists(mFilePath + ".db");
		}
		
		virtual public bool CanHandle(string file)
		{
			//Path.GetExtension(file);
			return true;
		}
		
		///@brief Get the pointer to a double array with all file data.
		///@details This method is inconvenient for large files since you have to allocate it all in memory although there is
		///no other way to do this.

		unsafe virtual public double * GetData()
		{
			if(mData == null)
				throw new System.Exception("No data area has been specified");
			else
				return mData;
		}
		
		public void Attach(string file)
		{
			mFilePath = file;
			mData = null;
			mSize = -1;
			InnerAttach(file);
		}
		
		public void Detach()
		{
			InnerDetach();
			
			if(mData != null)
			{
				UnsafePtr.free(mData);
			}
			
			mFilePath = null;
			mData = null;
		}
		
		public bool IsAttached()
		{
			return (mFilePath != null);
		}
		
		virtual protected void InnerAttach(string file)
		{
		}
		
		virtual protected void InnerDetach()
		{
		}
		
		abstract protected void Load();
		
		abstract public void Dispose();
	}
}