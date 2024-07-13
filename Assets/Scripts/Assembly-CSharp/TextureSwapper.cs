using System.Collections.Generic;
using UnityEngine;

public class TextureSwapper : SingletonMonoBehaviour
{
	private enum Quality
	{
		Medium = 0,
		High = 1
	}

	private const string HighQualityPathFormatString = "DynamicTextures/{0}_HQ";

	private const string MediumQualityPathFormatString = "DynamicTextures/{0}_Med";

	private const int HighQualityScreenWidth = 1025;

	private const int MedQualityScreenWidth = 960;

	public Material[] materials;

	private Dictionary<Material, Texture> _textureAssociations;

	protected override void AwakeOnce()
	{
		base.AwakeOnce();
		_textureAssociations = new Dictionary<Material, Texture>();
		if (Screen.width < 960)
		{
			return;
		}
		Quality quality = Quality.Medium;
		if (Screen.width >= 1025)
		{
			quality = Quality.High;
		}
		Material[] array = materials;
		foreach (Material material in array)
		{
			if (material == null || _textureAssociations.ContainsKey(material) || !(material != null))
			{
				continue;
			}
			string empty = string.Empty;
			if (material.mainTexture != null)
			{
				empty = material.mainTexture.name;
			}
			Texture texture = null;
			if (quality == Quality.High)
			{
				texture = TryLoadTexture(empty, Quality.High);
				if (!(texture == null))
				{
				}
			}
			if (texture == null)
			{
				texture = TryLoadTexture(empty, Quality.Medium);
			}
			if (texture != null)
			{
				_textureAssociations[material] = material.mainTexture;
				material.mainTexture = texture;
			}
			else if (quality != Quality.High)
			{
			}
		}
	}

	private Texture TryLoadTexture(string rootTextureName, Quality quality)
	{
		string format = ((quality != 0) ? "DynamicTextures/{0}_HQ" : "DynamicTextures/{0}_Med");
		string path = string.Format(format, rootTextureName);
		return (Texture)Resources.Load(path);
	}
}
