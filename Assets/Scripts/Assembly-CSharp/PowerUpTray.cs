using System.Collections.Generic;
using UnityEngine;

public class PowerUpTray : MonoBehaviour
{
	public Transform trayIconTransform;

	public Transform costTransform;

	public PowerUpTrayIcon trayIconPrefab;

	public SpriteText txtCost;

	public UIButton btnCheck;

	public int numSlots;

	public float trayIconSpacing;

	public List<PowerUpTrayIcon> powerupSlots;

	public List<PowerupData> selectedPowerups;

	private void Start()
	{
		float num = 0f;
		powerupSlots = new List<PowerUpTrayIcon>();
		for (int i = 0; i < numSlots; i++)
		{
			PowerUpTrayIcon powerUpTrayIcon = (PowerUpTrayIcon)Object.Instantiate(trayIconPrefab);
			powerUpTrayIcon.transform.parent = trayIconTransform;
			powerUpTrayIcon.transform.localPosition = new Vector3((float)i * (trayIconSpacing + powerUpTrayIcon.packedSprite.width), 0f, 0f);
			if (num == 0f)
			{
				num = powerUpTrayIcon.packedSprite.width;
			}
			powerUpTrayIcon.index = powerupSlots.Count;
			powerupSlots.Add(powerUpTrayIcon);
		}
		costTransform.localPosition = new Vector3((float)numSlots * (num + trayIconSpacing), 0f, 0f);
	}

	public void UpdatePowerupList(List<PowerupData> list)
	{
		selectedPowerups = list;
		if (selectedPowerups.Count > powerupSlots.Count)
		{
			selectedPowerups = selectedPowerups.GetRange(0, powerupSlots.Count);
		}
		int num = 0;
		for (int i = 0; i < powerupSlots.Count; i++)
		{
			if (i < selectedPowerups.Count)
			{
				powerupSlots[i].data = selectedPowerups[i];
				num += selectedPowerups[i].cost;
			}
			else
			{
				powerupSlots[i].data = null;
			}
		}
		txtCost.Text = string.Empty + num;
	}

	public void RemovePowerup(int index)
	{
		if (index >= selectedPowerups.Count)
		{
			Debug.LogError("Selected power up is out of range: " + index);
		}
	}

	public void FadeIn()
	{
		btnCheck.Hide(false);
		iTween.FadeTo(base.gameObject, iTween.Hash("alpha", 1f, "time", 0.25f));
	}

	public void FadeOut()
	{
		btnCheck.Hide(true);
		iTween.ColorTo(base.gameObject, iTween.Hash("alpha", 0.7f, "time", 0.25f));
	}
}
