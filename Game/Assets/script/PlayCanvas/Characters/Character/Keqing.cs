using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keqing : Hero
{
    public override void Heroinit()
    {
        if (Inited) return;
        Inited = true;
        MaxFreeMove = 2;
        FreeMoveCount = 2;
        Initial("Character_Keqing", 4, 1);
        heroType = HeroType.Sword;
        element = ElementType.Electro;

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
            "Normal_Burst",
            "Normal_Burst",
            "Normal_Electro",
            "Normal_Electro",
            "Normal_Electro",
            "Normal_Electro",
            "Normal_Electro",
            "Normal_Electro"
        };

        NormalEffects.Add("DeBurst", DeBurst);
        poses = posSword;
        AddUseCard("#+Normal_Move", Move,posesMove);

        AddUseCard("#+Normal_Attack", NormalAttack, poses);

        AddUseCard("#+Normal_Attack+Normal_Electro", Skill, posSkill);
        AddUseCard("#+Normal_Electro+Normal_Attack", Skill, posSkill);

        AddUseCard("#+Item_CrystalCore+Normal_Attack", CrystalSkill, posSkill);
        AddUseCard("#+Normal_Attack+Item_CrystalCore", CrystalSkill, posSkill);

        AddUseCard("#+Normal_Burst+Normal_Electro", Burst, poses);
        AddUseCard("#+Normal_Electro+Normal_Burst", Burst, poses);

        AddUseCard("#+Normal_Burst+Item_CrystalCore", CrystalBurst, poses);
        AddUseCard("#+Item_CrystalCore+Normal_Burst", CrystalBurst, poses);

        AddUseCard("#+Normal_Electro", ActionBonus, poses,ElementCanUse);

        AddUseCard("#+Item_CrystalCore", CrystalActionBonus, poses,ElementCanUse);

    }

    public bool isAfterAction = false;

    public bool ElementCanUse()
    {
        return isAfterAction;
    }

    public override bool Move(Vector2Int pos)
    {
        if (base.Move(pos))
        {
            isAfterAction = true;
            ShowNormalState();
            return true;
        }
        return false;
    }

    public override bool NormalAttack(Vector2Int targ)
    {
        if (stamina < 1) return false;
        CreateAttack(targ, 1, AttackType.NormalAttack, ElementType.Physics);
        isAfterAction = true;
        ShowNormalState();
        stamina--;
        return true;
    }

    public Vector2Int[] posSkill = new Vector2Int[]
    {
        new Vector2Int(0,2),
        new Vector2Int(-1,1),new Vector2Int(0,1),new Vector2Int(1,1),
        new Vector2Int(-2,0),new Vector2Int(-1,0),new Vector2Int(1,0),new Vector2Int(2,0),
        new Vector2Int(-1,-1),new Vector2Int(0,-1),new Vector2Int(1,-1),
        new Vector2Int(0,-2),
    };

    public bool Skill(Vector2Int pos)
    {
        if (stamina < 1) return false;
        Vector2Int targ = pos + position;
        MoveTo(targ);
        Vector2 dis = pos;
        dis.Normalize();
        transform.localPosition = new Vector3(targ.x - 3.5f-dis.x*0.5f, targ.y - 3.5f-dis.y*0.5f, -1);
        CreateAttack(new Vector2Int(1, 0), 1, AttackType.ElementalSkill, ElementType.Electro);
        CreateAttack(new Vector2Int(-1, 0), 1, AttackType.ElementalSkill, ElementType.Electro);
        CreateAttack(new Vector2Int(0, 1), 1, AttackType.ElementalSkill, ElementType.Electro);
        CreateAttack(new Vector2Int(0, -1), 1, AttackType.ElementalSkill, ElementType.Electro);
        CreateAttack(new Vector2Int(0, 0), 1, AttackType.ElementalSkill, ElementType.Electro);
        isAfterAction = true;
        ShowNormalState();
        gameObject.GetComponent<AudioSource>().PlayOneShot(audios.Keqing_Skill);
        stamina--;
        return true;
    }

    public bool CrystalSkill(Vector2Int pos)
    {
        if (stamina < 1) return false;
        Vector2Int targ = pos + position;
        MoveTo(targ);
        transform.localPosition = new Vector3(targ.x - 3.5f, targ.y - 3.5f, -1);
        CreateAttack(new Vector2Int(1, 0), 1, AttackType.ElementalSkill, ElementType.Physics);
        CreateAttack(new Vector2Int(-1, 0), 1, AttackType.ElementalSkill, ElementType.Physics);
        CreateAttack(new Vector2Int(0, 1), 1, AttackType.ElementalSkill, ElementType.Physics);
        CreateAttack(new Vector2Int(0, -1), 1, AttackType.ElementalSkill, ElementType.Physics);
        CreateAttack(new Vector2Int(0, 0), 1, AttackType.ElementalSkill, ElementType.Physics);
        isAfterAction = true;
        gameObject.GetComponent<AudioSource>().PlayOneShot(audios.Keqing_Skill);
        ShowNormalState();
        stamina--;
        return true;
    }

    public bool isBurstBonus = false;
    public bool Burst(Vector2Int pos)
    {
        if (stamina < 1) return false;
        isBurstBonus = true;
        ShowNormalState();
        isTarget = false;
        for(int i = -1; i < 2; i++)
        {
            for(int j = -1; j < 2; j++)
            {
                CreateAttack(new Vector2Int(j, i), 1, AttackType.ElementalBurst, ElementType.Electro);
            }
        }
        gameObject.GetComponent<AudioSource>().PlayOneShot(audios.Keqing_Burst);
        stamina--;
        return true;
    }

    public bool CrystalBurst(Vector2Int pos)
    {
        if (stamina < 1) return false;
        isBurstBonus = true;
        ShowNormalState();
        isTarget = false;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                CreateAttack(new Vector2Int(j, i), 1, AttackType.ElementalBurst, ElementType.Physics);
            }
        }
        gameObject.GetComponent<AudioSource>().PlayOneShot(audios.Keqing_Burst);
        stamina--;
        return true;
    }

    public void DeBurst()
    {
        if (isBurstBonus)
        {
            isBurstBonus = false;
            isTarget = true;
            ShowNormalState();
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    CreateAttack(new Vector2Int(j, i), 1, AttackType.ElementalBurst, ElementType.Electro);
                }
            }
        }
    }

    public bool ActionBonus(Vector2Int pos)
    {
        if (!isAfterAction) return false;
        isAfterAction = false;
        ShowNormalState();
        CreateAttack(pos, 1, AttackType.ChargedAttack, ElementType.Electro);
        return true;
    }

    public bool CrystalActionBonus(Vector2Int pos)
    {
        if (!isAfterAction) return false;
        isAfterAction = false;
        CreateAttack(pos, 1, AttackType.ChargedAttack, ElementType.Physics);
        return true;
    }

    public override void ShowNormalState()
    {
        base.ShowNormalState();
        int i = 0;
        if (isAfterAction)
        {
            AddImgOfNormalState(sprites.GetComponent<AllSprites>().Buff_Electro, new Vector3(i * 1.5f, 0.6f, 0), new Vector3(1.6f, 1.0f, 1));
            i++;
        }
        if (isBurstBonus)
        {
            AddImgOfNormalState(sprites.GetComponent<AllSprites>().Buff_Move, new Vector3(i * 1.5f, 0.6f, 0), new Vector3(1.6f, 1.0f, 1));
            i++;
        }
    }

    public override string StringGet()
    {
        string km="";
        if (isAfterAction)
        {
            km += (char)1;
        }
        else km += (char)0;
        if (isBurstBonus) km += (char)1;
        else km += (char)0;
        return base.StringGet()+km;
    }

    public override int StringSet(string msg,int pos)
    {
        pos = base.StringSet(msg,pos);
        if (msg[pos ++] == 1) isAfterAction = true;
        if (msg[pos ++] == 1) isBurstBonus = true;
        return pos;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Heroinit();
    }

    // Update is called once per frame
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
