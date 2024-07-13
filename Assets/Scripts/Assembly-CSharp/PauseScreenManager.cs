using System.Collections;
using UnityEngine;

public class PauseScreenManager : MonoBehaviour
{
	public GameObject pauseScreenPrefab;

	public VolumeGroup _musicVolumeGroup;

	public float musicMuteFactor = 0.25f;

	private Vector3 _pauseScreenPosition = new Vector3(0f, 0f, -60f);

	private float _originalMusicVolumeGroupLevel;

	private PauseScreen _pauseScreenInstance;

	private bool _isPauseScreenVisible;

	private void OnEnable()
	{
		GameManager.PauseStackChanged += HandleGameManagerPauseStackChanged;
	}

	private void OnDisable()
	{
		GameManager.PauseStackChanged -= HandleGameManagerPauseStackChanged;
	}

	private void Awake()
	{
		_pauseScreenInstance = ((GameObject)Object.Instantiate(pauseScreenPrefab)).GetComponent<PauseScreen>();
		HidePauseScreen();
	}

	private void MakePauseScreenVisible()
	{
		ScreenTimeoutUtility.Instance.AllowTimeout = true;
		_pauseScreenInstance.UpdateVisuals();
		_pauseScreenInstance.transform.position = _pauseScreenPosition;
		_isPauseScreenVisible = true;
	}

	private void HidePauseScreen()
	{
		ScreenTimeoutUtility.Instance.AllowTimeout = false;
		_pauseScreenInstance.transform.position = new Vector3(-10000f, 0f, 0f);
		_isPauseScreenVisible = false;
	}

	private void HandleGameManagerPauseStackChanged(object sender, PauseStackChangeEventArgs e)
	{
		if (e.PauseReason != PauseReason.System)
		{
			return;
		}
		if (e.WasPush && !_isPauseScreenVisible)
		{
			if (!GameManager.gameStarted)
			{
				Debug.LogError("Game is being paused before start. Possible race condition.");
			}
			SoundEventManager.Instance.Play2D(GlobalSoundEventData.Instance.PauseSound);
			MakePauseScreenVisible();
			StartCoroutine(MuteVolumeCoroutine());
		}
		else if (!e.WasPush && !GameManager.Instance.IsPauseReasonInStack(PauseReason.System))
		{
			HidePauseScreen();
			StartCoroutine(RestoreVolumeCoroutine());
		}
	}

	private IEnumerator MuteVolumeCoroutine()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		_originalMusicVolumeGroupLevel = _musicVolumeGroup.RuntimeVolume;
		_musicVolumeGroup.RuntimeVolume *= musicMuteFactor;
	}

	private IEnumerator RestoreVolumeCoroutine()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		_musicVolumeGroup.RuntimeVolume = _originalMusicVolumeGroupLevel;
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause && GameManager.Instance != null && !GameManager.Instance.IsPaused && GameManager.gameState == GameManager.GameState.Playing && GameManager.gameStarted && !HealingElixirScreen.IsActive)
		{
			Debug.Log("Application paused... Pausing game.");
			GameManager.Instance.PushPause(PauseReason.System);
		}
	}
}
