﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Ningguang : Hero
{
    // Start is called before the first frame update

       

    public Vector2Int[] posesNormal =
        {
            new Vector2Int(1,0), new Vector2Int(-1,0),
            new Vector2Int(1,1),new Vector2Int(0,1), new Vector2Int(-1,1),
            new Vector2Int(1,2), new Vector2Int(0,2), new Vector2Int(-1,2),
            new Vector2Int(0,3)
        };


    public Screen screen = null;

    public void ScreenDestroy()
    {
        if (screen == null) return;
        Destroy(screen);
        screen = null;
        NormalEffects.Remove("Screen");
    }//摧毁璇玑屏

    public bool ScreenCreate(Vector2Int pos)
    {
        if (stamina < 1 || screen != null) return false;
        Debug.Log("Screen Create");
        pos += position;
        if (pos.x < 2) pos.x = 2;
        else if (pos.x > 5) pos.x = 5;
        GameObject obj =  Screen.CreatScreen(parent,this);
        screen = obj.GetComponent<Screen>();
        obj.transform.localPosition = new Vector3(-3.5f + pos.x, -3.5f + pos.y, -1.0f);
        NormalEffects.Add("Screen", UpdateScreen);
        screen.ChangeApprence(parent.GetComponent<Player>().sprites.GetComponent<AllSprites>().Creator_Screen);

        stamina--;
        GeoUsed();
        return true;

    }//创造璇玑屏

    public void UpdateScreen()
    {
        if(screen != null)
        {
            screen.NewRound();
        }
    }//新回合更新璇玑屏

    public override void Heroinit()
    {
        if (Inited) return;
        Inited = true;
        Debug.Log("NingguangInit");
        Initial("Character_Ningguang", 4, 1);
        /* sprites = GameObject.Find("Sprites");
         appearance = sprites.GetComponent<AllSprites>().characterAvatar_Ningguang;
         card = sprites.GetComponent<AllSprites>().characterCard_Ningguang;
         Info = sprites.GetComponent<AllSprites>().characterInfo_Ningguang;*/
        heroType = HeroType.Mage;
        element = ElementType.Geo;

        BasicCardsCount = 15;
        BasicCards = new string[15]
        {
            "Normal_Attack",
            "Normal_Attack",
            "Normal_Move",
            "Normal_Defence",
            "Normal_Defence",
            "Normal_Burst",
            "Normal_Burst",
            "Normal_Geo",
            "Normal_Geo",
            "Normal_Geo",
            "Normal_Geo",
            "Normal_Geo",
            "Normal_Geo",
            "Normal_Geo",
            "Normal_Geo"
        };

        poses = posesNormal;

        Vector2Int[] posDontNeedTag = new Vector2Int[] { new Vector2Int(-1, -1) };
        normalAttackDamege = 1;
        normalAttackElemental = ElementType.Geo;

        UseCard normalAtk = new UseCard();
        normalAtk.postions = posesNormal;
        normalAtk.func = NormalAttack;

        UsingCardDic.Add("#+Normal_Geo", normalAtk);

        AddChargeAttack();

        UseCard skill = new UseCard();
        skill.postions = posesNormal;
        skill.func = ScreenCreate;

        UsingCardDic.Add("#+Normal_Defence+Normal_Geo", skill);
        UsingCardDic.Add("#+Normal_Geo+Normal_Defence", skill);

        UseCard burst = new UseCard();
        burst.postions = posDontNeedTag;
        burst.func = Burst;
        burst.CanUse = DefaultCanUse;
        burst.needTarget = false;
        UsingCardDic.Add("#+Normal_Burst+Normal_Geo", burst);
        UsingCardDic.Add("#+Normal_Geo+Normal_Burst", burst);

        UseCard move = new UseCard();
        move.postions = posesMove;
        move.func = Move;

        UsingCardDic.Add("#+Normal_Move", move);

        AddUseCard("#+Item_CrystalCore", CrystalNormal, posesNormal);

        AddUseCard("#+Item_CrystalCore+Normal_Attack", CrystalCharge, posesNormal);
        AddUseCard("#+Normal_Attack+Item_CrystalCore", CrystalCharge, posesNormal);

        AddUseCard("#+Item_CrystalCore+Normal_Defence", CrystalSkill, posesNormal);
        AddUseCard("#+Normal_Defence+Item_CrystalCore", CrystalSkill, posesNormal);

        AddUseCard("#+Normal_Burst+Item_CrystalCore", CrystalBurst, posDontNeedTag,false);
        AddUseCard("#+Item_CrystalCore+Normal_Burst", CrystalBurst, posDontNeedTag,false);
    }//初始化

    public override bool NormalAttack(Vector2Int pos)
    {
        if(stamina < 1)
        {
         return false;
        }
        pos += position;
        GameObject obj = CreatObject<Attack>(parent);
        obj.name = "Ningguang_Normal_Attack";
        Attack atk = obj.GetComponent<Attack>();
        atk.Initial(pos, 1, AttackType.NormalAttack, ElementType.Geo, this);
        atk.ChangeApprence(sprites.GetComponent<AllSprites>().Attack_Ningguang_Normal);
        atk.activeSprite = sprites.GetComponent<AllSprites>().Attack_Ningguang_Normal_Actived;
        stamina--;
        GeoUsed();
        return true;
    }//普通攻击

    public int ChargeAttackLimit = 0;


    public bool ChargeAttack(Vector2Int pos)
    {
        if (stamina < 1 || ChargeAttackLimit>0)
        {
            return false;
        }
        pos += position;
        GameObject obj = CreatObject<Attack>(parent);
        obj.name = "Ningguang_Charged_Attack";
        obj.transform.localScale = new Vector3(1.5f, 1.5f, 1);
        Attack atk = obj.GetComponent<Attack>();
        atk.Initial(pos, 2, AttackType.ChargedAttack, ElementType.Geo, this);
        ChargeAttackLimit = 2;
        NormalEffects.Add("ChargedAttackLimit", ChargedAttackLimit);
        atk.ChangeApprence(sprites.GetComponent<AllSprites>().Attack_Ningguang_Normal);
        atk.activeSprite = sprites.GetComponent<AllSprites>().Attack_Ningguang_Normal_Actived;
        stamina--;
        GeoUsed();
        return true;
    }//重击

    public void DeleteChargeAttack()
    {
        UsingCardDic.Remove("#+Normal_Geo+Normal_Attack");
        UsingCardDic.Remove("#+Normal_Attack+Normal_Geo");
    }//删除重击

    public void AddChargeAttack()
    {
        UseCard charge = new UseCard();
        charge.postions = posesNormal;
        charge.func = ChargeAttack;

        UsingCardDic.Add("#+Normal_Geo+Normal_Attack", charge);
        UsingCardDic.Add("#+Normal_Attack+Normal_Geo", charge);
    }//添加重击

    public void ChargedAttackLimit()
    {
        if(ChargeAttackLimit > 0)
        {
            ChargeAttackLimit--;
        }
        else
        {
            AddChargeAttack();
            NormalEffects.Remove("ChargedAttackLimit");
        }
    }//重击限制

    public bool Burst(Vector2Int pos)
    {
        if (stamina < 1) return false;
        Character[] enemys = parent.GetComponent<Player>().GetEnemyCharacters();
        if (enemys.Length == 0) return false;
        Character[] sorted =  SortEnemyWithDistence(enemys, position);
        int lenth = sorted.Length;
        int count = 4;
        if (screen != null) count++;
        /*for(int i = 0; i < lenth; i++)
        {
            if (count <= 0) break;
            int num = count / 2;
            if (num == 0) num = 1;
            if (i == lenth - 1) num = count;
            count -= num;
            for (int j = 0; j < num; j++)
            {
                GameObject obj = CreatObject<Attack>(parent);
                Attack atk = obj.GetComponent<Attack>();
                Vector2Int posi = new Vector2Int(7-sorted[i].position.x, 7-sorted[i].position.y);
                atk.Initial(posi, 1, AttackType.ElementalBurst, ElementType.Geo, this);
                atk.speed = 5 + j*2;
            }
        }*/
        for (int i = 0; i < count; i++)
        {
            int tar = RandomWithIndexLinear(lenth);
            Vector2Int position = sorted[tar].position;
            GameObject obj = CreatObject<Attack>(parent);
            Attack atk = obj.GetComponent<Attack>();
            Vector2Int posi = new Vector2Int(7 - position.x, 7 - position.y);
            atk.Initial(posi, 1, AttackType.ElementalBurst, ElementType.Geo, this);
            atk.ChangeApprence(sprites.GetComponent<AllSprites>().Attack_Ningguang_Normal);
            atk.activeSprite = sprites.GetComponent<AllSprites>().Attack_Ningguang_Normal_Actived;
        }
        stamina--;
        GeoUsed();
        return true;
    }//爆发

    public void GeoUsed()
    {
        int ran = Random.Range(0, 100);
        if (ran >= 50)
        {
            Character[] characters = parent.GetComponent<Player>().GetEnemyCharacters();
            int num = characters.Length;
            if (num == 0) return;
            int tar = Random.Range(0, num);
            Vector2Int pos = characters[tar].position;

            GameObject obj = CreatObject<Attack>(parent);
            Attack atk = obj.GetComponent<Attack>();
            atk.Initial(new Vector2Int(7,7)- pos, 1, AttackType.NormalAttack, ElementType.Geo, this);
            atk.ChangeApprence(sprites.GetComponent<AllSprites>().Attack_Ningguang_Normal);
            atk.activeSprite = sprites.GetComponent<AllSprites>().Attack_Ningguang_Normal_Actived;
        }
    }

    public bool CrystalNormal(Vector2Int pos)
    {
        if (stamina < 1)
        {
            return false;
        }
        pos += position;
        GameObject obj = CreatObject<Attack>(parent);
        Attack atk = obj.GetComponent<Attack>();
        atk.Initial(pos, 1, AttackType.NormalAttack, ElementType.Physics, this);
        atk.ChangeApprence(sprites.GetComponent<AllSprites>().Attack_Ningguang_Normal);
        atk.activeSprite = sprites.GetComponent<AllSprites>().Attack_Ningguang_Normal_Actived;
        stamina--;
        return true;
    }//用晶核的普攻

    public bool CrystalCharge(Vector2Int pos)
    {
        if (stamina < 1 || ChargeAttackLimit >0)
        {
            return false;
        }
        pos += position;
        GameObject obj = CreatObject<Attack>(parent);
        obj.name = "Ningguang_Charged_Attack";
        obj.transform.localScale = new Vector3(1.5f, 1.5f, 1);
        Attack atk = obj.GetComponent<Attack>();
        atk.Initial(pos, 2, AttackType.ChargedAttack, ElementType.Geo, this);
        atk.ChangeApprence(sprites.GetComponent<AllSprites>().Attack_Ningguang_Normal);
        atk.activeSprite = sprites.GetComponent<AllSprites>().Attack_Ningguang_Normal_Actived;
        ChargeAttackLimit = 2;
        NormalEffects.Add("ChargedAttackLimit", ChargedAttackLimit);
        stamina--;
        return true;
    }//用晶核的重击

    public bool CrystalSkill(Vector2Int pos)
    {
        return ScreenCreate(pos);
    }//用晶核的技能

    public bool CrystalBurst(Vector2Int pos)
    {
        if (stamina < 1) return false;
        Character[] enemys = parent.GetComponent<Player>().GetEnemyCharacters();
        if (enemys.Length == 0) return false;
        Character[] sorted = SortEnemyWithDistence(enemys, position);
        int lenth = sorted.Length;
        int count = 4;
        if (screen != null) count++;
        /*for (int i = 0; i < lenth; i++)
        {
            if (count <= 0) break;
            int num = count / 2;
            if (num == 0) num = 1;
            if (i == lenth - 1) num = count;
            count -= num;
            for (int j = 0; j < num; j++)
            {
                GameObject obj = CreatObject<Attack>(parent);
                Attack atk = obj.GetComponent<Attack>();
                Vector2Int posi = new Vector2Int(7 - sorted[i].position.x, 7 - sorted[i].position.y);
                atk.Initial(posi, 1, AttackType.ElementalBurst, ElementType.Physics, this);
                atk.speed = 5 + j * 2;
            }
        }*/
        for(int i = 0; i < count; i++)
        {
            int tar = RandomWithIndexLinear(lenth);
            Vector2Int position = sorted[tar].position;
            GameObject obj = CreatObject<Attack>(parent);
            Attack atk = obj.GetComponent<Attack>();
            Vector2Int posi = new Vector2Int(7 - position.x, 7 - position.y);
            atk.Initial(posi, 1, AttackType.ElementalBurst, ElementType.Physics, this);
            atk.ChangeApprence(sprites.GetComponent<AllSprites>().Attack_Ningguang_Normal);
            atk.activeSprite = sprites.GetComponent<AllSprites>().Attack_Ningguang_Normal_Actived;
        }
        stamina--;
        return true;
    }//用晶核的元素爆发

    public static GameObject CreatObject(GameObject parent)
    {
        GameObject obj = CreatObject<Ningguang>(parent);
        obj.GetComponent<Ningguang>().Area = parent.GetComponent<Player>().battleArea;
        obj.name = "Ningguang";
        obj.transform.localPosition = new Vector3(0.5f, 0.5f, 0.0f);
        obj.transform.localRotation = new Quaternion(0, 0, 0, 0);
        return obj;
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        NewFrameSettle();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (screen != null)
        {
            Destroy(screen.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Character>(out Character character))
        {
            CharacterCollision(character);
        }
    }
}