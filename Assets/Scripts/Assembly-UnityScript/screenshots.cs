using System;
using UnityEngine;

[Serializable]
public class screenshots : MonoBehaviour
{
	public int screenshotNumber;

	public string screenshotName;

	private string screenshotNameCombined;

	public float timeScale;

	public screenshots()
	{
		screenshotName = "MMTriggerHappy_Screenshot_";
		screenshotNameCombined = string.Empty;
		timeScale = 1f;
	}

	public virtual void Update()
	{
		screenshotNameCombined = "mmScreenShots/" + screenshotName + screenshotNumber + ".png";
		ScreenCapture.CaptureScreenshot(screenshotNameCombined);
		screenshotNumber++;
	}

	public virtual void Main()
	{
	}
}
