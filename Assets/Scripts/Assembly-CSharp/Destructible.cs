using UnityEngine;

public class Destructible : MonoBehaviour
{
	public enum Type
	{
		Crate = 0,
		Pottery = 1,
		Barrel = 2,
		Other = 3,
		ExplosiveBarrel = 4
	}

	public Type destructibleType = Type.Other;
}
