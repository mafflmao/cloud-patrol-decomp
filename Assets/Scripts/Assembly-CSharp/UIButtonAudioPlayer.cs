using UnityEngine;

public class UIButtonAudioPlayer : MonoBehaviour
{
	private AutoSpriteControlBase _btnTarget;

	public SoundEventData soundData;

	public bool useButtonInputEvent = true;

	public POINTER_INFO.INPUT_EVENT inputEvent = POINTER_INFO.INPUT_EVENT.RELEASE;

	public float delay;

	public bool playSoundAcrossSceneLoad;

	public bool onlyPlayWhenEnabled = true;

	private void Start()
	{
		_btnTarget = GetComponent<AutoSpriteControlBase>();
		if (_btnTarget == null)
		{
			Debug.LogError("No UIButton component found on this game object: " + base.gameObject.name);
		}
		else
		{
			_btnTarget.AddInputDelegate(onBtnAction);
		}
	}

	private void onBtnAction(ref POINTER_INFO ptr)
	{
		if (soundData == null)
		{
			Debug.LogError("UIButtonAudioManager: No Sound data has been provided for the button: " + _btnTarget.name);
		}
		else if ((ptr.targetObj.controlIsEnabled || !onlyPlayWhenEnabled) && ptr.evt == inputEvent)
		{
			if (playSoundAcrossSceneLoad)
			{
				SoundEventManager.Instance.PlayNoDestoryOnLoad(soundData);
			}
			else
			{
				SoundEventManager.Instance.Play2D(soundData, delay);
			}
		}
	}

	private void OnDestroy()
	{
		if (_btnTarget != null)
		{
			_btnTarget.RemoveInputDelegate(onBtnAction);
		}
	}
}
