using System;
using System.Collections;
using UnityEngine;

public class TrollShield : MonoBehaviour
{
	private float _shieldUpTime = 1f;

	private float _shieldDownTime = 2f;

	public float initWaitTime = 1f;

	private bool init;

	private float _invulnerableDelay = 0.8f;

	private float _vulnerableDelay = 0.15f;

	public bool hazardShield;

	protected HazardBombProxy hazardProxy;

	private AnimationStates myAnim;

	private Health myHealth;

	private bool hazardProxyTriggered;

	public SoundEventData SFX_ShieldUp;

	public SoundEventData SFX_ShieldDown;

	public SoundEventData VO_Victory;

	private bool block;

	private bool _blockNullified;

	private void OnEnable()
	{
		ShootThroughShieldsUpgrade passiveUpgradeOrDefault = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<ShootThroughShieldsUpgrade>();
		_blockNullified = passiveUpgradeOrDefault != null;
		myAnim = GetComponent<AnimationStates>();
		if ((bool)myAnim)
		{
			myAnim.speed = 1f;
			myAnim.Shield_Ready();
			myAnim.Offset(myAnim.CurrentClipLength);
			Invulnerable();
		}
		myHealth = GetComponent<Health>();
		if (myHealth != null)
		{
			myHealth.isDeflecting = true && !_blockNullified;
		}
		hazardProxy = GetComponent<HazardBombProxy>();
		if (hazardShield)
		{
			if (hazardProxy != null)
			{
				hazardProxy.SetActive(true && !_blockNullified);
			}
			Hazard.HazardHurtPlayer += HandleHazardHazardHurtPlayer;
			HazardBombProxy.HazardProxyTriggered += HandleHazardProxyTriggered;
		}
		else if (hazardProxy != null)
		{
			hazardProxy.SetActive(false);
		}
		_shieldUpTime = DifficultyManager.Instance.ShieldUpTime;
		_shieldDownTime = DifficultyManager.Instance.ShieldDownTime;
		ShieldBlock();
	}

	private void OnDisable()
	{
		Vulnerable();
		CancelInvoke();
		StopAllCoroutines();
		init = false;
		if (hazardShield)
		{
			Hazard.HazardHurtPlayer -= HandleHazardHazardHurtPlayer;
			HazardBombProxy.HazardProxyTriggered -= HandleHazardProxyTriggered;
		}
	}

	public void Disable()
	{
		CancelInvoke();
		StopAllCoroutines();
		Vulnerable();
		InvokeHelper.InvokeSafe(DetachShield, _vulnerableDelay, this);
	}

	private void ShieldBlock()
	{
		if (hazardProxyTriggered)
		{
			if (SkyIronShield.ActiveShield != null)
			{
				KnockTroll();
			}
			return;
		}
		myAnim.speed = 1f;
		myAnim.Shield_Ready();
		if (!init)
		{
			InvokeHelper.InvokeSafe(ShieldUp, initWaitTime / DifficultyManager.Instance.ShieldAnimationSpeed, this);
			myAnim.Offset(myAnim.CurrentClipLength);
			init = true;
		}
		else
		{
			SoundEventManager.Instance.Play(SFX_ShieldDown, base.gameObject, 0.7f);
			InvokeHelper.InvokeSafe(ShieldUp, _shieldUpTime + myAnim.CurrentClipLength, this);
			InvokeHelper.InvokeSafe(Invulnerable, _invulnerableDelay, this);
		}
	}

	private void ShieldUp()
	{
		if (hazardProxyTriggered)
		{
			if (SkyIronShield.ActiveShield != null)
			{
				KnockTroll();
			}
			return;
		}
		if ((bool)myAnim && base.gameObject.activeInHierarchy)
		{
			myAnim.speed = 1f;
			myAnim.StartCoroutine(myAnim.Shield_Reload());
			SoundEventManager.Instance.Play(SFX_ShieldUp, base.gameObject);
		}
		InvokeHelper.InvokeSafe(Vulnerable, _vulnerableDelay, this);
		InvokeHelper.InvokeSafe(ShieldBlock, _shieldDownTime + myAnim.CurrentClipLength, this);
	}

