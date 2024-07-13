using System;
using System.Runtime.Serialization;

namespace Boo.Lang.Runtime
{
	[Serializable]
	public class RuntimeException : Exception
	{
		public RuntimeException(string message)
			: base(message)
		{
		}

		protected RuntimeException(SerializationInfo si, StreamingContext sc)
			: base(si, sc)
		{
		}
	}
}
