using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AnimationStates))]
public abstract class TrollBase : SafeMonoBehaviour
{
	protected AnimationStates _animationStates;

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(TrollBase), LogLevel.Debug);

	protected virtual void Awake()
	{
		_animationStates = GetComponent<AnimationStates>();
	}

	protected virtual void OnEnable()
	{
		_log.LogDebug("Enabled");
		Activator.RoomActivated += HandleActivatorRoomActivated;
	}

	protected virtual void OnDisable()
	{
		_log.LogDebug("Disabled");
		Activator.RoomActivated -= HandleActivatorRoomActivated;
		CancelInvoke();
		StopAllCoroutines();
	}

	private void HandleActivatorRoomActivated(object sender, ActivatorEventArgs e)
	{
		_log.LogDebug("HandleActivatorRoomActivated");
		if (base.transform.IsChildOf(e.LevelRoot.transform))
		{
			StartTrollBehaviour();
		}
	}

	public abstract void StartTrollBehaviour();

	public virtual void Disable()
	{
		StopAllCoroutines();
		CancelInvoke();
	}

	public void VictoryDance()
	{
		StopAllCoroutines();
		CancelInvoke();
		StartCoroutine(VictoryDanceCoroutine());
	}

	protected abstract IEnumerator VictoryDanceCoroutine();
}