	public virtual void Invulnerable()
	{
		block = true && !_blockNullified;
		if (myHealth != null)
		{
			myHealth.isDeflecting = true && !_blockNullified;
		}
		if (hazardShield && (bool)hazardProxy)
		{
			hazardProxy.SetActive(true && !_blockNullified);
		}
	}

	public virtual void Vulnerable()
	{
		block = false;
		if (myHealth != null)
		{
			myHealth.isDeflecting = false;
		}
		if (hazardShield && (bool)hazardProxy)
		{
			hazardProxy.SetActive(false);
		}
	}

	private void HandleHazardHazardHurtPlayer(object sender, EventArgs e)
	{
		if ((bool)hazardProxy && hazardProxy.spawnedExplosion != null && sender == hazardProxy.spawnedExplosion.GetComponent<Hazard>() && hazardProxy.spawnedExplosion != null)
		{
			hazardProxy.spawnedExplosion.GetComponent<Hazard>().originatingGameObject = base.gameObject.name;
			KnockTroll();
		}
	}

	private void KnockTroll()
	{
		block = false;
		if (myHealth != null)
		{
			myHealth.isDeflecting = false;
			DamageInfo damageInfo = new DamageInfo();
			damageInfo.damageAmount = 100;
			damageInfo.damageType = DamageTypes.normal;
			damageInfo.comboNum = 0;
			myHealth.TakeHit(damageInfo);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		ShipManager.instance.dragMultiTarget[0].targetQueue.RemoveGameObject(base.gameObject);
		base.gameObject.layer = Layers.EnemiesDontTarget;
	}

	private void HandleHazardProxyTriggered(object sender, EventArgs args)
	{
		if (sender == hazardProxy)
		{
			if (block)
			{
				hazardProxyTriggered = true;
			}
			if (SkyIronShield.ActiveShield != null)
			{
				KnockTroll();
			}
		}
	}

	private void DetachShield()
	{
		Accessory[] components = GetComponents<Accessory>();
		Accessory[] array = components;
		foreach (Accessory accessory in array)
		{
			accessory.Detach();
		}
	}

	private IEnumerator VictoryDanceLogic()
	{
		int rand = UnityEngine.Random.Range(1, 4);
		float randWait = UnityEngine.Random.Range(0.4f, 0.8f);
		yield return new WaitForSeconds(randWait);
		myAnim.speed = 1f;
		myAnim.StopCoroutine("Shield_Reload");
		if (rand <= 2)
		{
			Debug.Log("inside dance");
			Vulnerable();
			if (block)
			{
				SoundEventManager.Instance.Play(SFX_ShieldUp, base.gameObject);
				myAnim.PlayAnim("Shield_reload");
				yield return new WaitForSeconds(myAnim.anim.clip.length);
				myAnim.Shield_Victory();
			}
			else
			{
				myAnim.Shield_Victory();
			}
			SoundEventManager.Instance.Play(SFX_ShieldUp, base.gameObject, 0.5f);
			SoundEventManager.Instance.Play(VO_Victory, base.gameObject, 0.3f);
			yield return new WaitForSeconds(myAnim.anim.clip.length);
			myAnim.PlayAnim("Shield_reload");
			myAnim.Offset(0.2f);
			yield return new WaitForSeconds(myAnim.anim.clip.length);
			myAnim.PlayAnim("Shield_idle");
		}
		else
		{
			myAnim.PlayAnim("Shield_idle");
			yield return 0;
		}
	}

	private void VictoryDance()
	{
		StopAllCoroutines();
		StartCoroutine(VictoryDanceLogic());
	}
}
