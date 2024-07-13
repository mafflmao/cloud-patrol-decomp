using System;
using System.Collections;
using UnityEngine;

public class SharedContentManager : SingletonMonoBehaviour
{
	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(SharedContentManager), LogLevel.Debug);

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void OnEnable()
	{
		GameManager.GameStarted += HandleGameManagerGameStarted;
	}

	private void OnDisable()
	{
		GameManager.GameStarted -= HandleGameManagerGameStarted;
	}

	private void HandleGameManagerGameStarted(object sender, EventArgs e)
	{
		_log.LogDebug("HandleGameManagerGameStarted()");
	}

	private IEnumerator UpdateSharedContentCoroutine(uint serverTime)
	{
		_log.LogDebug("UpdateSharedContentCoroutine({0})", serverTime);
		if (!ServerVariables.TrackToyUsage)
		{
			_log.LogDebug("Toy tracking disabled. Aborting.");
			yield break;
		}
		int toyId = StartGameSettings.Instance.activeSkylander.ToyId;
		int toySubType = StartGameSettings.Instance.activeSkylander.SubType;
		_log.LogDebug("Active skylander Toy ID:{0}, Sub-Type:{1}", toyId, toySubType);
		short taskHandle = Bedrock.UpdateSharedContentUsageForUser(Bedrock.brBedrockApplications.BR_APPLICATION_CLOUDPATROL, toyId, toySubType, serverTime);
		if (taskHandle == -1)
		{
			_log.LogWarning("Skipped notification - task handle was not valid.");
			yield break;
		}
		using (BedrockTask task = new BedrockTask(taskHandle))
		{
			_log.LogDebug("Starting to wait for task...");
			yield return StartCoroutine(task.WaitForTaskToCompleteCoroutine());
			_log.LogDebug("Task complete: " + task);
		}
	}
}
