using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate bool ObjectEvent(BaseObject obj);
public enum ObjectType
{
    Default,
    Button
}
public class BaseObject : MonoBehaviour
{
    public ObjectEvent mouseEnter;
    public ObjectEvent mouseExit;
    public ObjectEvent mouseDown;
    public ObjectEvent mouseUp;
    public ObjectEvent mouseOver;
    public ObjectEvent mouseDrag;

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



    public virtual bool ObjectInitial(ObjectAdmin admin, Vector2 pos)
    {
        if(!gameObject.TryGetComponent(out BoxCollider2D _))
        {
            boxcollider = gameObject.AddComponent<BoxCollider2D>();
        }
        if (this.admin != null && admin != null)
        {
            this.admin = admin;
            admin.AddObject(this);
        }
        transform.localPosition = new Vector3(pos.x, pos.y);
        return true;
    }

    public virtual bool MoveTo(Vector2 pos)
    {
        transform.localPosition = new Vector3(pos.x, pos.y);
        return true;
    }

    public virtual bool DestroySelf()
    {
        Destroy(gameObject);
        return true;
    }

    public ObjectAdmin admin;
    public BoxCollider2D boxcollider;
    public ObjectType objectType = ObjectType.Default;
}
