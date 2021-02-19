using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public delegate void DefEff(Attack atk);
    public delegate void NorEff();
    public delegate void AtkEff(Attack atk);
    public delegate void HealEff(Heal heal);
    public delegate bool UseFunc(Vector2Int Target);
    public delegate bool ComfirmUse();
    public delegate Vector2Int[] GetPos();
public delegate bool GetNameFunc(out string str);
    public enum AttackType { NormalAttack,ChargedAttack, ElementalSkill, ElementalBurst, ElementalReaction};//普攻，重击，小技能，元素爆发，元素反应
    public enum ElementalReactionType { Frozen, Melt,Vaporize, ElectroCharged, Overloaded, SuperConduct,Burning,Swirl,Crystallize};//冻结，融化，蒸发，感电，超载，超导，燃烧，扩散，结晶
    public enum ElementType {Physics, Hydro,  Pyro, Electro, Cryo, Anemo, Geo, Dendro};//物理，水，火，雷，冰，风，岩，草
    public enum CharacterState {Alive, Dead,  Frozen, Burning, ElectroCharged,SuperConducted};//生存，死亡，冻结，燃烧，感电，超导

    public class UseCard
    {
        public UseFunc func;//使用的方法
        public Vector2Int[] postions; //可用的位置

        public GetPos TargetPos;

        public ComfirmUse CanUse;

        public GetNameFunc GetName = (out string str) => {
            str = null;
            return false;
        };

        public bool needTarget = true;

        public bool IsPositionRight(Vector2Int pos)//pos:目标相对于自身的相对位置
        {
            foreach(Vector2Int position in postions)
            {
                if(position.x == pos.x && position.y == pos.y)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Use(Vector2Int pos)//pos：目标位置
        {
        Debug.Log(pos);
            if (IsPositionRight(pos))
            {
                if (func(pos))
                {
                    Debug.Log("用牌成功");
                    return true;
                }
            }
            return false;
        }
    }

    public class GameBase : MonoBehaviour
    {

        public GameObject sprites;//精灵集中
        public Sprite appearance;//模样
        public GameObject parent;//父节点

        public GameObject Area;//战斗区域
        
        public float Abs(float x)
    {
        if (x < 0) return -x;
        return x;
    }
        
        public int Abs(int x)
        {
            if (x > 0)
            {
                return x;
            }
            else
            {
                return -x;
            }
        }
        public static GameObject CreatObject<T>(GameObject parent) where T : GameBase//生成一个父节点为parent的T类型实例
        {
            GameObject charcter = new GameObject();
            charcter.transform.parent = parent.transform;
         

            charcter.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            charcter.transform.localPosition = new Vector3(0.0f, 0.0f, -2.0f);
            charcter.transform.localRotation = new Quaternion(0, 0, 0, 0);

            charcter.AddComponent <T>();
            charcter.AddComponent<AudioSource>().volume=0.2f;    

            SpriteRenderer render = charcter.AddComponent<SpriteRenderer>();

            BoxCollider2D collider = charcter.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(0.5f, 0.5f);

            Rigidbody2D rigid = charcter.AddComponent<Rigidbody2D>();
            rigid.gravityScale = 0;

            charcter.GetComponent<T>().Initial();
            render.sprite = charcter.GetComponent<T>().appearance;
            charcter.GetComponent<T>().parent = parent;

            return charcter;
        }

    public Vector2Int NearPosition(Vector3 pos)
    {
        int x = (int)(3.5 + pos.x);
        int y = (int)(3.5 + pos.y);
        if (gameObject.transform.position.x <  pos.x) x++;
        if (gameObject.transform.position.y <  pos.y) y++;
        return new Vector2Int( x,  y);
    }

    public Vector2Int EnemyNearPosition(Vector3 pos)//敌方
    {
        int x = (int)(3.5-pos.x );
        int y = (int)(3.5-pos.y );
        if (gameObject.transform.position.x <7- pos.x) x++;
        if (gameObject.transform.position.y <7- pos.y) y++;
        return new Vector2Int(7 - x,7 - y);
    }
    public void ChangeApprence(Sprite sprite)
    {
        appearance = sprite;
        SpriteRenderer render = gameObject.GetComponent<SpriteRenderer>();
        render.sprite = sprite;
    }

    public int RandomWithIndexPower(int Max)//获取0-max-1的随机值，且每增加一个数，概率降低一半
    {
        if (Max > 30) Max = 30;
        int max = (int)Mathf.Pow(2, Max) - 1;
        int x = Random.Range(1, max);
        int i = 1;
        for (; Mathf.Pow(2, i) < x + 1; i++) ;
        return Max - i;
    }

    public int RandomWithIndexLinear(int Max)//获取0-max-1的随机值，数字越小几率越大，几率线性增长
    {
        int ranmax = Max * (Max - 1) / 2;
        int x = Random.Range(1, ranmax);
        int i = 1;
        for (; i * (i - 1) / 2 < x; i++) ;
        return Max - i;
    }

    public virtual void Initial()
        {
        }

        public virtual void OnDestroy()
        {

        }
    }

    public class ElementalReaction:Attack//元素反应
    {
        public ElementalAffect affect;//附着元素
        public ElementalReactionType reactionType;//反应类型

        private void OnTriggerEnter(Collider other)
        {
            other.gameObject.TryGetComponent<Character>(out Character another);
            if (another != null)
            {
                another.OnAttack(this);
            }
        }
    }//元素反应伤害
    
