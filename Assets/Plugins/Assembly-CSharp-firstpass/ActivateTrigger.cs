using UnityEngine;

public class ActivateTrigger : MonoBehaviour
{
	public enum Mode
	{
		Trigger = 0,
		Replace = 1,
		Activate = 2,
		Enable = 3,
		Animate = 4,
		Deactivate = 5
	}

	public Mode action = Mode.Activate;

	public Object target;

	public GameObject source;

	public int triggerCount = 1;

	public bool repeatTrigger;

	private void DoActivateTrigger()
	{
		triggerCount--;
		if (triggerCount != 0 && !repeatTrigger)
		{
			return;
		}
		Object @object = ((!(target != null)) ? base.gameObject : target);
		Behaviour behaviour = @object as Behaviour;
		GameObject gameObject = @object as GameObject;
		if (behaviour != null)
		{
			gameObject = behaviour.gameObject;
		}
		switch (action)
		{
		case Mode.Trigger:
			gameObject.BroadcastMessage("DoActivateTrigger");
			break;
		case Mode.Replace:
			if (source != null)
			{
				Object.Instantiate(source, gameObject.transform.position, gameObject.transform.rotation);
				Object.DestroyObject(gameObject);
			}
			break;
		case Mode.Activate:
			gameObject.SetActive(true);
			break;
		case Mode.Enable:
			if (behaviour != null)
			{
				behaviour.enabled = true;
			}
			break;
		case Mode.Animate:
			gameObject.GetComponent<Animation>().Play();
			break;
		case Mode.Deactivate:
			gameObject.SetActive(false);
			break;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		DoActivateTrigger();
	}
}
