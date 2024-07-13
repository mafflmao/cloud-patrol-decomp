using System.Collections.Generic;

namespace Boo.Lang
{
	public interface IReflectiveQuackFu : IQuackFu
	{
		IEnumerable<IQuackFuMember> QuackGetMembers();
	}
}
