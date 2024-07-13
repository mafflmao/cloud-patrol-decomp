using System;
using System.Collections;
using UnityEngine;

public class BombAnimator : MonoBehaviour
{
	public WrapMode wrapMode;

	public float offset;

	public bool localAnimation;

	public bool destroyOnComplete;

	public float startDelay;

	private Animation _animation;

	private GameObject _tempAnimationParent;

	private bool _started;

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(BombAnimator), LogLevel.Log);

	private void Awake()
	{
		if (localAnimation)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "Temporary Animation Parent - " + base.gameObject.name;
			gameObject.transform.parent = base.transform.parent;
			gameObject.transform.localPosition = base.transform.localPosition;
			gameObject.transform.localRotation = base.transform.localRotation;
			gameObject.transform.localScale = base.transform.localScale;
			base.transform.parent = gameObject.transform;
			_tempAnimationParent = gameObject;
		}
		_animation = GetComponentInChildren<Animation>();
		if (_animation == null)
		{
			_log.LogError("Animation component not found on child");
		}
		_animation.playAutomatically = false;
		_animation.Stop();
	}

	private void OnEnable()
	{
		Health.TookHit += HandleHealthTookHit;
	}

	private void HandleHealthTookHit(object sender, EventArgs e)
	{
		Health health = (Health)sender;
		if (health.GetComponent<BombAnimator>() != null && health.GetComponent<BombAnimator>() == this)
		{
			Health.TookHit -= HandleHealthTookHit;
			UnityEngine.Object.Destroy(_animation);
			base.transform.parent = base.transform.parent.parent;
			UnityEngine.Object.Destroy(_tempAnimationParent);
		}
	}

	private void OnDestroy()
	{
		if (localAnimation)
		{
			UnityEngine.Object.Destroy(_tempAnimationParent);
		}
	}

	private IEnumerator Start()
	{
		_animation.Play();
		if (startDelay > 0f)
		{
			yield return new WaitForSeconds(0.1f);
			_animation.Stop();
		}
		InvokeHelper.InvokeSafe(Play, startDelay, this);
	}

	private void Play()
	{
		if (_animation == null)
		{
			return;
		}
		_animation.Play();
		_started = true;
		if (wrapMode != 0)
		{
			_animation.clip.wrapMode = wrapMode;
		}
		float num = DifficultyManager.Instance.AnimatedMovementSpeed;
		if (base.transform.childCount > 0 && base.transform.GetChild(0).tag == "Bomb")
		{
			BombAndProjectileSpeedUpgrade passiveUpgradeOrDefault = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<BombAndProjectileSpeedUpgrade>();
			if (passiveUpgradeOrDefault != null)
			{
				num *= passiveUpgradeOrDefault.GetBombSpeed(LevelManager.Instance.RoomsCleared);
				passiveUpgradeOrDefault.SpawnWindOnProjectile(base.transform.GetChild(0), new Vector3(4f, 4f, 4f));
				_log.Log("Spawning some wind");
			}
		}
		_animation[_animation.clip.name].time = offset;
		_animation[_animation.clip.name].speed = num;
	}

	private void LateUpdate()
	{
		if (_animation != null && _started && !_animation.isPlaying && destroyOnComplete)
		{
			if (localAnimation)
			{
				UnityEngine.Object.Destroy(base.transform.parent.gameObject);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
