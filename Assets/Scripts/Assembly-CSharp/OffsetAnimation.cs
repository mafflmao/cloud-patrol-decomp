using UnityEngine;

public class OffsetAnimation : MonoBehaviour
{
	private void Start()
	{
		base.GetComponent<Animation>()[base.GetComponent<Animation>().clip.name].time = Random.Range(0f, base.GetComponent<Animation>()[base.GetComponent<Animation>().clip.name].length);
		base.GetComponent<Animation>().Play();
	}
}
