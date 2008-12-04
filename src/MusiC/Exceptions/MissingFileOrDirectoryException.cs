using System;

namespace MusiC.Exceptions
{
	class MissingFileOrDirectoryException : MCException
	{
		public MissingFileOrDirectoryException(String msg) : base(msg)
		{
		}
		
		public MissingFileOrDirectoryException(Exception e, String msg) : base(e, msg)
		{
		}
	}
}
