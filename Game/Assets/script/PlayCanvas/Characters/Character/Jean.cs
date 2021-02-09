using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jean : Hero
{
    Vector2Int[] poseNormal = new Vector2Int[]
    {
        new Vector2Int(-1,-1),new Vector2Int(0,-1),new Vector2Int(1,-1),
        new Vector2Int(-1,0),new Vector2Int(1,0),
        new Vector2Int(-1,1),new Vector2Int(0,1),new Vector2Int(1,1)
    };
    public override void Heroinit()
    {
        if (Inited) return;
        Inited = true;
        Initial("Character_Jean",6,0);
        MaxFreeMove = 2;
        FreeMoveCount = 2;
        heroType = HeroType.Sword;
        element = ElementType.Anemo;

        poses = poseNormal;

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
            "Normal_Heal",
            "Normal_Heal",
            "Normal_Heal",
            "Normal_Anemo",
            "Normal_Anemo",
            "Normal_Anemo",
            "Normal_Anemo"
        };
        Vector2Int[] poseDontNeedTarg = new Vector2Int[]
        {
            new Vector2Int(-1,-1)
        };

        AddUseCard("#+Normal_Attack", NormalAttack, poseNormal);

        AddUseCard("#+Normal_Attack+Normal_Heal", ChargeAttack, poseNormal);
        AddUseCard("#+Normal_Heal+Normal_Attack", ChargeAttack, poseNormal);

        AddUseCard("#+Normal_Attack+Normal_Anemo", Skill, poseNormal);
        AddUseCard("#+Normal_Anemo+Normal_Attack", Skill, poseNormal);

        AddUseCard("#+Normal_Attack+Item_CrystalCore", CrystalSkill, poseNormal);
        AddUseCard("#+Item_CrystalCore+Normal_Attack", CrystalSkill, poseNormal);

        AddUseCard("#+Normal_Burst+Normal_Anemo", Burst, poseDontNeedTarg,false);
        AddUseCard("#+Normal_Anemo+Normal_Burst", Burst, poseDontNeedTarg,false);

        AddUseCard("#+Normal_Burst+Item_CrystalCore", CrystalBurst, poseDontNeedTarg,false);
        AddUseCard("#+Item_CrystalCore+Normal_Burst", CrystalBurst, poseDontNeedTarg,false);

        AddUseCard("#+Normal_Move", Move, posesMove);

        AttackEffects.Add("SkillEffect", SkillEffect);

        HealEffects.Add("BurstHealEffect", BurstHealEffect);
    }
    // Start is called before the first frame update

    public override bool NormalAttack(Vector2Int pos)
    {
        if (stamina < 1)
        {
            return false;
        }
        pos += position;
        GameObject obj = CreatObject<Attack>(parent);
        Attack atk = obj.GetComponent<Attack>();
        atk.Initial(pos, 1, AttackType.NormalAttack, ElementType.Physics, this);
        
        stamina--;
        return true;
    }

    public bool ChargeAttack(Vector2Int pos)
    {
        if (stamina < 1)
        {
            return false;
        }
        pos += position;
        GameObject obj = CreatObject<Attack>(parent);
        Attack atk = obj.GetComponent<Attack>();
        atk.Initial(pos, 1, AttackType.ChargedAttack, ElementType.Physics, this);
        SelfHeal(1, 0);
        stamina--;
        return true;
    }

    public bool Skill(Vector2Int pos)
    {
        if (stamina < 1)
        {
            return false;
        }
        CreateAttack(pos, 2, AttackType.ElementalSkill, ElementType.Anemo);
        stamina--;
        return true;
    }

    public bool CrystalSkill(Vector2Int pos)
    {
        if (stamina < 1)
        {
            return false;
        }
        CreateAttack(pos, 2, AttackType.ElementalSkill, ElementType.Physics);
        stamina--;
        return true;
    }

    public void SkillEffect(Attack atk)
    {
        if(atk.attackOwner == this && atk.type == AttackType.ElementalSkill)
        {
            atk.attackTarget.KickBack(position, 2);
        }
    }

    public bool Burst(Vector2Int pos)
    {
        if (stamina < 1)
        {
            return false;
        }
        pos = position;
        for(int i = -1; i < 2; i++)
        {
            for(int j = -1; j < 2; j++)
            {
                GameObject obj = CreatObject<Attack>(parent);
                Attack atk = obj.GetComponent<Attack>();
                atk.Initial(new Vector2Int(i,j)+pos, 1, AttackType.ElementalBurst, ElementType.Anemo, this);
                
                GameObject obj2 = CreatObject<Heal>(parent);
                Heal heal = obj2.GetComponent<Heal>();
                heal.Initial(new Vector2Int(i, j) + pos, 1, 0, HealType.Jean_Burst, this);
            }
        }
        gameObject.GetComponent<AudioSource>().PlayOneShot(audios.Jean_Burst);
        stamina--;
        return true;
    }

    public bool CrystalBurst(Vector2Int pos)
    {
        if (stamina < 1)
        {
            return false;
        }
        pos = position;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                GameObject obj = CreatObject<Attack>(parent);
                Attack atk = obj.GetComponent<Attack>();
                atk.Initial(new Vector2Int(i, j) + pos, 1, AttackType.ElementalBurst, ElementType.Physics, this);

                GameObject obj2 = CreatObject<Heal>(parent);
                Heal heal = obj2.GetComponent<Heal>();
                heal.Initial(new Vector2Int(i, j) + pos, 1, 0, HealType.Jean_Burst, this);
            }
        }
        gameObject.GetComponent<AudioSource>().PlayOneShot(audios.Jean_Burst);
        stamina--;
        return true;
    }

    public void BurstHealEffect(Heal heal)
    {
        if(heal.healOwner == this && heal.type == HealType.Jean_Burst)
        {
            heal.healTarget.affected = null;
        }
    }

    public override void SelfHeal(int Hp, int Shield)
    {
        Character[] characters = parent.GetComponent<Player>().myCharacters;
        int count = parent.GetComponent<Player>().characterCount;
        bool used = false;
        if (Hp > 0)
        {
            for (int i = 0; i < count; i++)
            {
                if (characters[i].HP < characters[i].MAXHP) { 
                    characters[i].SelfHeal(Hp, 0); 
                    used = true;
                    break;
                }
            }
        }
        if (shield > 0)
        {
            for (int i = 0; i < count; i++)
            {
                if (characters[i].shield < characters[i].MAXShield)
                {
                    characters[i].SelfHeal(0, shield);
                    used = true;
                    break;
                }
            }
        }
        if (used) return;
        base.SelfHeal(Hp, Shield);
    }


    void Start()
    {
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
