using UnityEngine;

public class SkylandersCollectionController : MonoBehaviour
{
	private string mLastState = string.Empty;

	private void OnStateLoaded()
	{
		if (StateManager.Instance.StateCount > 1)
		{
			HideState();
		}
	}

	private void OnStateActivate(string oldState)
	{
		mLastState = oldState;
		ShowState();
	}

	private void OnStateDeactivate(string oldState)
	{
		HideState();
	}

	private void OnStateBack()
	{
		StateManager.Instance.LoadAndActivateState(mLastState);
	}

	private void HideState()
	{
		base.transform.position = new Vector3(0f, 10000f, 0f);
	}

	private void ShowState()
	{
		base.transform.position = Vector3.zero;
	}

	private void OnGUI()
	{
	}
}
