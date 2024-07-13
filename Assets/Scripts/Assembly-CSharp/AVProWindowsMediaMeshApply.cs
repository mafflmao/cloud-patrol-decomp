using UnityEngine;

[AddComponentMenu("AVPro Windows Media/Mesh Apply")]
public class AVProWindowsMediaMeshApply : MonoBehaviour
{
	public MeshRenderer _mesh;

	public AVProWindowsMediaMovie _movie;

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
		if (_mesh != null)
		{
			Material[] materials = _mesh.materials;
			foreach (Material material in materials)
			{
				material.mainTexture = texture;
			}
		}
	}

	public void OnDisable()
	{
		ApplyMapping(null);
	}
}
