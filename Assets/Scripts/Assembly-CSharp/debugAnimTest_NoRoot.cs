using UnityEngine;

public class debugAnimTest_NoRoot : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		AnimationUtils.PlayClip(base.gameObject.GetComponent<Animation>(), "Tumble_Root");
	}
}
