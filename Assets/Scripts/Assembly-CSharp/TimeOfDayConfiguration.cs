using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TimeOfDayConfiguration
{
	public string name;

	public ColorTime[] colorTimes;

	public bool multiplyColor;

	public Material[] materials;

	public GameObject[] gameObjects;

	public Light[] lights;

	public Color calculatedColor;

	[NonSerialized]
	public Dictionary<Material, Color> originalMaterialColors;

	public void RememberColors()
	{
		originalMaterialColors = new Dictionary<Material, Color>();
		Material[] array = materials;
		foreach (Material material in array)
		{
			if (!originalMaterialColors.ContainsKey(material))
			{
				if (material.HasProperty("_Color"))
				{
					originalMaterialColors.Add(material, material.color);
				}
				else if (material.HasProperty("_TintColor"))
				{
					originalMaterialColors.Add(material, material.GetColor("_TintColor"));
				}
			}
		}
		GameObject[] array2 = gameObjects;
		foreach (GameObject gameObject in array2)
		{
			originalMaterialColors.Add(gameObject.GetComponent<Renderer>().material, gameObject.GetComponent<Renderer>().material.color);
		}
	}

	public void RestoreColors()
	{
		foreach (KeyValuePair<Material, Color> originalMaterialColor in originalMaterialColors)
		{
			if (originalMaterialColor.Key.HasProperty("_Color"))
			{
				originalMaterialColor.Key.color = originalMaterialColor.Value;
			}
			else if (originalMaterialColor.Key.HasProperty("_TintColor"))
			{
				originalMaterialColor.Key.SetColor("_TintColor", originalMaterialColor.Value);
			}
		}
	}

	public void UpdateGameObjectOriginalColor(GameObject gobj, Color newColor)
	{
		originalMaterialColors[gobj.GetComponent<Renderer>().material] = newColor;
	}

	public void SetTime(float time)
	{
		ColorTime colorTime = null;
		ColorTime colorTime2 = null;
		ColorTime[] array = colorTimes;
		foreach (ColorTime colorTime3 in array)
		{
			if (colorTime3.time > time)
			{
				if (colorTime2 == null)
				{
					colorTime2 = colorTime3;
				}
				else if (colorTime3.time - time < colorTime2.time - time)
				{
					colorTime2 = colorTime3;
				}
			}
			else if (colorTime == null)
			{
				colorTime = colorTime3;
			}
			else if (time - colorTime3.time < time - colorTime.time)
			{
				colorTime = colorTime3;
			}
		}
		if (colorTime2 == null || colorTime == null)
		{
			if (colorTime2 != null)
			{
				calculatedColor = colorTime2.color;
			}
			else
			{
				calculatedColor = colorTime.color;
			}
		}
		else
		{
			float t = (time - colorTime.time) / (colorTime2.time - colorTime.time);
			calculatedColor = Color.Lerp(colorTime.color, colorTime2.color, t);
		}
		Material[] array2 = materials;
		foreach (Material material in array2)
		{
			if (multiplyColor)
			{
				if (originalMaterialColors != null)
				{
					material.color = originalMaterialColors[material] * calculatedColor;
				}
			}
			else
			{
				material.color = calculatedColor;
			}
		}
		GameObject[] array3 = gameObjects;
		foreach (GameObject gameObject in array3)
		{
			if (!(gameObject != null))
			{
				continue;
			}
			if (multiplyColor)
			{
				if (originalMaterialColors != null && originalMaterialColors.ContainsKey(gameObject.GetComponent<Renderer>().material))
				{
					gameObject.GetComponent<Renderer>().material.color = originalMaterialColors[gameObject.GetComponent<Renderer>().material] * calculatedColor;
				}
			}
			else
			{
				gameObject.GetComponent<Renderer>().material.color = calculatedColor;
			}
		}
		Light[] array4 = lights;
		foreach (Light light in array4)
		{
			if (light != null)
			{
				light.color = calculatedColor;
			}
		}
	}
}
