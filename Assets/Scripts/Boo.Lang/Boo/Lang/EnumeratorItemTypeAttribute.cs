using System;

namespace Boo.Lang
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Class)]
	public class EnumeratorItemTypeAttribute : Attribute
	{
		protected Type _itemType;

		public Type ItemType
		{
			get
			{
				return _itemType;
			}
		}

		public EnumeratorItemTypeAttribute(Type itemType)
		{
			if (itemType == null)
			{
				throw new ArgumentNullException("itemType");
			}
			_itemType = itemType;
		}
	}
}
