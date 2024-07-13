using System;
using System.Collections.Generic;
using UnityEngine;

public class GhostSwords : Powerup
{
    public const string StorageKey = "ghostswords";

    private const float _baseRadius = 0.4f;
    private const float raycastDistance = 7.5f;

    public float rotationSpeed = 20f;

    public GhostSword goldSwordPrefab;
    public GhostSword blueSwordPrefab;

    public SoundEventData sfxIn3D;
    public SoundEventData sfxImpact3D;

    public GameObject prefabClang;

    private static GhostSwords _inst;

    private float _radius = 0.4f;
    private DragMultiTarget _dragTarget;

    private int _enemyLayerMask;
    private int _comboCoinMask;

    private GhostSword _sword1;
    private GhostSword _sword2;

    private bool _initPosition = true;

    private static DamageInfo _damageInfo;

    private bool _isTap = true;
    private bool _swordsDropped;

    private static float tapTime = 0.2f;

    private Shooter _shooter;

    private BombSliceGhostSwordsUpgrade _bombSliceUpgrade;

    public static GhostSwords Instance
    {
        get
        {
            return _inst;
        }
        private set
        {
            _inst = value;
        }
    }

    private void Start()
    {
        if (_damageInfo == null)
        {
            _damageInfo = new DamageInfo();
            _damageInfo.damageAmount = 10;
            _damageInfo.damageType = DamageTypes.ghost_swords;
            _damageInfo.comboNum = 5;
        }

        _shooter = ShipManager.instance.shooter[0];
        _dragTarget = ShipManager.instance.dragMultiTarget[m_DragMultiTargetIndex];
        _dragTarget.GhostSword = this;

        _radius = 0.4f;
        _sword1 = UnityEngine.Object.Instantiate(goldSwordPrefab) as GhostSword;
        _sword2 = UnityEngine.Object.Instantiate(blueSwordPrefab) as GhostSword;
        _sword2.isReversed = true;
        _sword1.DragMultiTargetIndex = m_DragMultiTargetIndex;
        _sword2.DragMultiTargetIndex = m_DragMultiTargetIndex;

        LargeGhostSwordsUpgrade passiveUpgradeOrDefault = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<LargeGhostSwordsUpgrade>();
        if (passiveUpgradeOrDefault != null)
        {
            _sword1.transform.localScale *= passiveUpgradeOrDefault.scale;
            _sword2.transform.localScale *= passiveUpgradeOrDefault.scale;
            _radius = 0.4f * passiveUpgradeOrDefault.scale;
            rotationSpeed *= passiveUpgradeOrDefault.speedMultiplier;
        }

        _bombSliceUpgrade = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<BombSliceGhostSwordsUpgrade>();

        _enemyLayerMask = 1 << Layers.Enemies;
        _comboCoinMask = 1 << Layers.ComboCoin;

        Instance = this;
    }

    public void BombHit()
    {
        if (_swordsDropped)
        {
            return;
        }

        if ((bool)prefabClang)
        {
            UnityEngine.Object.Instantiate(prefabClang, _dragTarget.transform.position, Quaternion.identity);
            SoundEventManager.Instance.Play(sfxImpact3D, _dragTarget.gameObject);
        }

        DropSwords();
        InvokeHelper.InvokeSafe(DelayDestruction, 1f, this);

        for (int i = 0; i < ShipManager.instance.dragMultiTarget.Count; i++)
        {
            if (!(ShipManager.instance.dragMultiTarget[i].GhostSword == null))
            {
                ShipManager.instance.dragMultiTarget[i].GhostSword.BombHit();
            }
        }
    }

    private void DelayDestruction()
    {
        DestroyAndFinish(false);
    }

