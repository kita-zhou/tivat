using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diluc : Hero
{
    public override void Heroinit()
    {
        if (Inited) return;
        Inited = true;
        Initial("Character_Diluc", 50, 1);
        MaxFreeMove = 2;
        FreeMoveCount = 2;
        heroType = HeroType.Claymore;
        element = ElementType.Pyro;

        poses = posClayMore;

        BasicCardsCount = 15;
        BasicCards = new string[15]
        {
            "Normal_Attack",
            "Normal_Attack",
            "Normal_Attack",
            "Normal_Attack",
            "Normal_Attack",
            "Normal_Attack",
            "Normal_Attack",
            "Normal_Attack",
            "Normal_Burst",
            "Normal_Burst",
            "Normal_Pyro",
            "Normal_Pyro",
            "Normal_Pyro",
            "Normal_Pyro",
            "Normal_Pyro"
        };

        Vector2Int[] poseDontNeedTarg = new Vector2Int[]
        {
            new Vector2Int(-1,-1)
        };

        NormalEffects.Add("BonusCountDelete", BonusCountDelete);

        AttackEffects.Add("ChargeAttackEffect", ChargedAttackEffect);

        AddUseCard("#+Normal_Attack", NormalAttack, poses);

        AddUseCard("#+Normal_Attack+Normal_Attack", ChargeAttack, poses);

        AddUseCard("#+Normal_Attack+Item_Claymore", ClaymoreCharge, poses);
        AddUseCard("#+Item_Claymore+Normal_Attack", ClaymoreCharge, poses);

        AddUseCard("#+Normal_Attack+Normal_Pyro", Skill, poses);
        AddUseCard("#+Normal_Pyro+Normal_Attack", Skill, poses);

        AddUseCard("#+Normal_Attack+Item_Chill", Skill, poses);
        AddUseCard("#+Item_Chill+Normal_Attack", Skill, poses);

        AddUseCard("#+Normal_Pyro", SkillBonusAttack, poses,SkillBonusCanUse);
        AddUseCard("#+Item_Chill", SkillBonusAttack, poses,SkillBonusCanUse);
        AddUseCard("#+Item_CrystalCore", CrystalSkillBnousAttack, poses, SkillBonusCanUse);

        AddUseCard("#+Normal_Attack+Item_CrystalCore", CrystalSkill, poses);
        AddUseCard("#+Item_CrystalCore+Normal_Attack", CrystalSkill, poses);

        AddUseCard("#+Normal_Burst+Normal_Pyro", Burst, poseDontNeedTarg,false);
        AddUseCard("#+Normal_Pyro+Normal_Burst", Burst, poseDontNeedTarg,false);

        AddUseCard("#+Normal_Burst+Item_Chill", Burst, poseDontNeedTarg, false);
        AddUseCard("#+Item_Chill+Normal_Burst", Burst, poseDontNeedTarg, false);

        AddUseCard("#+Normal_Burst+Item_CrystalCore", CrystalBurst, poseDontNeedTarg, false);
        AddUseCard("#+Item_CrystalCore+Normal_Burst", CrystalBurst, poseDontNeedTarg, false);

        AddUseCard("#+Normal_Move", Move, posesMove);

    }
    // Start is called before the first frame update
    public int AttackUsedCount=0;
    public int PyroUsedCount=0;

    public override bool NormalAttack(Vector2Int pos)
    {
        if (stamina < 1) return false;

        int damage = 10;
        if(PyroUsedCount >= 3)
        {
            damage+=10;
            PyroUsedCount = 0;
        }
        ElementType elementType = ElementType.Physics;
        if (BurstBonusCount > 0)
        {
            elementType = ElementType.Pyro;
            damage+=10;
        }
        CreateAttack(pos, damage,  AttackType.NormalAttack, elementType);
        AttackUsedCount++;
        stamina--;
        ShowNormalState();
        return true;
    }

    public bool ChargeAttack(Vector2Int pos)
    {
        if (stamina < 1) return false;
        int damage = 20;
        if (PyroUsedCount >= 3)
        {
            damage+=10;
            PyroUsedCount = 0;
        }

        ElementType elementType = ElementType.Physics;
        if (BurstBonusCount > 0)
        {
            elementType = ElementType.Pyro;
            damage+=10;
        }

        CreateAttack(pos, damage, AttackType.ChargedAttack, elementType);
        AttackUsedCount += 2;
        stamina--;
        ShowNormalState();
        return true;
    }

    public bool ClaymoreCharge(Vector2Int pos)
    {
        return ChargeAttack(pos);
    }

    public void ChargedAttackEffect(Attack atk)
    {
        if(atk.attackOwner == this && atk.type == AttackType.ChargedAttack)
        {
            atk.attackTarget.KickBack(position, 1);
        }
    }

    public int SkillBonusCount = 0;
    public Vector2Int SkillBonusPos;
    public bool Skill(Vector2Int pos)
    {
        if (stamina < 1) return false;
        int damage = 20;
        if (AttackUsedCount >= 3)
        {
            damage+=10;
            AttackUsedCount = 0;
        }
        CreateAttack(pos, damage, AttackType.ElementalSkill, ElementType.Pyro);
        SkillBonusPos = pos + position;
        SkillBonusCount = 2;
        AttackUsedCount ++;
        PyroUsedCount++;
        stamina--;
        gameObject.GetComponent<AudioSource>().PlayOneShot(audios.Diluc_Skill);
        ShowNormalState();
        return true;
    }

    public bool CrystalSkill(Vector2Int pos)
    {
        if (stamina < 1) return false;
        int damage = 20;
        if (AttackUsedCount >= 3)
        {
            damage+=10;
            AttackUsedCount = 0;
        }
        CreateAttack(pos, damage, AttackType.ElementalSkill, ElementType.Physics);
        SkillBonusPos = pos + position;
        SkillBonusCount = 2;
        AttackUsedCount++;
        stamina--;
        gameObject.GetComponent<AudioSource>().PlayOneShot(audios.Diluc_Skill);
        ShowNormalState();
        return true;
    }

    public bool SkillBonusCanUse()
    {
        if (SkillBonusCount > 0) return true;
        return false;
    }
    public bool SkillBonusAttack(Vector2Int pos)
    {
        if (SkillBonusCount < 1) return false;
        SkillBonusCount--;
        CreateAttack(SkillBonusPos - position, 10, AttackType.ElementalSkill, ElementType.Pyro);
        PyroUsedCount++;
        ShowNormalState();
        return true;
    }

    public bool CrystalSkillBnousAttack(Vector2Int pos)
    {
        if (SkillBonusCount < 1) return false;
        SkillBonusCount--;
        ShowNormalState();
        CreateAttack(SkillBonusPos - position, 10, AttackType.ElementalSkill, ElementType.Physics);
        return true;
    }

    public int BurstBonusCount = 0;

    public override void ShowNormalState()
    {
        base.ShowNormalState();
        int i = 0;
        if (SkillBonusCount > 0)
        {
            AddImgOfNormalState(sprites.GetComponent<AllSprites>().Buff_Pyro, new Vector3(i * 1.5f, 0.6f, 0), new Vector3(1.6f, 1.0f, 1));
            i++;
        }
        if (BurstBonusCount > 0)
        {
            AddImgOfNormalState(sprites.GetComponent<AllSprites>().Buff_Attack_Add, new Vector3(i * 1.5f, 0.6f, 0), new Vector3(1.6f, 1.0f, 1));
            i++;
        }
        if (AttackUsedCount >= 3)
        {
            AddImgOfNormalState(sprites.GetComponent<AllSprites>().Buff_Cert_Hunt, new Vector3(i * 1.5f, 0.6f, 0), new Vector3(1.6f, 1.0f, 1));
            i++;
        }
        if (PyroUsedCount >= 3)
        {
            AddImgOfNormalState(sprites.GetComponent<AllSprites>().Buff_Cert_Rate, new Vector3(i * 1.5f, 0.6f, 0), new Vector3(1.6f, 1.0f, 1));
            i++;
        }
    }

    public void BonusCountDelete()
    {
        SkillBonusCount = 0;
        if(BurstBonusCount>0)
            BurstBonusCount--;
    }

    public bool Burst(Vector2Int pos)
    {
        if (stamina < 1) return false;
        int damage = 20;
        if (AttackUsedCount >= 3)
        {
            damage+=10;
            AttackUsedCount = 0;
        }
        for(int i = 1; i < 4; i++)
        {
            CreateAttack(new Vector2Int(0,i), damage, AttackType.ElementalBurst, ElementType.Pyro);
        }
        BurstBonusCount = 3;
        PyroUsedCount++;
        gameObject.GetComponent<AudioSource>().PlayOneShot(audios.Diluc_Burst);
        return true;
    }

    public bool CrystalBurst(Vector2Int pos)
    {
        if (stamina < 1) return false;
        int damage = 20;
        if (AttackUsedCount >= 3)
        {
            damage+=10;
            AttackUsedCount = 0;
        }
        for (int i = 1; i < 4; i++) 
        {
            CreateAttack( new Vector2Int(0, i), damage, AttackType.ElementalBurst, ElementType.Physics);
        }
        BurstBonusCount = 3;
        gameObject.GetComponent<AudioSource>().PlayOneShot(audios.Diluc_Burst);
        return true;
    }

    public override string StringGet()
    {
        return base.StringGet()+(char)AttackUsedCount+(char)PyroUsedCount+(char)BurstBonusCount+(char)SkillBonusCount+(char)SkillBonusPos.x+(char)SkillBonusPos.y;
    }

    public override int StringSet(string msg,int pos)
    {
        pos =  base.StringSet(msg,pos);
        AttackUsedCount = msg[pos++];
        PyroUsedCount = msg[pos++];
        BurstBonusCount = msg[pos++];
        SkillBonusCount = msg[pos++];
        SkillBonusPos = new Vector2Int(msg[pos++], msg[pos++]);
        return pos;
    }

    void Start()
    {
        //Heroinit();
    }

    // Update is called once per frame
    void Update()
    {
        NewFrameSettle();
    }
}
