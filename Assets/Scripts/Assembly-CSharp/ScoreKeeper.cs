using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : SingletonMonoBehaviour
{
	private const int SpriteTextsToGenerateOnStart = 10;

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(ScoreKeeper), LogLevel.Log);

	public SpriteText scoreText;

	public ScoreData scoreData;

	public SpriteText powerupMultiplierText;

	public SpriteText distanceMultiplierText;

	public GameObject distanceMultiplierAward;

	public SpriteText distanceMultiplierAwardText;

	public int scoreMultiplierAreaClearInterval = 10;

	public int distanceScoreIncrement = 5;

	public ParticleSystem multiplierCombineFlash;

	public SpriteText magicItemNameText;

	public SpriteText magicItemLevelText;

	public Animation magicItemTextAnimation;

	public Animation scoreAnimation;

	public Animation highScoreAnimation;

	public ParticleSystem scoreParticles;

	private Material particleMaterial;

	public SoundEventData newHighScoreSFX;

	public ParticleSystem highScoreParticles;

	public Color bonusColor;

	public Color regularColor;

	private int permanentScoreMultiplier;

	private int distanceScoreMultiplier = 1;

	private int powerupScoreMultiplier;

	public int combinedScoreMultiplier = 1;

	private int increasedScoreTimes;

	public GameObject scoreNumber;

	public static bool newHighScore = false;

	public SoundEventData multiplierUpgradeTinkleSFX;

	private Queue<SpriteText> _scoreTextQueue = new Queue<SpriteText>();

	private bool doRocketBoosterScoreIncrease;

	private float rocketBoosterLastXPosition;

	private Transform mainCameraTransform;

	public Vector2 rocketBoosterScoreRange = new Vector2(10f, 20f);

	private Health lastHitHealth;

	public static ScoreKeeper Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<ScoreKeeper>();
		}
	}

	protected override void AwakeOnce()
	{
		base.AwakeOnce();
		scoreData.BuildDictionary();
		newHighScore = false;
	}

	private void Start()
	{
		scoreText.Text = GameManager.currentScore.ToString("n0");
		UpdateCombinedMultiplier();
		magicItemNameText.GetComponent<Renderer>().enabled = false;
		magicItemLevelText.GetComponent<Renderer>().enabled = false;
		particleMaterial = scoreParticles.GetComponent<Renderer>().materials[0];
		increasedScoreTimes = 0;
		for (int i = 0; i < 10; i++)
		{
			_scoreTextQueue.Enqueue(InstantiateScoreSpriteText());
		}
		DistanceBonusMultiplierUpgrade passiveUpgradeOrDefault = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<DistanceBonusMultiplierUpgrade>();
		if (passiveUpgradeOrDefault != null && passiveUpgradeOrDefault.roomToApply == 0)
		{
			UpgradeDistanceMultiplier(true);
		}
		distanceMultiplierAward.GetComponent<Animation>().Play("HideDistanceBonusAward");
	}

	public void AddScore(ScoreData.ScoreType scoreType)
	{
		int scoreFromType = scoreData.GetScoreFromType(scoreType);
		if (scoreType == ScoreData.ScoreType.BOMBMALUS)
		{
			GameManager.bombsPtsLostInVoyage += scoreFromType;
		}
		AddScore(scoreFromType, Vector3.zero);
	}

	public void AddScore(ScoreData.ScoreType scoreType, Vector3 worldPosition, bool useMultiplier)
	{
		int num = scoreData.GetScoreFromType(scoreType);
		if (useMultiplier)
		{
			num *= combinedScoreMultiplier;
		}
		if (scoreType == ScoreData.ScoreType.BOMBMALUS)
		{
			GameManager.bombsPtsLostInVoyage += num;
		}
		AddScore(num, worldPosition);
	}

	public void AddScore(int score, Vector3 worldPosition, bool useMultiplier)
	{
		int num = score;
		if (useMultiplier)
		{
			num *= combinedScoreMultiplier;
		}
		AddScore(num, worldPosition);
	}

	private void AddScore(int score, Vector3 worldPosition)
	{
		UpdateScoreCounter(score, true);
		SpawnScoreNumber(score, worldPosition);
	}

	public void AddScore(int score)
	{
		UpdateScoreCounter(score, false);
	}

	public void AddPermanentScoreMultiplier(int score)
	{
		permanentScoreMultiplier += score;
		UpdateCombinedMultiplier();
	}

	public void Dim(bool doDim)
	{
		if (doDim)
		{
			scoreText.SetColor(new Color(scoreText.Color.r, scoreText.Color.g, scoreText.Color.b, 0.1f));
			powerupMultiplierText.SetColor(new Color(powerupMultiplierText.Color.r, powerupMultiplierText.Color.g, powerupMultiplierText.Color.b, 0.1f));
			distanceMultiplierText.SetColor(new Color(distanceMultiplierText.Color.r, distanceMultiplierText.Color.g, distanceMultiplierText.Color.b, 0.1f));
		}
		else
		{
			scoreText.SetColor(new Color(scoreText.Color.r, scoreText.Color.g, scoreText.Color.b, 1f));
			powerupMultiplierText.SetColor(new Color(powerupMultiplierText.Color.r, powerupMultiplierText.Color.g, powerupMultiplierText.Color.b, 1f));
			distanceMultiplierText.SetColor(new Color(distanceMultiplierText.Color.r, distanceMultiplierText.Color.g, distanceMultiplierText.Color.b, 1f));
		}
	}

	private void OnEnable()
	{
		Health.Killed += HandleHealthKilled;
		Shooter.ComboCompleted += HandleComboScore;
		LevelManager.RoomClear += HandleRoomClear;
		Powerup.Triggered += HandlePowerupTriggered;
		PowerupCutscene.CutsceneStarted += HandlePowerupCutsceneStarted;
		PowerupCutscene.Completed += HandlePowerupCutsceneCompleted;
		Powerup.Finished += HandlePowerupFinished;
		TimedSlider.SlideInOutComplete += HandleSlideInOutComplete;
		TimedSlider.SlideInBegin += HandleTimedSliderSlideInBegin;
	}

	private void HandlePowerupCutsceneCompleted(object sender, EventArgs e)
	{
		UpdateCombinedMultiplier();
	}

	private void OnDisable()
	{
		Health.Killed -= HandleHealthKilled;
		Shooter.ComboCompleted -= HandleComboScore;
		LevelManager.RoomClear -= HandleRoomClear;
		PowerupCutscene.CutsceneStarted -= HandlePowerupCutsceneStarted;
		PowerupCutscene.Completed -= HandlePowerupCutsceneCompleted;
		Powerup.Triggered -= HandlePowerupTriggered;
		Powerup.Finished -= HandlePowerupFinished;
		TimedSlider.SlideInOutComplete -= HandleSlideInOutComplete;
		TimedSlider.SlideInBegin -= HandleTimedSliderSlideInBegin;
	}

	private void HandleTimedSliderSlideInBegin(object sender, EventArgs e)
	{
		if (sender is TimedSlider && (sender as TimedSlider).obscuresScore)
		{
			Dim(true);
		}
	}

	private void HandleSlideInOutComplete(object sender, EventArgs args)
	{
		if (sender is TimedSlider && (sender as TimedSlider).obscuresScore)
		{
			Dim(false);
		}
	}

	private void HandlePowerupCutsceneStarted(object sender, EventArgs e)
	{
		Dim(false);
		string clipName = "MagicMomentText";
		PowerupCutscene powerupCutscene = sender as PowerupCutscene;
		Powerup component = powerupCutscene.gameObject.GetComponent<Powerup>();
		if (component != null && component.PowerupData.affectsScoreMultiplier)
		{
			powerupScoreMultiplier = component.PowerupData.GetScoreModifier(component.Level);
			powerupMultiplierText.GetComponent<Renderer>().enabled = true;
			powerupMultiplierText.Text = LocalizationManager.Instance.GetFormatString("GAME_MAGIC_ITEM_BONUS", powerupScoreMultiplier);
		}
		magicItemNameText.Text = component.PowerupData.LocalizedName;
		if (!component.PowerupData.canUpgrade)
		{
			magicItemLevelText.Text = string.Empty;
		}
		else if (component.Level > 0)
		{
			magicItemLevelText.Text = LocalizationManager.Instance.GetFormatString("GAME_MAGIC_ITEM_LEVEL", component.Level);
		}
		else
		{
			magicItemLevelText.Text = LocalizationManager.Instance.GetString("GAME_MAGIC_ITEM_SNEAK_PREVIEW");
		}
		magicItemTextAnimation.Stop();
		StartCoroutine(AnimationUtils.PlayIgnoringTimescale(magicItemTextAnimation, clipName, UpdateCombinedMultiplier));
	}

	public void UpdateCombinedMultiplier()
	{
		_log.LogDebug("UpdateCombinedMultiplier()");
		float num = combinedScoreMultiplier;
		combinedScoreMultiplier = permanentScoreMultiplier + distanceScoreMultiplier + powerupScoreMultiplier;
		if (combinedScoreMultiplier == 0)
		{
			combinedScoreMultiplier = 1;
		}
		if (powerupScoreMultiplier > 0)
		{
			_log.LogDebug("Powerup Multiplier is active");
			powerupMultiplierText.Text = LocalizationManager.Instance.GetFormatString("GAME_MAGIC_ITEM_BONUS", combinedScoreMultiplier);
			distanceMultiplierText.GetComponent<Renderer>().enabled = false;
			magicItemTextAnimation.PlayQueued("BonusActive");
		}
		else if (distanceScoreMultiplier > 1 || permanentScoreMultiplier > 1)
		{
			_log.LogDebug("Only Distance Score Multiplier is active");
			distanceMultiplierText.GetComponent<Renderer>().enabled = true;
			distanceMultiplierText.Text = "x" + combinedScoreMultiplier + " score bonus";
			powerupMultiplierText.GetComponent<Renderer>().enabled = false;
		}
		else
		{
			_log.LogDebug("No Multipliers Are Active");
			distanceMultiplierText.GetComponent<Renderer>().enabled = false;
			powerupMultiplierText.GetComponent<Renderer>().enabled = false;
		}
		if ((float)combinedScoreMultiplier > num)
		{
			SoundEventManager.Instance.Play2D(multiplierUpgradeTinkleSFX);
		}
	}

	private void HandlePowerupTriggered(object sender, PowerupEventArgs e)
	{
		if (sender is RocketBooster)
		{
			mainCameraTransform = Camera.main.transform;
			doRocketBoosterScoreIncrease = true;
			rocketBoosterLastXPosition = mainCameraTransform.position.x;
		}
	}

	private void Update()
	{
		if (doRocketBoosterScoreIncrease)
		{
			float num = Mathf.Abs(rocketBoosterLastXPosition - mainCameraTransform.position.x);
			if (num > 0.5f)
			{
				AddScore(100);
				rocketBoosterLastXPosition = mainCameraTransform.position.x;
			}
		}
	}

	private void HandlePowerupFinished(object sender, EventArgs e)
	{
		if ((sender as Powerup).PowerupData.affectsScoreMultiplier)
		{
			powerupScoreMultiplier = 0;
			UpdateCombinedMultiplier();
		}
		if (sender is RocketBooster)
		{
			doRocketBoosterScoreIncrease = false;
		}
	}

	private void HandleHealthKilled(object sender, EventArgs args)
	{
		if (sender is Health)
		{
			Health health = (lastHitHealth = (Health)sender);
			if (GhostSwords.Instance != null || AnvilRain.IsActive || GoldAnvilRain.IsActive)
			{
				AddScore(health.scoreType, health.GetComponent<Collider>().bounds.center, true);
			}
		}
	}

	private void SpawnScoreNumber(int score, Vector3 worldPosition)
	{
		if (score != 0)
		{
			if (worldPosition == Vector3.zero)
			{
				worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 3, 3f));
			}
			Vector3 position = Camera.main.WorldToScreenPoint(worldPosition);
			Vector3 vector = GUISystem.Instance.guiCamera.GetComponent<Camera>().ScreenToWorldPoint(position);
			vector = new Vector3(vector.x, vector.y, 100f);
			SpriteText nextScoreSpriteText = GetNextScoreSpriteText();
			nextScoreSpriteText.Hide(false);
			nextScoreSpriteText.transform.position = vector;
			nextScoreSpriteText.transform.rotation = Quaternion.identity;
			Color white = Color.white;
			string text = "ScoreNumberSmall";
			float characterSize = 120f;
			if (score >= 20)
			{
				characterSize = 120f;
			}
			if (score >= 100)
			{
				characterSize = 130f;
			}
			if (score >= 300)
			{
				characterSize = 150f;
			}
			if (score >= 1000)
			{
				characterSize = 200f;
				text = "ScoreNumberLarge";
			}
			if (score >= 5000)
			{
				characterSize = 300f;
				text = "ScoreNumberLarge";
			}
			if (score >= 0)
			{
				white = ((powerupScoreMultiplier <= 0) ? Color.white : new Color(1f, 0.8f, 0.2f, 1f));
			}
			else
			{
				white = Color.red;
				characterSize = 200f;
			}
			string text2 = ((score <= 0) ? string.Empty : "+");
			nextScoreSpriteText.Text = text2 + score;
			nextScoreSpriteText.Color = white;
			nextScoreSpriteText.SetCharacterSize(characterSize);
			nextScoreSpriteText.GetComponent<Animation>().Play(text);
			StartCoroutine(RecycleScoreSpriteTextCoroutine(nextScoreSpriteText, nextScoreSpriteText.GetComponent<Animation>()[text].length));
		}
	}

	private SpriteText GetNextScoreSpriteText()
	{
		if (_scoreTextQueue.Count > 0)
		{
			return _scoreTextQueue.Dequeue();
		}
		return InstantiateScoreSpriteText();
	}

	private SpriteText InstantiateScoreSpriteText()
	{
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(scoreNumber);
		SpriteText component = gameObject.GetComponent<SpriteText>();
		component.Hide(true);
		return component;
	}

	private IEnumerator RecycleScoreSpriteTextCoroutine(SpriteText scoreNumberSpriteText, float time)
	{
		yield return new WaitForSeconds(time);
		scoreNumberSpriteText.Hide(true);
		_scoreTextQueue.Enqueue(scoreNumberSpriteText);
	}

	private void HandleRoomClear(object sender, EventArgs args)
	{
		if (LevelManager.Instance.RoomsCleared > 0 && !RocketBooster.IsActive)
		{
			int scoreFromType = scoreData.GetScoreFromType(ScoreData.ScoreType.ROOMCLEARED);
			scoreFromType *= combinedScoreMultiplier;
			UpdateScoreCounter(scoreFromType, true);
		}
	}

	public void UpgradeDistanceMultiplier(bool disableIncrement)
	{
		increasedScoreTimes++;
		switch (increasedScoreTimes)
		{
		case 1:
			distanceScoreIncrement = OperatorMenu.Instance.m_ScoreBonus1 - 1;
			break;
		case 2:
			distanceScoreIncrement = OperatorMenu.Instance.m_ScoreBonus2 - OperatorMenu.Instance.m_ScoreBonus1;
			break;
		case 3:
			distanceScoreIncrement = OperatorMenu.Instance.m_ScoreBonus3 - OperatorMenu.Instance.m_ScoreBonus2;
			break;
		default:
			distanceScoreIncrement = 1;
			break;
		}
		if (!disableIncrement)
		{
			distanceScoreMultiplier += distanceScoreIncrement;
		}
		DistanceBonusMultiplierUpgrade passiveUpgradeOrDefault = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<DistanceBonusMultiplierUpgrade>();
		if (passiveUpgradeOrDefault != null && !passiveUpgradeOrDefault.AlreadyAppliedUpdate && LevelManager.Instance.RoomsCleared >= passiveUpgradeOrDefault.roomToApply)
		{
			distanceScoreMultiplier += passiveUpgradeOrDefault.multiplierBoost;
			passiveUpgradeOrDefault.AlreadyAppliedUpdate = true;
		}
		distanceScoreIncrement = Mathf.Max(0, distanceScoreIncrement);
		if (distanceScoreIncrement > 0)
		{
			distanceMultiplierAwardText.Text = "+" + distanceScoreIncrement + "x";
			distanceMultiplierAward.GetComponent<Animation>().Play("AwardDistanceBonus");
			InvokeHelper.InvokeSafe(UpdateCombinedMultiplier, distanceMultiplierAward.GetComponent<Animation>().clip.length, this);
		}
	}

	private void UpdateScoreCounter(int score, bool doFX)
	{
	
	}

	private void HandleComboScore(object sender, Shooter.ComboCompletedEventArgs args)
	{
		ScoreData.ScoreType scoreType;
		switch (args.Number)
		{
		case 1:
			scoreType = ((!(lastHitHealth != null)) ? ScoreData.ScoreType.COMBO_1X : lastHitHealth.scoreType);
			break;
		case 2:
			scoreType = ScoreData.ScoreType.COMBO_2X;
			break;
		case 3:
			scoreType = ScoreData.ScoreType.COMBO_3X;
			break;
		case 4:
			scoreType = ScoreData.ScoreType.COMBO_4X;
			break;
		case 5:
			scoreType = ScoreData.ScoreType.COMBO_5X;
			break;
		case 6:
			scoreType = ScoreData.ScoreType.COMBO_MAX;
			break;
		default:
			scoreType = ((lastHitHealth != null) ? lastHitHealth.scoreType : ScoreData.ScoreType.DEFAULT);
			break;
		}
		bool useMultiplier = GhostSwords.Instance == null && !AnvilRain.IsActive;
		AddScore(scoreType, Shooter.lastTargetWorldPosition, useMultiplier);
	}
}
