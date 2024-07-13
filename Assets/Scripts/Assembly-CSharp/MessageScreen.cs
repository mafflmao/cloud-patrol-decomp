public class MessageScreen : ScreenSequenceScreen
{
	public const float AnimateTime = 0.5f;

	public SpriteText messageText;

	protected override void AnimateIn()
	{
		ScreenSequenceScreen.MoveFrom(base.gameObject, base.transform.position - ScreenSequenceScreen.ScreenHeightVector, 0.5f, 0f);
		StartTimeout(3.5f);
	}

	protected override void AnimateOut()
	{
		ScreenSequenceScreen.MoveTo(base.gameObject, base.transform.position + ScreenSequenceScreen.ScreenHeightVector, 0.5f, 0f);
		Suicide(0.5f);
	}

	public void SetText(string text)
	{
		messageText.Text = text;
	}
}
