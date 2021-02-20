using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lisa : Hero
{
    public override void Heroinit()
    {
        if (Inited) return;
        Inited = true;
        Initial("Character_Lisa", 40, 1);
        poses = posMage;
        heroType = HeroType.Mage;
        element = ElementType.Electro;

        BasicCardsCount = 15;
        BasicCards = new string[15]
        {
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
            "Normal_Electro",
            "Normal_Electro",
            "Normal_Electro",
            "Normal_Electro",
            "Normal_Electro"
        };
        Vector2Int[] posdontneedtarg = new Vector2Int[]
        {
            new Vector2Int(-1,-1)
        };
        AttackEffects.Add("CheckIsElectroAffect", CheckIsElectroAffect);
        AttackEffects.Add("ChargedAttackEffect", ChargedAttackEffect);

        NormalEffects.Add("RoseNewRound", RoseNewRound);

        AddUseCard("#+Normal_Move", Move, posesMove);

        AddUseCard("#+Normal_Electro", NormalAttack, poses);
        AddUseCard("#+Item_CrystalCore", CrystalNormal, poses);

        AddUseCard("#+Normal_Attack+Normal_Electro", ChargedAttack, poses);
       // AddUseCard("#+Normal_Electro+Normal_Attack", ChargedAttack, poses);

        AddUseCard("#+Normal_Attack+Item_CrystalCore", CrystalCharged, poses);
        //AddUseCard("#+Item_CrystalCore+Normal_Attack", CrystalCharged, poses);

        AddUseCard("#+Normal_Electro+Normal_Electro", Skill, posdontneedtarg,false);
        
        AddUseCard("#+Normal_Electro+Item_CrystalCore", CrystalSkill, posdontneedtarg, false);
        //AddUseCard("#+Item_CrystalCore+Normal_Electro", CrystalSkill, posdontneedtarg, false);

        AddUseCard("#+Item_CrystalCore+Item_CrystalCore", CrystalSkill, posdontneedtarg, false);

        AddUseCard("#+Normal_Burst+Normal_Electro", Burst, poses);
        //AddUseCard("#+Normal_Electro+Normal_Burst", Burst, poses);

        AddUseCard("#+Normal_Burst+Item_CrystalCore", CrystalBurst, poses);
        //AddUseCard("#+Item_CrystalCore+Normal_Burst", CrystalBurst, poses);
    }

    public void CheckIsElectroAffect(Attack atk)
    {
        if(atk.attackOwner == this && atk.attackTarget.affected !=null && atk.attackTarget.affected.affectElemental == ElementType.Electro)
        {
            atk.Damage+=10;
        }
    }

    public override bool NormalAttack(Vector2Int pos)
    {
        if (stamina < 1) return false;
        CreateAttack(pos, 10, AttackType.NormalAttack, ElementType.Electro);
        stamina--;
        return true;
    }

    public bool CrystalNormal(Vector2Int pos)
    {
        if (stamina < 1) return false;
        CreateAttack(pos, 10, AttackType.NormalAttack, ElementType.Physics);
        stamina--;
        return true;
    }

    public void ChargedAttackEffect(Attack atk)
    {
        if(atk.attackOwner==this && atk.type == AttackType.ChargedAttack)
        {
            Vector2Int pos = BattleArea.GetReverse( atk.attackTarget.position);
            Character[] characters = parent.GetComponent<Player>().GetEnemyCharacters();
            bool getPos=false;
            Vector2Int targ = new Vector2Int(0,0);
            foreach(Character character in characters)
            {
                Vector2Int tpos =BattleArea.GetReverse( character.position);
                if ((tpos - pos).sqrMagnitude <2.9f  && (tpos - pos).sqrMagnitude>0.2f)
                {
                    getPos = true;
                    targ = tpos;
                    break;
                }
            }
            if (getPos)
            {
                Attack.CreateAttack(parent, targ, 10, AttackType.NormalAttack, ElementType.Electro, this).transform.localPosition = atk.transform.localPosition;
            }
        }
    }

    public override bool ChargedAttack(Vector2Int targ)
    {
        if (stamina < 1) return false;
        CreateAttack(targ, 20, AttackType.ChargedAttack, ElementType.Electro);
        stamina--;
        return true;
    }

    public bool CrystalCharged(Vector2Int targ)
    {
        if (stamina < 1) return false;
        CreateAttack(targ, 20, AttackType.ChargedAttack, ElementType.Physics);
        stamina--;
        return true;
    }


    public bool Skill(Vector2Int pos)
    {
        if (stamina < 1) return false;

        for(int i = -3; i < 4; i++)
        {
            for(int j = Abs(i) - 3; j < 4 - Abs(i); j++)
            {
                CreateAttack(new Vector2Int(j, i), 10, AttackType.ElementalSkill, ElementType.Electro);
            }
        }
        gameObject.GetComponent<AudioSource>().PlayOneShot(audios.Lisa_Skill);
        stamina--;
        return true;
    }

    public bool CrystalSkill(Vector2Int pos)
    {
        if (stamina < 1) return false;

        for (int i = -3; i < 4; i++)
        {
            for (int j = Abs(i)-3; j < 4 - Abs(i); j++)
            {
                CreateAttack(new Vector2Int(j, i), 10, AttackType.ElementalSkill, ElementType.Physics);
            }
        }
        gameObject.GetComponent<AudioSource>().PlayOneShot(audios.Lisa_Skill);
        stamina--;
        return true;
    }

    public Rose rose=null;

    public bool CreateRose(Vector2Int pos)
    {
        if (rose != null) return false;
        GameObject obj = Rose.CreatRose(pos + position, this);
        rose = obj.GetComponent<Rose>();
        return true;
    }

    public void DestroyRose()
    {
        if (rose != null)
        {
            Destroy(rose.gameObject);
            rose = null;
        }
    }

    public void RoseNewRound()
    {
        if (rose != null)
        {
            rose.newRound();
        }
    }

    public bool Burst(Vector2Int pos)
    {
        if (stamina < 1) return false;
        if (!CreateRose(pos)) return false;
        gameObject.GetComponent<AudioSource>().PlayOneShot(audios.Lisa_Burst);
        stamina--;
        return true;
    }

    public bool CrystalBurst(Vector2Int pos)
    {
        return Burst(pos);
    }

    public override string StringGet()
    {
        string rm;
        if (rose != null)
        {
            rm = (char)1 + rose.StringGet();
        }
        else
        {
            rm = ""+(char)0;
        }
        return base.StringGet()+rm;
    }

    public override int StringSet(string msg,int pos)
    {
        pos = base.StringSet(msg,pos);
        if (msg[pos ++] == 1)
        {
            rose = Rose.CreatRose(new Vector2Int(msg[pos], msg[pos+1]),this).GetComponent<Rose>();
            pos = rose.StringSet(msg,pos);
        }
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

    public override void OnDestroy()
    {
        base.OnDestroy();
        DestroyRose();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Character>(out Character character))
        {
            CharacterCollision(character);
        }
    }
}
