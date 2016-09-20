using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PackItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{

    Text text;

    Image img;

    void Awake()
    {
        this.text = this.transform.FindChild("Text").GetComponent<Text>();
        this.img = this.transform.FindChild("GoodImg").GetComponent<Image>();
    }

    private PackModel model;

    public PackModel Model
    {
        get { return model; }
        set
        {
            model = value;
            if (model.GoodId != 0)
            {
                this.img.enabled = true;
                this.text.text = model.Count.ToString();
                this.img.sprite = Resources.Load<Sprite>(model.good.Src);

            }
            else
            {
                this.img.enabled = false;
                this.text.text = "0";
                this.img.sprite = null;
            }


        }
    }





    #region IBeginDragHandler 成员
    static public T FindInParents<T>(GameObject go) where T : Component
    {
        if (go == null) return null;
        var comp = go.GetComponent<T>();

        if (comp != null)
            return comp;

        Transform t = go.transform.parent;
        while (t != null && comp == null)
        {
            comp = t.gameObject.GetComponent<T>();
            t = t.parent;
        }
        return comp;
    }
    Canvas canvas;
    private GameObject m_DraggingIcon;
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!this.img.isActiveAndEnabled)
        {
            return;
        }
        canvas = FindInParents<Canvas>(gameObject);
        if (canvas == null)
            return;
        m_DraggingIcon = new GameObject("icon");
        m_DraggingIcon.transform.SetParent(canvas.transform, false);
        m_DraggingIcon.transform.SetAsLastSibling();

        var image = m_DraggingIcon.AddComponent<Image>();
        CanvasGroup group = m_DraggingIcon.AddComponent<CanvasGroup>();
        group.blocksRaycasts = false;

        image.sprite = this.img.sprite;
        SetDraggedPosition(eventData);
    }

    #endregion

    #region IDragHandler 成员

    public void OnDrag(PointerEventData eventData)
    {
        if (!this.img.isActiveAndEnabled)
        {
            return;
        }
        if (m_DraggingIcon != null)
            SetDraggedPosition(eventData);
    }
    private void SetDraggedPosition(PointerEventData data)
    {
        var rt = m_DraggingIcon.GetComponent<RectTransform>();
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.GetComponent<RectTransform>(), data.position, data.pressEventCamera, out globalMousePos))
        {
            rt.position = globalMousePos;
        }
    }
    #endregion

    #region IDropHandler 成员
    PackItem temp;
    PackItem drag;
    public void OnDrop(PointerEventData eventData)
    {
        Debug.LogError(gameObject.name);
        temp = gameObject.GetComponent<PackItem>();
        drag = eventData.pointerDrag.GetComponent<PackItem>();

    }

    #endregion

    #region IEndDragHandler 成员

    public void OnEndDrag(PointerEventData eventData)
    {
        if (temp != null && drag != null)
        {
            AppFacade.Intance.ExcuteToController(new INotifier("ChangeItemCommand", drag, temp));
        }
        if (m_DraggingIcon != null)
            Destroy(m_DraggingIcon);
    }

    #endregion
}
