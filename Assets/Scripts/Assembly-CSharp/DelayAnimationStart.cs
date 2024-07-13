using System.Collections;
using UnityEngine;

public class DelayAnimationStart : MonoBehaviour
{
	public float delayTime;

	private IEnumerator Start()
	{
		yield return new WaitForSeconds(delayTime);
		if (base.GetComponent<Animation>() != null)
		{
			base.GetComponent<Animation>().Play();
		}
	}
}
