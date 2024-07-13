using UnityEngine;

public class ScreenResolution : MonoBehaviour
{
	public const string resolution480_320 = "480X320";

	public const string resolution800_480 = "800X480";

	public const string resolution854_480 = "854X480";

	public const string resolution960_640 = "960X640";

	public const string resolution800_400 = "800X400";

	public const string resolution1024_768 = "1024X768";

	public const string resolution320_480 = "320X480";

	public const string resolution480_800 = "480X800";

	public const string resolution480_854 = "480X854";

	public const string resolution640_960 = "640X960";

	public const string resolution400_800 = "400X800";

	public const string resolution768_1024 = "768X1024";

	public static Rect viewPowerRect;

	public static float cameraSize;

	public static Rect GetViewPortRect(string resolution)
	{
		switch (resolution)
		{
		case "320X480":
			viewPowerRect = new Rect(0f, 0f, 2f, 3f);
			break;
		case "480X800":
			viewPowerRect = new Rect(0f, 0f, 2f, 3f);
			break;
		case "480X854":
			viewPowerRect = new Rect(0f, 0f, 2f, 3f);
			break;
		case "960X640":
			viewPowerRect = new Rect(0f, 0f, 2f, 3f);
			break;
		case "400X800":
			viewPowerRect = new Rect(0f, 0f, 2f, 3f);
			break;
		case "1024X768":
			viewPowerRect = new Rect(0f, 0f, 2f, 3f);
			break;
		}
		return viewPowerRect;
	}

	public static float GetCameraSize(string resolution)
	{
		cameraSize = 0f;
		switch (resolution)
		{
		case "480X320":
			cameraSize = 160f;
			break;
		case "800X480":
			cameraSize = 240f;
			break;
		case "854X480":
			cameraSize = 240f;
			break;
		case "960X640":
			cameraSize = 319f;
			break;
		case "800X400":
			cameraSize = 383f;
			break;
		case "1024X768":
			cameraSize = 383f;
			break;
		case "320X480":
			cameraSize = 240f;
			break;
		case "480X800":
			cameraSize = 240f;
			break;
		case "480X854":
			cameraSize = 240f;
			break;
		case "640X960":
			cameraSize = 480f;
			break;
		case "400X800":
			cameraSize = 383f;
			break;
		case "768X1024":
			cameraSize = 511f;
			break;
		}
		if (cameraSize == 0f)
		{
			cameraSize = 383f;
		}
		return cameraSize;
	}
}
