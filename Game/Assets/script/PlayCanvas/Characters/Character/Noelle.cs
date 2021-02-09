using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Noelle: Hero
{
    public override void Heroinit()
    {
        if (Inited) return;
        Inited = true;
        MaxFreeMove = 2;
        FreeMoveCount = 2;
        Initial("Character_Noelle", 4, 2);
        heroType = HeroType.Claymore;
        element = ElementType.Geo;

        BasicCardsCount = 15;
        BasicCards = new string[15]
        {
            "Normal_Attack",
            "Normal_Attack",
            "Normal_Attack",
            "Normal_Attack",
            "Normal_Attack",
            "Normal_Defence",
            "Normal_Defence",
            "Normal_Defence",
            "Normal_Defence",
            "Normal_Defence",
            "Normal_Burst",
            "Normal_Geo",
            "Normal_Geo",
            "Normal_Geo",
            "Normal_Geo"
        };

        Vector2Int[] posDontNeedTag = new Vector2Int[]
        {
            new Vector2Int(-1,-1)
        };

        poses = posClayMore;
        NormalEffects.Add("SheildBonus", SheildBonus);
        NormalEffects.Add("SkillBonusEffect", SkillBonusEffect);

        AddUseCard("#+Normal_Move", Move, posesMove);

        AddUseCard("#+Normal_Attack", NormalAttack, poses);

        AddUseCard("#+Normal_Attack+Normal_Defence", ChargedAttack, poses);
        AddUseCard("#+Normal_Defence+Normal_Attack", ChargedAttack, poses);

        AddUseCard("#+Item_Claymore+Normal_Defence", ChargedAttack, poses);
        AddUseCard("#+Item_Claymore+Normal_Attack", ChargedAttack, poses);

        AddUseCard("#+Normal_Attack+Item_Claymore", ChargedAttack, poses);
        AddUseCard("#+Normal_Defence+Item_Claymore", ChargedAttack, poses);

        AddUseCard("#+Normal_Defence+Normal_Geo", Skill, posDontNeedTag,false);
        AddUseCard("#+Normal_Geo+Normal_Defence", Skill, posDontNeedTag,false);

        AddUseCard("#+Item_CrystalCore+Normal_Defence", CrystalSkill, posDontNeedTag,false);
        AddUseCard("#+Normal_Defence+Item_CrystalCore", CrystalSkill, posDontNeedTag,false);

        AddUseCard("#+Normal_Burst+Normal_Geo", Burst, posDontNeedTag,false);
        AddUseCard("#+Normal_Geo+Normal_Burst", Burst, posDontNeedTag,false);

        AddUseCard("#+Item_CrystalCore+Normal_Burst", CrystalBurst, posDontNeedTag,false);
        AddUseCard("#+Normal_Burst+Item_CrystalCore", CrystalBurst, posDontNeedTag,false);

        AddUseCard("#", BurstBonusAttack, posBurstBonus,CanUseVoid);

    }

    public bool CanUseVoid()
    {
        if (BurstBonus > 0) return true;
        return false;
    }
    public int ShieldEmptyCount=0;
    public void SheildBonus()
    {
        if (shield == 0)
        {
            if(ShieldEmptyCount<1)ShieldEmptyCount++;
            else
            {
                ShieldEmptyCount=0;
                SelfHeal(0, 1);
                if(parent.TryGetComponent(out Player player))
                {
                    string log = "";
                    if (player.isPlayer) log += "友方 ";
                    else log += " 敌方 ";
                    log += characterName;
                    log += " 被动 恢复 1 护盾";
                    player.Log(log);
                }
            }
        }
        else
        {
            ShieldEmptyCount=0;
        }
    }

    public override bool NormalAttack(Vector2Int targ)
    {
        if (stamina < 1) return false;
        CreateAttack(targ, 1, AttackType.NormalAttack, ElementType.Physics);
        stamina--;
        return true;
    }

    public override bool ChargedAttack(Vector2Int targ)
    {
        if (stamina < 1) return false;
        CreateAttack(targ, 1+shield, AttackType.ChargedAttack, ElementType.Physics);
        stamina--;
        return true;
    }

    public bool SkillBonus = false;
    public bool Skill(Vector2Int pos)
    {
        if (stamina < 1) return false;
        SelfHeal(0, 1);
        SkillBonus = true;
        ShowNormalState();
        stamina--;
        gameObject.GetComponent<AudioSource>().PlayOneShot(audios.Noelle_Skill);
        return true;
    }

    public bool CrystalSkill(Vector2Int pos)
    {
        return Skill(pos);
    }

    public bool CanReach(Character character,Vector2Int[] poses) {
        Vector2Int dis = new Vector2Int(7, 7) - character.position - position;
        foreach(Vector2Int pos in poses)
        {
            if (pos == dis) return true;
        }
        return false;
    }

    public int CanReach(Character[] chas,out Character[] canReach,Vector2Int[] poses)
    {
        canReach = new Character[chas.Length];
        int count = 0;
        foreach(Character character in chas)
        {
            if (CanReach(character, poses))
            {
                canReach[count++] = character;
            }
        }
        return count;
    }

    public void SkillBonusEffect()
    {
        if (SkillBonus)
        {
            SkillBonus = false;
            ShowNormalState();
            if (shield == 0) return;
            for(int i = 0; i < parent.GetComponent<Player>().characterCount; i++)
            {
                parent.GetComponent<Player>().myCharacters[i].SelfHeal(1, 0);
            }
            int count = CanReach(parent.GetComponent<Player>().GetEnemyCharacters(), out Character[] canReach, poses);
            if (count > 0)
            {
                int index = Random.Range(0, count);
                CreateAttack(new Vector2Int(7, 7) - canReach[index].position - position, 1, AttackType.ElementalSkill, ElementType.Geo);
            }
        }
    }

    public int BurstBonus = 0;

    public bool Burst(Vector2Int pos)
    {
        if (stamina < 1) return false;
        BurstBonus = 2;
        ShowNormalState();
        stamina--;
        gameObject.GetComponent<AudioSource>().PlayOneShot(audios.Noelle_Burst);
        return true;
    }

    public bool CrystalBurst(Vector2Int pos)
    {
        return Burst(pos);
    }

    public Vector2Int[] posBurstBonus = new Vector2Int[]
    {
        new Vector2Int(0,3),
        new Vector2Int(-1,2),new Vector2Int(0,2),new Vector2Int(1,2),
        new Vector2Int(-2,1),new Vector2Int(-1,1),new Vector2Int(0,1),new Vector2Int(1,1),new Vector2Int(2,1),
        new Vector2Int(-2,0),new Vector2Int(-1,0),new Vector2Int(1,0),new Vector2Int(2,0)
    };

    public override void ShowNormalState()
    {
        base.ShowNormalState();
        int i = 0;
        if (SkillBonus)
        {
            AddImgOfNormalState(sprites.GetComponent<AllSprites>().Buff_Hp_Add_All, new Vector3(i*1.5f, 0.6f, 0), new Vector3(1.6f,1.0f,1));
            i++;
        }
        if (BurstBonus>0)
        {
            AddImgOfNormalState(sprites.GetComponent<AllSprites>().Buff_Attack_Add, new Vector3(i*1.5f, 0.6f, 0), new Vector3(1.6f, 1.0f, 1));
        }
    }


    public bool BurstBonusAttack(Vector2Int pos)
    {
        if (BurstBonus < 1) return false;
        CreateAttack(pos, 1+shield, AttackType.ElementalBurst, ElementType.Geo);
        BurstBonus--;
        ShowNormalState();
        return true;
    }

    public override string StringGet()
    {
        string msg = "";
        if (SkillBonus) msg += (char)1;
        else msg += (char)0;
        msg += (char)BurstBonus;
        return base.StringGet()+msg;
    }

    public override int StringSet(string msg,int pos)
    {
        pos =  base.StringSet(msg,pos);
        if (msg[pos++] == 1)
        {
            SkillBonus = true;
        }
        BurstBonus = msg[pos++];
        return pos;
    }

    // Start is called before the first frame update
    void Update()
    {
        NewFrameSettle();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Character>(out Character character))
        {
            CharacterCollision(character);
        }
    }
}
