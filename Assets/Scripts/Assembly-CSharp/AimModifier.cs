using UnityEngine;

public class AimModifier : MonoBehaviour
{
	public Vector3 rotationOffset = Vector3.zero;

	public Vector3 altRotationOffset = Vector3.zero;

	public Vector3 introPositionOffset = Vector3.zero;

	public Vector3 introScale = new Vector3(0.8f, 0.8f, 0.8f);

	public Vector3 basePositionOffset = Vector3.zero;

	public float timeToRotateForVictoryAnimation = 0.5f;

	public Vector3 rotationOffsetForVictoryAnimation = new Vector3(0f, -30f, 0f);

	public GameObject victoryFXRef;

	public Vector3 recoilTranslation = Vector3.zero;

	public Vector3 altRecoilTranslation = Vector3.zero;

	private string muzzleBone = "Muzzle";

	private string muzzleBoneAlt = "MuzzleAlt";

	private bool _useAlt;

	private Transform _mainMuzzle;

	private Transform _altMuzzle;

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(AimModifier), LogLevel.Log);

	private Transform MainMuzzle
	{
		get
		{
			if (_mainMuzzle == null)
			{
				_mainMuzzle = TransformUtil.FindRecursive(base.transform, muzzleBone);
				if (_mainMuzzle == null)
				{
					_mainMuzzle = TransformUtil.FindRecursive(base.transform, "Bone R HandAttach");
				}
			}
			return _mainMuzzle;
		}
	}

	private Transform AltMuzzle
	{
		get
		{
			if (_altMuzzle == null)
			{
				_altMuzzle = TransformUtil.FindRecursive(base.transform, muzzleBoneAlt);
				if (_altMuzzle == null)
				{
					_altMuzzle = TransformUtil.FindRecursive(base.transform, "Bone L HandAttach");
				}
			}
			return _altMuzzle;
		}
	}

	public bool isAlt
	{
		get
		{
			if (AltMuzzle == null)
			{
				return false;
			}
			return _useAlt;
		}
	}

	public Transform GetMuzzleParent()
	{
		Transform transform = null;
		transform = ((!_useAlt || !(AltMuzzle != null)) ? MainMuzzle : AltMuzzle);
		_log.Log("Using alt fire = {0}, Transform name = {1}", _useAlt, transform);
		return transform;
	}

	public void SwitchMuzzle()
	{
		_useAlt = !_useAlt;
	}

	public void ResetMuzzle()
	{
		_useAlt = false;
	}
}
