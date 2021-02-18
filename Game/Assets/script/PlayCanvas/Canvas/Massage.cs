using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Massage : MonoBehaviour
{
    public float liveTime;

    public bool isStarted = false;

    public float activeTime;

    public Font font;

    public static GameObject CreateMsg(string msg, Vector3 localPos,GameObject parent, float scale = 0.01f,float liveTime = 0.5f)
    {
        GameObject obj = new GameObject();
        obj.transform.parent = parent.transform;
        MeshRenderer textrender = obj.AddComponent<MeshRenderer>();
        TMPro.TextMeshPro text = obj.AddComponent<TMPro.TextMeshPro>();
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(1000,100);
        text.text = msg;
        //text.font = GameObject.Find("Font/DengXian").GetComponent<Massage>().font;
        text.color = new Color(1, 1, 1);
        text.fontSize = 35;
        text.alignment = TMPro.TextAlignmentOptions.CenterGeoAligned;
        Massage massage = obj.AddComponent<Massage>();
        massage.liveTime = liveTime;
        
        obj.transform.localScale = new Vector3(scale, scale, 1);
        obj.transform.localPosition = localPos;
        obj.transform.localRotation = new Quaternion(0, 0, 0, 0);
        return obj;
    }

    public static GameObject CreateMsg(Sprite img, Vector3 localPos, GameObject parent, float scale = 1, float liveTime = 0.5f)
    {
        GameObject obj = new GameObject();
        obj.transform.parent = parent.transform;
        SpriteRenderer render = obj.AddComponent<SpriteRenderer>();
        render.sprite = img;
        Massage massage = obj.AddComponent<Massage>();
        massage.liveTime = liveTime;
        obj.transform.localScale = new Vector3(scale, scale, 1);
        obj.transform.localPosition = localPos;
        obj.transform.localRotation = new Quaternion(0, 0, 0, 0);
        return obj;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(liveTime < 0)
        {
            return;
        }
        if (!isStarted)
        {
            isStarted = true;
            activeTime = Time.time;
        }
        else
        {
            if ((Time.time - activeTime) > liveTime)
            {
                Destroy(gameObject);
            }
        }
    }
}
