using UnityEngine;

public class LightningBolt : MonoBehaviour
{
	private GameObject target;

	public int zigs = 100;

	public float speed = 1f;

	public float scale = 1f;

	public Light startLight;

	public Light endLight;

	private Perlin noise;

	private float oneOverZigs;

	private bool isEnabled;

	private ParticleSystem particles;

	private void Start()
	{
	}

	private void Update()
	{
		if (!isEnabled)
		{
			return;
		}
		if (noise == null)
		{
			noise = new Perlin();
		}
		float num = Time.time * speed * 0.1365143f;
		float num2 = Time.time * speed * 1.21688f;
		float num3 = Time.time * speed * 2.5564f;
	}

	public void Target(Transform aTarget)
	{
		target.transform.position = aTarget.position;
	}

	public void Enable()
	{
		isEnabled = true;
	}

	public void Disable()
	{
		isEnabled = false;
	}
}
