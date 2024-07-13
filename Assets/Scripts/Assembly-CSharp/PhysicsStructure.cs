using System.Collections.Generic;
using UnityEngine;

public class PhysicsStructure : MonoBehaviour
{
	public bool isParent = true;

	private void PhysicsOn()
	{
		List<Rigidbody> list = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>());
		foreach (Rigidbody item in list)
		{
			item.isKinematic = false;
			item.useGravity = true;
		}
	}
}
