using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public enum CharacterName
{
    Character_Ningguang,
    Character_Lisa,
    Character_Diluc,
    Character_Amber,
    Character_Noelle,
    Character_Keqing,
    Character_Jean
}

public class Character : GameBase
{
    public virtual void Initial(string str, int maxhp, int maxshield)//初始化
    {
        sprites = GameObject.Find("Sprites");
        audios = GameObject.Find("Audio").GetComponent<AllAudio>();
        characterName = str;
        type = (CharacterName)System.Enum.Parse(typeof(CharacterName), str);
        MAXHP = maxhp;
        MAXShield = maxshield;
        HP = maxhp;
        shield = 0;
        state = CharacterState.Alive;
        affected = null;

        MAXStamina = 1;
        stamina = 1;
        ShowNormalState();
        InitBaseUseCard();
    }

    public CharacterName type;

    public override void Initial()
    {

    }

    public void InitBaseUseCard()
    {
        NormalEffects.Add("SpecialStateSettle", SpecialStateSettle);
        NormalEffects.Add("_InitStamina", InitStamina);
        NormalEffects.Add("UpdateFreeMoveCount", UpdateFreeMoveCount);

        normal = new UseCard();
        charge = new UseCard();

        normal.postions = poses;
        normal.func = NormalAttack;

        charge.postions = poses;
        charge.func = ChargedAttack;

        UsingCardDic.Add("NormalAttack", normal);
        UsingCardDic.Add("ChargedAttack", charge);

        AddUseCard("#+FreeMove", FreeMove, posesMove, CanFreeMove);
    }

    public virtual void InitStamina()//体力初始化,护盾衰减
    {
        stamina = MAXStamina;
        if (shield > 0)
        {
            int reduce = shield / 2;
            if (reduce == 0) reduce = 1;
            shield -= reduce;
        }
    }
    public AllAudio audios;
    public string characterName;//名字
    public Vector2Int position;//位置
    public Vector2Int oldPosition;

    public ElementType element = ElementType.Physics;

    public int MAXHP;//最大血量
    public int MAXShield;//最大护盾值
    public int HP;//血量
    public int shield;//护盾值

    public int MAXStamina;//最大体力
    public int stamina;//体力

    public CharacterState state;//状态
    public int stateTimeRemain;//状态剩余时间

    public ElementalAffect affected;//元素附着

    public bool isTarget = true;//是否可被选中

    public SortedDictionary<string, UseCard> UsingCardDic = new SortedDictionary<string, UseCard>();//使用卡牌时调用的函数

    public SortedDictionary<string, DefEff> DefenceEffects = new SortedDictionary<string, DefEff>();//被攻击时的特效
    public SortedDictionary<string, AtkEff> AttackEffects =  new SortedDictionary<string, AtkEff>();//攻击时的特效

    public SortedDictionary<string, HealEff> HealEffects = new SortedDictionary<string, HealEff>();//治疗时的特效
    public SortedDictionary<string, HealEff> HealTargetEffects = new SortedDictionary<string, HealEff>();//被治疗时的特效

    public SortedDictionary<string, NorEff> NormalEffects = new SortedDictionary<string, NorEff>();//每回合定期执行的被动效果

    public SortedDictionary<string, NorEff> NormalEffectsPerFrame = new SortedDictionary<string, NorEff>();//每帧执行的效果

    public void AddUseCard(string name, UseFunc function, Vector2Int[] poses,ComfirmUse comfirm,bool needTag=true)
    {
        UseCard useCard = new UseCard
        {
            func = function,
            postions = poses,
            CanUse=comfirm,
            needTarget = needTag
        };

        UsingCardDic.Add(name, useCard);

    }

    public void AddUseCard(string name, UseFunc function, Vector2Int[] poses,bool needTag=true)
    {
        AddUseCard(name, function, poses, DefaultCanUse,needTag);
    }

    public GameObject normalState;

    public void Log(string msg)
    {
        if (parent.TryGetComponent<Player>(out Player player)) player.Log(msg);
    }

