using UnityEngine;

public class PlayerDamageExplosion : MonoBehaviour
{
	private static GameObject inst;

	private void Awake()
	{
		inst = base.gameObject;
	}

	public static void Explode(Vector3 pos)
	{
		if (inst != null)
		{
			Debug.Log("Exploding player");
			inst.transform.position = pos;
			inst.BroadcastMessage("OnExplosionStart", SendMessageOptions.DontRequireReceiver);
		}
	}
}
