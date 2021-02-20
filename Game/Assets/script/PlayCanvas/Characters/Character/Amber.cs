using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amber : Hero
{
    public override void Heroinit()
    {
        if (Inited) return;
        Inited = true;
        Initial("Character_Amber", 40, 1);

        heroType = HeroType.Bow;
        element = ElementType.Pyro;

        poses = posBow;

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
            "Normal_Defence",
            "Normal_Burst",
            "Normal_Pyro",
            "Normal_Pyro",
            "Normal_Pyro",
            "Normal_Pyro",
            "Normal_Pyro"
        };

        NormalEffects.Add("InitMovingEffect", InitMovingEffect);
        NormalEffects.Add("BurstNewRoundBurn", BurstNewRoundBurn);
        NormalEffects.Add("RabbitNewRount", RabbitNewRound);
        DefenceEffects.Add("CheckIsTarget", CheckIsTarget);

        AddUseCard("#+Normal_Attack", NormalAttack, poses);
        AddUseCard("#+Item_Bow", BowNormal, posAll);

        AddUseCard("#+Normal_Attack+Normal_Pyro", ChargeAttack, posAll);
        //AddUseCard("#+Normal_Pyro+Normal_Attack", ChargeAttack, posAll);

        AddUseCard("#+Item_Bow+Normal_Pyro", BowCharge, posAll);
        //AddUseCard("#+Normal_Pyro+Item_Bow", BowCharge, posAll);

        AddUseCard("#+Normal_Attack+Item_Chill", ChargeAttack, posAll);
        //AddUseCard("#+Item_Chill+Normal_Attack", ChargeAttack, posAll);

        AddUseCard("#+Item_Bow+Item_Chill", BowCharge, posAll);
        //AddUseCard("#+Item_Chill+Item_Bow", BowCharge, posAll);

        AddUseCard("#+Normal_Attack+Item_CrystalCore", CrystalCharge, posAll);
        //AddUseCard("#+Item_CrystalCore+Normal_Attack", CrystalCharge, posAll);

        AddUseCard("#+Item_Bow+Item_CrystalCore", BowCrystalCharge, posAll);
        //AddUseCard("#+Item_CrystalCore+Item_Bow", BowCrystalCharge, posAll);

        AddUseCard("#+Normal_Pyro+Normal_Defence", RabbitCreate, poses);
        //AddUseCard("#+Normal_Defence+Normal_Pyro", RabbitCreate, poses);

        AddUseCard("#+Item_Chill+Normal_Defence", RabbitCreate, poses);
        //ddUseCard("#+Normal_Defence+Item_Chill", RabbitCreate, poses);

        AddUseCard("#+Normal_Defence+Item_CrystalCore", CrystalSkill, poses);
        //AddUseCard("#+Item_CrystalCore+Normal_Defence", CrystalSkill, poses);

        AddUseCard("#+Normal_Burst+Normal_Pyro", Burst, poses);
        //AddUseCard("#+Normal_Pyro+Normal_Burst", Burst, poses);

        AddUseCard("#+Normal_Burst+Item_Chill", Burst, poses);
        //AddUseCard("#+Item_Chill+Normal_Burst", Burst, poses);

        AddUseCard("#+Normal_Burst+Item_CrystalCore", CrystalBurst, poses);
        //AddUseCard("#+Item_CrystalCore+Normal_Burst", CrystalBurst, poses);

        AddUseCard("#+Normal_Move", Move, posesMove,CanMove);

    }
    // Start is called before the first frame update
    void Start()
    {
        //Heroinit();
    }

    public bool moved = false;//是否移动
    public Sprite notTargetSprite;

    public Rabbit rabbit = null;

    public void RabbitNewRound()
    {
        if(rabbit != null)
        {
            rabbit.NewRound();
        }
    }

    public void RabbitDestroy()
    {
        if(rabbit != null)
        {
            Destroy(rabbit.gameObject);
            rabbit = null;
        }
    }

    public bool RabbitCreate(Vector2Int pos)
    {
        if (stamina<1 || rabbit != null) return false;
        pos += position;
        GameObject obj = Rabbit.CreatRabbit(parent, pos, this);
        rabbit = obj.GetComponent<Rabbit>();
        gameObject.GetComponent<AudioSource>().PlayOneShot(audios.Amber_Skill);
        return true;
    }

    public void InitMovingEffect()
    {
        moved = false;
        isTarget = true;
    }

    public void CheckIsTarget(Attack atk)//如果不可选中，所有伤害归0，元素清空
    {
        if (!isTarget)
        {
            atk.Damage = 0;
            atk.attackelemental = ElementType.Physics;
        }
    }

    public bool CanMove()
    {
        return true;
    }

    public override bool Move(Vector2Int pos)//移动被动
    {
        pos += position;
        MoveTo(pos);
        if (!moved)
        {
            moved = true;
            isTarget = false;
        }
        else
        {
            isTarget = true;
        }
        return true;
    }

    public override bool NormalAttack(Vector2Int pos)
    {
        if (stamina < 1)
        {
            return false;
        }
        
        CreateAttack(pos, 10, AttackType.NormalAttack, ElementType.Physics);

        stamina--;
        return true;
    }


    public bool ChargeAttack(Vector2Int pos)
    {
        if (stamina < 1)
        {
            return false;
        }
        CreateAttack(pos, 20, AttackType.ChargedAttack, ElementType.Pyro);
        stamina--;
        return true;
    }


    public void BurstNewRoundBurn()
    {
        if (burstburn != null)
        {
            burstburn.newRound();
        }
    }
    public AmberBurst burstburn=null;
    public void CreateBurstBurn(Vector2Int pos)
    {
        if (burstburn != null) DestroyBurstBurn();
        burstburn =  AmberBurst.CreateSelf(pos, this);
    }

    public void DestroyBurstBurn()
    {
        if (burstburn == null) return;
        Destroy(burstburn.gameObject);
        burstburn = null;
    }

    public bool Burst(Vector2Int pos)
    {
        if (stamina < 1 || burstburn!=null) return false;
        pos += position;
        BurstBurn(pos);
        CreateBurstBurn(pos);
        stamina--;
        gameObject.GetComponent<AudioSource>().PlayOneShot(audios.Amber_Burst);
        return true;
    }

    public void BurstBurn(Vector2Int pos)
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                GameObject obj = CreatObject<Attack>(parent);
                Attack atk = obj.GetComponent<Attack>();
                atk.Initial(new Vector2Int(i, j) + pos, 10, AttackType.ElementalBurst, ElementType.Pyro, this);
                atk.transform.position = BattleArea.GetLocalPosition(pos);
            }
        }
    }

    public bool CrystalCharge(Vector2Int pos)
    {
        if (stamina < 1)
        {
            return false;
        }
        CreateAttack(pos, 20, AttackType.ChargedAttack, ElementType.Physics);
        stamina--;
        return true;
    }

    public bool CrystalSkill(Vector2Int pos)
    {
        return RabbitCreate(pos);
    }

    public bool CrystalBurst(Vector2Int pos)
    {
        return Burst(pos);
    }

    public bool BowNormal(Vector2Int pos)
    {
        if (stamina < 1)
        {
            return false;
        }

        CreateAttack(pos, 20, AttackType.NormalAttack, ElementType.Physics);

        stamina--;
        return true;
    }

    public bool BowCharge(Vector2Int pos)
    {
        if (stamina < 1)
        {
            return false;
        }
        CreateAttack(pos, 30, AttackType.ChargedAttack, ElementType.Pyro);
        stamina--;
        return true;
    }

    public bool BowCrystalCharge(Vector2Int pos)
    {
        if (stamina < 1)
        {
            return false;
        }
        CreateAttack(pos, 30, AttackType.ChargedAttack, ElementType.Physics);
        stamina--;
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        NewFrameSettle();
    }

    public override string StringGet()
    {

        string rmsg;
        if (rabbit != null)
        {
            rmsg = (char)1 + rabbit.StringGet();
        }
        else
        {
            rmsg = ""+(char)0;
        }
        string bmsg;
        if (burstburn != null)
        {
            bmsg = (char)1 + burstburn.StringGet();
        }
        else
        {
            bmsg = ""+(char)0;
        }
        return base.StringGet()+rmsg+bmsg;
    }

    public override int StringSet(string msg,int pos)
    {
        pos = base.StringSet(msg,pos);
        if (msg[pos++] == 1)
        {
            GameObject obj = Rabbit.CreatRabbit(parent, new Vector2Int(msg[pos], msg[pos+1]), this);
            rabbit = obj.GetComponent<Rabbit>();
            pos = rabbit.StringSet(msg,pos);
        }
        if (msg[pos++] == 1)
        {
            burstburn = AmberBurst.CreateSelf(new Vector2Int(msg[pos], msg[pos+1]),this);
            pos = burstburn.StringSet(msg,pos);
        }
        return pos;
    }

    public override void ShowNormalState()
    {
        base.ShowNormalState();
        int i = 0;
        if (!isTarget)
        {
            AddImgOfNormalState(sprites.GetComponent<AllSprites>().Buff_Move, new Vector3(i * 1.5f, 0.6f, 0), new Vector3(1.6f, 1.0f, 1));
            i++;
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (rabbit != null)
        {
            Destroy(rabbit.gameObject);
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