    public GameObject AddImgOfNormalState(Sprite sprite, Vector3 pos, Vector3 Scale)
    {
        GameObject obj = new GameObject();
        obj.transform.parent = normalState.transform;
        obj.transform.localScale = Scale;
        obj.transform.localRotation = new Quaternion(0, 0, 0, 0);
        obj.AddComponent<SpriteRenderer>().sprite = sprite;
        obj.transform.localPosition = pos;

        return obj;
    }
    public virtual void ShowNormalState()
    {
        if (normalState != null) DeleteNormalState();

        float scale = 6.0f / (HP + shield);
        if (scale > 1) scale = 1;

        normalState = new GameObject();
        normalState.transform.parent = transform;
        normalState.transform.localPosition = new Vector3(-(0.35f + scale * 0.075f), -(0.4f), -3);
        normalState.transform.localRotation = new Quaternion(0, 0, 0, 0);
        normalState.transform.localScale = new Vector3(0.15f, 0.25f, 1);

        Sprite HpBar = BattleArea.sprites.Bar_Red;

        if (parent!=null && parent.TryGetComponent(out Player player) && player.isPlayer) HpBar = BattleArea.sprites.Bar_Green;

        for (int i = 0; i < HP; i++)
        {
            GameObject obj = new GameObject();
            obj.transform.parent = normalState.transform;
            obj.transform.localScale = new Vector3(scale, 1, 1);
            obj.transform.localRotation = new Quaternion(0, 0, 0, 0);
            obj.AddComponent<SpriteRenderer>().sprite = HpBar;
            obj.transform.localPosition = new Vector3((i+0.5f)*scale, 0);
        }
        for (int i = 0; i < shield; i++)
        {
            GameObject obj = new GameObject();
            obj.transform.parent = normalState.transform;
            obj.transform.localScale = new Vector3(scale, 1, 1);
            obj.transform.localRotation = new Quaternion(0, 0, 0, 0);
            obj.AddComponent<SpriteRenderer>().sprite = sprites.GetComponent<AllSprites>().Bar_Yellow;
            obj.transform.localPosition = new Vector3((i + HP+0.5f)*scale, 0);
        }
        

        if (affected != null && affected.affectElemental != ElementType.Physics)
        {
            GameObject obj = new GameObject();
            obj.transform.parent = normalState.transform;
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.transform.localRotation = new Quaternion(0, 0, 0, 0);
            Sprite sprite = null;
            switch (affected.affectElemental)
            {
                case ElementType.Anemo:
                    sprite = sprites.GetComponent<AllSprites>().Buff_Elemental_Anemo;
                    break;
                case ElementType.Cryo:
                    sprite = sprites.GetComponent<AllSprites>().Buff_Elemental_Cryo;
                    break;
                case ElementType.Dendro:
                    sprite = sprites.GetComponent<AllSprites>().Buff_Elemental_Dendro;
                    break;
                case ElementType.Electro:
                    sprite = sprites.GetComponent<AllSprites>().Buff_Elemental_Electro;
                    break;
                case ElementType.Geo:
                    sprite = sprites.GetComponent<AllSprites>().Buff_Elemental_Geo;
                    break;
                case ElementType.Hydro:
                    sprite = sprites.GetComponent<AllSprites>().Buff_Elemental_Hydro;
                    break;
                case ElementType.Pyro:
                    sprite = sprites.GetComponent<AllSprites>().Buff_Elemental_Pyro;
                    break;
                default:
                    break;
            }
            obj.AddComponent<SpriteRenderer>().sprite = sprite;
            obj.transform.localPosition = new Vector3(2.66f, 3.6f);
            obj.transform.localScale = new Vector3(2.48f, 1.5f, 0);
        }
    }

