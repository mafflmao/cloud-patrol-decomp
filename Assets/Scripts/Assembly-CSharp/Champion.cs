using UnityEngine;

public class Champion : MonoBehaviour
{
	private void Start()
	{
		AnimationStates component = GetComponent<AnimationStates>();
		StartCoroutine(component.Alive());
	}
}
