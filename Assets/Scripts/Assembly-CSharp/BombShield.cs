using UnityEngine;

public class BombShield : TrollShield
{
	public Transform bombExplosion;

	public Transform shieldExplosion;

	public Vector3 bombRotationTweak = Vector3.zero;

	public Vector3 shieldRotationTweak = Vector3.zero;

	public GameObject bombVisual;

	public override void Invulnerable()
	{
		base.Invulnerable();
		if (!(hazardProxy == null))
		{
			if (hazardShield)
			{
				hazardProxy.explodeFX = shieldExplosion;
				hazardProxy.objectToDestroy = base.gameObject;
				hazardProxy.rotationTweak = shieldRotationTweak;
				hazardProxy.detachShield = false;
			}
			else
			{
				hazardProxy.SetActive(false);
			}
		}
	}

	public override void Vulnerable()
	{
		base.Vulnerable();
		if (hazardProxy != null)
		{
			hazardProxy.explodeFX = bombExplosion;
			hazardProxy.objectToDestroy = bombVisual;
			hazardProxy.rotationTweak = bombRotationTweak;
			hazardProxy.detachShield = true;
			hazardProxy.SetActive(true);
		}
	}
}
