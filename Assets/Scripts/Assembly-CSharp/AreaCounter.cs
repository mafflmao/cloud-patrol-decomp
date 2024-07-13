using System;
using System.Collections;
using UnityEngine;

public class AreaCounter : MonoBehaviour
{
	public SpriteText roomCountText;

	public SpriteText difficultyCountText;

	public DropShadowSpriteText areasClearedText;

	public SpriteText recordLabel;

	public ParticleSystem bonusParticles;

	public SoundEventData distanceBonusSFX;

	public SoundEventData distanceBonusFlipSFX;

	public SoundEventData UI_SceneClearAttenuated;

	public SoundEventData difficultyUpSFX;

	private TimedSlider _slider;

	private int _record;

	private bool _isShowingBonusAnimation;

	private LevelManager.NextRoomEventArgs _nextRoomEventArgs;

	public GameObject difficultyUpCutscene;

	public float difficultyUpCutsceneOffset = 300f;

	private void OnEnable()
	{
		LevelManager.DifficultyUp += HandleRoomClearNotification;
		LevelManager.MovingToNextRoom += HandleLevelManagerMovingToNextRoom;
		GameManager.PauseChanged += HandleGameManagerPauseStackChanged;
	}

	private void OnDisable()
	{
		LevelManager.DifficultyUp -= HandleRoomClearNotification;
		GameManager.PauseChanged -= HandleGameManagerPauseStackChanged;
		LevelManager.MovingToNextRoom -= HandleLevelManagerMovingToNextRoom;
	}

	private void Start()
	{
		_slider = GetComponent<TimedSlider>();
		recordLabel.Text = string.Empty;
	}

	private void HandleLevelManagerMovingToNextRoom(object sender, LevelManager.NextRoomEventArgs e)
	{
		if (_isShowingBonusAnimation && !RocketBooster.IsActive && !WingedBoots.IsActive)
		{
			_nextRoomEventArgs = e;
			_nextRoomEventArgs.DelayMove();
		}
	}

	private void HandleGameManagerPauseStackChanged(object sender, EventArgs args)
	{
		bool flag = !GameManager.Instance.IsPauseReasonInStack(PauseReason.System);
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			renderer.enabled = flag;
		}
	}

	private void HandleRoomClearNotification(object sender, EventArgs e)
	{
		int roomsCleared = LevelManager.Instance.RoomsCleared;
		roomCountText.Text = "DIFFICULTY UP";
		difficultyCountText.Text = "Level " + LevelManager.Instance.CurDifficultyIndex + " of " + (LevelManager.Instance.DifficultyCount - 1);
		areasClearedText.Text = string.Empty;
		if (roomsCleared > 0)
		{
			if (roomsCleared % ScoreKeeper.Instance.scoreMultiplierAreaClearInterval == 0)
			{
				_slider.SlideIn();
				_isShowingBonusAnimation = true;
				InvokeHelper.InvokeSafe(ShowBonus, 0.75f, this);
				_slider.restTime = 2f;
			}
			else
			{
				if (!_isShowingBonusAnimation)
				{
					base.GetComponent<Animation>().Play("AreasClearedTickerResetTransform");
					_slider.restTime = 1f;
					_slider.SlideIn();
				}
				SoundEventManager.Instance.Play2D(GlobalSoundEventData.Instance.UI_Scene_Clear);
			}
		}
		ShowBonus();
	}

	private void ShowBonus()
	{
		base.GetComponent<Animation>().Play("AreasClearedTickerSpecialNotification");
	}

	private void EmitBonusParticles()
	{
		bonusParticles.Emit(50);
		SoundEventManager.Instance.Play2D(distanceBonusSFX);
		SoundEventManager.Instance.Play2D(GlobalSoundEventData.Instance.UI_Scene_Clear);
	}

	private void PlayFlipSFX()
	{
	}

	private void ResumeRoomMove()
	{
		if (_nextRoomEventArgs != null)
		{
			_nextRoomEventArgs.ResumeMove();
			_nextRoomEventArgs = null;
		}
	}

	private IEnumerator StartDifficultyCutscene()
	{
		yield return null;
	}

	private void TriggerScorekeeperUpgradeDistanceMultiplier()
	{
		if (GameManager.gameState == GameManager.GameState.Playing)
		{
			SoundEventManager.Instance.Play2D(distanceBonusFlipSFX);
			_isShowingBonusAnimation = false;
			ScoreKeeper.Instance.UpgradeDistanceMultiplier(false);
		}
	}
}
