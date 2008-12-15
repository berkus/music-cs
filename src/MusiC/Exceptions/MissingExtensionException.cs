using System;

namespace MusiC.Exceptions
{
	public class MissingExtensionException : MCException
	{
		public MissingExtensionException(String msg) : base(msg)
		{
		}
		
		public MissingExtensionException(Exception e, String msg) : base(e, msg)
		{
		}
	}
}
