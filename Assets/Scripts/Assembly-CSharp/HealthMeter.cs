using System.Collections;
using UnityEngine;

public class HealthMeter : MonoBehaviour
{
	private const string EnabledAnimationName = "Enabled";

	public const string DisabledAnimationName = "Disabled";

	public HealthMeterLight[] lights;

	private float priorHealthValue;

	public Color goodColor;

	public Color mediumColor;

	public Color badColor;

	public Color offColor;

	private Color currentColor;

	private void Start()
	{
		priorHealthValue = GameManager.currentHealth;
	}

	private void Update()
	{
		switch ((int)GameManager.currentHealth)
		{
		case 3:
			currentColor = goodColor;
			break;
		case 2:
			currentColor = mediumColor;
			break;
		case 1:
			currentColor = badColor;
			break;
		case 0:
			currentColor = offColor;
			break;
		}
		if (priorHealthValue != GameManager.currentHealth)
		{
			UpdateHealthMeter();
			priorHealthValue = GameManager.currentHealth;
			if (GameManager.currentHealth == 1f)
			{
				lights[0].StartWarningLight();
			}
			else
			{
				lights[0].StopWarningLight();
			}
		}
	}

	public void RefillHealthMeter()
	{
		StartCoroutine(ActivateHealthMeterCoroutine(0f));
	}

	public void ActivateHealthMeter()
	{
		StartCoroutine(ActivateHealthMeterCoroutine(0.4f));
	}

	private IEnumerator ActivateHealthMeterCoroutine(float timeBetweenLights)
	{
		for (int i = 0; (float)i < GameManager.currentHealth; i++)
		{
			lights[i].onColor = currentColor;
			lights[i].offColor = offColor;
			lights[i].Activate();
			yield return new WaitForSeconds(timeBetweenLights);
		}
	}

	private void UpdateHealthMeter()
	{
		int num = 1;
		HealthMeterLight[] array = lights;
		foreach (HealthMeterLight healthMeterLight in array)
		{
			if ((float)num <= GameManager.currentHealth)
			{
				healthMeterLight.SetColor(currentColor);
			}
			else
			{
				GameManager.KillAllProjectiles();
				healthMeterLight.Deactivate();
			}
			num++;
		}
	}
}
