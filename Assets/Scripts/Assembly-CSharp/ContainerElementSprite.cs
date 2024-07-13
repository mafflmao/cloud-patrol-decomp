using UnityEngine;

[RequireComponent(typeof(SimpleSprite))]
public class ContainerElementSprite : MonoBehaviour
{
	public Material matNone;

	public Material matSome;

	public Material matAll;

	public SimpleSprite sprite;

	private void Awake()
	{
		sprite.SetUVs(new Rect(0f, 0f, 1f, 1f));
	}

	public void UpdateIcon(int index)
	{
		Material material = matNone;
		switch (index)
		{
		case 1:
			material = matSome;
			break;
		case 2:
			material = matAll;
			break;
		}
		sprite.gameObject.GetComponent<Renderer>().material = material;
		sprite.width = material.mainTexture.width * 2;
		sprite.height = material.mainTexture.height * 2;
		sprite.pixelDimensions = new Vector2(material.mainTexture.width, material.mainTexture.height);
		sprite.lowerLeftPixel = new Vector2(0f, material.mainTexture.height);
	}
}
