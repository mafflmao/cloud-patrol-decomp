using System;
using System.Collections;
using UnityEngine;

public class SkyIronShield : Powerup
{
	public GameObject shieldPrefab;

	public GameObject displayShieldPrefab;

	public SoundEventData deflectSFX;

	public SoundEventData warningSFX;

	public float introTime = 0.2f;

	public float warningTime = 2f;

	public float outroTime = 0.2f;

	public float pulseTime = 0.25f;

	private GameObject _displayShield;

	private int _currentlyExplodingBombs;

	public static SkyIronShield ActiveShield { get; private set; }

	protected override void OnEnable()
	{
		base.OnEnable();
		GameManager.GameOver += HandleGameManagerGameOver;
		BombController.BombControllerStarted += HandleBombControllerStarted;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		GameManager.GameOver -= HandleGameManagerGameOver;
		BombController.BombControllerStarted -= HandleBombControllerStarted;
		SoundEventManager.Instance.Stop(warningSFX, base.gameObject);
	}

	protected override void Update()
	{
		if (_currentlyExplodingBombs == 0)
		{
			base.Update();
		}
		else
		{
			_timeLastFrame = Time.realtimeSinceStartup;
		}
	}

	private void HandleBombControllerStarted(object sender, EventArgs e)
	{
		_currentlyExplodingBombs++;
	}

	private IEnumerator DoShieldCoroutine()
	{
		_displayShield = (GameObject)UnityEngine.Object.Instantiate(displayShieldPrefab, base.transform.position, Quaternion.identity);
		_displayShield.transform.parent = base.Holder.transform.parent;
		iTween.MoveTo(_displayShield, iTween.Hash("position", base.Holder.transform.localPosition + new Vector3(0f, 0.25f, 0f), "time", introTime, "isLocal", true));
		Debug.Log("SkyIronShield waiting for intro...");
		while (lifeTimeInSeconds - base.TimeLeft < introTime)
		{
			yield return new WaitForEndOfFrame();
		}
		iTween.RotateTo(_displayShield, iTween.Hash("y", 190f, "looptype", "pingPong", "easetype", "linear", "time", 1f, "isLocal", true));
		Debug.Log("SkyIronShield Waiting for warning period...");
		while (base.TimeLeft > warningTime)
		{
			yield return new WaitForEndOfFrame();
		}
		Debug.Log("SkyIronShield is running out!");
		SoundEventManager.Instance.Play(warningSFX, base.gameObject);
		iTween.ScaleBy(_displayShield, iTween.Hash("amount", new Vector3(1.5f, 1.5f, 1.5f), "looptype", "pingPong", "time", pulseTime, "isLocal", true));
		iTween.ColorTo(_displayShield, iTween.Hash("color", Color.red, "time", pulseTime, "looptype", "pingPong"));
		Debug.Log("SkyIronShield waiting for outtro...");
		while (base.TimeLeft > outroTime)
		{
			yield return new WaitForEndOfFrame();
		}
		iTween.ColorTo(_displayShield, iTween.Hash("color", Color.red, "time", outroTime));
		iTween.ScaleBy(_displayShield, iTween.Hash("amount", new Vector3(0f, 0f, 0f), "time", outroTime));
		Debug.Log("SkyIronShield waiting for timeout...");
		while (base.TimeLeft > 0f)
		{
			yield return new WaitForEndOfFrame();
		}
		DestroyAndFinish(true);
	}

	private void HandleGameManagerGameOver(object sender, EventArgs e)
	{
		DestroyAndFinish(true);
	}

	protected override void HandleTriggered()
	{
		base.HandleTriggered();
		ActiveShield = this;
		GameManager.invincible = true;
		ShipManager.instance.ResetTargetting();
		StartCoroutine(DoShieldCoroutine());
	}

	public void SpawnShield(Vector3 location, bool isBomb)
	{
		if (isBomb)
		{
			_currentlyExplodingBombs--;
		}
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(shieldPrefab, location, Quaternion.identity);
		gameObject.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
		SoundEventManager.Instance.Play(deflectSFX, gameObject);
	}

	public override void DestroyAndFinish(bool waitForCutscene)
	{
		ActiveShield = null;
		GameManager.invincible = false;
		if (_displayShield != null)
		{
			UnityEngine.Object.Destroy(_displayShield);
		}
		base.DestroyAndFinish(waitForCutscene);
	}
}
