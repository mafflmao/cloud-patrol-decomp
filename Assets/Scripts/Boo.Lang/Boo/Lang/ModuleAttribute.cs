using System;

namespace Boo.Lang
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Class)]
	[Obsolete("[Boo.Lang.Module] attribute is deprecated. Use [System.Runtime.CompilerServices.CompilerGlobalScope] instead.", true)]
	public class ModuleAttribute : Attribute
	{
	}
}
