using System.Collections;
using System.Linq;
using UnityEngine;

public class RocketBooster : Powerup
{
	public const string StorageKey = "rocketBooster";

	public static int RoomsRemaining;

	public static bool IsActive;

	public GameObject rocketOnShipPrefab;

	public SoundEventData sfxRocketRise;

	public SoundEventData sfxRocketStart;

	public SoundEventData sfxRocketLoop;

	public SoundEventData sfxRocketEnd;

	private GameObject _rocketOnShip;

	private Animation _rocketOnShipAnimation;

	private GameObject _particle;

	private int _roomCount;

	public int RoomsToSkip
	{
		get
		{
			return Mathf.RoundToInt(lifeTimeInSeconds);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		LevelManager.ArrivedAtNextRoom += ArrivedAtNextRoomHandler;
		GameManager.GameStateChanged += HandleGameManagerGameStateChanged;
		IsActive = true;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		LevelManager.ArrivedAtNextRoom -= ArrivedAtNextRoomHandler;
		GameManager.GameStateChanged -= HandleGameManagerGameStateChanged;
		IsActive = false;
		Object.Destroy(_rocketOnShip.gameObject, 2f);
		StopSounds();
	}

	private void StopSounds()
	{
		if (sfxRocketRise != null)
		{
			SoundEventManager.Instance.Stop(sfxRocketRise, base.gameObject);
		}
		if (sfxRocketStart != null)
		{
			SoundEventManager.Instance.Stop(sfxRocketStart, base.gameObject);
		}
		if (sfxRocketLoop != null)
		{
			SoundEventManager.Instance.Stop(sfxRocketLoop, base.gameObject);
		}
		if (sfxRocketEnd != null)
		{
			SoundEventManager.Instance.Stop(sfxRocketEnd, base.gameObject);
		}
	}

	private void HandleGameManagerGameStateChanged(object sender, GameManager.GameStateChangedEventArgs e)
	{
		if (e.NewState != GameManager.GameState.Playing)
		{
			StopSounds();
		}
	}

	protected override void HandleTriggered()
	{
		base.HandleTriggered();
		StartCoroutine(RocketStartCoroutine());
	}

	private IEnumerator RocketStartCoroutine()
	{
		_rocketOnShip = Object.Instantiate(rocketOnShipPrefab) as GameObject;
		_rocketOnShip.transform.parent = ShipManager.instance.shipVisual.transform;
		_rocketOnShip.transform.localPosition = Vector3.zero;
		_rocketOnShip.transform.localRotation = Quaternion.identity;
		_rocketOnShipAnimation = _rocketOnShip.GetComponentInChildren<Animation>();
		_rocketOnShipAnimation.Play("RevealRocketAttached");
		if (sfxRocketRise != null)
		{
			SoundEventManager.Instance.Play(sfxRocketRise, base.gameObject);
		}
		yield return new WaitForSeconds(2f);
		ShipManager.instance.DisableTargetting();
		GameManager.CameraShakeLoopStart();
		if (sfxRocketStart != null)
		{
			SoundEventManager.Instance.Play(sfxRocketStart, base.gameObject);
		}
		if (sfxRocketLoop != null)
		{
			SoundEventManager.Instance.Play(sfxRocketLoop, base.gameObject);
		}
		if (!LevelManager.Instance.IsTransitioning)
		{
			SkipRoom(LevelManager.Instance.currentScreenRoot.GetComponentInChildren<ScreenManager>());
		}
	}

	public override void SetLevel(int newLevel, float newLifeTimeInSeconds)
	{
		base.SetLevel(newLevel, newLifeTimeInSeconds);
		int num = Mathf.CeilToInt(10f * Random.value);
		int num2 = Mathf.CeilToInt(10f * Random.value * 0.5f);
		lifeTimeInSeconds = Mathf.RoundToInt(newLifeTimeInSeconds) + num - num2;
	}

	public void ArrivedAtNextRoomHandler(object sender, LevelManager.NextRoomEventArgs e)
	{
		RoomsRemaining = RoomsToSkip - _roomCount;
		if (!base.IsTriggered)
		{
			return;
		}
		if (RoomsRemaining <= 0)
		{
			DestroyAndFinish(true);
		}
		else if (RoomsRemaining == 1)
		{
			_rocketOnShipAnimation.Play("HideRocketAttached");
			GameManager.CameraShakeLoopStop();
			if (sfxRocketLoop != null)
			{
				SoundEventManager.Instance.Stop(sfxRocketLoop, base.gameObject);
			}
			if (sfxRocketEnd != null)
			{
				SoundEventManager.Instance.Play(sfxRocketEnd, base.gameObject);
			}
			SkipRoom(e.ScreenManager);
			ShipManager.instance.EnableTargetting();
		}
		else
		{
			SkipRoom(e.ScreenManager);
		}
	}

	private void SkipRoom(ScreenManager roomToSkip)
	{
		_roomCount++;
		GameManager.sessionStats.totalAreaSkipsUsed++;
		roomToSkip.skipLevel = true;
		foreach (Health item in Object.FindObjectsOfType(typeof(Health)).Cast<Health>())
		{
			item.gameObject.SendMessage("Disable", SendMessageOptions.DontRequireReceiver);
		}
		GameManager.KillAllProjectiles();
	}
}
