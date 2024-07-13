using System.Collections;
using UnityEngine;

public class OutOfFuelMessage : MonoBehaviour
{
	public float fadeInTime = 1f;

	public float fadeOutTime = 1f;

	public float lingerTime = 3f;

	public SpriteText finalScoreLabel;

	public SpriteText gameOverLabel;

	public SpriteText scoreKeeperLabel;

	public SpriteText highScore;

	public SpriteText highScoreLabel;

	public GameObject healingElixirMenu;

	public Transform healingElixirScreenPosition;

	private void Start()
	{
		HealingElixir.WasUsed = false;
		StartCoroutine(EndOfGameSequence());
	}

	public IEnumerator EndOfGameSequence()
	{
		highScore.Text = GameManager.highScore.ToString("n0");
		scoreKeeperLabel.Text = GameManager.currentScore.ToString("n0");
		iTween.FadeFrom(finalScoreLabel.gameObject, iTween.Hash("alpha", 0f, "time", fadeInTime));
		iTween.FadeFrom(gameOverLabel.gameObject, iTween.Hash("alpha", 0f, "time", fadeInTime));
		iTween.FadeFrom(scoreKeeperLabel.gameObject, iTween.Hash("alpha", 0f, "time", fadeInTime));
		iTween.FadeFrom(highScore.gameObject, iTween.Hash("alpha", 0f, "time", fadeInTime));
		iTween.FadeFrom(highScoreLabel.gameObject, iTween.Hash("alpha", 0f, "time", fadeInTime));
		if (!ActivateWatcher.Instance.isForcingReboot)
		{
			yield return new WaitForSeconds(lingerTime);
		}
		if (GameManager.Instance.ContinueCount < GameManager.Instance.m_MaxContinue)
		{
			Debug.Log("Healing Elixir screen is go");
			gameOverLabel.Hide(true);
			GameObject menu = Object.Instantiate(healingElixirMenu) as GameObject;
			menu.transform.parent = base.transform;
			menu.transform.localPosition = healingElixirScreenPosition.localPosition;
		}
		while (HealingElixirScreen.IsActive)
		{
			yield return new WaitForEndOfFrame();
		}
		iTween.FadeTo(finalScoreLabel.gameObject, iTween.Hash("alpha", 0f, "time", fadeOutTime));
		iTween.FadeTo(scoreKeeperLabel.gameObject, iTween.Hash("alpha", 0f, "time", fadeOutTime));
		iTween.FadeTo(highScore.gameObject, iTween.Hash("alpha", 0f, "time", fadeOutTime));
		iTween.FadeTo(highScoreLabel.gameObject, iTween.Hash("alpha", 0f, "time", fadeOutTime));
		yield return new WaitForSeconds(1f);
		if (!HealingElixir.WasUsed)
		{
			yield return StartCoroutine(EndGame());
		}
	}

	private IEnumerator EndGame()
	{
		GameManager.gameState = GameManager.GameState.Dead;
		ScreenTimeoutUtility.Instance.AllowTimeout = true;
		iTween.FadeTo(finalScoreLabel.gameObject, iTween.Hash("alpha", 0f, "time", fadeOutTime));
		iTween.FadeTo(gameOverLabel.gameObject, iTween.Hash("alpha", 0f, "time", fadeOutTime));
		iTween.FadeTo(scoreKeeperLabel.gameObject, iTween.Hash("alpha", 0f, "time", fadeOutTime));
		iTween.FadeTo(highScore.gameObject, iTween.Hash("alpha", 0f, "time", fadeOutTime));
		iTween.FadeTo(highScoreLabel.gameObject, iTween.Hash("alpha", 0f, "time", fadeOutTime));
		if (!ActivateWatcher.Instance.isForcingReboot)
		{
			yield return new WaitForSeconds(fadeOutTime);
		}
		bool showShip = GameManager.currentHealth <= 0f;
		Object.Destroy(base.gameObject);
		TransitionController.Instance.GameOverTransition(showShip);
		GameManager.Instance.FinishGame(false);
		GameManager.Instance.ForceGameOverEvent();
		StateManager.Instance.LoadAndActivateState("Results");
	}

	public void HideScore()
	{
		finalScoreLabel.Hide(true);
		scoreKeeperLabel.Hide(true);
		highScore.Hide(true);
		highScoreLabel.Hide(true);
	}

	public void OnDestroy()
	{
		StopAllCoroutines();
	}
}
