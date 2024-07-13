using System;
using System.Collections;
using UnityEngine;

public class BedrockTask : IDisposable
{
	public const float TimeoutTime = 10f;

	public const float FailsafeTimeoutTime = 180f;

	public short Handle { get; private set; }

	public Bedrock.brTaskStatus Status { get; set; }

	public virtual int ErrorCode
	{
		get
		{
			return Bedrock.getTaskErrorCode(Handle);
		}
	}

	public bool IsComplete
	{
		get
		{
			return Status != Bedrock.brTaskStatus.BR_TASK_PENDING && Status != Bedrock.brTaskStatus.BR_TASK_INIT;
		}
	}

	public object Tag { get; set; }

	public BedrockTask(short taskHandle)
	{
		Handle = taskHandle;
	}

	public IEnumerator WaitForTaskToCompleteCoroutine()
	{
		return WaitForTaskToCompleteCoroutine_Internal(false);
	}

	public IEnumerator WaitForTaskToCompleteOrTimeoutCoroutine()
	{
		return WaitForTaskToCompleteCoroutine_Internal(true);
	}

	private IEnumerator WaitForTaskToCompleteCoroutine_Internal(bool useShortTimeout)
	{
		float timeoutTime = Time.realtimeSinceStartup + ((!useShortTimeout) ? 180f : 10f);
		UpdateStatus();
		while (Time.realtimeSinceStartup < timeoutTime && !IsComplete)
		{
			yield return new WaitForEndOfFrame();
			UpdateStatus();
		}
	}

	public static IEnumerator WaitForAllTasksToCompleteOrTimeoutCoroutine(BedrockTask[] tasks)
	{
		float timeoutTime = Time.realtimeSinceStartup + 10f;
		while (Time.realtimeSinceStartup < timeoutTime)
		{
			bool allTasksComplete = true;
			foreach (BedrockTask task in tasks)
			{
				if (task != null)
				{
					task.UpdateStatus();
					if (!task.IsComplete)
					{
						allTasksComplete = false;
					}
				}
			}
			if (allTasksComplete)
			{
				break;
			}
			yield return new WaitForEndOfFrame();
		}
	}

	public virtual void UpdateStatus()
	{
	}

	public override string ToString()
	{
		return string.Format("[BedrockTask: Handle={0}, Status={1}, ErrorCode={2}, IsComplete={3}, Tag={4}]", Handle, Status, ErrorCode, IsComplete, Tag);
	}

	public virtual void Dispose()
	{
		short taskHandle = Handle;
		Bedrock.ReleaseTaskHandle(ref taskHandle);
	}
}
