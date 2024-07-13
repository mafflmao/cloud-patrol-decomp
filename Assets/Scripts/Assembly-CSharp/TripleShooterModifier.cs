using UnityEngine;

public class TripleShooterModifier : MonoBehaviour
{
	public static Color TrollColor = new Color(0.5f, 0.25f, 0.25f, 1f);

	public static Color BulletColor = Color.red;

	public static float Speed = 1f;

	public static float SpeedMultiplier = 1f;

	public static float GetTime(float time, bool isAngry)
	{
		return (!isAngry) ? time : (time * Speed);
	}
}
