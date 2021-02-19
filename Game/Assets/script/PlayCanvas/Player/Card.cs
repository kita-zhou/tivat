using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum CardType { Normal, Item, Reward, Character, Monster }//卡牌类型

public enum CardName
{
    Move,
    FreeMove,
    NormalAttack,
    ChargedAttack,
    Run,

    Character_Ningguang,
    Character_Keqing,
    Character_Diluc,
    Character_Noelle,
    Character_Jean,
    Character_Amber,
    Character_Lisa,

    Normal_Move,
    Normal_Attack,
    Normal_Defence,
    Normal_Heal,
    Normal_Burst,
    Normal_Control,

    Normal_Geo,
    Normal_Anemo,
    Normal_Electro,
    Normal_Hydro,
    Normal_Pyro,
    Normal_Cryo,
    Normal_Dendro,

    Item_Clock,
    Item_Tea,
    Item_Book,
    Item_Trap,
    Item_Polearm,
    Item_Sword,
    Item_Sunsettia,
    Item_CrystalCore,
    Item_Advice,
    Item_Teaport,
    Item_Bow,
    Item_Sigil,
    Item_Ore,
    Item_Chill,
    Item_Claymore
}

public class ButtonBase:MonoBehaviour, IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler//按键基类
{
    public Sprite buttonNormal;
    public Sprite buttonHighLight;
    public Sprite buttonOncurse;
    public Sprite buttonSelect;
    public Sprite buttonDisabled;

    public bool isHighLighted;
    public ButtonState state;

    public bool isNormalHighLighted=false;
    public virtual void ChangeState(ButtonState state) {
        this.state = state;
        switch (state)
        {
            case ButtonState.Normal:
                this.gameObject.GetComponent<Image>().sprite = buttonNormal;
                isHighLighted = false;
                isNormalHighLighted = false;
                break;
            case ButtonState.OnCurse:
                this.gameObject.GetComponent<Image>().sprite = buttonOncurse;
                break;
            case ButtonState.HighLight:
                this.gameObject.GetComponent<Image>().sprite = buttonHighLight;
                isHighLighted = true;
                isNormalHighLighted = false;
                break;
            case ButtonState.Selected:
                this.gameObject.GetComponent<Image>().sprite = buttonSelect;
                break;
            case ButtonState.Disabled:
                this.gameObject.GetComponent<Image>().sprite = buttonDisabled;
                break;
            case ButtonState.NormalHighlighted:
                this.gameObject.GetComponent<Image>().sprite = buttonHighLight;
                isHighLighted = false;
                isNormalHighLighted = true;
                break;
            default:
                break;
        }
    }
    public virtual void Initial(ButtonBase other) {
        buttonNormal = other.buttonNormal;
        buttonHighLight = other.buttonHighLight;
        buttonDisabled = other.buttonDisabled;
        buttonOncurse = other.buttonOncurse;
        buttonSelect = other.buttonSelect;
        state = ButtonState.Normal;
    }
    public virtual void Enable()
    {
        this.state = ButtonState.Normal;
        this.gameObject.GetComponent<Image>().sprite = buttonNormal;
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (state == ButtonState.Normal)
        {
            ChangeState(ButtonState.OnCurse);
        }
    }

    public virtual  void OnPointerExit(PointerEventData eventData)
    {
        if(state == ButtonState.OnCurse )
        {
            if (isHighLighted)
            {
                ChangeState(ButtonState.HighLight);
            }
            else if (isNormalHighLighted)
            {
                ChangeState(ButtonState.NormalHighlighted);
            }
            else
            {
                ChangeState(ButtonState.Normal);
            }
        }

    }

    public virtual  void OnPointerClick(PointerEventData eventData)
    {
        if(state == ButtonState.Selected)
        {
            ChangeState(ButtonState.OnCurse);
        }
        else if(state == ButtonState.OnCurse || state == ButtonState.HighLight ||state==ButtonState.NormalHighlighted)
        {
            ChangeState(ButtonState.Selected);
        }
    }
}

public class Card : ButtonBase {

    public string cardName;//卡牌名字
    public CardType type;//卡牌类型
    public Sprite front;//卡面
    public Sprite back;//卡背
    public Sprite backIfEnemy;//敌人眼中的卡背

    public GameObject sprites;//精灵集中

