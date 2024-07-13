using System.Collections;
using UnityEngine;

public class DelayedActivate : MonoBehaviour
{
	public float delay = 1f;

	public string state = string.Empty;

	private void Start()
	{
		StartCoroutine(DelayedActivateState());
	}

	private IEnumerator DelayedActivateState()
	{
		yield return new WaitForSeconds(delay);
		StateManager.Instance.LoadAndActivateState(state);
	}
}
