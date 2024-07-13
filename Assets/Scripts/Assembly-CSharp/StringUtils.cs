using UnityEngine;

public static class StringUtils
{
	public static string TrimToLength(this string candidateString, int maxCharacters)
	{
		if (string.IsNullOrEmpty(candidateString) || candidateString.Length <= maxCharacters)
		{
			return candidateString;
		}
		int length = Mathf.Clamp(maxCharacters - "...".Length, 0, maxCharacters);
		return candidateString.Substring(0, length) + "...";
	}
}
