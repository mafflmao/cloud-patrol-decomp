using System;
using UnityEngine;

public class ForceField : MonoBehaviour
{
	[HideInInspector]
	public Health owner;

	public TrollProtectorBeam beam;

	public Color lightningPulseColor;

	public SoundEventData deflectionSound;

	private void Awake()
	{
		Health.Deflected += DeflectHandler;
		base.GetComponent<Animation>().Play("anim_forceFieldSpawn");
		base.GetComponent<Animation>().PlayQueued("anim_forceField");
	}

	public void Remove()
	{
		Health.Deflected -= DeflectHandler;
		base.GetComponent<Animation>().Play("anim_forceField_destroy");
		UnityEngine.Object.Destroy(base.gameObject, 0.5f);
	}

	private void DeflectHandler(object sender, EventArgs args)
	{
		if (!(owner == null) && (Health)sender == owner)
		{
			beam.GetComponent<Animation>().Play("forceFieldBeamPulse");
			base.GetComponent<Animation>().Play("anim_forceFieldHitReact");
			base.GetComponent<Animation>().PlayQueued("anim_forceField");
			if (deflectionSound != null)
			{
				SoundEventManager.Instance.Play(deflectionSound, base.gameObject);
			}
		}
	}
}
