using UnityEngine;

public class BossWantedPoster : MonoBehaviour
{
	public SpriteText text;

	public Animation anim;

	public GameObject poofFX;

	public SoundEventData poofSound;

	public SoundEventData posterAppearSound;

	public Vector3 viewportPosition = new Vector3(0f, -0.38f, 2f);

	public Renderer[] renderers;

	public void Start()
	{
		Renderer[] array = renderers;
		foreach (Renderer renderer in array)
		{
			renderer.enabled = false;
		}
	}

	public void Play()
	{
		Renderer[] array = renderers;
		foreach (Renderer renderer in array)
		{
			renderer.enabled = true;
		}
		base.transform.position = Camera.main.ViewportToWorldPoint(viewportPosition);
		base.transform.rotation = Quaternion.identity;
		anim.Play();
		SoundEventManager.Instance.Play(posterAppearSound, base.gameObject);
		InvokeHelper.InvokeSafe(PlayEffects, 1.4f, this);
		InvokeHelper.InvokeSafe(Unpause, 3f, this);
		HealthBar.Instance.Pause = true;
	}

	private void PlayEffects()
	{
		Object.Instantiate(poofFX, base.transform.position, Quaternion.identity);
		SoundEventManager.Instance.Play(poofSound, base.gameObject);
	}

	private void Unpause()
	{
		HealthBar.Instance.Pause = false;
	}
}
