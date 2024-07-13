using UnityEngine;

public class EmitParticles : MonoBehaviour
{
	public ParticleSystem emitter;

	public Transform left;

	public Transform right;

	public Transform up;

	public Transform down;

	public void Emit()
	{
	}

	public void EmitLeft()
	{
		emitter.transform.rotation = left.rotation;
		Emit();
	}

	public void EmitRight()
	{
		emitter.transform.rotation = right.rotation;
		Emit();
	}

	public void EmitUp()
	{
		emitter.transform.rotation = up.rotation;
		Emit();
	}

	public void EmitDown()
	{
		emitter.transform.rotation = down.rotation;
		Emit();
	}
}
