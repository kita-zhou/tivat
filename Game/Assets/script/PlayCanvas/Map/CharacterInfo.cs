using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    public GameObject moreInfo;
    public Hero showd = null;
    public GameObject AllSprites;
    public GameObject State = null;

    public void Initial()
    {
        if(showd == null)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = null;
            moreInfo.GetComponent<SpriteRenderer>().sprite = null;
            DeleteState();
            return;
        }
        Sprite card = showd.card;
        Sprite info = showd.Info;
        gameObject.GetComponent<SpriteRenderer>().sprite = card;
        moreInfo.GetComponent<SpriteRenderer>().sprite = info;
        ShowState(showd.HP, showd.shield, showd.stamina, showd.affected);
        showd.ShowState(gameObject);
    }

    public void ShowState(int blood, int sheild, int stamina , ElementalAffect affect)
    {
        float scale = 6.0f / ((blood + sheild)*0.85f + (blood + sheild)/10);
        if (scale > 1) scale = 1;
        if (State != null) DeleteState();
        State = new GameObject();
        State.transform.parent = transform;
        State.transform.localPosition = new Vector3(-(0.7f), -(1.4f), 0);
        State.transform.localRotation = new Quaternion(0, 0, 0, 0);
        State.transform.localScale = new Vector3(0.2f, 0.2f, 1);
        for(int i = 0; i < blood; i++)
        {
            GameObject obj = new GameObject();
            obj.transform.parent = State.transform;
            obj.transform.localScale = new Vector3(scale, 1, 1);
            obj.transform.localRotation = new Quaternion(0, 0, 0, 0);
            obj.AddComponent<SpriteRenderer>().sprite = AllSprites.GetComponent<AllSprites>().Bar_Red;
            obj.transform.localPosition = new Vector3((i * 0.85f + 0.5f + i / 10) * scale, 0);
        }
        for(int i = 0; i < sheild; i++)
        {
            GameObject obj = new GameObject();
            obj.transform.parent = State.transform;
            obj.transform.localScale = new Vector3(scale, 1, 1);
            obj.transform.localRotation = new Quaternion(0, 0, 0, 0);
            obj.AddComponent<SpriteRenderer>().sprite = AllSprites.GetComponent<AllSprites>().Bar_Yellow;
            obj.transform.localPosition = new Vector3((i * 0.85f + blood*0.85f + 0.5f + ((i + blood) / 10)) * scale, 0);
        }
        Massage.CreateMsg("<color=\"red\">" + blood.ToString() + "</color>-<color=\"yellow\">" + sheild.ToString(), new Vector3(6.8f, 0, 0), State, 0.2f, -1);
        for (int i = 0; i < stamina; i++)
        {
            GameObject obj = new GameObject();
            obj.transform.parent = State.transform;
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.transform.localRotation = new Quaternion(0, 0, 0, 0);
            obj.AddComponent<SpriteRenderer>().sprite = AllSprites.GetComponent<AllSprites>().Bar_Blue;
            obj.transform.localPosition = new Vector3(i+0.5f, -0.5f);
        }

        if(affect!=null && affect.affectElemental!= ElementType.Physics)
        {
            GameObject obj = new GameObject();
            obj.transform.parent = State.transform;
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.transform.localRotation = new Quaternion(0, 0, 0, 0);
            Sprite sprite = null;
            switch (affect.affectElemental)
            {
                case ElementType.Anemo:
                    sprite = AllSprites.GetComponent<AllSprites>().Buff_Elemental_Anemo;
                    break;
                case ElementType.Cryo:
                    sprite = AllSprites.GetComponent<AllSprites>().Buff_Elemental_Cryo;
                    break;
                case ElementType.Dendro:
                    sprite = AllSprites.GetComponent<AllSprites>().Buff_Elemental_Dendro;
                    break;
                case ElementType.Electro:
                    sprite = AllSprites.GetComponent<AllSprites>().Buff_Elemental_Electro;
                    break;
                case ElementType.Geo:
                    sprite = AllSprites.GetComponent<AllSprites>().Buff_Elemental_Geo;
                    break;
                case ElementType.Hydro:
                    sprite = AllSprites.GetComponent<AllSprites>().Buff_Elemental_Hydro;
                    break;
                case ElementType.Pyro:
                    sprite = AllSprites.GetComponent<AllSprites>().Buff_Elemental_Pyro;
                    break;
                default:
                    break;
            }
            obj.AddComponent<SpriteRenderer>().sprite = sprite;
            obj.transform.localPosition = new Vector3(2.5f, -2f);
            obj.transform.localScale = new Vector3(1.5f, 1.5f, 0);
        }

    }

    public void DeleteState()
    {
        if(showd!=null)showd.DeleteShowState(gameObject);
        if (State != null)
        {
            int count = State.transform.childCount;
            for(int i = 0; i < count; i++)
            {
                Destroy(State.transform.GetChild(i).gameObject);
            }
            Destroy(State);
            State = null;
        }
    }

    public void show(Hero hero)
    {
        Debug.Log("show");
        showd = hero;
        Initial();
    }
    // Start is called before the first frame update
    void Start()
    {
        Initial();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
