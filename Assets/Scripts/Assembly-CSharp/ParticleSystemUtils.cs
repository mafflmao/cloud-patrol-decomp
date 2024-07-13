using UnityEngine;

public class ParticleSystemUtils : MonoBehaviour
{
	public static void EmitRecursive(GameObject obj, int emitCount)
	{
		if (!(obj != null))
		{
			return;
		}
		ParticleSystem component = obj.GetComponent<ParticleSystem>();
		if (component != null)
		{
			component.Emit(emitCount);
			for (int i = 0; i < obj.transform.childCount; i++)
			{
				EmitRecursive(obj.transform.GetChild(i).gameObject, emitCount);
			}
		}
	}
}
