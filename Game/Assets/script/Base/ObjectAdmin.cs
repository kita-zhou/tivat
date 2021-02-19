using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAdmin : MonoBehaviour
{
    public List<BaseObject> objects;

    public static ObjectAdmin CreateAdmin(Vector3 pos)
    {
        GameObject obj = new GameObject();
        obj.transform.position = pos;
        return obj.AddComponent<ObjectAdmin>();
    }

    public static ObjectAdmin CreateAdmin(GameObject parent, Vector3 pos)
    {
        GameObject obj = new GameObject();
        obj.transform.parent = parent.transform;
        obj.transform.localPosition = pos;
        return obj.AddComponent<ObjectAdmin>();
    }

    public bool AddObject(BaseObject obj)
    {
        if (objects.Exists(t => t==obj)) return false;
        objects.Add(obj);
        obj.transform.parent = transform;
        return true;
    }
    public bool AddObject(BaseObject obj, Vector2 pos)
    {
        if (!AddObject(obj)) return false;
        obj.transform.localPosition = new Vector3(pos.x,pos.y);
        return true;
    }
    public bool DeleteObject(BaseObject obj)
    {
        if (!objects.Remove(obj)) return false;
        obj.DestroySelf();
        return true;
    }
}
