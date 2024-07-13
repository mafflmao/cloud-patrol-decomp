using UnityEngine;

public class StartupHackLocalizedSpriteText : SpriteText
{
	public override void Start()
	{
		base.Start();
		if (Application.isPlaying)
		{
			Text = LocalizationManager.Instance.GetString(Text);
		}
	}
}
