using System;
using UnityEngine;

public class TextureScroller : MonoBehaviour
{
	public float scrollRateX;

	public float scrollRateY;

	public string[] materialTextureProperties;

	public bool scrollingOn;

	private Material _material;

	private Vector2[] _uvOffsets;

	public int materialIndexToScroll;

	private void OnEnable()
	{
		LoadingPanel.PanelDisplayed += StopAnimation;
		LoadingPanel.PanelDismissed += StartAnimation;
	}

	private void OnDisable()
	{
		LoadingPanel.PanelDisplayed -= StopAnimation;
		LoadingPanel.PanelDismissed -= StartAnimation;
	}

	private void StartAnimation(object sender, EventArgs args)
	{
		scrollingOn = true;
	}

	private void StopAnimation(object sender, EventArgs args)
	{
		scrollingOn = false;
	}

	private void Awake()
	{
		if (materialTextureProperties == null || materialTextureProperties.Length == 0)
		{
			materialTextureProperties = new string[1] { "_MainTex" };
		}
		if (materialIndexToScroll > 0)
		{
			_material = base.GetComponent<Renderer>().materials[materialIndexToScroll];
		}
		else
		{
			_material = base.GetComponent<Renderer>().material;
		}
		_uvOffsets = new Vector2[materialTextureProperties.Length];
		for (int i = 0; i < materialTextureProperties.Length; i++)
		{
			_uvOffsets[i] = _material.GetTextureOffset(materialTextureProperties[i]);
		}
	}

	private void Update()
	{
		if (scrollingOn)
		{
			for (int i = 0; i < materialTextureProperties.Length; i++)
			{
				_uvOffsets[i] += new Vector2(Time.deltaTime * scrollRateX, Time.deltaTime * scrollRateY);
				_material.SetTextureOffset(materialTextureProperties[i], _uvOffsets[i]);
			}
		}
	}
}
