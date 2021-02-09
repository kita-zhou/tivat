using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : GameBase//攻击类
{
    public bool active;//是否激活
    public int Damage;//伤害量
    public AttackType type;//攻击类型
    public ElementType attackelemental;//元素属性
    public Character attackOwner;//施放者
    public Character attackTarget;//受击者

    public Sprite activeSprite;//激活时的模样

    public Vector2 target;//目标位置
    public float speed = 10.0f;//速度

    public float aliveTime = 0.3f;

    public float activeTime;//激活的时间点

    public void Log(string msg)
    {
        if (parent.TryGetComponent<Player>(out Player player)) player.Log(msg);
    }
    public static GameObject CreateAttack(GameObject parent, Vector2Int target, int Damage, AttackType type, ElementType elementType,Character owner,Sprite Normal=null,Sprite Active = null)
    {
        GameObject obj = CreatObject<Attack>(parent);
        Attack atk = obj.GetComponent<Attack>();
        atk.Initial(target, Damage, type, elementType, owner);
        if (Normal != null)
        {
            atk.ChangeApprence(Normal);
        }
        if (Active != null)
        {
            atk.activeSprite = Active;
        }
        return obj;
    }

    public override void Initial()
    {
        sprites = GameObject.Find("Sprites");
        appearance = sprites.GetComponent<AllSprites>().characterAvatar_Diona;
        activeSprite = sprites.GetComponent<AllSprites>().characterAvatar_Babara;
    }

    public void Initial(Vector2Int targ, int damege, AttackType typ, ElementType eleTyp, Character owner)
    {
        Damage = damege;
        target = targ;
        type = typ;
        attackelemental = eleTyp;
        attackOwner = owner;
        active = false;
        gameObject.transform.position = owner.transform.position;
    }
    private void Update()
    {
        Vector3 tar = BattleArea.GetLocalPosition(new Vector2Int((int)target.x,(int)target.y));
        tar.z = -1;
        Vector3 dis = tar - transform.localPosition;
        if (dis.sqrMagnitude > 0.001)
        {//如果没到目的地，则运动
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, tar, speed * Time.deltaTime);
        }
        else
        {
            if (active)//到达目的地，激活
            {
                if (Time.time - activeTime > aliveTime)
                {
                    Destroy(this.gameObject);
                }
            }
            else
            {
                active = true;
                ChangeApprence(activeSprite);
                
                activeTime = Time.time;
                speed = 0;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D other)//与其他物体触碰
    {
        if (active == true)//如果激活
        {
            other.gameObject.TryGetComponent<Character>(out Character another);
            if (another != null &&another.isTarget &&(another.parent != parent))
            {
                another.OnAttack(this);
                Destroy(this.gameObject);
            }
        }
    }
}//攻击
