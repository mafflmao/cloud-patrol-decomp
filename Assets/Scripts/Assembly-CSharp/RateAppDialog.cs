using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RateAppDialog : MonoBehaviour
{
	private class Settings
	{
		private const bool DefaultIsEnabled = true;

		private const int DefaultFirstLevelToShow = 4;

		private const int DefaultSecondLevelToShow = 10;

		private const int DefaultThirdLevelToShow = 15;

		private const string DefaultReviewUrl = "none";

		public bool IsEnabled { get; private set; }

		public int FirstLevelToShow { get; private set; }

		public int SecondLevelToShow { get; private set; }

		public int ThirdLevelToShow { get; private set; }

		public string ReviewUrl { get; private set; }

		public Settings()
		{
			IsEnabled = true;
			FirstLevelToShow = 4;
			SecondLevelToShow = 10;
			ThirdLevelToShow = 15;
			ReviewUrl = "none";
		}

		public void LoadDataFromSwrve()
		{
			Dictionary<string, string> resourceDictionary;
			if (Bedrock.GetRemoteUserResources("UX Rate Reminder", out resourceDictionary))
			{
				IsEnabled = Bedrock.GetFromResourceDictionaryAsBool(resourceDictionary, "EnableRatePopup", IsEnabled);
				FirstLevelToShow = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "FirstRatePopupRank", FirstLevelToShow);
				SecondLevelToShow = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "SecondRatePopupRank", SecondLevelToShow);
				ThirdLevelToShow = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "ThirdRatePopupRank", ThirdLevelToShow);
				ReviewUrl = Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "AppId", "none");
			}
		}
	}

	public static bool ShowDialogOverride;

	private static Settings _settings;

	public GameObject destroyParticle;

	public GameObject visibleStuff;

	public SpriteText laterButtonText;

	public SoundEventData OpeningSound;

	public SoundEventData DismissedSound;

	public SoundEventData OnYesPressedSound;

	public SoundEventData OnNoPressedSound;

	public SoundEventData OnLaterPressedSound;

	private bool _userHasChosen;

	private void OnEnable()
	{
		StateManager.StateDeactivated += HandleStateDeactivated;
	}

	private void OnDisable()
	{
		StateManager.StateDeactivated -= HandleStateDeactivated;
	}

	private void HandleStateDeactivated(object sender, StateEventArgs e)
	{
		Object.Destroy(base.gameObject);
	}

	public IEnumerator Display()
	{
		Debug.Log("Display App Rate Dialog");
		int currentRank = RankDataManager.Instance.CurrentRank.Rank.RankNumber;
		laterButtonText.Text = LocalizationManager.Instance.GetString((currentRank < _settings.ThirdLevelToShow) ? "RATE_APP_DIALOG_LATER" : "RATE_APP_DIALOG_NO");
		iTween.ScaleFrom(visibleStuff.gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.333f, "ignoretimescale", true));
		SoundEventManager.Instance.Play2D(OpeningSound);
		while (!_userHasChosen)
		{
			yield return new WaitForEndOfFrame();
		}
		Debug.Log("Done Waiting...");
		UIManager.instance.blockInput = true;
		yield return new WaitForSeconds(0.25f);
		if (DismissedSound != null)
		{
			SoundEventManager.Instance.Play2D(DismissedSound);
		}
		iTween.ScaleTo(visibleStuff.gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.333f, "ignoretimescale", true, "oncomplete", "Close", "oncompletetarget", base.gameObject));
	}

	private void Close()
	{
		UIManager.instance.blockInput = false;
		Object.Destroy(base.gameObject);
	}

	private void okButtonPressed()
	{
		Debug.Log("Ok Pressed");
		_userHasChosen = true;
		if (OnYesPressedSound != null)
		{
			SoundEventManager.Instance.Play2D(OnYesPressedSound);
		}
	}

	private void laterButtonPressed()
	{
		Debug.Log("Later Pressed");
		_userHasChosen = true;
		if (RankDataManager.Instance.CurrentRank.Rank.RankNumber >= _settings.ThirdLevelToShow)
		{
			Debug.Log("User is past last level of prompt! Don't ask them again.");
			if (OnNoPressedSound != null)
			{
				SoundEventManager.Instance.Play2D(OnNoPressedSound);
			}
		}
		else if (OnLaterPressedSound != null)
		{
			SoundEventManager.Instance.Play2D(OnLaterPressedSound);
		}
	}

	public void Update()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
		}
	}

	public static bool RatingConditionsHaveBeenMet()
	{
		if (_settings == null)
		{
			_settings = new Settings();
			_settings.LoadDataFromSwrve();
		}
		if (ShowDialogOverride)
		{
			Debug.Log("Show rate dialog because of debug override...");
			return true;
		}
		if (!_settings.IsEnabled)
		{
			Debug.Log("Should not show dialog, swrve says it should be disabled.");
			return false;
		}
		int rankNumber = RankDataManager.Instance.CurrentRank.Rank.RankNumber;
		if (rankNumber < _settings.FirstLevelToShow)
		{
			Debug.Log("Should not show dialog, not at the first prompt level (" + _settings.FirstLevelToShow + ") yet.");
			return false;
		}
		return true;
	}
}
