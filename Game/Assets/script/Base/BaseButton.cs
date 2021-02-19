using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseButton : BaseObject
{
    private void OnMouseEnter()
    {
        mouseEnter?.Invoke(this);
    }

    private void OnMouseExit()
    {
        mouseExit?.Invoke(this);
    }

    private void OnMouseDown()
    {
        mouseDown?.Invoke(this);
    }

    private void OnMouseUp()
    {
        mouseUp?.Invoke(this);
    }

    private void OnMouseOver()
    {
        mouseOver?.Invoke(this);
    }

    private void OnMouseDrag()
    {
        mouseDrag?.Invoke(this);
    }

    public TMPro.TextMeshPro msg;
    public GameObject background;
    public bool ButtonInitial(string massage, Sprite background)
    {
        if(msg==null && massage != null)
        {
            if (!gameObject.TryGetComponent(out MeshRenderer _)) gameObject.AddComponent<MeshRenderer>();
            if (msg==null || !gameObject.TryGetComponent(out TMPro.TextMeshPro _))msg = gameObject.AddComponent<TMPro.TextMeshPro>();
            msg.text = massage;
            msg.color = new Color(0, 0, 0);
            msg.alignment = TMPro.TextAlignmentOptions.CenterGeoAligned;
        }
        if(background!=null && this.background == null)
        {
            this.background = new GameObject();
            this.background.AddComponent<SpriteRenderer>().sprite = background;
        }
        return true;
    }

    public static BaseButton CreatButton(ObjectAdmin admin, Vector2 pos,string massage,Sprite backgroundimage=null)
    {
        GameObject obj = new GameObject();
        BaseButton baseObject = obj.AddComponent<BaseButton>();
        baseObject.ObjectInitial(admin,pos);
        baseObject.ButtonInitial(massage, backgroundimage);
        return baseObject;
    }
}
