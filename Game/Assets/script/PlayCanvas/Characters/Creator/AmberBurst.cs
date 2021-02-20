using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmberBurst : Creator
{
    public void Init()
    {
        remainTime = 3;
        MaxRemainTine = 3;
    }

    public static AmberBurst CreateSelf(Vector2Int pos, Amber amber)
    {
        GameObject obj = CreatObject<AmberBurst>(amber.parent);
        obj.transform.localPosition = BattleArea.GetLocalPosition(pos);
        SpriteRenderer render = obj.GetComponent<SpriteRenderer>();
        render.sprite = amber.parent.GetComponent<Player>().sprites.GetComponent<AllSprites>().Creator_Rose;
        AmberBurst rose = obj.GetComponent<AmberBurst>();
        rose.sprites = amber.parent.GetComponent<Player>().sprites;
        rose.Init();
        rose.amber = amber;
        rose.ShowNormalState();
        rose.burnPos = pos;
        return rose;
    }
    public Amber amber;

    public Vector2Int burnPos;

    public void newRound()
    {
        remainTime--;
        amber.BurstBurn(burnPos);
        if (remainTime < 1)
        {
            amber.DestroyBurstBurn();
        }
        ShowNormalState();
    }

    public string StringGet()
    {
        return ""  + (char)burnPos.x + (char)burnPos.y + (char)remainTime;
    }

    public int StringSet(string msg,int pos)
    {
        burnPos = new Vector2Int(msg[pos++], msg[pos++]);
        remainTime = msg[pos++];
        return pos;
    }
}
