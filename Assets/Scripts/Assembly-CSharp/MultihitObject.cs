using UnityEngine;

public class MultihitObject : MonoBehaviour
{
	public Transform myArmor;

	private void Start()
	{
		base.gameObject.layer = Layers.EnemiesDontTarget;
		myArmor.gameObject.layer = Layers.Enemies;
	}

	private void Update()
	{
		if (myArmor == null)
		{
			base.gameObject.layer = Layers.Enemies;
		}
	}
}
