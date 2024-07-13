using UnityEngine;

public class EndGameTrigger : MonoBehaviour
{
	public Transform endOfLevel;

	private bool isScreenEnabled;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.name == "CameraTrigger")
		{
			SpawnEndScreen();
		}
	}

	private void SpawnEndScreen()
	{
		if (endOfLevel != null && !isScreenEnabled)
		{
			Object.Instantiate(endOfLevel);
			isScreenEnabled = true;
		}
	}
}
