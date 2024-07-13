using System.Collections.Generic;
using UnityEngine;

public class RaycastComparer
{
	public const string ComboCoinTag = "ComboCoin";

	public const string MagicItemCollectableTag = "MagicItemCollectable";

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(RaycastComparer), LogLevel.None);

	public static List<RaycastHit> Sort(IEnumerable<RaycastHit> hits)
	{
		List<RaycastHit> list = new List<RaycastHit>();
		list.AddRange(hits);
		HashSet<int> hashSet = new HashSet<int>();
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (!hashSet.Add(list[num].collider.GetInstanceID()))
			{
				list.RemoveAt(num);
			}
		}
		list.Sort(delegate(RaycastHit h1, RaycastHit h2)
		{
			GameObject gameObject = h1.transform.gameObject;
			GameObject gameObject2 = h2.transform.gameObject;
			if (gameObject.CompareTag("ComboCoin") && gameObject2.CompareTag("ComboCoin"))
			{
				return 0;
			}
			if (gameObject.CompareTag("ComboCoin"))
			{
				return -1;
			}
			if (gameObject2.CompareTag("ComboCoin"))
			{
				return 1;
			}
			if (gameObject.CompareTag("MagicItemCollectable"))
			{
				return -1;
			}
			return gameObject2.CompareTag("MagicItemCollectable") ? 1 : h2.transform.position.z.CompareTo(h1.transform.position.z);
		});
		return list;
	}
}
