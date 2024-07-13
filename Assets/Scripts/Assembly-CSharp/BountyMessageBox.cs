using UnityEngine;

[RequireComponent(typeof(MessageBox))]
public class BountyMessageBox : MonoBehaviour
{
	public Vector3 showPosition;

	public Vector3 hidePosition;

	private MessageBox _messageBox;

	public UIButton btnCancel;

	public UIButton btnConfirm;

	public int replaceCost;

	public MessageBox messageBox
	{
		get
		{
			return _messageBox;
		}
	}

	private void Awake()
	{
		_messageBox = GetComponent<MessageBox>();
	}

	private void Start()
	{
		hidePosition = base.transform.position;
	}

	public void Hide()
	{
		messageBox.Hide();
	}

	public void Show()
	{
		messageBox.Show();
		UIManager.AddAction<MessageBox>(btnCancel, _messageBox.gameObject, "Hide", POINTER_INFO.INPUT_EVENT.RELEASE);
		btnConfirm.spriteText.Text = string.Format("${0}", replaceCost);
		base.transform.position = showPosition;
	}
}
