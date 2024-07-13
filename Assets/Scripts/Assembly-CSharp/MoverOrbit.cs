using UnityEngine;

public class MoverOrbit : MonoBehaviour
{
	public float radius = 1f;

	public float speed = 1f;

	public OrbitFacing facing;

	public OrbitAxis axisOfRotation = OrbitAxis.Z;

	public float angle;

	private bool _isEnabled = true;

	private Vector3 _origin;

	private Vector3 _offsetVector;

	private Vector3 _axisOfRotationVector;

	private void Start()
	{
		_origin = base.transform.localPosition;
		_offsetVector = new Vector3(radius, 0f, 0f);
		_axisOfRotationVector = new Vector3((axisOfRotation == OrbitAxis.X) ? 1 : 0, (axisOfRotation == OrbitAxis.Y) ? 1 : 0, (axisOfRotation == OrbitAxis.Z) ? 1 : 0);
		if (speed >= 0f)
		{
			speed = DifficultyManager.Instance.OrbitSpeed;
		}
		else
		{
			speed = 0f - DifficultyManager.Instance.OrbitSpeed;
		}
		if (base.gameObject.tag == "Bomb")
		{
			BombAndProjectileSpeedUpgrade passiveUpgradeOrDefault = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<BombAndProjectileSpeedUpgrade>();
			if (passiveUpgradeOrDefault != null)
			{
				speed *= passiveUpgradeOrDefault.GetBombSpeed(LevelManager.Instance.RoomsCleared);
				passiveUpgradeOrDefault.SpawnWindOnProjectile(base.transform, new Vector3(4f, 4f, 4f));
			}
		}
	}

	public void Disable()
	{
		_isEnabled = false;
	}

	public void StopMoving()
	{
		speed = 0f;
	}

	private void Update()
	{
		if (_isEnabled && !GameManager.Instance.IsPaused)
		{
			angle += speed * Time.deltaTime;
			if (angle >= 360f)
			{
				angle -= 360f;
			}
			else if (angle < 0f)
			{
				angle = 360f - Mathf.Abs(angle % 360f);
			}
			Quaternion quaternion = Quaternion.AngleAxis(angle, _axisOfRotationVector);
			base.transform.localPosition = _origin + quaternion * _offsetVector;
			switch (facing)
			{
			case OrbitFacing.ORIGIN:
				base.transform.LookAt(_origin);
				break;
			case OrbitFacing.AWAY_FROM_ORIGIN:
				base.transform.LookAt(_origin);
				base.transform.rotation = Quaternion.Inverse(base.transform.rotation);
				break;
			case OrbitFacing.CAMERA:
				base.transform.LookAt(Camera.main.transform);
				break;
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
		Gizmos.DrawSphere(base.transform.position, radius);
	}
}
