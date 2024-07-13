using System;
using System.Collections.Generic;
using UnityEngine;

public class BonusManager : MonoBehaviour
{
	public const int PerfectRewardNumber = 6;

	private float timer;

	private GameObject bonusText;

	private int numToKill;

	private int numKilled;

	public GameObject[] objsToDestroy;

	private int showBonusStart;

	public GameObject[] explodingThings;

	public GameObject explodingFX;

	public bool barrelRoom;

	public GameObject comboNumber;

	private MeshRenderer[] bonusRenders;

	private List<ParticleSystem> bonusParticles;

	private Animation bonusAnim;

	private int musicChanged;

	public static event EventHandler<BonusRoomCompletedEventArgs> BonusRoomCompleted;

	public static event EventHandler<EventArgs> BonusRoomVictoryAnimation;

	private void Start()
	{
		timer = Time.time + 2f;
		bonusText = GameObject.FindWithTag("Bonus");
		if (bonusText == null)
		{
			Debug.LogError("There was no object named \"Bonus\". Please make sure one is present");
		}
		else
		{
			bonusRenders = bonusText.gameObject.GetComponentsInChildren<MeshRenderer>();
			MeshRenderer[] array = bonusRenders;
			foreach (MeshRenderer meshRenderer in array)
			{
				meshRenderer.GetComponent<Renderer>().enabled = false;
			}
			bonusParticles = new List<ParticleSystem>(bonusText.GetComponentsInChildren<ParticleSystem>());
			bonusAnim = bonusText.GetComponentInChildren<Animation>();
		}
		for (int j = 0; j < objsToDestroy.Length; j++)
		{
			ObjectSpawner component = objsToDestroy[j].GetComponent<ObjectSpawner>();
			if (component != null)
			{
				numToKill += component.totalNumToSpawn;
			}
		}
		numKilled = 0;
	}

	private void OnEnable()
	{
		timer = Time.time + 2f;
		showBonusStart = 0;
		Health.Killed += HandleHealthKilled;
	}

	private void OnDisable()
	{
		Health.Killed -= HandleHealthKilled;
		if (musicChanged == 1 && !GameManager.Instance.IsGameOver && !HealingElixirScreen.IsActive)
		{
			MusicManager.Instance.StopMusic();
			MusicManager.Instance.PlayCurrentGameplayMusic();
		}
		else
		{
			SoundEventManager.Instance.Stop2D(GlobalSoundEventData.Instance.Bonus_Intro_Stinger);
			SoundEventManager.Instance.Stop2D(GlobalSoundEventData.Instance.Bonus_Outro_Stinger);
			SoundEventManager.Instance.Stop2D(GlobalSoundEventData.Instance.Bonus_Results_Neutral);
			SoundEventManager.Instance.Stop2D(GlobalSoundEventData.Instance.Bonus_Results_Perfect);
		}
		BonusTextHide();
	}

	private void HandleHealthKilled(object sender, EventArgs e)
	{
		Health health = (Health)sender;
		BounceSFX component = health.gameObject.GetComponent<BounceSFX>();
		if (IsHealthAttachedToDestructible(health) && barrelRoom && (bool)component)
		{
			numKilled++;
		}
		else if (health.gameObject.CompareTag("Sheep"))
		{
			numKilled++;
		}
	}

	public static bool IsHealthAttachedToDestructible(Health healthComponent)
	{
		return !healthComponent.isEnemy && !healthComponent.gameObject.CompareTag("Projectile");
	}

	private void BonusTextLoop()
	{
		bonusAnim.GetComponent<Animation>().Play("BonusText_Loop");
	}

	private void BonusTextHide()
	{
		bonusText.transform.localScale = new Vector3(1f, 1f, 1f);
		MeshRenderer[] array = bonusRenders;
		foreach (MeshRenderer meshRenderer in array)
		{
			if (meshRenderer != null)
			{
				meshRenderer.GetComponent<Renderer>().enabled = false;
			}
		}
	}

