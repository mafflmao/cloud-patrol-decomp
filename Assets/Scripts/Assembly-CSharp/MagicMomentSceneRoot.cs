using System;
using UnityEngine;

public class MagicMomentSceneRoot : MonoBehaviour
{
	private void Start()
	{
		if (StartGameSettings.InstanceExists)
		{
			AudioListener[] componentsInChildren = GetComponentsInChildren<AudioListener>();
			foreach (AudioListener obj in componentsInChildren)
			{
				UnityEngine.Object.Destroy(obj);
			}
		}
	}

	public void OnEnable()
	{
		MagicMoment.MagicMomentComplete += HandleMagicMomentComplete;
	}

	public void OnDisable()
	{
		MagicMoment.MagicMomentComplete -= HandleMagicMomentComplete;
	}

	private void HandleMagicMomentComplete(object sender, EventArgs args)
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
