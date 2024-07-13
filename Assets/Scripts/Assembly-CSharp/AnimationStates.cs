using System.Collections;
using UnityEngine;

public class AnimationStates : MonoBehaviour
{
	public const string SPAWN = "spawn";

	public const string WALK = "walk";

	public const string RUN = "run";

	public const string JUMP = "jump";

	public const string DEATH = "death";

	public const string DEATHSPIN = "deathspin";

	public const string IDLE = "idle";

	public const string VICTORY = "victory";

	public const string VICTORYSPIN = "victorySpin";

	public const string DAMAGE = "damage";

	public const string CELEBRATE = "celebrate";

	public const string SHOOT = "shoot";

	public const string ALIVE = "alive";

	public const string AIM = "aim";

	public const string READY = "ready";

	public const string SHOOTOUT = "shootout";

	public const string CARRY = "carry";

	public const string FALL = "fall";

	public const string TAKEHIT = "takeHit";

	public const string RELOAD = "reload";

	public const string REACT = "react";

	public const string COVER = "cover";

	public const string FLY = "fly";

	public const string TAUNT = "taunt";

	public const string TAUNTIN = "tauntIn";

	public const string SHOOT_LOB = "Lobber_shoot";

	public const string RELOAD_LOB = "Lobber_reload";

	public const string READY_LOB = "Lobber_ready";

	public const string AIM_LOB = "Lobber_aim";

	public const string IDLE_LOB = "Lobber_idle";

	public const string SHOOT_GUNNER = "Gunner_shoot";

	public const string READY_GUNNER = "Gunner_ready";

	public const string AIM_GUNNER = "Gunner_aim";

	public const string IDLE_GUNNER = "Gunner_idle";

	public const string VICTORY_GUNNER = "Gunner_victory";

	public const string IDLE_SHIELD = "Shield_idle";

	public const string READY_SHIELD = "Shield_ready";

	public const string RELOAD_SHIELD = "Shield_reload";

	public const string VICTORY_SHIELD = "Shield_victory";

	public const string SHOOT_GUNNER_SHIELD = "ShieldGunner_shoot";

	public const string AIM_GUNNER_SHIELD = "ShieldGunner_aim";

	public const string IDLE_WIZARD = "Wizard_idle";

	public const string APPEAR_WIZARD = "Wizard_appear";

	public const string READY_WIZARD = "Wizard_ready";

	public const string IDLE_JESTER = "Jester_idle";

	public const string APPEAR_JESTER = "Jester_appear";

	public const string DISAPPEAR_JESTER = "Jester_disappear";

	public const string DIZZY_JESTER = "Jester_dizzy";

	public const string IDLE_JETPACK = "jetpack_idle";

	public const string AIM_JETPACK = "jetpack_aim";

	public const string SHOOT_JETPACK = "jetpack_shoot";

	public const string IDLE_JETPACK_BOTTOM = "Partial_JetpackIdle";

	public const string IDLE_JETPACK_TOP = "Partial_GunnerIdle";

	private float crossfadeLength = 0.2f;

	public Animation anim;

	[HideInInspector]
	public float speed = 1f;

	private int layerDefault;

	private int layerFly = 1;

	private int layerDeath = 10;

	public float CurrentClipLength
	{
		get
		{
			return anim.clip.length;
		}
	}

	private void Start()
	{
		if (!(anim == null))
		{
			return;
		}
		if (GetComponent<Animation>() == null)
		{
			Animation[] componentsInChildren = GetComponentsInChildren<Animation>();
			Animation[] array = componentsInChildren;
			foreach (Animation animation in array)
			{
				if (animation.GetClipCount() > 0)
				{
					anim = animation;
					break;
				}
			}
		}
		else
		{
			anim = base.GetComponent<Animation>();
		}
	}

	public float PlayAnim(string _clip)
	{
		return PlayAnim(_clip, layerDefault, false);
	}

	public float PlayAnim(string _clip, int _layer)
	{
		return PlayAnim(_clip, _layer, false);
	}

	public float PlayAnim(string _clip, int _layer, bool stopAll)
	{
		if ((bool)anim)
		{
			if ((bool)anim.gameObject)
			{
				anim.gameObject.SetActive(true);
			}
			if ((bool)anim[_clip])
			{
				anim[_clip].layer = _layer;
				anim[_clip].speed = speed;
				anim.clip = anim[_clip].clip;
				if (stopAll)
				{
					anim.CrossFade(_clip, crossfadeLength, PlayMode.StopAll);
				}
				else
				{
					anim.CrossFade(_clip, crossfadeLength, PlayMode.StopSameLayer);
				}
				return anim[_clip].length;
			}
		}
		return -1f;
	}

