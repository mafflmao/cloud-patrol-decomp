using UnityEngine;

public class PauseButton : MonoBehaviour
{
	private readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(PauseButton), LogLevel.Log);

	private GameObject _buttonVisual;

	private UIButton _button;

	public bool IsDragTargetInBounds
	{
		get
		{
			BoxCollider componentInChildren = GetComponentInChildren<BoxCollider>();
			if (componentInChildren == null)
			{
				return false;
			}
			GUICamera guiCamera = GUISystem.Instance.guiCamera;
			Vector3 position = ShipManager.instance.dragMultiTarget[0].transform.position;
			Vector3 vector = guiCamera.GetComponent<Camera>().WorldToScreenPoint(componentInChildren.bounds.min);
			Vector3 vector2 = guiCamera.GetComponent<Camera>().WorldToScreenPoint(componentInChildren.bounds.max);
			Vector3 vector3 = Camera.main.WorldToScreenPoint(position);
			if (vector3.x >= vector.x && vector3.x <= vector2.x && vector3.y >= vector.y && vector3.y <= vector2.y)
			{
				return true;
			}
			return false;
		}
	}

	private void Start()
	{
		_button = GetComponentInChildren<UIButton>();
		_buttonVisual = _button.gameObject;
	}

	private void OnEnable()
	{
		GameManager.GameStateChanged += HandleGameManagerGameStateChanged;
	}

	private void OnDisable()
	{
		GameManager.GameStateChanged -= HandleGameManagerGameStateChanged;
	}

	private void HandleGameManagerGameStateChanged(object sender, GameManager.GameStateChangedEventArgs e)
	{
		EnablePause(e.NewState == GameManager.GameState.Playing);
	}

	public void EnablePause(bool on)
	{
		_log.LogDebug("EnablePause({0})", on);
		if ((bool)_button)
		{
			_button.controlIsEnabled = on;
			_buttonVisual.GetComponent<Renderer>().enabled = on;
		}
	}

	public void TogglePause()
	{
		if (_button.controlIsEnabled && !ShipManager.instance.hasTargets && !GameManager.Instance.IsPaused && GameManager.gameStarted && GameManager.gameState == GameManager.GameState.Playing)
		{
			GameManager.Instance.PushPause(PauseReason.System);
			SwrveEventsGameplay.GamePaused();
		}
	}
}
