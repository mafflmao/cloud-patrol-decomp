using UnityEngine;

public class LocalizedSpriteText : SpriteText
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
