using System;

namespace Boo.Lang.Runtime
{
	public interface ICoercible
	{
		object Coerce(Type to);
	}
}
