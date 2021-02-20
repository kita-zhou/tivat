using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screen : Creator
{ 
    public void Init()
    {
        blood = 20;
        MaxBlood = 20;
        remainTime = 2;
        MaxRemainTine = 2;
    }


    public Ningguang owner;
    public Vector2Int position;
    public void NewRound()
    {
        remainTime--;
        if (remainTime <= 0)
        {
            owner.ScreenDestroy();
        }
        if(parent.TryGetComponent<Player>(out Player player))
        {
            for(int i=0;i<player.characterCount;i++)
            {
                Character character = player.myCharacters[i];
                if (Abs(character.position.x - transform.localPosition.x-3.5f) <= 2.2 && Abs(character.position.y - transform.localPosition.y-3.5f) <= 1.2)
                {
                    if (character.shield < character.MAXShield)
                    {
                        character.SelfHeal(0, 10);
                        break;
                    }
                }
            }
        }
        ShowNormalState();
    }

    public static GameObject CreatScreen(GameObject parent, Ningguang ningguang) {
        GameObject obj = CreatObject<Screen>(parent);
        obj.GetComponent<BoxCollider2D>().size = new Vector2(3, 1);
        obj.GetComponent<Screen>().owner = ningguang;
        obj.GetComponent<Screen>().sprites = ningguang.parent.GetComponent<Player>().sprites;
        obj.GetComponent<Screen>().ShowNormalState();
        return obj;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (parent != collision.gameObject.transform.parent.gameObject)//父物体不同
        {
            if (collision.gameObject.TryGetComponent<Character>(out Character other))//如果是敌方角色
            {
                other.MovingBack();
            }
            else if(collision.gameObject.TryGetComponent<Attack>(out Attack atk))//如果是攻击
            {
                if (atk.Damage >= blood)
                {
                    owner.ScreenDestroy();//碎裂
                    Destroy(this.gameObject);
                }
                else
                {
                    blood -= atk.Damage;
                }
                Destroy(atk.gameObject);//阻挡本次攻击
                ShowNormalState();
            }
        }
    }


    public string StringGet()
    {
        return "" +(char) position.x + (char)position.y + (char)remainTime + (char)(blood/100)+(char)(blood%100) ;
    }

    public int StringSet(string msg,int pos)
    {
        position = new Vector2Int(msg[pos++], msg[pos++]);
        remainTime = msg[pos++];
        blood = msg[pos++]*100+msg[pos++];
        return pos;
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