    public virtual void DeleteNormalState()
    {
        if (normalState == null) return;
        int count = normalState.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            Destroy(normalState.transform.GetChild(i).gameObject);
        }
        Destroy(normalState);
        normalState = null;
    }

    public virtual void Init(Character other)
    {
        sprites = other.sprites;
        appearance = other.appearance;
        Area = other.Area;
        characterName = other.characterName;
        MAXHP = other.MAXHP;
        MAXShield = other.MAXShield;
        MAXStamina = other.MAXStamina;
        HP = MAXHP;
        shield = 0;
        stamina = 0;
        state = CharacterState.Alive;
        stateTimeRemain = 0;
        affected = null;
    }

    public virtual void OnAttack(Attack atk)//受到攻击
    {
        atk.attackTarget = this;
        if (atk.attackOwner != null)
        {
            List<AtkEff> effs = new List<AtkEff>(atk.attackOwner.AttackEffects.Values);
            foreach (AtkEff eff in effs)
            {
                eff(atk);
            }

        }
            List<DefEff> defs = new List<DefEff>(DefenceEffects.Values);
            foreach (DefEff eff in defs)
            {
                eff(atk);
            }
        
        if(atk.attackOwner!=null) atk.attackOwner.AttackSettle(atk);
        DefenceSettle(atk);
    }

    public virtual void OnHeal(Heal heal)//受到治疗
    {
        heal.healTarget = this;
        if (heal.healOwner != null)
        {
            List<HealEff> heffs = new List<HealEff>(heal.healOwner.HealEffects.Values);
            foreach (HealEff eff in heffs)
            {
                eff(heal);
            }
        }
        List<HealEff> teffs = new List<HealEff>(HealTargetEffects.Values);
        foreach (HealEff eff in teffs)
        {
            eff(heal);
        }
        if (heal.healOwner != null) heal.healOwner.HealingSettle(heal);
        HealSettle(heal);
    }

    public virtual void HealSettle(Heal heal)//受治疗结算
    {
        SelfHeal(heal.HPHeal, heal.shieldHeal);
        LogHeal(heal);
        ShowNormalState();
    }

    public void LogHeal(Heal heal)
    {
        string log = "";
        if (heal.healOwner != null)
        {
            if(heal.healOwner.parent.TryGetComponent<Player>(out Player player))
            {
                if (player.isPlayer) log += "友方 ";
                else log+="敌方 ";
            }
            log += heal.healOwner.characterName + "(" + heal.healOwner.HP.ToString() + "," + heal.healOwner.shield.ToString() + ")";
        }
        if (heal.healTarget != null)
        {
            log += " 为 " ;
            if (heal.healTarget.parent.TryGetComponent<Player>(out Player player))
            {
                if (player.isPlayer) log += "友方 ";
                else log += "敌方 ";
            }
            log +=  heal.healTarget.characterName+ "(" + heal.healTarget.HP.ToString() + "," + heal.healTarget.shield.ToString() + ")";
        }
        if (heal.HPHeal > 0)
        {
            log += " 治疗了 " + heal.HPHeal.ToString() + " 点生命值";
        }
        if (heal.shieldHeal > 0)
        {
            log += " 恢复了 " + heal.shieldHeal.ToString() + " 点护盾";
        }
        Log(log);
    }

    public void LogAtk(Attack atk)
    {
        string log = "";
        if (atk.attackOwner != null)
        {
            if(atk.attackOwner.parent.TryGetComponent<Player>(out Player player))
            {
                if (player.isPlayer) log += "友方 ";
                else log += "敌方 ";
            }
            log += atk.attackOwner.characterName +"("+atk.attackOwner.HP.ToString()+","+atk.attackOwner.shield.ToString()+")";
        }
        log += " 使用 " + atk.type.ToString();
        if (atk.attackTarget != null)
        {
            log += " 对 ";
            if (atk.attackTarget.parent.TryGetComponent<Player>(out Player player))
            {
                if (player.isPlayer) log += "友方 ";
                else log += "敌方 ";
            }
            log += atk.attackTarget.characterName + "(" + atk.attackTarget.HP.ToString() + "," + atk.attackTarget.shield.ToString() + ")";
        }
        log += " 造成了 "+atk.Damage.ToString() + " 点 " + atk.attackelemental.ToString()+" 伤害";
        Log(log);
    }

    public virtual void DefenceSettle(Attack atk) //受击结算
    {
        SelfDamege(atk.Damage);
        LogAtk(atk);
        if (atk.attackelemental != ElementType.Physics) //当攻击类型不是元素反应，且不是物理伤害时，触发元素反应与附着 
        {
            if (affected != null) //当已有元素附着时，触发元素反应
            {
                Character.ElementalReactionFunc(this, atk);
            }
            else//当没有元素附着时，触发元素附着
            {
                ElementalAffect(atk);
            }
        }

        if (HP <= 0 && state != CharacterState.Dead)//角色死亡
        {
            state = CharacterState.Dead;
            DeathSettle();//触发死亡结算
        }
        ShowNormalState();
    }

    public virtual void AttackSettle(Attack atk)//攻击结算
    {

    }

    public virtual void HealingSettle(Heal heal)//治疗结算
    {

    }
    public virtual void ElementalAffect(Attack atk)//元素附着
    {
        if (atk.attackelemental != ElementType.Geo && atk.attackelemental != ElementType.Anemo)//当攻击元素属性不为风、岩时，附加元素附着
        {
            affected = new ElementalAffect();
            affected.AffectCount = 1;
            affected.affectElemental = atk.attackelemental;
            affected.affectOwner = atk.attackOwner;
        }
    }
    public virtual void DeathSettle()//死亡结算
    {
        
    }
    public virtual void SpecialStateSettle()//特殊状态结算
    {
        if (state != CharacterState.Alive)//处于异常状态中
        {
            switch (state)
            {
                case CharacterState.Alive:
                    break;
                case CharacterState.Burning:
                    if (shield > 0) shield--;
                    else HP--;
                    if (shield > 0) shield--;
                    else HP--;
                    stateTimeRemain--;
                    break;
                case CharacterState.Frozen:
                    stamina = 0;
                    stateTimeRemain--;
                    break;
                case CharacterState.Dead:
                    stamina = 0;
                    stateTimeRemain--;
                    break;
                default:
                    stateTimeRemain--;
                    break;
            }
            if (stateTimeRemain == 0)
            {
                switch (state)
                {
                    case CharacterState.SuperConducted:
                        DeSuperConducted();
                        break;
                    default:
                        break;
                }
                StateTransform(CharacterState.Alive, 0);
            }
        }
        ShowNormalState();
    }

    public void NewRoundSettle()//新回合结算
    {
        List<NorEff> neffs = new List<NorEff>(NormalEffects.Values);
        foreach(NorEff norEff in neffs)
        {
            norEff();
        }
        ShowNormalState();
    }

    public void NewFrameSettle()//新帧结算
    {
        List<NorEff> neff = new List<NorEff>(NormalEffectsPerFrame.Values);
        foreach(NorEff norEff in neff)
        {
            norEff();
        }
    }

    public static Vector2 PosToVec(Vector2 pos)//相对位置到绝对位置的转换
    {
        Vector2 vec = new Vector2();
        vec.x = pos.x - 2.5f;
        vec.y = pos.y - 2.5f;
        return vec;
    }

    public virtual void SelfHeal(int Hp, int Shield)
    {
        shield += Shield;
        for(int i = 0; i < shield; i++)
        {
            ShowImgDefault(sprites.GetComponent<AllSprites>().Num_Yellow_Add_1);
        }
        HP += Hp;
        if (HP > MAXHP)
        {
            HP = MAXHP;
        }
        for (int i = 0; i < Hp; i++)
        {
            ShowImgDefault(sprites.GetComponent<AllSprites>().Num_Green_Add_1);
        }
        ShowNormalState();
    }//元素反应型治疗

    public virtual void SelfDamege(int damege) {
        shield -= damege;
        if(shield < 0)
        {
            HP += shield;
            shield = 0;
        }
        for(int i = 0; i < damege; i++)
        {
            ShowImgDefault(sprites.GetComponent<AllSprites>().Num_Red_Minus_1);
        }
        if (HP <= 0 && state!= CharacterState.Dead){
            state = CharacterState.Dead;
            DeathSettle();
        }
        ShowNormalState();
    }//元素反应型伤害

    public static void Crystallize(Character target, Attack atk)//结晶反应，给释放者2点元素反应护盾值
    {
        Character owner = atk.attackOwner;
        owner.SelfHeal(0, 2);
        target.affected = null;
    }

    //无图像
    public static void Swirl(Character target, Attack atkack)//扩散反应, 给自己和周围1个单位以内的角色附加元素
    {
        ElementType type = target.affected.affectElemental;
        Vector2Int pos =BattleArea.GetReverse(target.position);
        GameObject Creat(Vector2Int position)
        {
            GameObject obj = Attack.CreatObject<Attack>(atkack.parent);
            Attack atk = obj.GetComponent<Attack>();
            atk.target = position;
            atk.Damage = 0;
            atk.type = AttackType.ElementalReaction;
            atk.attackelemental = type;
            atk.attackOwner = obj.GetComponent<Character>();
            atk.speed = 10;
            atk.active = false;
            return obj;
        }

        for(int i = -1; i < 2; i++)
            for(int j = -1; j < 2; j++)
            {
                Creat(pos + new Vector2Int(i, j)).transform.localPosition = BattleArea.GetLocalPosition(pos);
            }
        target.affected = null;
    }

    public static void Frozen(Character Target, Attack atk)//冻结反应，本回合及下回合行动力清零
    {
        Target.stamina = 0;
        Target.StateTransform(CharacterState.Frozen, 1);
        Target.affected = null;
    }

    public static void ElectroCharge(Character Target, Attack atk)//感电反应，受到1点元素反应伤害，并附加雷元素
    {
        Target.SelfDamege(1);
        Target.affected = new ElementalAffect();
        Target.affected.affectElemental = ElementType.Electro;
        Target.affected.AffectCount = 1;
    }

    public static void Melt(Character Target, Attack atk)//融化反应，受到1点真实伤害，并附加水元素
    {
        Target.SelfDamege(1);
        Target.affected = new ElementalAffect();
        Target.affected.affectElemental = ElementType.Hydro;
        Target.affected.AffectCount = 1;
    }

    public static void Vaporize(Character Target, Attack atk)//蒸发反应，受到1点元素反应伤害，并附加火元素
    {
        Target.SelfDamege(1);
        Target.affected = new ElementalAffect();
        Target.affected.affectElemental = ElementType.Pyro;
        Target.affected.AffectCount = 1;
    }

    public static void Overloaded(Character Target, Attack attack)//超载反应，自己以及周围一格所有角色受到1元素反应伤害
    {
        Vector2Int pos =BattleArea.GetReverse(Target.position);
        GameObject Creat(Vector2Int position)
        {
            GameObject obj = CreatObject<ElementalReaction>(attack.parent);
            ElementalReaction atk = obj.GetComponent<ElementalReaction>();
            atk.target = position;
            atk.Damage = 1;
            atk.type = AttackType.ElementalReaction;
            atk.attackelemental = ElementType.Physics;
            atk.attackOwner = obj.GetComponent<Character>();
            atk.speed = 10;
            atk.active = false;
            return obj;
        }

        for (int i = -1; i < 2; i++)
            for (int j = -1; j < 2; j++)
            {
                Creat(pos + new Vector2Int(i, j)).transform.localPosition = BattleArea.GetLocalPosition(pos);
            }
        Target.affected = null;
    }

    public static void SuperConduct(Character Target, Attack atk)//超导反应，受到1点元素反应伤害，本回合受到所有来源的伤害+1（不包括元素反应伤害）//未实现
    {
        Target.SelfDamege(1);
        Target.StateTransform(CharacterState.SuperConducted, 1);
        Target.affected = null;
    }

    public void SuperConducted(Attack atk)//被超导时的特效
    {
        if(state == CharacterState.SuperConducted)atk.Damage++;
    }

    public void BeSuperConducted()//被超导时运行
    {
        this.DefenceEffects["0_SuperConducted"] = SuperConducted;
    }

    public void DeSuperConducted()//退超导时运行
    {
        this.DefenceEffects.Remove("0_SuperConducted");
    }
    public static void Burning(Character Target, Attack atk)//燃烧，附加燃烧状态，每次回合开始时造成2元素反应伤害，持续2个回合, 并附加火元素，可被水/冰元素浇灭
    {
        Target.StateTransform(CharacterState.Burning, 2);
        Target.affected = null;
    }

    public static void ElementalReactionFunc(Character target, Attack atk)//元素反应
    {
        switch (atk.attackelemental)
        {
            case ElementType.Anemo://风
                Swirl(target,atk);
                break;
            case ElementType.Geo://岩
                Crystallize(target, atk);
                break;
            case ElementType.Pyro://火
                switch (target.affected.affectElemental)
                {
                    case ElementType.Cryo://冰
                        Melt(target, atk);
                        break;
                    case ElementType.Electro://雷
                        Overloaded(target, atk);
                        break;
                    case ElementType.Hydro://水
                        Vaporize(target, atk);
                        break;
                    case ElementType.Pyro://火
                        break;
                    case ElementType.Dendro://草
                        Burning(target, atk);
                        break;
                    default:
                        break;
                }
                break;
            case ElementType.Hydro://水
                switch (target.affected.affectElemental)
                {
                    case ElementType.Cryo://冰
                        Frozen(target, atk);
                        break;
                    case ElementType.Electro://雷
                        ElectroCharge(target, atk);
                        break;
                    case ElementType.Hydro://水
                        break;
                    case ElementType.Pyro://火
                        Vaporize(target, atk);
                        break;
                    default:
                        break;
                }
                break;
            case ElementType.Cryo://冰
                switch (target.affected.affectElemental)
                {
                    case ElementType.Cryo://冰
                        break;
                    case ElementType.Electro://雷
                        SuperConduct(target, atk);
                        break;
                    case ElementType.Hydro://水
                        Frozen(target, atk);
                        break;
                    case ElementType.Pyro://火
                        Melt(target, atk);
                        break;
                    default:
                        break;
                }
                break;
            case ElementType.Electro://雷
                switch (target.affected.affectElemental)
                {
                    case ElementType.Cryo://冰
                        SuperConduct(target, atk);
                        break;
                    case ElementType.Electro://雷
                        break;
                    case ElementType.Hydro://水
                        ElectroCharge(target, atk);
                        break;
                    case ElementType.Pyro://火
                        Overloaded(target, atk);
                        break;
                    default:
                        break;
                }
                break;
            case ElementType.Dendro://草
                break;
            default:
                break;
        }
    }

    public virtual void StateTransform(CharacterState state, int time) //更改状态
    {
        this.state = state;
        stateTimeRemain = time;
        switch (state)
        {
            case CharacterState.Alive:
                break;
            case CharacterState.Dead:
                break;
            case CharacterState.Burning:
                break;
            case CharacterState.Frozen:
                break;
            case CharacterState.SuperConducted:
                BeSuperConducted();
                break;
            default:
                break;
        }
    }
    public virtual void MoveTo(Vector3 pos)
    {
        this.gameObject.transform.localPosition = pos;
    }//更换位置

    public bool IsMoving = false;
    public virtual void MoveTo(Vector2Int target)
    {
        if (target.x > BattleArea.xmax) target.x = BattleArea.xmax;
        if (target.y > BattleArea.ymax) target.y = BattleArea.ymax;
        if (target.x < 0) target.x = 0;
        if (target.y < 0) target.y = 0;
        oldPosition = position;
        position = target;
        if (IsMoving) return;
        IsMoving = true;
        NormalEffectsPerFrame.Add("Moving", Moving);
    }//更换位置

    public void Moving()
    {
        Vector3 target = BattleArea.GetLocalPosition(position);
        target.z = -1;
        Vector3 dir = target - gameObject.transform.localPosition;
        if (dir.sqrMagnitude > 0.01)
        {
            gameObject.transform.localPosition += dir * Time.deltaTime*10;
        }
        else
        {
             gameObject.transform.localPosition = target;
            IsMoving = false;
            NormalEffectsPerFrame.Remove("Moving");
        }
    }

    public Vector2Int[] poses = new Vector2Int[]
    {
        new Vector2Int(-1,-1),new Vector2Int(-1,0),new Vector2Int(-1,1),
        new Vector2Int(0,-1),new Vector2Int(0,0),new Vector2Int(0,1),
        new Vector2Int(1,-1),new Vector2Int(1,0),new Vector2Int(1,1)
    };

    public ElementType normalAttackElemental = ElementType.Physics;//普攻元素类型
    public int normalAttackDamege = 1;//普攻伤害

    public virtual bool NormalAttack(Vector2Int targ)//普攻
    {
        if (stamina < 1) { return false; }
        targ += position;
        GameObject obj = CreatObject<Attack>(gameObject.transform.parent.gameObject);
        Attack atk = obj.GetComponent<Attack>();
        atk.target = targ;
        atk.Damage = normalAttackDamege;
        atk.type = AttackType.NormalAttack;
        atk.attackelemental = normalAttackElemental;
        atk.attackOwner = this;
        atk.speed = 10;
        atk.active = false;

        stamina--;
        return true;
    }

    public virtual bool TryNormalAttack(Vector2Int targ)
    {
        bool posRight = false;
        foreach(Vector2Int pos in poses)
        {
            if(pos == targ)
            {
                posRight = true;
                break;
            }
        }

        if (!posRight) return false;

        targ += position;
        GameObject obj = CreatObject<Attack>(gameObject.transform.parent.gameObject);
        Attack atk = obj.GetComponent<Attack>();
        atk.target = targ;
        atk.Damage = normalAttackDamege;
        atk.type = AttackType.NormalAttack;
        atk.attackelemental = normalAttackElemental;
        atk.attackOwner = this;
        atk.speed = 10;
        atk.active = false;

        return true;
    }

    UseCard normal;

    public ElementType chargedAttackElemental = ElementType.Physics;//重击类型
    public int chargedAttackDamege = 2;//重击伤害
    public virtual bool ChargedAttack(Vector2Int targ)//重击
    {
        if (stamina < 1) { return false; }
        targ += position;
        GameObject obj = Attack.CreatObject<Attack>(gameObject.transform.parent.gameObject);
        ElementalReaction atk = obj.GetComponent<ElementalReaction>();
        atk.target = targ;
        atk.Damage = chargedAttackDamege;
        atk.type = AttackType.ChargedAttack;
        atk.attackelemental = chargedAttackElemental;
        atk.attackOwner = this;
        atk.speed = 10;
        atk.active = false;

        return true;
    }

    UseCard charge;

    public Vector2Int[] posesMove =
    {
        new Vector2Int(1,0),new Vector2Int(-1,0),
        new Vector2Int(0,1),new Vector2Int(0,-1)
    };

    public int FreeMoveCount=1;
    public int MaxFreeMove = 1;

    public int FreeNormalAttack = 1;
    public int MaxNormalAttack = 1;

    public int ChargedAttackCount = 1;
    public int MaxCharged = 1;

    public void UpdateFreeMoveCount()
    {
        FreeMoveCount = MaxFreeMove;
    }
    public virtual bool CanFreeMove()
    {
        if (FreeMoveCount < 1) return false;
        return true;
    }
    public virtual bool FreeMove(Vector2Int pos)
    {
        if (FreeMoveCount < 1) return false;
        string log = "";
        if (parent.TryGetComponent(out Player player))
        {
            if (player.isPlayer) log += " 我方 ";
            else log += " 敌方 ";
            player.ResetButtonDown();
        }
        log += characterName;
        log += " 移动到 " + pos.ToString();
        Log(log);
        pos += position;
        MoveTo(pos);
        FreeMoveCount--;
        return true;
    }

    public virtual bool Move(Vector2Int pos)
    {
        if (stamina < 1) return false;
        pos += position;
        MoveTo(pos);
        stamina--;
        return true;
    }

    public static Character[] SortEnemyWithDistence(Character[] old, Vector2Int pos )//敌方
    {
        float dis(Character character, Vector2Int position)
        {
            return (BattleArea.GetReverse( character.position + position)).sqrMagnitude;
        }
        int lenth = old.Length;
        Character[] sorted = new Character[lenth];
        for(int i = 0; i < lenth; i++)
        {
            sorted[i] = old[i];
            for (int j = 0; j < i; j++)
            {
                if(dis(sorted[j],pos) > dis(sorted[i], pos))
                {
                    Character tmp = sorted[i];
                    for(int k = i; k > j; k--)
                    {
                        sorted[k] = sorted[k - 1];
                    }
                    sorted[j] = tmp;
                }
                break;
            }
        }
        return sorted;
    }

    public static Character[] SortEnemyWithDistence(Character[] old)
    {
        return SortEnemyWithDistence(old, new Vector2Int(0, 0));
    }

    private void Update()
    {
    }

    private void Start()
    {
        //Initial();
    }

    public int SunsettiaLimit = 0;
    public void AddSunsettiaLimit()
    {
        SunsettiaLimit = 2;
        NormalEffects.Add("SunsettiaLimit", DeleteSunsettiaLimit);
    }
    public void DeleteSunsettiaLimit()
    {
        SunsettiaLimit--;
        if (SunsettiaLimit < 1)
        {
            NormalEffects.Remove("SunsettiaLimit");
        }
    }

    public void AddClockLimit()
    {
        if(!NormalEffects.ContainsKey("ClockLimit"))
            NormalEffects.Add("ClockLimit", ClockLimit);
    }

    public void ClockLimit()
    {
        stamina = 0;
        NormalEffects.Remove("ClockLimit");
    }

    public GameObject CreateAttack(Vector2Int pos, int Damage, AttackType type, ElementType elementType)
    {
        pos += position;
        return Attack.CreateAttack(parent, pos, Damage, type, elementType, this);
    }

    public virtual void KickBack(Vector2Int EnemyPos, int num)
    {
        EnemyPos = BattleArea.GetReverse(EnemyPos);
        Vector2Int target = new Vector2Int(position.x,position.y);
        if (EnemyPos.y > position.y) target.y -= num;
        else if (EnemyPos.y < position.y) target.y += num;
        else
        {
            if (EnemyPos.x > position.x) target.x -= num;
            else target.x += num;
        }
        MoveTo(target);
    }//击退

    public virtual void MovingBack()
    {
        Vector2Int dir = position - oldPosition;

        Vector2Int currentPos = BattleArea.GetPos(transform.localPosition);
        if (dir.x >= 0) currentPos.x--;
        if (dir.y >= 0) currentPos.y--;

        position = currentPos;
    }

    public Vector3 msgPosition = new Vector3(0,0.45f,-10);

    public GameObject ShowMsgDefault(string msg)
    {
        return ShowMsgDefault(msg,new Color(1,1,1));
    }
    public GameObject ShowMsgDefault(string msg, Color color)
    {
        msgPosition.x += 0.2f;
        if (msgPosition.x > 0.4f) msgPosition.x = -0.3f;
        return ShowMessage(msg, color, msgPosition, 0.002f);
    }

    public GameObject ShowImgDefault(Sprite img)
    {
        msgPosition.x += 0.2f;
        if (msgPosition.x > 0.4f) msgPosition.x = -0.3f;
        return ShowImage(img, msgPosition,0.4f);
    }

    public GameObject ShowMessage(string msg, Vector3 localPos , float scale = 0.01f, float liveTime = 1.0f)
    {
        return Massage.CreateMsg(msg, localPos, gameObject, scale, liveTime);
    }
    public GameObject ShowMessage(string msg, Color color, Vector3 localPos, float scale = 0.01f, float liveTime = 1.0f)
    {
        GameObject obj = Massage.CreateMsg(msg, localPos, gameObject, scale, liveTime);
        obj.GetComponent<Text>().color = color;
        return obj;
    }
    public GameObject ShowImage(Sprite img, Vector3 localPos, float scale = 1, float liveTime = 1.0f)
    {
        return Massage.CreateMsg(img, localPos, gameObject, scale, liveTime);
    }

    public bool DefaultCanUse()
    {
        if (stamina > 0) return true;
        return false;
    }
    public virtual void ShowState(GameObject info)
    {

    }

    public virtual void DeleteShowState(GameObject info)
    {

    }

    public virtual void CharacterCollision(Character other)
    {
        if (IsMoving)
        {
            MovingBack();
        }
    }

    public virtual string StringGet()
    {
        int isplayer = 0;
        if (parent.GetComponent<Player>().isPlayer) isplayer = 1;
        string affstr = ""+(char)255+ (char)255+ (char)255;
        if (affected != null)
        {
            affstr = ""+(char)1+(char)affected.affectElemental + (char)affected.AffectCount;
        }
        char target = (char)1;
        if (!isTarget) target = (char)0;
        return ""+(char)isplayer+(char)(((int)type)/100)+ (char)(((int)type)%100)+ (char)position.x + (char)position.y + (char)(HP/100)+ (char)(HP %100) + (char)(shield/100)+ (char)(shield %100)+(char)stamina+(char)(int)state+(char)stateTimeRemain+affstr+target;
    }

    public virtual MemoryStream StremGet()
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(parent.GetComponent<Player>().isPlayer);
        writer.Write((int)type);
        writer.Write(position.x);
        writer.Write(position.y);
        writer.Write(HP);
        writer.Write(shield);
        writer.Write(stamina);
        writer.Write((int)state);
        writer.Write(stateTimeRemain);
        if (affected != null)
        {
            writer.Write(true);
            writer.Write((int)affected.affectElemental);
            writer.Write(affected.AffectCount);
        }
        writer.Write(isTarget);
        writer.Write(FreeMoveCount);
        writer.Write(FreeNormalAttack);
        writer.Write(ChargedAttackCount);
        return stream;
    }

    public static void StaticStreamSet(MemoryStream stream)
    {
        BinaryReader reader = new BinaryReader(stream);
        bool isplayer = reader.ReadBoolean();
        Player player;
        if (isplayer) player = BattleArea.player;
        else player = BattleArea.enemy;

        CharacterName type =(CharacterName)reader.ReadInt32();
        Debug.Log("type"+type.ToString());
        Vector2Int pos = new Vector2Int(reader.ReadInt32(), reader.ReadInt32());
        switch (type)
        {
            case CharacterName.Character_Amber:
                player.PutAmber(pos);
                break;
            case CharacterName.Character_Diluc:
                player.PutDiluc(pos);
                break;
            case CharacterName.Character_Jean:
                player.PutJean(pos);
                break;
            case CharacterName.Character_Keqing:
                player.PutKeqing(pos);
                break;
            case CharacterName.Character_Lisa:
                player.PutLisa(pos);
                break;
            case CharacterName.Character_Ningguang:
                player.PutNingguang(pos);
                break;
            case CharacterName.Character_Noelle:
                player.PutNoelle(pos);
                break;
            default:
                return;
        }
        player.myCharacters[player.characterCount - 1].StreamSet(stream);
    }

    public virtual void StreamSet(MemoryStream stream)
    {
        BinaryReader reader = new BinaryReader(stream);
        HP = reader.ReadInt32();
        shield = reader.ReadInt32();
        stamina = reader.ReadInt32();
        state = (CharacterState)reader.ReadInt32();
        stateTimeRemain = reader.ReadInt32();
        if (reader.ReadBoolean())
        {
            affected = new ElementalAffect();
            affected.affectElemental = (ElementType)reader.ReadInt32();
            affected.AffectCount = reader.ReadInt32();
        }
        isTarget = reader.ReadBoolean();
        FreeMoveCount = reader.ReadInt32();
        FreeNormalAttack = reader.ReadInt32();
        ChargedAttackCount = reader.ReadInt32();
    }

    public virtual int StringSet(string msg,int pos)
    {
        HP = msg[pos++]*100+msg[pos++];
        Debug.Log("HP:" + HP.ToString());
        shield = msg[pos++] * 100 + msg[pos++];
        Debug.Log("SHED:"+ shield.ToString());
        stamina = msg[pos++];
        state = (CharacterState)msg[pos++];
        stateTimeRemain = msg[pos++];
        if (msg[pos++] == 1)
        {
            ElementType typ = (ElementType)msg[pos++];
            affected = new ElementalAffect();
            affected.affectElemental = typ;
            affected.AffectCount = msg[pos++];
        }
        if (msg[pos++] == 0)
        {
            isTarget = false;
        }
        ShowNormalState();
        return pos;
    }
    public static  void StaticStringSet(string msg)
    {
        int msgpos = 0;
        int isplayer = msg[msgpos++];
        CharacterName type = (CharacterName)(msg[msgpos++]*100+msg[msgpos++]);
        Debug.Log(type.ToString());
        Vector2Int pos = new Vector2Int(msg[msgpos++], msg[msgpos++]);
        Player player;
        if (isplayer == 0) player = BattleArea.player;
        else player = BattleArea.enemy;
        switch (type)
        {
            case CharacterName.Character_Amber:
                player.PutAmber(pos);
                break;
            case CharacterName.Character_Diluc:
                player.PutDiluc(pos);
                break;
            case CharacterName.Character_Jean:
                player.PutJean(pos);
                break;
            case CharacterName.Character_Keqing:
                player.PutKeqing(pos);
                break;
            case CharacterName.Character_Lisa:
                player.PutLisa(pos);
                break;
            case CharacterName.Character_Ningguang:
                player.PutNingguang(pos);
                break;
            case CharacterName.Character_Noelle:
                player.PutNoelle(pos);
                break;
            default:
                return;
        }
        player.myCharacters[player.characterCount - 1].StringSet(msg,msgpos);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Character>(out Character character))
        {
            CharacterCollision(character);
        }
    }

}//角色基类

