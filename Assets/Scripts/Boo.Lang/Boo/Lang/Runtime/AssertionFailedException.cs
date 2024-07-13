using System;
using System.Runtime.Serialization;

namespace Boo.Lang.Runtime
{
	[Serializable]
	public class AssertionFailedException : RuntimeException
	{
		public AssertionFailedException(string message)
			: base(message)
		{
		}

		protected AssertionFailedException(SerializationInfo si, StreamingContext sc)
			: base(si, sc)
		{
		}
	}
}
