using System.ComponentModel;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

[AddComponentMenu("EZ GUI/Controls/List Radio Button")]
public class UIListRadioBtn : UIRadioBtn, IUIListObject, IEZDragDrop, IUIObject
{
    [HideInInspector]
    public bool activeOnlyWhenSelected = true;

    protected int m_index;

    protected bool m_selected;

    protected UIScrollList list;

    protected Vector2 colliderTL;

    protected Vector2 colliderBR;

    protected Vector3 colliderCenter;

    public bool selected
    {
        get
        {
            return m_selected;
        }
        set
        {
            m_selected = value;
            if (m_selected)
            {
                SetLayerState(CONTROL_STATE.Active);
            }
            else
            {
                SetLayerState(CONTROL_STATE.Over);
            }
        }
    }

    public override string Text
    {
        set
        {
            base.Text = value;
            FindOuterEdges();
            if (spriteText != null && spriteText.maxWidth > 0f && list != null)
            {
                list.PositionItems();
            }
        }
    }

    public int Index
    {
        get
        {
            return m_index;
        }
        set
        {
            m_index = value;
        }
    }

    public SpriteText TextObj
    {
        get
        {
            return spriteText;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        if (customCollider && base.GetComponent<Collider>() is BoxCollider)
        {
            BoxCollider boxCollider = (BoxCollider)base.GetComponent<Collider>();
            colliderTL.x = boxCollider.center.x - boxCollider.size.x * 0.5f;
            colliderTL.y = boxCollider.center.y + boxCollider.size.y * 0.5f;
            colliderBR.x = boxCollider.center.x + boxCollider.size.x * 0.5f;
            colliderBR.y = boxCollider.center.y - boxCollider.size.y * 0.5f;
            colliderCenter = boxCollider.center;
        }
    }

    protected void DoNeccessaryInput(ref POINTER_INFO ptr)
    {
        switch (ptr.evt)
        {
            case POINTER_INFO.INPUT_EVENT.NO_CHANGE:
                if (list != null && ptr.active)
                {
                    list.ListDragged(ptr);
                }
                break;
            case POINTER_INFO.INPUT_EVENT.DRAG:
                if (list != null && !ptr.isTap)
                {
                    list.ListDragged(ptr);
                }
                break;
            case POINTER_INFO.INPUT_EVENT.RELEASE:
            case POINTER_INFO.INPUT_EVENT.TAP:
            case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
                if (list != null)
                {
                    list.PointerReleased();
                }
                break;
        }
        if (list != null && ptr.inputDelta.z != 0f && ptr.type != POINTER_INFO.POINTER_TYPE.RAY)
        {
            list.ScrollWheel(ptr.inputDelta.z);
        }
        if (Container != null)
        {
            ptr.callerIsControl = true;
            Container.OnInput(ptr);
        }
    }

    public override void OnInput(ref POINTER_INFO ptr)
    {
        if (deleted)
        {
            return;
        }
        if (!m_controlIsEnabled)
        {
            DoNeccessaryInput(ref ptr);
            return;
        }
        if (list != null && Vector3.SqrMagnitude(ptr.origPos - ptr.devicePos) > list.dragThreshold * list.dragThreshold)
        {
            ptr.isTap = false;
            if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
            {
                ptr.evt = POINTER_INFO.INPUT_EVENT.RELEASE;
            }
        }
        else
        {
            ptr.isTap = true;
        }
        if (inputDelegate != null)
        {
            inputDelegate(ref ptr);
        }
        if (!m_controlIsEnabled)
        {
            DoNeccessaryInput(ref ptr);
            return;
        }
        switch (ptr.evt)
        {
            case POINTER_INFO.INPUT_EVENT.NO_CHANGE:
                if (list != null && ptr.active)
                {
                    list.ListDragged(ptr);
                }
                break;
            case POINTER_INFO.INPUT_EVENT.MOVE:
                if (!selected)
                {
                    SetLayerState(CONTROL_STATE.Over);
                }
                break;
            case POINTER_INFO.INPUT_EVENT.DRAG:
                if (!ptr.isTap)
                {
                    if (!selected)
                    {
                        SetLayerState(CONTROL_STATE.Over);
                    }
                    if (list != null)
                    {
                        list.ListDragged(ptr);
                    }
                }
                break;
            case POINTER_INFO.INPUT_EVENT.RELEASE:
            case POINTER_INFO.INPUT_EVENT.TAP:
                if (list != null)
                {
                    if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
                    {
                        list.DidClick(this, ptr);
                        SetLayerState(CONTROL_STATE.True);
                        Value = true;
                    }
                    list.PointerReleased();
                }
                break;
            case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
                if (!selected)
                {
                    SetLayerState(CONTROL_STATE.Over);
                }
                if (list != null)
                {
                    list.PointerReleased();
                }
                break;
            case POINTER_INFO.INPUT_EVENT.MOVE_OFF:
                if (!selected)
                {
                    SetLayerState(CONTROL_STATE.Over);
                }
                break;
        }
        if (list != null && ptr.inputDelta.z != 0f && ptr.type != POINTER_INFO.POINTER_TYPE.RAY)
        {
            list.ScrollWheel(ptr.inputDelta.z);
        }
        if (Container != null)
        {
            ptr.callerIsControl = true;
            Container.OnInput(ptr);
        }
        if (EventMatchWithButton(ptr.evt) && changeDelegate != null)
        {
            changeDelegate(this);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (controlIsEnabled)
        {
            if (selected)
            {
                SetLayerState(CONTROL_STATE.Active);
            }
            else
            {
                SetLayerState(CONTROL_STATE.Over);
            }
        }
        else
        {
            DisableMe();
        }
    }

    protected override void OnDisable()
    {
        CONTROL_STATE cONTROL_STATE = layerState;
        base.OnDisable();
        SetLayerState(cONTROL_STATE);
    }

    public override void Copy(SpriteRoot s)
    {
        Copy(s, ControlCopyFlags.All);
    }

    public override void Copy(SpriteRoot s, ControlCopyFlags flags)
    {
        base.Copy(s, flags);
        if (s is UIListRadioBtn)
        {
            UIListRadioBtn uIListRadioBtn = (UIListRadioBtn)s;
            if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
            {
                list = uIListRadioBtn.list;
            }
            if ((flags & ControlCopyFlags.Appearance) == ControlCopyFlags.Appearance)
            {
                topLeftEdge = uIListRadioBtn.topLeftEdge;
                bottomRightEdge = uIListRadioBtn.bottomRightEdge;
                colliderTL = uIListRadioBtn.colliderTL;
                colliderBR = uIListRadioBtn.colliderBR;
                colliderCenter = uIListRadioBtn.colliderCenter;
                customCollider = uIListRadioBtn.customCollider;
            }
        }
    }

    public override void FindOuterEdges()
    {
        base.FindOuterEdges();
        if (!customCollider)
        {
            colliderTL = topLeftEdge;
            colliderBR = bottomRightEdge;
        }
    }

    public override void TruncateRight(float pct)
    {
        base.TruncateRight(pct);
        if (base.GetComponent<Collider>() != null && base.GetComponent<Collider>() is BoxCollider)
        {
            if (customCollider)
            {
                BoxCollider boxCollider = (BoxCollider)base.GetComponent<Collider>();
                Vector3 center = boxCollider.center;
                center.x = (1f - pct) * (colliderBR.x - colliderTL.x) * -0.5f;
                boxCollider.center = center;
                center = boxCollider.size;
                center.x = pct * (colliderBR.x - colliderTL.x);
                boxCollider.size = center;
            }
            else
            {
                UpdateCollider();
            }
        }
    }

    public override void TruncateLeft(float pct)
    {
        base.TruncateLeft(pct);
        if (base.GetComponent<Collider>() != null && base.GetComponent<Collider>() is BoxCollider)
        {
            if (customCollider)
            {
                BoxCollider boxCollider = (BoxCollider)base.GetComponent<Collider>();
                Vector3 center = boxCollider.center;
                center.x = (1f - pct) * (colliderBR.x - colliderTL.x) * 0.5f;
                boxCollider.center = center;
                center = boxCollider.size;
                center.x = pct * (colliderBR.x - colliderTL.x);
                boxCollider.size = center;
            }
            else
            {
                UpdateCollider();
            }
        }
    }

    public override void TruncateTop(float pct)
    {
        base.TruncateTop(pct);
        if (base.GetComponent<Collider>() != null && base.GetComponent<Collider>() is BoxCollider)
        {
            if (customCollider)
            {
                BoxCollider boxCollider = (BoxCollider)base.GetComponent<Collider>();
                Vector3 center = boxCollider.center;
                center.y = (1f - pct) * (colliderBR.y - colliderTL.y) * 0.5f;
                boxCollider.center = center;
                center = boxCollider.size;
                center.y = pct * (colliderTL.y - colliderBR.y);
                boxCollider.size = center;
            }
            else
            {
                UpdateCollider();
            }
        }
    }

    public override void TruncateBottom(float pct)
    {
        base.TruncateBottom(pct);
        if (base.GetComponent<Collider>() != null && base.GetComponent<Collider>() is BoxCollider)
        {
            if (customCollider)
            {
                BoxCollider boxCollider = (BoxCollider)base.GetComponent<Collider>();
                Vector3 center = boxCollider.center;
                center.y = (1f - pct) * (colliderBR.y - colliderTL.y) * -0.5f;
                boxCollider.center = center;
                center = boxCollider.size;
                center.y = pct * (colliderTL.y - colliderBR.y);
                boxCollider.size = center;
            }
            else
            {
                UpdateCollider();
            }
        }
    }

    public override void Untruncate()
    {
        base.Untruncate();
        if (!(base.GetComponent<Collider>() != null) || !(base.GetComponent<Collider>() is BoxCollider))
        {
            return;
        }
        if (customCollider)
        {
            BoxCollider boxCollider = (BoxCollider)base.GetComponent<Collider>();
            if (!customCollider)
            {
                boxCollider.center = Vector3.zero;
            }
            else
            {
                boxCollider.center = colliderCenter;
            }
            boxCollider.size = new Vector3(colliderBR.x - colliderTL.x, colliderTL.y - colliderBR.y, 0.001f);
        }
        else
        {
            UpdateCollider();
        }
    }

    public override void Hide(bool tf)
    {
        base.Hide(tf);
        for (int i = 0; i < layers.Length; i++)
        {
            layers[i].Hide(tf);
        }
        if (spriteText != null)
        {
            if (tf)
            {
                spriteText.gameObject.SetActive(false);
            }
            else
            {
                spriteText.gameObject.SetActive(true);
            }
        }
    }

    public void SetList(UIScrollList c)
    {
        list = c;
    }

    public virtual UIScrollList GetScrollList()
    {
        return list;
    }

    public void FindText()
    {
        if (spriteText == null)
        {
            spriteText = (SpriteText)GetComponentInChildren(typeof(SpriteText));
        }
        if (spriteText != null)
        {
            spriteText.gameObject.layer = base.gameObject.layer;
            spriteText.Parent = this;
        }
    }

    public bool IsContainer()
    {
        return false;
    }

    public new static UIListRadioBtn Create(string name, Vector3 pos)
    {
        GameObject gameObject = new GameObject(name);
        gameObject.transform.position = pos;
        return (UIListRadioBtn)gameObject.AddComponent(typeof(UIListRadioBtn));
    }

    public new static UIListRadioBtn Create(string name, Vector3 pos, Quaternion rotation)
    {
        GameObject gameObject = new GameObject(name);
        gameObject.transform.position = pos;
        gameObject.transform.rotation = rotation;
        return (UIListRadioBtn)gameObject.AddComponent(typeof(UIListRadioBtn));
    }

    void IUIListObject.UpdateCamera()
    {
        UpdateCamera();
    }

    // Properties for IUIListObject
    public Vector2 TopLeftEdge
    {
        get { return base.TopLeftEdge; }
    }

    public Vector2 BottomRightEdge
    {
        get { return base.BottomRightEdge; }
    }

    public bool Managed
    {
        get { return base.Managed; }
    }
}
