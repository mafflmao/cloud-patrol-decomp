using UnityEngine;

public class BedrockUtils
{
	public static readonly bool debugMode;

	public static string debugString;

	public static Bedrock.brKeyValueArray Hash(params string[] args)
	{
		if (args.Length % 2 != 0)
		{
			Debug.LogError("Hash needs an even number of arguments");
			return default(Bedrock.brKeyValueArray);
		}
		Bedrock.brKeyValueArray result = default(Bedrock.brKeyValueArray);
		result.size = args.Length / 2;
		result.pairs = new Bedrock.brKeyValuePair[result.size];
		if (debugMode)
		{
			debugString = string.Empty;
		}
		for (int i = 0; i < args.Length - 1; i += 2)
		{
			result.pairs[i / 2].key = args[i];
			result.pairs[i / 2].val = args[i + 1];
			if (debugMode)
			{
				string text = debugString;
				debugString = text + "|" + args[i] + ":" + args[i + 1];
			}
		}
		if (debugMode)
		{
			Debug.Log(debugString);
		}
		return result;
	}
}
