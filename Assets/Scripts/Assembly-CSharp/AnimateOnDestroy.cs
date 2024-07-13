using System.Collections.Generic;
using UnityEngine;

public class AnimateOnDestroy : MonoBehaviour
{
	public List<Transform> destroyObjs;

	public Transform[] switchToTargetable;

	private bool doOnce;

	public float startTimer;

	private void Start()
	{
	}

	private void Update()
	{
		if (destroyObjs.Count > 0)
		{
			Transform item = null;
			foreach (Transform destroyObj in destroyObjs)
			{
				if (destroyObj == null)
				{
					item = destroyObj;
				}
			}
			destroyObjs.Remove(item);
			startTimer = Time.time + 1.5f;
		}
		else if (!doOnce && Time.time > startTimer)
		{
			base.GetComponent<Animation>().Play("Take 001");
			doOnce = true;
			for (int i = 0; i < switchToTargetable.Length; i++)
			{
				switchToTargetable[i].gameObject.layer = Layers.Enemies;
			}
		}
	}
}
