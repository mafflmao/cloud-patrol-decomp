using System;
using System.Collections;
using UnityEngine;

public class ShipIntroVisual : MonoBehaviour
{
	public GameObject staticSkylanderParent;

	public SoundEventData victoryJumpSFX;

	public SoundEventData victoryLandSFX;

	public SoundEventData landInTurretSFX;

	public MeshRenderer theBigShipMeshRenderer;

	public MeshRenderer theBigShipTurretMeshRenderer;

	public GameObject turretGeometry;

	public GameObject shipBowGeometry;

	public SoundEventData trollsEscapedVo;

	private GameObject staticSkylander;

	public GameObject elementOfTheDaySequence;

	public GameObject skylanderUpgradeSplashScreen;

	private GameObject _spawnFxPrefab;

	private SoundEventData _skylanderSpawnSfx;

	public GameObject _turretDestructionFX;

	public GameObject introCameraGO;

	private bool isGiant
	{
		get
		{
			return StartGameSettings.Instance.activeSkylander.isGiant;
		}
	}

	public static event EventHandler<EventArgs> SkylanderSpawnComplete;

	private void Start()
	{
		ElementData elementData = StartGameSettings.Instance.activeSkylander.elementData;
		_spawnFxPrefab = elementData.LoadTurretSpawnFxPrefab();
		_skylanderSpawnSfx = elementData.LoadTurretSpawnSfx();
	}

	public IEnumerator SpawnInSequence()
	{
		if (introCameraGO == null)
		{
			introCameraGO = Camera.main.gameObject;
		}
		theBigShipMeshRenderer.enabled = true;
		theBigShipTurretMeshRenderer.enabled = true;
		turretGeometry.GetComponent<Renderer>().enabled = false;
		shipBowGeometry.GetComponent<Renderer>().enabled = false;
		GameObject prefab = StartGameSettings.Instance.activeSkylander.GetStaticModelPrefab();
		staticSkylander = UnityEngine.Object.Instantiate(prefab) as GameObject;
		GameObjectUtils.SetLayerRecursive(staticSkylander, LayerMask.NameToLayer("LitHUD"));
		staticSkylander.transform.parent = staticSkylanderParent.transform;
		staticSkylander.transform.localScale = Vector3.one;
		staticSkylander.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		if (isGiant)
		{
			AimModifier aimMod = staticSkylander.GetComponent<AimModifier>();
			if (aimMod != null)
			{
				staticSkylander.transform.localPosition = aimMod.introPositionOffset;
				staticSkylander.transform.localScale = aimMod.introScale;
			}
		}
		else
		{
			staticSkylander.transform.localPosition = Vector3.zero;
		}
		GameObjectUtils.HideObject(staticSkylander);
		yield return new WaitForSeconds(4f);
		GameObjectUtils.ShowObject(staticSkylander);
		if (staticSkylanderParent == null)
		{
			yield break;
		}
		staticSkylanderParent.GetComponent<Animation>().Play("SkylanderPartialShow", PlayMode.StopAll);
		yield return new WaitForSeconds(0.25f);
		float fxDelayDestroy = 2f;
		GameObject spawnFXtmp;
		if (isGiant)
		{
			spawnFXtmp = UnityEngine.Object.Instantiate(_turretDestructionFX) as GameObject;
			fxDelayDestroy = 3f;
		}
		else
		{
			if (turretGeometry == null)
			{
				yield break;
			}
			turretGeometry.GetComponent<Renderer>().enabled = true;
			spawnFXtmp = UnityEngine.Object.Instantiate(_spawnFxPrefab) as GameObject;
		}
		theBigShipMeshRenderer.enabled = false;
		theBigShipTurretMeshRenderer.enabled = false;
		shipBowGeometry.GetComponent<Renderer>().enabled = true;
		spawnFXtmp.transform.position = new Vector3(turretGeometry.transform.position.x, -0.37f, turretGeometry.transform.position.z);
		spawnFXtmp.transform.localScale = new Vector3(1.22f, 1.22f, 1.22f);
		UnityEngine.Object.Destroy(spawnFXtmp, fxDelayDestroy);
		GameManager.CameraShake(introCameraGO.GetComponent<Camera>());
		SoundEventManager.Instance.Play2D(landInTurretSFX);
		SoundEventManager.Instance.Play2D(_skylanderSpawnSfx);
		if (!isGiant)
		{
			turretGeometry.GetComponent<Renderer>().material.SetTexture("_MainTex", StartGameSettings.Instance.activeSkylander.elementData.LoadTurretTexture());
		}
		yield return new WaitForSeconds(1f);
		OnSkylanderSpawnComplete();
	}

	public void OnSkylanderSpawnComplete()
	{
		if (ShipIntroVisual.SkylanderSpawnComplete != null)
		{
			ShipIntroVisual.SkylanderSpawnComplete(this, new EventArgs());
		}
	}
}
