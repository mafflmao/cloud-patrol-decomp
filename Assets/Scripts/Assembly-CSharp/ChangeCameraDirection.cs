using UnityEngine;

public class ChangeCameraDirection : MonoBehaviour
{
	private Mover cameraMover;

	private Health myHealth;

	private int startingHP;

	private GameObject mainCameraParent;

	public Vector3 cameraVectorChange;

	public Vector3 defaultCameraVector;

	private void Start()
	{
		mainCameraParent = GameObject.Find("!CameraFinal");
		if ((bool)mainCameraParent)
		{
			cameraMover = mainCameraParent.GetComponent<Mover>();
		}
		else
		{
			Debug.LogError("There's no !CameraFinal in the scene. This is a problem");
		}
		myHealth = GetComponent<Health>();
	}

	private void Update()
	{
		if ((float)myHealth.hitPoints <= 0f)
		{
			if (!cameraMover.moveNow)
			{
				cameraMover.direction = cameraVectorChange;
				cameraMover.StartMoving();
			}
			myHealth.noKill = false;
			myHealth.Kill();
		}
	}
}
