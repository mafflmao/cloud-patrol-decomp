using UnityEngine;

public class TextureStateController : MonoBehaviour
{
	private float startingHP;

	private int numStates;

	public Texture[] myDamageTextures;

	private int state;

	private void Start()
	{
		Health component = GetComponent<Health>();
		startingHP = component.hitPoints;
		numStates = myDamageTextures.Length;
		base.GetComponent<Renderer>().material.mainTexture = myDamageTextures[state];
		state++;
	}

	private void SetDamageState(int hitPoints)
	{
		if ((float)hitPoints <= startingHP - (float)(1 / numStates))
		{
			base.GetComponent<Renderer>().material.mainTexture = myDamageTextures[state];
			if (state < numStates - 1)
			{
				state++;
			}
			startingHP = hitPoints;
		}
	}
}