	public void Offset(float time)
	{
		if ((bool)anim && (bool)anim.clip)
		{
			anim[anim.clip.name].time = time;
		}
	}

	public void Spawn()
	{
		PlayAnim("spawn");
	}

	public void Walk()
	{
		PlayAnim("walk");
	}

	public void TakeHit()
	{
		PlayAnim("takeHit");
	}

	public void Reload()
	{
		PlayAnim("reload");
	}

	public void Run()
	{
		PlayAnim("run");
	}

	public void Fall()
	{
		PlayAnim("fall");
	}

	public void Jump()
	{
		PlayAnim("jump");
	}

	public void Fly()
	{
		PlayAnim("fly");
	}

	public IEnumerator Sheep_Jump()
	{
		PlayAnim("jump");
		if (anim != null)
		{
			yield return new WaitForSeconds(anim.clip.length / speed);
			Fall();
		}
	}

	public IEnumerator Sheep_Takehit()
	{
		PlayAnim("takeHit");
		if (anim != null)
		{
			yield return new WaitForSeconds(anim.clip.length / speed);
			Idle();
		}
	}

	public IEnumerator Sheep_TakeAirHit()
	{
		PlayAnim("takeHit");
		if (anim != null)
		{
			yield return new WaitForSeconds(anim.clip.length / speed);
			Fly();
		}
	}

	public void Idle()
	{
		PlayAnim("idle");
	}

	public void Idle(bool crossfade, float crossfadeLength)
	{
		if (crossfade)
		{
			anim.CrossFade("idle", crossfadeLength, PlayMode.StopAll);
		}
	}

	public void Victory()
	{
		PlayAnim("victory");
	}

	public IEnumerator VictoryToIdle()
	{
		PlayAnim("victory");
		if (anim != null)
		{
			yield return new WaitForSeconds(anim.clip.length / speed);
			Idle();
		}
	}

	public IEnumerator VictorySpinToIdle()
	{
		PlayAnim("victorySpin");
		if (anim != null)
		{
			yield return new WaitForSeconds(anim.clip.length / speed);
			Idle();
		}
	}

	public void VictorySpin()
	{
		PlayAnim("victorySpin");
	}

	public void Damage()
	{
		PlayAnim("damage");
	}

	public void Celebrate()
	{
		PlayAnim("celebrate");
	}

	public void Shoot()
	{
		PlayAnim("shoot");
	}

	public void Carry()
	{
		PlayAnim("carry");
	}

	public void React()
	{
		PlayAnim("react");
	}

	public void Cover()
	{
		PlayAnim("cover");
	}

	public void Taunt()
	{
		PlayAnim("taunt");
	}

	public void TauntIn()
	{
		PlayAnim("tauntIn");
	}

	public void Jetpack_Idle()
	{
		PlayAnim("jetpack_idle", layerFly);
	}

	public void Jetpack_Idle_NoGun()
	{
		PlayAnim("Partial_JetpackIdle", layerFly);
		PlayAnim("idle");
	}

	public void Jetpack_Idle_Init()
	{
		PlayAnim("jetpack_idle", layerFly);
	}

	public IEnumerator Ready()
	{
		PlayAnim("ready");
		if (anim != null)
		{
			yield return new WaitForSeconds(anim.clip.length / speed);
			Aim();
		}
	}

	public IEnumerator JetpackGunner_Ready()
	{
		PlayAnim("jetpack_aim", layerFly);
		if (anim != null)
		{
			yield return new WaitForSeconds(anim.clip.length / speed);
			JetpackGunner_Aim();
		}
	}

	public IEnumerator Gunner_Ready()
	{
		PlayAnim("Gunner_ready");
		if (anim != null)
		{
			yield return new WaitForSeconds(anim.clip.length / speed);
			Gunner_Aim();
		}
	}

	public IEnumerator Shield_Reload()
	{
		PlayAnim("Shield_reload");
		if (anim != null)
		{
			yield return new WaitForSeconds(anim.clip.length / speed);
			Shield_Idle();
		}
	}

