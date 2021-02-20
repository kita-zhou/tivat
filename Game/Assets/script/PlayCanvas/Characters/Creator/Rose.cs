using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rose : Creator
{
    public void Init()
    {
        blood = 40;
        MaxBlood = 40;
        remainTime = 5;
        MaxRemainTine = 5;
    }

    public static GameObject CreatRose(Vector2Int pos ,Lisa lisa)
    {
        GameObject obj = GameBase.CreatObject<Rose>(lisa.parent);
        obj.transform.localPosition = BattleArea.GetLocalPosition(pos);
        SpriteRenderer render = obj.GetComponent<SpriteRenderer>();
        render.sprite = lisa.parent.GetComponent<Player>().sprites.GetComponent<AllSprites>().Creator_Rose;
        Rose rose = obj.GetComponent<Rose>();
        rose.sprites = lisa.parent.GetComponent<Player>().sprites;
        rose.Init();
        rose.lisa = lisa;
        rose.ShowNormalState();
        rose.positon = pos;
        return obj;
    }

    public Lisa lisa;
    public Vector2Int positon;
    public void newRound()
    {
        remainTime--;
        if (remainTime < 1)
        {
            lisa.DestroyRose();
            return;
        }

        Character[] characters = lisa.parent.GetComponent<Player>().GetEnemyCharacters();
        if (characters.Length < 1) return;
        Character cha = characters[0];
        foreach(Character character in characters)
        {
            if (character.HP < cha.HP)
            {
                cha = character;
            }
            else if (character.HP == cha.HP)
            {
                if(character.shield < cha.shield)
                {
                    cha = character;
                }
            }
        }
        Attack.CreateAttack(parent, BattleArea.GetReverse( cha.position), 10, AttackType.ElementalBurst, ElementType.Electro, lisa).transform.localPosition = transform.localPosition;
        ShowNormalState();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.transform.parent != parent)
        {
            if(collision.TryGetComponent<Attack>(out Attack atk) && atk.active)
            {
                if (atk.Damage >= blood)
                {
                    blood = 0;
                    lisa.DestroyRose();
                }
                else
                {
                    blood -= atk.Damage;
                }
                Destroy(atk.gameObject);
                ShowNormalState();
            }
        }
    }

    public string StringGet()
    {
        return "" + (char)positon.x + (char)positon.y + (char)(blood/100)+(char)(blood%100) + (char)remainTime;
    }
    public int StringSet(string msg,int pos)
    {
        positon = new Vector2Int(msg[pos++], msg[pos++]);
        blood = msg[pos++]*100+msg[pos++];
        remainTime = msg[pos++];
        return pos;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
