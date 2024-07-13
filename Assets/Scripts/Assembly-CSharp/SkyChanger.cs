using System;
using UnityEngine;

public class SkyChanger : MonoBehaviour
{
	private const float FadeTime = 5f;

	private const string BlendAttributeName = "_Blend";

	private const string Texture1AttributeName = "_Texture1";

	private const string Texture2AttributeName = "_Texture2";

	public GameObject skyObject;

	public Material blendMaterial;

	private bool _hasModifiedTexture;

	private Texture _originalTexture1;

	private Texture _originalTexture2;

	private float _originalBlendValue;

	private void OnDestroy()
	{
		if (_hasModifiedTexture)
		{
			blendMaterial.SetTexture("_Texture1", _originalTexture1);
			blendMaterial.SetTexture("_Texture2", _originalTexture2);
			blendMaterial.SetFloat("_Blend", _originalBlendValue);
		}
	}

	private void OnEnable()
	{
		LevelManager.DifficultyUp += HandleLevelManagerLevelChanged;
	}

	private void OnDisable()
	{
		LevelManager.DifficultyUp -= HandleLevelManagerLevelChanged;
	}

	private void HandleLevelManagerLevelChanged(object sender, EventArgs e)
	{
		if (LevelManager.Instance.CurSkyTexture != null)
		{
			SwapSky(LevelManager.Instance.CurSkyTexture);
		}
	}

	private void SwapSky(Texture2D newSkyTexture)
	{
		Debug.Log("Changing sky texture...");
		if (skyObject != null && blendMaterial == null)
		{
			blendMaterial = skyObject.GetComponent<Renderer>().material;
		}
		SaveTextureSettings();
		blendMaterial.SetFloat("_Blend", 0f);
		blendMaterial.SetTexture("_Texture2", newSkyTexture);
		iTween.ValueTo(base.gameObject, iTween.Hash("from", 0, "to", 1, "time", 5f, "onupdate", "FadeUpdate", "oncomplete", "FadeComplete"));
	}

	private void FadeUpdate(float newValue)
	{
		blendMaterial.SetFloat("_Blend", newValue);
	}

	private void FadeComplete()
	{
		Debug.Log("Sky Fade Complete");
		Texture texture = blendMaterial.GetTexture("_Texture2");
		blendMaterial.SetTexture("_Texture1", texture);
		blendMaterial.SetFloat("_Blend", 0f);
	}

	private void SaveTextureSettings()
	{
		_originalTexture1 = blendMaterial.GetTexture("_Texture1");
		_originalTexture2 = blendMaterial.GetTexture("_Texture2");
		_originalBlendValue = blendMaterial.GetFloat("_Blend");
		_hasModifiedTexture = true;
	}
}
