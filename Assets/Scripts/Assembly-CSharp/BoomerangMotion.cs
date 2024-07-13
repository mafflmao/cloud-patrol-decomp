using UnityEngine;

public class BoomerangMotion : MonoBehaviour
{
	public float amplitude = 0.5f;

	public float period = 0.5f;

	public float spinTime = 0.15f;

	private void Start()
	{
		iTween.MoveTo(base.gameObject, iTween.Hash("y", base.transform.localPosition.y + amplitude, "time", period, "isLocal", true, "loopType", "pingPong", "easetype", "easeInOutSine"));
		iTween.RotateBy(base.gameObject, iTween.Hash("z", 1f, "time", spinTime, "space", Space.Self, "looptype", "loop", "easetype", "linear"));
	}
}
