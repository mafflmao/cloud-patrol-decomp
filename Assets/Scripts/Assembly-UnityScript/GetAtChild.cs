using System;
using Boo.Lang.Runtime;
using UnityEngine;

[Serializable]
public class GetAtChild : MonoBehaviour
{
	public Transform poop;

	private object alphaFromZ;

	public virtual void Update()
	{
		alphaFromZ = poop.transform.position.z;
		object value = alphaFromZ;
		Color color = GetComponent<Renderer>().material.color;
		float num = (color.a = RuntimeServices.UnboxSingle(value));
		Color color3 = (GetComponent<Renderer>().material.color = color);
		MonoBehaviour.print(alphaFromZ);
	}

	public virtual void Main()
	{
	}
}
