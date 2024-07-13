using System.Collections;
using System.Linq;
using UnityEngine;

public class WingedBoots : Powerup
{
	public const string StorageKey = "wingedboots";

	public static bool IsActive;

	public SoundEventData sfxRoomTransition2D;

	public GameObject particlePrefab;

	private GameObject _particle;

	private int _roomsSkipped;

	private float currentTime;

	private bool isFading;

	private float fadeTime = 0.2f;

	private Color _fadeInColor = new Color(0.5f, 0.5f, 0.5f, 1f);

	private Color _fadeOutColor = new Color(0.5f, 0.5f, 0.5f, 0f);

	private Color _targetColor = Color.black;

	public int RoomsToSkip
	{
		get
		{
			return Mathf.RoundToInt(lifeTimeInSeconds);
		}
	}

	private int RoomSkipsRemaining
	{
		get
		{
			return Mathf.RoundToInt(base.TimeLeft);
		}
	}

	public static int GetNumberOfRoomsToSkipAtLevel(int level)
	{
		return level;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		LevelManager.ArrivedAtNextRoom += ArrivedAtNextRoomHandler;
		LevelManager.MovingToNextRoom += HandleLevelManagerMovingToNextRoom;
		IsActive = true;
	}

	private void HandleLevelManagerMovingToNextRoom(object sender, LevelManager.NextRoomEventArgs e)
	{
		if (e.MoveDirection == LevelManager.MoveDirections.Down)
		{
			MoveRoomFX(new Vector3(0f, 180f, 90f));
		}
		else if (e.MoveDirection == LevelManager.MoveDirections.Up)
		{
			MoveRoomFX(new Vector3(0f, 180f, -90f));
		}
		else if (e.MoveDirection == LevelManager.MoveDirections.Right)
		{
			MoveRoomFX(new Vector3(0f, 180f, 180f));
		}
		StartCoroutine(FadeParticle(_fadeInColor, 0f));
		StartCoroutine(FadeParticle(_fadeOutColor, 0.5f));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		LevelManager.ArrivedAtNextRoom -= ArrivedAtNextRoomHandler;
		LevelManager.MovingToNextRoom -= HandleLevelManagerMovingToNextRoom;
		if ((bool)_particle)
		{
			Object.Destroy(_particle);
		}
		IsActive = false;
	}

	protected override void HandleTriggered()
	{
		base.HandleTriggered();
		ShipManager.instance.DisableTargetting();
		if (!LevelManager.Instance.IsTransitioning)
		{
			SkipRoom();
		}
	}

	private void MoveRoomFX(Vector3 direction)
	{
		if (!_particle)
		{
			_particle = (GameObject)Object.Instantiate(particlePrefab);
			_particle.transform.parent = Camera.main.transform;
			_particle.transform.position = Camera.main.transform.position + new Vector3(0f, 0.3f, -2.5f);
		}
		_particle.transform.rotation = Quaternion.Euler(direction);
	}

	public void ArrivedAtNextRoomHandler(object sender, LevelManager.NextRoomEventArgs e)
	{
		if (base.IsTriggered)
		{
			if (_roomsSkipped >= RoomsToSkip)
			{
				ShipManager.instance.EnableTargetting(false);
				DestroyAndFinish(true);
			}
			else
			{
				SkipRoom();
			}
		}
	}

	private void SkipRoom()
	{
		float percentLeft = Mathf.Clamp((float)_roomsSkipped / (float)RoomsToSkip, 0f, 0.99f);
		base.Holder.UpdateTime(percentLeft);
		_roomsSkipped++;
		SoundEventManager.Instance.Play2D(sfxRoomTransition2D);
		GameManager.sessionStats.totalAreaSkipsUsed++;
		foreach (ScreenManager item in Object.FindObjectsOfType(typeof(ScreenManager)).Cast<ScreenManager>())
		{
			item.skipLevel = true;
		}
		foreach (Health item2 in Object.FindObjectsOfType(typeof(Health)).Cast<Health>())
		{
			item2.gameObject.SendMessage("Disable", SendMessageOptions.DontRequireReceiver);
		}
		GameManager.KillAllProjectiles();
	}

	private IEnumerator FadeParticle(Color fadeColor, float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		isFading = true;
		_targetColor = fadeColor;
		currentTime = 0f;
	}

	protected override void Update()
	{
		if (!isFading || !_particle)
		{
			return;
		}
		Color color = _particle.GetComponent<Renderer>().material.GetColor("_TintColor");
		if (currentTime < fadeTime)
		{
			float t = Mathf.Clamp01(currentTime / fadeTime);
			_particle.GetComponent<Renderer>().material.SetColor("_TintColor", Color.Lerp(color, _targetColor, t));
			currentTime += Time.deltaTime;
			return;
		}
		isFading = false;
		currentTime = 0f;
	}
}