public enum HeroType {Sword, Claymore, Bow, Polearm, Mage }//单手剑，双手剑，弓，长柄，法器

public class Hero : Character//可控制角色基类
{
    public Sprite card;
    public Sprite Info;

    public HeroType heroType;

    public string[] BasicCards;
    public int BasicCardsCount;

    public void Init(Hero other)
    {
        base.Init((Character)other);
        card = other.card;
        Info = other.Info;
        BasicCards = other.BasicCards;
        BasicCardsCount = other.BasicCardsCount;
        InitPos();
    }

    public override void DeathSettle()
    {
        Debug.Log("DeathSettle");
        base.DeathSettle();
        if (HP <= 0 && state == CharacterState.Dead)
            parent.GetComponent<Player>().CharacterDead(gameObject);
    }
    public bool Inited = false;
    public virtual void Heroinit()
    {
        if (Inited) return;
        Inited = true;
    }

    public static Hero GetHeroWithString(string name)
    {
        GameObject obj = GameObject.Find("Characters/" + name);
        return obj.GetComponent<Hero>();
    }

    public static Vector2Int[] posSword = new Vector2Int[]
    {
        new Vector2Int(-1,-1),new Vector2Int(0,-1),new Vector2Int(1,-1),
        new Vector2Int(-1,0),new Vector2Int(1,0),
        new Vector2Int(-1,1),new Vector2Int(0,1),new Vector2Int(1,1)
    };

