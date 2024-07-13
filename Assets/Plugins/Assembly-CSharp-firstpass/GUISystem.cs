using System;
using UnityEngine;

public class GUISystem : MonoBehaviour
{
	private static GUISystem _inst;

	[SerializeField]
	public static float ReferenceWidth = 2048f;

	[SerializeField]
	public static float ReferenceHeight = 1536f;

	public float CalculatedScreenWidth;

	private GUICamera _guiCamera;

	private UIManager _uiManager;

	public static GUISystem Instance
	{
		get
		{
			if (_inst == null)
			{
				_inst = UnityEngine.Object.FindObjectOfType(typeof(GUISystem)) as GUISystem;
			}
			if (_inst == null)
			{
			}
			return _inst;
		}
	}

	public GUICamera guiCamera
	{
		get
		{
			return _guiCamera;
		}
	}

	public UIManager uiManager
	{
		get
		{
			return _uiManager;
		}
	}

	public static event EventHandler CalculatedScreenWidthUpdated;

	private void Awake()
	{
		if (_inst != null)
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
			return;
		}
		_inst = this;
		if (_guiCamera == null)
		{
			_guiCamera = base.gameObject.GetComponentInChildren<GUICamera>();
		}
		if (_uiManager == null)
		{
			_uiManager = base.gameObject.GetComponentInChildren<UIManager>();
		}
		_guiCamera.resolutionChangedEvt += OnResolutionChanged;
	}

	private void OnResolutionChanged()
	{
		CalculatedScreenWidth = ReferenceWidth * guiCamera.autoAdjustScales[1].x;
		Debug.Log("GUISystem Updated screen width: " + CalculatedScreenWidth);
		OnCalculatedScreenWidthUpdated();
	}

	private void OnDestroy()
	{
		if (_inst == this)
		{
			_guiCamera.resolutionChangedEvt -= OnResolutionChanged;
			_inst = null;
		}
	}

	private void OnCalculatedScreenWidthUpdated()
	{
		if (GUISystem.CalculatedScreenWidthUpdated != null)
		{
			GUISystem.CalculatedScreenWidthUpdated(this, new EventArgs());
		}
	}
}
