using System.Collections;
using UnityEngine;

public class StateRoot : MonoBehaviour
{
	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(StateRoot), LogLevel.Log);

	public string stateName = "undefined";

	public string stateTitle = string.Empty;

	public bool setHeaderTextToTitle = true;

	public bool usesHeader = true;

	public bool usesFooter;

	public bool canGoBack = true;

	public string backStateName = string.Empty;

	public bool storeStateInBackHistory = true;

	private HeaderUI _header;

	private FooterUI _footer;

	private void Awake()
	{
		SceneQueue.Instance.SceneLoaded();
		StateManager.Instance.StateLoaded(this);
		base.gameObject.name = "StateRoot-" + stateName;
	}

	private void OnStateActivate()
	{
		_header = HeaderUI.Instance;
		_footer = FooterUI.Instance;
		StartCoroutine(DelayedHeaderFooterSetup());
	}

	public void SetTitle(string title)
	{
		_header.titleString = title;
	}

	private IEnumerator DelayedHeaderFooterSetup()
	{
		yield return new WaitForEndOfFrame();
		_log.LogDebug("Waiting for load...");
		while (StateManager.Instance.Loading)
		{
			yield return new WaitForEndOfFrame();
		}
		_log.LogDebug("Done waiting... Showing header.");
		_header.visible = usesHeader;
		if (setHeaderTextToTitle)
		{
			_header.titleString = LocalizationManager.Instance.GetString(stateTitle).ToUpper();
		}
		_header.ShowBackButton(canGoBack);
		_footer.visible = usesFooter;
		_footer.OnStateChanged();
	}
}