    public static Vector2Int[] posMage=
        {
            new Vector2Int(1,0), new Vector2Int(-1,0),
            new Vector2Int(1,1),new Vector2Int(0,1), new Vector2Int(-1,1),
            new Vector2Int(1,2), new Vector2Int(0,2), new Vector2Int(-1,2),
            new Vector2Int(0,3)
        };

    public static Vector2Int[] posClayMore =
    {
        new Vector2Int(1,0),new Vector2Int(-1,0),
        new Vector2Int(1,1),new Vector2Int(-1,1),new Vector2Int(0,1),
        new Vector2Int(0,2)
    };

    public static Vector2Int[] posBow =
    {
        new Vector2Int(-2,0),new Vector2Int(-1,0),new Vector2Int(1,0),new Vector2Int(2,0),
        new Vector2Int(-1,1),new Vector2Int(0,1),new Vector2Int(1,1),
        new Vector2Int(-1,2),new Vector2Int(0,2),new Vector2Int(1,2),
        new Vector2Int(-1,3),new Vector2Int(0,3),new Vector2Int(1,3),
        new Vector2Int(0,4)
    };

    public static Vector2Int[] posPolearm =
    {
        new Vector2Int(-2,0),new Vector2Int(-1,0),new Vector2Int(1,0),new Vector2Int(2,0),
        new Vector2Int(0,1),
        new Vector2Int(0,2),
        new Vector2Int(0,3)
    };

