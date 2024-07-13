using UnityEngine;

public class PlatformUtils
{
	private static bool? _isLowQualityPlatformCache;

	public static bool IsLowQualityPlatform
	{
		get
		{
			if (!_isLowQualityPlatformCache.HasValue)
			{
				if (Application.isEditor)
				{
					_isLowQualityPlatformCache = false;
				}
				_isLowQualityPlatformCache = false;
			}
			return _isLowQualityPlatformCache.Value;
		}
		set
		{
			_isLowQualityPlatformCache = value;
		}
	}
}
