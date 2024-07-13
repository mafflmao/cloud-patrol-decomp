using System.Collections;
using UnityEngine;

public class MessageScreenSequenceController : ScreenSequenceController
{
	public string[] messages;

	public MessageScreen messageScreenPrefab;

	public override void AdvanceToNextScreen()
	{
		if (base.CurrentScreenNumber < messages.Length)
		{
			StartCoroutine(AnimateToNextScreen(base.CurrentScreenNumber));
		}
		else
		{
			OnSequenceComplete(0.5f);
		}
		base.CurrentScreenNumber++;
	}

	private IEnumerator AnimateToNextScreen(int screenNumber)
	{
		if (base.CurrentScreen != null)
		{
			base.CurrentScreen.StartAnimateOut();
			yield return new WaitForSeconds(0.25f);
		}
		ScreenSequenceScreen screenInstance = InstantiateScreen(messageScreenPrefab);
		screenInstance.transform.position = base.transform.position;
		MessageScreen screenScript = screenInstance.GetComponent<MessageScreen>();
		screenScript.SetText(messages[screenNumber]);
		base.CurrentScreen = screenInstance;
	}

	public static GameObject InstantiateAsChild(GameObject parent, MessageScreenSequenceController sequencePrefab, float zOffset)
	{
		GameObject gameObject = (GameObject)Object.Instantiate(sequencePrefab.gameObject);
		if (parent != null)
		{
			gameObject.transform.parent = parent.transform;
		}
		gameObject.transform.localPosition = new Vector3(0f, 0f, zOffset);
		return gameObject;
	}

	public static IEnumerator InstantiateAsChildAndWaitForDestruction(GameObject parent, MessageScreenSequenceController screenSequence, float zOffset)
	{
		GameObject instance = InstantiateAsChild(parent, screenSequence, zOffset);
		while (instance != null)
		{
			yield return new WaitForSeconds(0.1f);
		}
	}
}
