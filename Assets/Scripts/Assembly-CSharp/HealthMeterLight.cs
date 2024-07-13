using UnityEngine;

public class HealthMeterLight : MonoBehaviour
{
	public GameObject deactivateFX;

	public SoundEventData activateSFX;

	public bool on;

	public Color onColor;

	public Color offColor;

	public Mesh brokenMesh;

	private Mesh originalMesh;

	private bool blinkOn;

	public void Start()
	{
		originalMesh = GetComponent<MeshFilter>().sharedMesh;
	}

	public void SetColor(Color color)
	{
		onColor = color;
		base.GetComponent<Renderer>().material.color = onColor;
	}

	public void Activate()
	{
		if (!on)
		{
			on = true;
			base.GetComponent<Renderer>().material.color = onColor;
			SoundEventManager.Instance.Play2D(activateSFX);
			GetComponent<MeshFilter>().sharedMesh = originalMesh;
		}
	}

	public void Deactivate()
	{
		if (on)
		{
			StopWarningLight();
			on = false;
			base.GetComponent<Renderer>().material.color = offColor;
			Object.Instantiate(deactivateFX, base.transform.position, base.transform.rotation);
			GetComponent<MeshFilter>().sharedMesh = brokenMesh;
		}
	}

	public void StartWarningLight()
	{
		InvokeRepeating("Blink", 0.5f, 0.25f);
	}

	public void StopWarningLight()
	{
		CancelInvoke("Blink");
	}

	public void Blink()
	{
		if (blinkOn)
		{
			base.GetComponent<Renderer>().material.color = offColor;
			blinkOn = false;
		}
		else
		{
			base.GetComponent<Renderer>().material.color = onColor;
			blinkOn = true;
		}
	}
}
