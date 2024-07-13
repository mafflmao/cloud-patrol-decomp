using UnityEngine;

public class TestSalePrefab : MonoBehaviour
{
	public void OnClicked()
	{
		Object.Destroy(base.gameObject);
	}
}
