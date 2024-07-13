using System.Collections;
using UnityEngine;

public class PrintAnimationInfo : MonoBehaviour
{
	public Animation anim;

	private void Start()
	{
		StartCoroutine(Print());
	}

	private IEnumerator Print()
	{
		yield return new WaitForSeconds(0.5f);
		Debug.Log("Num animations: " + anim.GetClipCount());
		int count = 0;
		foreach (AnimationState animState in anim)
		{
			count++;
			Debug.Log(count + ">  " + animState.name);
		}
		yield return new WaitForSeconds(0.5f);
	}
}
