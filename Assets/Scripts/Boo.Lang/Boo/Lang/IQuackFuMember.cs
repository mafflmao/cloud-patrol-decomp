using System;

namespace Boo.Lang
{
	public interface IQuackFuMember
	{
		string Name { get; }

		QuackFuMemberKind Kind { get; }

		Type ReturnType { get; }

		string[] ArgumentNames { get; }

		Type[] ArgumentTypes { get; }

		string Info { get; }
	}
}
