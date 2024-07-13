using System;
using UnityEngine;

public class SuicideOnGameOverTransitionComplete : MonoBehaviour
{
	private void Start()
	{
		TransitionController.GameOverTransitionComplete += HandleTransitionControllerGameOverTransitionComplete;
	}

	private void OnDestroy()
	{
		TransitionController.GameOverTransitionComplete -= HandleTransitionControllerGameOverTransitionComplete;
	}

	private void HandleTransitionControllerGameOverTransitionComplete(object sender, EventArgs e)
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