	private void Update()
	{
		if (GameManager.gameState != GameManager.GameState.Playing || HealingElixirScreen.IsActive || !(Time.time >= timer))
		{
			return;
		}
		if (showBonusStart == 0)
		{
			bonusAnim.GetComponent<Animation>().Play("BonusText_Intro");
			Invoke("BonusTextLoop", bonusAnim.GetComponent<Animation>().clip.length);
			if (LocalizationManager.Instance.IsEnglish)
			{
				MeshRenderer[] array = bonusRenders;
				foreach (MeshRenderer meshRenderer in array)
				{
					meshRenderer.GetComponent<Renderer>().enabled = true;
				}
			}
			MusicManager.Instance.StopMusic();
			SoundEventManager.Instance.Play2D(GlobalSoundEventData.Instance.Bonus_Intro_Stinger);
			timer = Time.time;
			showBonusStart = 1;
			musicChanged = 1;
		}
		else if (showBonusStart == 1)
		{
			MusicManager.Instance.PlayBonusMusic();
			if (explodingThings.Length > 0)
			{
				for (int j = 0; j < explodingThings.Length; j++)
				{
					BossBarrier component = explodingThings[j].GetComponent<BossBarrier>();
					if (component != null)
					{
						component.Explode();
						continue;
					}
					UnityEngine.Object.Instantiate(explodingFX, explodingThings[j].transform.position, Quaternion.Euler(0f, 0f, 0f));
					BoxCollider component2 = explodingThings[j].GetComponent<BoxCollider>();
					if (component2 == null)
					{
						explodingThings[j].gameObject.AddComponent<BoxCollider>();
						component2 = explodingThings[j].GetComponent<BoxCollider>();
					}
					if (explodingThings[j].gameObject.GetComponent<Rigidbody>() == null)
					{
						explodingThings[j].gameObject.AddComponent<Rigidbody>();
					}
					Rotator rotator = explodingThings[j].gameObject.AddComponent<Rotator>();
					rotator.randomRotationX = 1;
					rotator.randomRotationY = 1;
					explodingThings[j].GetComponent<Rigidbody>().useGravity = true;
					explodingThings[j].GetComponent<Rigidbody>().AddForce(new Vector3(UnityEngine.Random.Range(-1f, 1f) * 100f, UnityEngine.Random.value * 400f, 50f));
					UnityEngine.Object.Destroy(explodingThings[j], 2f);
				}
			}
			timer = Time.time + 9.5f;
			showBonusStart = 2;
		}
		else if (showBonusStart == 2)
		{
			SoundEventManager.Instance.Play2D(GlobalSoundEventData.Instance.Bonus_Outro_Stinger);
			timer = Time.time + 1.933f;
			showBonusStart = 3;
		}
		else if (showBonusStart == 3)
		{
			MusicManager.Instance.StopMusic();
			MusicManager.Instance.PlayCurrentGameplayMusic();
			musicChanged = 0;
			if (objsToDestroy.Length > 0)
			{
				for (int k = 0; k < objsToDestroy.Length; k++)
				{
					if (objsToDestroy[k] != null)
					{
						UnityEngine.Object.Destroy(objsToDestroy[k]);
					}
				}
			}
			Health[] componentsInChildren = LevelManager.Instance.CurrentScreenManager.GetComponentsInChildren<Health>();
			Health[] array2 = componentsInChildren;
			foreach (Health health in array2)
			{
				if ((bool)health.poofEffect)
				{
					UnityEngine.Object.Instantiate(health.poofEffect, health.transform.position, Quaternion.Euler(0f, 0f, 0f));
				}
				UnityEngine.Object.Destroy(health.gameObject);
			}
			if (numKilled > numToKill)
			{
				numKilled = numToKill;
			}
			string message = numKilled + "/" + numToKill;
			timer = Time.time + 1.5f;
			StringNotificationPanelSettings stringNotificationPanelSettings = StringNotificationPanelSettings.BuildDismissAfterTime(message, 1.5f);
			stringNotificationPanelSettings.Size = StringNotificationPanel.Size.Large;
			NotificationPanel.Instance.Display(stringNotificationPanelSettings);
			showBonusStart = 4;
		}
		else if (showBonusStart == 4)
		{
			int num = 0;
			string @string;
			if (numKilled >= numToKill)
			{
				num = 6;
				@string = LocalizationManager.Instance.GetString("BONUS_PERFECT");
				ScoreKeeper.Instance.AddScore(ScoreData.ScoreType.BONUS_LARGE, Vector3.zero, true);
			}
			else if (numKilled == numToKill - 1)
			{
				num = 5;
				@string = LocalizationManager.Instance.GetString("BONUS_SO_CLOSE");
				ScoreKeeper.Instance.AddScore(ScoreData.ScoreType.BONUS_MEDIUM, Vector3.zero, true);
			}
			else if (numKilled == numToKill - 2)
			{
				num = 4;
				@string = LocalizationManager.Instance.GetString("BONUS_ALMOST");
				ScoreKeeper.Instance.AddScore(ScoreData.ScoreType.BONUS_SMALL, Vector3.zero, true);
			}
			else if (numKilled < numToKill / 3)
			{
				num = 2;
				@string = LocalizationManager.Instance.GetString("BONUS_OKAY");
			}
			else if (numKilled == 0)
			{
				num = 0;
				@string = LocalizationManager.Instance.GetString("BONUS_POOR");
			}
			else
			{
				num = 3;
				@string = LocalizationManager.Instance.GetString("BONUS_NOT_BAD");
			}
			StringNotificationPanelSettings stringNotificationPanelSettings2 = StringNotificationPanelSettings.BuildDismissAfterTime(@string, 1.5f);
			stringNotificationPanelSettings2.DismissOnRoomTransition = false;
			NotificationPanel.Instance.Display(stringNotificationPanelSettings2);
			if (comboNumber != null && num != 0)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(comboNumber, Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 3, 3f)), Quaternion.identity) as GameObject;
				ComboCoin component3 = gameObject.GetComponent<ComboCoin>();
				component3.number = num;
				component3.isSheep = true;
			}
			if (num == 6)
			{
				SoundEventManager.Instance.Play2D(GlobalSoundEventData.Instance.Bonus_Results_Perfect);
			}
			else
			{
				SoundEventManager.Instance.Play2D(GlobalSoundEventData.Instance.Bonus_Results_Neutral);
			}
			OnBonusRoomVictoryAnimation();
			iTween.ScaleBy(bonusText, iTween.Hash("amount", new Vector3(1.2f, 1.2f, 1.2f), "time", 0.15f));
			OnBonusRoomCompleted(num);
			timer = Time.time + 1.5f;
			showBonusStart = 6;
		}
		else
		{
			BonusTextHide();
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnBonusRoomCompleted(int rewardNumber)
	{
		if (BonusManager.BonusRoomCompleted != null)
		{
			BonusManager.BonusRoomCompleted(this, new BonusRoomCompletedEventArgs(rewardNumber));
		}
	}

	private void OnBonusRoomVictoryAnimation()
	{
		if (BonusManager.BonusRoomVictoryAnimation != null)
		{
			BonusManager.BonusRoomVictoryAnimation(this, new EventArgs());
		}
	}
}
