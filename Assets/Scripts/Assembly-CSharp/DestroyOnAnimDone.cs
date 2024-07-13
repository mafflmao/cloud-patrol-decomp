using UnityEngine;

public class DestroyOnAnimDone : MonoBehaviour
{
	public bool destroyParent;

	public bool destroySelf;

	public GameObject sourceGameObject;

	private void Start()
	{
		if (sourceGameObject == null)
		{
			sourceGameObject = base.gameObject;
		}
	}

	private void Update()
	{
		if (!sourceGameObject || sourceGameObject.GetComponent<Animation>().isPlaying)
		{
			return;
		}
		if (destroyParent)
		{
			Object.Destroy(base.transform.root.gameObject);
		}
		if (!(base.GetComponent<AudioSource>() != null) || !base.GetComponent<AudioSource>().isPlaying)
		{
			if (destroySelf)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				Object.Destroy(sourceGameObject);
			}
		}
	}
}