    private void DropSwords()
    {
        if (!_swordsDropped)
        {
            _swordsDropped = true;
            _sword1.Detach();
            _sword2.Detach();
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        BombController.BombControllerStarted += HandleBombControllerStarted;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        BombController.BombControllerStarted -= HandleBombControllerStarted;
    }

    private void HandleBombControllerStarted(object sender, EventArgs args)
    {
        DestroyAndFinish(false);
    }

    public override void DestroyAndFinish(bool waitForCutscene)
    {
        base.DestroyAndFinish(waitForCutscene);
        DropSwords();
        _dragTarget.GhostSword = null;
        Instance = null;
    }

    protected override void Update()
    {
        if (!base.IsTriggered || _swordsDropped)
        {
            return;
        }

        if (GameManager.Instance.IsGameOver || (base.IsTriggered && base.TimeLeft == 0f))
        {
            DestroyAndFinish(false);
        }

        if (_dragTarget.isSelecting && !GameManager.Instance.IsPaused && !HealingElixirScreen.IsActive && !GameManager.Instance.IsGameOver)
        {
            if (_isTap)
            {
                base.TimeLeft = Mathf.Clamp(base.TimeLeft - tapTime, 0f, lifeTimeInSeconds);
                _isTap = false;
            }

            GameObject[] targets = _shooter.targetQueue.Targets;
            foreach (GameObject gameObjectToRemove in targets)
            {
                _shooter.targetQueue.RemoveGameObject(gameObjectToRemove);
            }

            if (base.TimeLeft != 0f && !GameManager.Instance.IsPaused)
            {
                base.TimeLeft -= Time.realtimeSinceStartup - _timeLastFrame;
                base.TimeLeft = Mathf.Clamp(base.TimeLeft, 0f, float.PositiveInfinity);
            }

            if (_initPosition)
            {
                _initPosition = false;
                _sword1.StartRotating(rotationSpeed);
                _sword2.StartRotating(rotationSpeed);
                SoundEventManager.Instance.Play(sfxIn3D, _dragTarget.gameObject);
            }

            Vector3 position = Camera.main.WorldToScreenPoint(_dragTarget.transform.position);
            Ray ray = Camera.main.ScreenPointToRay(position);
            IEnumerable<RaycastHit> hits = ConeCast.GetHits(ray, _radius, 7.5f, _enemyLayerMask | _comboCoinMask);

            foreach (RaycastHit item in hits)
            {
                GameObject gameObject = item.collider.gameObject;
                Hazard component = gameObject.GetComponent<Hazard>();
                if (component == null || !component._isActive)
                {
                    Health component2 = gameObject.GetComponent<Health>();
                    if (component2 != null && !component2.isDeflecting && !component2.isForceFielded && !component2.isDead)
                    {
                        _shooter.GhostSwordsShotEvent(gameObject);
                        component2.TakeHit(_damageInfo);
                        SoundEventManager.Instance.Play(sfxImpact3D, gameObject);
                    }
                    else
                    {
                        Loot component3 = gameObject.GetComponent<Loot>();
                        if (component3 != null && !component3.IsCollected)
                        {
                            component3.Collect();
                        }
                        ComboCoin component4 = gameObject.GetComponent<ComboCoin>();
                        if (component4 != null && component4.isElemental)
                        {
                            component4.Pop();
                        }
                    }
                }
                if (!(component != null) || !component._isActive || !(_bombSliceUpgrade != null))
                {
                    continue;
                }
                Health component5 = gameObject.GetComponent<Health>();
                if (component5 != null)
                {
                    component5.isDeflecting = false;
                    component5.TakeHit(_damageInfo);
                    Accessory component6 = gameObject.GetComponent<Accessory>();
                    if (component6 != null && component6.accessory != null)
                    {
                        UnityEngine.Object.Destroy(gameObject.GetComponent<Accessory>().accessory.gameObject);
                    }
                    _bombSliceUpgrade.SpawnSlicedShieldAtTarget(gameObject);
                    component.DefusedHazard(false);
                }
                else
                {
                    _bombSliceUpgrade.SpawnSlicedBombAtTarget(gameObject);
                    component.DefusedHazard(false);
                    UnityEngine.Object.Destroy(gameObject);
                }
            }
        }
        else
        {
            _sword1.StopRotating();
            _sword2.StopRotating();
            _initPosition = true;
            _isTap = true;
        }
        _timeLastFrame = Time.realtimeSinceStartup;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(ShipManager.instance.dragMultiTarget[m_DragMultiTargetIndex].transform.position, _radius);
    }
}
