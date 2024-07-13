using UnityEngine;

public static class ColorUtils
{
	public static Color ColorFromInt(int r, int g, int b)
	{
		return ColorFromInt(r, g, b, 255);
	}

	public static Color ColorFromInt(int r, int g, int b, int a)
	{
		return new Color((float)r / 255f, (float)g / 255f, (float)b / 255f, (float)a / 255f);
	}
}
