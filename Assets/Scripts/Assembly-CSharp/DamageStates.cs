using UnityEngine;

public class DamageStates : MonoBehaviour
{
	private int startingHP;

	public string[] damageStates;

	private int numStates;

	private int state;

	private void Start()
	{
		Health component = GetComponent<Health>();
		startingHP = component.hitPoints;
		numStates = damageStates.Length;
		state = 0;
	}

	private void SetDamageState(int hitPoints)
	{
		if (hitPoints <= startingHP - 1 / numStates && state < numStates - 1)
		{
			state++;
		}
	}
}
