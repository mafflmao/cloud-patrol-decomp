using System;

namespace Boo.Lang
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
	public class DuckTypedAttribute : Attribute
	{
	}
}
