using UnityEngine;

[AddComponentMenu("AVPro Windows Media/Material Apply")]
public class AVProWindowsMediaMaterialApply : MonoBehaviour
{
	public Material _material;

	public AVProWindowsMediaMovie _movie;

	public string _textureName;

	private void Start()
	{
		if (_movie != null && _movie.OutputTexture != null)
		{
			ApplyMapping(_movie.OutputTexture);
		}
	}

	private void Update()
	{
		if (_movie != null && _movie.OutputTexture != null)
		{
			ApplyMapping(_movie.OutputTexture);
		}
	}

	private void ApplyMapping(Texture texture)
	{
		if (_material != null)
		{
			if (string.IsNullOrEmpty(_textureName))
			{
				_material.mainTexture = texture;
			}
			else
			{
				_material.SetTexture(_textureName, texture);
			}
		}
	}

	public void OnDisable()
	{
		ApplyMapping(null);
	}
}
