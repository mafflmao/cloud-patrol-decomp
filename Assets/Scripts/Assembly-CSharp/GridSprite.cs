using UnityEngine;

[RequireComponent(typeof(SimpleSprite))]
public class GridSprite : MonoBehaviour
{
	public int row;

	public int col;

	public float uvWidth;

	public float uvHeight;

	private SimpleSprite sprite;

	private void Awake()
	{
		sprite = GetComponent<SimpleSprite>();
		sprite.SetUVs(new Rect((float)col * uvWidth, (float)row * uvHeight, uvWidth, uvHeight));
	}

	public void UpdatePosition(int r, int c)
	{
		if (r != row || col == c)
		{
		}
		sprite.SetUVs(new Rect((float)c * uvWidth, (float)r * uvHeight, uvWidth, uvHeight));
	}
}
