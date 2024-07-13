using UnityEngine;

public class BombAndProjectileSpeedUpgrade : ProjectileSpeedUpgrade
{
	public enum BombSpeedRate
	{
		Constant = 0,
		Linear = 1,
		Curve = 2
	}

	public BombSpeedRate bombSpeedRate;

	public float bombSpeedMultiplier = 0.33f;

	public int bombRoomThreshold = 30;

	private readonly float _finalSpeed = 1f;

	public float GetBombSpeed(int roomNumber)
	{
		bombSpeedMultiplier = Mathf.Clamp01(bombSpeedMultiplier);
		if (roomNumber >= bombRoomThreshold)
		{
			return _finalSpeed;
		}
		if (bombSpeedRate == BombSpeedRate.Constant)
		{
			return bombSpeedMultiplier;
		}
		float num = Mathf.Clamp01((float)roomNumber * 1f / (float)bombRoomThreshold);
		if (bombSpeedRate == BombSpeedRate.Linear)
		{
			float num2 = (_finalSpeed - bombSpeedMultiplier) * num;
			return bombSpeedMultiplier + num2;
		}
		if (bombSpeedRate == BombSpeedRate.Curve)
		{
			float num3 = (_finalSpeed - bombSpeedMultiplier) * num * num;
			return bombSpeedMultiplier + num3;
		}
		return _finalSpeed;
	}
}