    public GameObject parent;//父节点

    public bool selected;//是否被选择

    public Vector3 NormalPos;
    public void Initial(Card other)//初始化
    {
        Initial((ButtonBase)other);
        this.cardName = other.cardName;
        this.name = other.name;
        type = other.type;
        front = other.front;
        back = other.back;
        sprites = other.sprites;
        backIfEnemy = other.backIfEnemy;
    }

    public void Initial()//初始化
    {
        buttonNormal = front;
        buttonHighLight = front;
        buttonDisabled = front;
        buttonOncurse = front;
        buttonSelect = front;
        isHighLighted = false;
    }

    public override void ChangeState(ButtonState state)
    {
        if (!parent.GetComponent<Hand>().IsPlayer) return;
        ForceChangeState(state);
    }

    public GameObject selfShow = null;
    public void SelfShow()
    {
        if (selfShow == null)
        {
            selfShow = new GameObject();
            selfShow.transform.parent = this.transform;
            selfShow.AddComponent<SpriteRenderer>().sprite = front;
            selfShow.transform.localPosition = new Vector3(0, 4, -10);
            selfShow.transform.localScale = new Vector3(2, 2, 1);
            selfShow.transform.localRotation = new Quaternion(0, 0, 0, 0);
            selfShow.GetComponent<SpriteRenderer>().sortingOrder = 11;
        }
    }

    public void DeleteSelfShow()
    {
        if(selfShow != null)
        {
            Destroy(selfShow);
            selfShow = null;
        }
    }

    public bool isPlayer()
    {
        return parent.transform.parent.GetComponent<Player>().isPlayer;
    }

    public void ForceChangeState(ButtonState state)
    {
        base.ChangeState(state);
        switch (state)
        {
            case ButtonState.OnCurse:
                gameObject.transform.localPosition = NormalPos + new Vector3(0f, 0.1f, -0.9f);
                if (parent.transform.parent.GetComponent<Player>().isPlayer)
                {
                    SelfShow();
                }
                break;
            case ButtonState.Normal:
                gameObject.transform.localPosition = NormalPos;
                if (parent.transform.parent.GetComponent<Player>().isPlayer)
                {
                   DeleteSelfShow();
                }
                break;
            case ButtonState.Selected:
                if (selected == false)
                {
                    gameObject.transform.parent.GetComponent<Hand>().SelectCard(this);
                    selected = true;
                }
                gameObject.transform.localPosition = NormalPos + new Vector3(0, 0.2f, 0);
                if (parent.transform.parent.GetComponent<Player>().isPlayer)
                {
                    DeleteSelfShow();
                }
                break;
            default:
                gameObject.transform.localPosition = NormalPos;
                if (parent.transform.parent.GetComponent<Player>().isPlayer)
                {
                    DeleteSelfShow();
                }
                break;
        }
        if (state != ButtonState.Selected && selected == true)
        {
            gameObject.transform.parent.GetComponent<Hand>().DeSelect(this);
            selected = false;
        }
    }
    public static GameObject CreatObject(GameObject parent,Card other)//创造一个以parent为父节点的other的副本
    {
        GameObject obj = new GameObject();
        Card newcard = obj.AddComponent<Card>();
        newcard.Initial(other);
        newcard.Initial();
        obj.transform.parent = parent.transform;
        newcard.parent = parent;
        SpriteRenderer render = obj.AddComponent<SpriteRenderer>();
        if (parent.GetComponent<Hand>().IsPlayer) render.sprite = newcard.front;
        else render.sprite = newcard.backIfEnemy;
        render.sortingOrder = 1;

        Image img = obj.AddComponent<Image>();
        img.sprite = newcard.buttonNormal;

        obj.GetComponent<RectTransform>().rect.Set(0.0f, 0.0f, 1.8f, 2.9f);

        obj.transform.localScale = new Vector3(1, 1, 1);
        obj.transform.localRotation = new Quaternion(0, 0, 0, 0);

        newcard.ChangeState(ButtonState.Normal);

        return obj;
    }

    public static Card GetCard(string name)//获取名字为name的卡牌的Object
    {
        GameObject obj = GameObject.Find("Cards/" + name);
        return obj.GetComponent<Card>();
    }

    private void Start()
    {
        Initial();
    }
}//卡牌基类