	public void Gunner_Aim()
	{
		PlayAnim("Gunner_aim");
	}

	public void JetpackGunner_Aim()
	{
		PlayAnim("jetpack_aim", layerFly);
	}

	public void Shield_Gunner_Aim()
	{
		PlayAnim("ShieldGunner_aim");
	}

	public IEnumerator Shield_Gunner_Ready()
	{
		PlayAnim("Shield_reload");
		if (anim != null)
		{
			yield return new WaitForSeconds(anim.clip.length / speed);
			Shield_Gunner_Aim();
		}
	}

	public void Gunner_Idle()
	{
		PlayAnim("Gunner_idle", layerDefault);
	}

	public void Gunner_Idle_Init()
	{
		PlayAnim("Gunner_idle", layerDefault, true);
	}

	public void Gunner_Shoot()
	{
		PlayAnim("Gunner_shoot");
	}

	public void Gunner_Victory()
	{
		PlayAnim("Gunner_victory");
	}

	public void JetpackGunner_Shoot()
	{
		PlayAnim("jetpack_shoot", layerFly);
	}

	public void Shield_Gunner_Shoot()
	{
		PlayAnim("ShieldGunner_shoot");
	}

	public void Shield_Idle()
	{
		speed = 1f;
		PlayAnim("Shield_idle");
	}

	public float Shield_Ready()
	{
		return PlayAnim("Shield_ready");
	}

	public void Shield_Victory()
	{
		PlayAnim("Shield_victory");
	}

	public void Jester_Idle()
	{
		PlayAnim("Jester_idle");
	}

	public void Jester_Disappear()
	{
		PlayAnim("Jester_disappear");
	}

	public void Jester_AppearFast()
	{
		PlayAnim("Jester_appear");
	}

	public IEnumerator Jester_Appear()
	{
		PlayAnim("Jester_appear");
		if (anim != null)
		{
			yield return new WaitForSeconds(anim.clip.length / speed);
			Jester_Dizzy();
		}
	}

	public void Jester_Dizzy()
	{
		PlayAnim("Jester_dizzy");
	}

	public void Wizard_Idle()
	{
		PlayAnim("Wizard_idle");
	}

	public void Wizard_Ready()
	{
		PlayAnim("Wizard_ready");
	}

	public IEnumerator Wizard_Appear()
	{
		PlayAnim("Wizard_appear");
		if (anim != null)
		{
			yield return new WaitForSeconds(anim.clip.length / speed);
			Wizard_Idle();
		}
	}

	public void Lobber_Reload()
	{
		PlayAnim("Lobber_reload");
	}

	public void Lobber_Aim()
	{
		PlayAnim("Lobber_aim");
	}

	public void Lobber_Ready()
	{
		PlayAnim("Lobber_ready");
	}

	public void Lobber_Idle()
	{
		PlayAnim("Lobber_idle");
	}

	public void Lobber_Shoot()
	{
		PlayAnim("Lobber_shoot");
	}

	public void Aim()
	{
		PlayAnim("aim");
	}

	public void ShootOut()
	{
		PlayAnim("shootout");
	}

	public IEnumerator Alive()
	{
		PlayAnim("alive");
		if (anim != null)
		{
			yield return new WaitForSeconds(anim.clip.length / speed);
			Idle();
		}
	}

	public void Death(float time)
	{
		if (anim != null)
		{
			string clip = "death";
			if (Random.Range(0f, 1f) > 0.5f)
			{
				clip = "deathspin";
			}
			if (base.gameObject.activeSelf)
			{
				StartCoroutine(DeathCoroutine(clip, time));
			}
		}
		else
		{
			Object.Destroy(base.gameObject, time);
		}
	}

	public void Death()
	{
		Death(0f);
	}

	private IEnumerator DeathCoroutine(string _clip, float time)
	{
		PlayAnim(_clip, layerDeath);
		yield return new WaitForSeconds(time);
		Health poof = GetComponent<Health>();
		if ((bool)poof.poofEffect)
		{
			Object.Instantiate(poof.poofEffect, base.transform.position, Quaternion.Euler(0f, 0f, 0f));
		}
		SoundEventManager.Instance.Play(GlobalSoundEventData.Instance.EnemyPoof, base.gameObject);
		StopAllCoroutines();
		CancelInvoke();
		Object.Destroy(base.gameObject);
	}
}
