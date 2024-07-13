using UnityEngine;

public class Shield : MonoBehaviour
{
	public SoundEventData sfxSpawn;

	public SoundEventData sfxDeflect;

	private float distanceFromCamera = 2f;

	private void Start()
	{
		SoundEventManager.Instance.Play(sfxSpawn, base.gameObject);
		SoundEventManager.Instance.Play(sfxDeflect, base.gameObject, 0.25f);
		base.transform.position = TransformUtil.MoveDistanceFromCamera(base.transform.position, distanceFromCamera);
	}
}
