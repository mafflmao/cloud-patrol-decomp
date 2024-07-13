using System;

namespace Boo.Lang
{
	[Flags]
	public enum QuackFuMemberKind
	{
		Any = 0,
		Method = 1,
		Getter = 2,
		Setter = 4,
		Property = 6
	}
}