    public static Vector2Int[] posAll =
    {
        new Vector2Int(-5,-1),new Vector2Int(-4,-1),new Vector2Int(-3,-1),new Vector2Int(-2,-1),new Vector2Int(-1,-1),new Vector2Int(0,-1),new Vector2Int(1,-1),new Vector2Int(2,-1),new Vector2Int(3,-1),new Vector2Int(4,-1),new Vector2Int(5,-1),
        new Vector2Int(-5,-2),new Vector2Int(-4,-2),new Vector2Int(-3,-2),new Vector2Int(-2,-2),new Vector2Int(-1,-2),new Vector2Int(0,-2),new Vector2Int(1,-2),new Vector2Int(2,-2),new Vector2Int(3,-2),new Vector2Int(4,-2),new Vector2Int(5,-2),
        new Vector2Int(-5,-3),new Vector2Int(-4,-3),new Vector2Int(-3,-3),new Vector2Int(-2,-3),new Vector2Int(-1,-3),new Vector2Int(0,-3),new Vector2Int(1,-3),new Vector2Int(2,-3),new Vector2Int(3,-3),new Vector2Int(4,-3),new Vector2Int(5,-3),
        new Vector2Int(-5,-4),new Vector2Int(-4,-4),new Vector2Int(-3,-4),new Vector2Int(-2,-4),new Vector2Int(-1,-4),new Vector2Int(0,-4),new Vector2Int(1,-4),new Vector2Int(2,-4),new Vector2Int(3,-4),new Vector2Int(4,-4),new Vector2Int(5,-4),
        new Vector2Int(-5,-5),new Vector2Int(-4,-5),new Vector2Int(-3,-5),new Vector2Int(-2,-5),new Vector2Int(-1,-5),new Vector2Int(0,-5),new Vector2Int(1,-5),new Vector2Int(2,-5),new Vector2Int(3,-5),new Vector2Int(4,-5),new Vector2Int(5,-5),
        new Vector2Int(-5,0),new Vector2Int(-4,0),new Vector2Int(-3,0),new Vector2Int(-2,0),new Vector2Int(-1,0),new Vector2Int(0,0),new Vector2Int(1,0),new Vector2Int(2,0),new Vector2Int(3,0),new Vector2Int(4,0),new Vector2Int(5,0),
        new Vector2Int(-5,1),new Vector2Int(-4,1),new Vector2Int(-3,1),new Vector2Int(-2,1),new Vector2Int(-1,1),new Vector2Int(0,1),new Vector2Int(1,1),new Vector2Int(2,1),new Vector2Int(3,1),new Vector2Int(4,1),new Vector2Int(5,1),
        new Vector2Int(-5,2),new Vector2Int(-4,2),new Vector2Int(-3,2),new Vector2Int(-2,2),new Vector2Int(-1,2),new Vector2Int(0,2),new Vector2Int(1,2),new Vector2Int(2,2),new Vector2Int(3,2),new Vector2Int(4,2),new Vector2Int(5,2),
        new Vector2Int(-5,3),new Vector2Int(-4,3),new Vector2Int(-3,3),new Vector2Int(-2,3),new Vector2Int(-1,3),new Vector2Int(0,3),new Vector2Int(1,3),new Vector2Int(2,3),new Vector2Int(3,3),new Vector2Int(4,3),new Vector2Int(5,3),
        new Vector2Int(-5,4),new Vector2Int(-4,4),new Vector2Int(-3,4),new Vector2Int(-2,4),new Vector2Int(-1,4),new Vector2Int(0,4),new Vector2Int(1,4),new Vector2Int(2,4),new Vector2Int(3,4),new Vector2Int(4,4),new Vector2Int(5,4),
        new Vector2Int(-5,5),new Vector2Int(-4,5),new Vector2Int(-3,5),new Vector2Int(-2,5),new Vector2Int(-1,5),new Vector2Int(0,5),new Vector2Int(1,5),new Vector2Int(2,5),new Vector2Int(3,5),new Vector2Int(4,5),new Vector2Int(5,5)
    };


    public static bool isPosiInitied=false;

    public static bool InitPos(){
        if(isPosiInitied)return false;
        isPosiInitied=true;
        posAll = new Vector2Int[400];
        for(int i=-9;i<11;i++){
            for(int j=-9;j<11;j++){
                posAll[(i+9)*20+j+9] = new Vector2Int(i,j);
            }
        }
        return true;
    }




}