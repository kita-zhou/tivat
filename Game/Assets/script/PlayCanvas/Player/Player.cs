using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;

public class Player : MonoBehaviour//玩家类
{

    public GameObject round;//谁的回合

    public GameObject InitialObject;//初始化所用对象

    public int stamina;

    public string playerName;//名字

    public Sprite apperence;//头像

    public bool isPlayer;//是否是玩家

    public GameObject sprites;//精灵集中

    public GameObject battleArea;//战斗区域
    public GameObject hand;//手牌
    public GameObject deck;//牌库
    public GameObject characterInfo;//卡牌信息

    public int statueEnergy;//神像能量

    public bool isMyRound;//是否是我的回合

    public Character selectedCharacter;//选择的角色
    public bool isCharacterSelected;//是否角色选择完成
    public Vector2Int selectCharacterPosition;

    public Card[] selectedCard;//选择的卡组
    public int cardCount;//卡牌数

    public bool isTargeting;//是否正在选择目标

    public Vector2 selectedTarget;//选择的目标
    public bool isTargetSelected;//是否目标选择完成

    public Character[] myCharacters;//我的角色
    public int characterCount;//角色数

    public GameObject State; 

    public void Log(string msg)
    {
        battleArea.GetComponent<BattleArea>().Log(msg);
    }

    public void ShowState() 
    {
        if (State != null) DeleteState();
        State = new GameObject();
        State.transform.parent = transform;
        State.transform.localPosition = new Vector3(-1.7f, -5.3f, 0);
        State.transform.localRotation = new Quaternion(0, 0, 0, 0);
        State.transform.localScale = new Vector3(0.8f, 0.8f, 1);

        if (statueEnergy < 0)
        {
            if (isPlayer) battleArea.GetComponent<BattleArea>().Defeat();
            else battleArea.GetComponent<BattleArea>().Win();
        }

        for (int i = 0; i < statueEnergy; i++)
        {
            GameObject obj = new GameObject();
            obj.transform.parent = State.transform;
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.transform.localRotation = new Quaternion(0, 0, 0, 0);
            obj.AddComponent<SpriteRenderer>().sprite = sprites.GetComponent<AllSprites>().Bar_Blue_Light;
            obj.transform.localPosition = new Vector3(i, 0);
        }
        int index = 0;
        if (statueEnergy > 1) index = statueEnergy - 1;
        if (index > 4) index = 4;
        GameObject objStatu = new GameObject();
        objStatu.transform.parent = State.transform;
        objStatu.transform.localScale = new Vector3(2, 2, 1);
        objStatu.transform.localRotation = new Quaternion(0, 0, 0, 0);
        objStatu.AddComponent<SpriteRenderer>().sprite = sprites.GetComponent<AllSprites>().Statu[index];
        objStatu.transform.localPosition = new Vector3(-1, -0.15f);
        //Massage.CreateMsg(stamina.ToString(), new Vector3(-1, -5, -10), gameObject,0.2f,-1);
    }

    public void DeleteState()
    {
        if (State != null)
        {
            int count = State.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                Destroy(State.transform.GetChild(i).gameObject);
            }
            Destroy(State);
            State = null;
        }
    }

    public void Initial()//初始化
    {
        selectedCard = new Card[10];
        cardCount = 0;
        myCharacters = new Character[10];
        characterCount = 0;
        statueEnergy = 6;
        stamina = 100;
        ShowState();
    }

    public bool InitFromString(string[] cards)
    {
        
            Debug.Log("StringStart");
            for (int i = 0; i < 4; i++)
            {
                hand.GetComponent<Hand>().AddCard(Card.GetCard(cards[i]));
            }
            Debug.Log("Point1");
            for (int i = 0; i < 4; i++)
            {
                Hero hero = Hero.GetHeroWithString(cards[i]);
                hero.Heroinit();
                for (int j = 0; j < hero.BasicCardsCount; j++)
                {
                    deck.GetComponent<Deck>().PutCard(hero.BasicCards[j]);
                }
            }
            Debug.Log("Point2");
            for (int i = 4; i < 24; i++)
            {
                deck.GetComponent<Deck>().PutCard(cards[i]);
            }
            Debug.Log("InitSuccess");
            hand.GetComponent<Hand>().Refresh();
        if (isMyRound)
        {
            hand.GetComponent<Hand>().GetCard();
        }
            return true;
       
    }

    // Start is called before the first frame update
    void Start()
    {
        Initial();
        InitUseCard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeUseCardPositionState(UseCard useclass, ButtonState state)//改变位置状态
    {
        Debug.Log("StartChangeState");
        if (useclass != null)
        {
            Vector2Int[] poses = useclass.postions;
            Vector2Int[] posesback = new Vector2Int[poses.Length];
            for(int i = 0; i < poses.Length; i++)
            {
                posesback[i].x = poses[i].x;
                posesback[i].y = poses[i].y;
            }
            if (isCharacterSelected)//如果有卡牌备选
            {
                for (int i = 0; i < posesback.Length; i++)
                {
                    posesback[i] += selectCharacterPosition;
                }
            }
            battleArea.GetComponent<BattleArea>().ButtonState(posesback, state, isPlayer);
        }
    }

    public void MoveButtonDown()
    {
        if (isTargeting)
        {
            ResetButtonDown();
            return;
        }
        if (isMyRound && !isTargeting)
        {
            if (isCharacterSelected)
            {
                selectedCard = new Card[1];
                selectedCard[0] = Card.GetCard("FreeMove");
                cardCount = 1;
                if (!selectedCharacter.UsingCardDic.TryGetValue(GetCardString(), out UseCard useclass))
                {
                    Debug.Log("卡牌搭配错误");
                    ResetButtonDown();
                    return;
                }
                if (!useclass.CanUse())
                {
                    ResetButtonDown();
                    Debug.Log("当前无法使用该牌");
                    return;
                }
                if (!useclass.needTarget)
                {
                    Debug.Log("直接使用");
                    UseCard(new Vector2Int(-1, -1));
                    return;
                }
                ChangeUseCardPositionState(useclass, ButtonState.HighLight);
                isTargeting = true;
            }
        }
    }

    public void UseButtonDown()//按下使用按钮
    {
        if (isTargeting)
        {
            Debug.Log("请选择使用顺序");
            ResetButtonDown();
            return;
        }
        if (isMyRound && !isTargeting)//在我的回合
        {
            UpdateCard();
            if (isCharacterSelected)//如果有角色被选中
            {
                if (selectedCharacter.UsingCardDic.TryGetValue(GetCardString(), out UseCard useclass))
                {
                    if (!useclass.CanUse())
                    {
                        ResetButtonDown();
                        Debug.Log("当前无法使用该牌");
                        return;
                    }
                    if (!useclass.needTarget)
                    {
                        Debug.Log("直接使用");
                        UseCard(new Vector2Int(-1, -1));
                        return;
                    }
                    ChangeUseCardPositionState(useclass, ButtonState.HighLight);
                }
                else
                {
                    isCharacterSelected = false;
                    if (!UsingCardDic.TryGetValue(GetCardString(), out useclass))
                    {
                        Debug.Log("卡牌搭配错误");
                        ResetButtonDown();
                        return;
                    }
                    if (!useclass.CanUse())
                    {
                        ResetButtonDown();
                        Debug.Log("当前无法使用该牌");
                        return;
                    }
                    if (!useclass.needTarget)
                    {
                        Debug.Log("直接使用");
                        UseCard(new Vector2Int(-1, -1));
                        return;
                    }
                    ChangeUseCardPositionState(useclass, ButtonState.HighLight);
                }
            }
            else//玩家用牌
            {
                if(!UsingCardDic.TryGetValue(GetCardString(),out UseCard useclass))
                {
                    Debug.Log("卡牌搭配错误");
                    ResetButtonDown();
                    return;
                }
                if (!useclass.CanUse())
                {
                    ResetButtonDown();
                    Debug.Log("当前无法使用该牌");
                    return;
                }
                if (!useclass.needTarget)
                {
                    Debug.Log("直接使用");
                    UseCard(new Vector2Int(-1, -1));
                    return;
                }
                ChangeUseCardPositionState(useclass, ButtonState.HighLight);
            }
            isTargeting = true;
        }
        else//不在我的回合
        {
            Debug.Log("不在我的回合");
            ResetButtonDown();
        }
    }

    public SortedDictionary<string, UseCard> UsingCardDic = new SortedDictionary<string, UseCard>();

    public SortedDictionary<string, NorEff> NormalEffect = new SortedDictionary<string, NorEff>();

    public void UseCard(Vector2Int position) //用卡
    {
        Debug.Log("StartUse");
            if ((isCharacterSelected && selectedCharacter.UsingCardDic.TryGetValue(GetCardString(), out UseCard useCard) && useCard.Use(position)) ||//选定了角色，则通过角色使用卡牌
                ((!isCharacterSelected) && UsingCardDic.TryGetValue(GetCardString(), out UseCard useCard1) && useCard1.Use(position))//未选定角色，则通过自身使用卡牌
                )//如果正常使用卡牌, 删除掉手牌中的对应卡牌
            {
            UseCardLog();
            Debug.Log("StartClearHandCard");
                Hand hands = hand.GetComponent<Hand>();
                battleArea.GetComponent<BattleArea>().CardUsed(selectedCard, cardCount);
                for (int i = 0; i < cardCount; i++)
                {
                    hands.DeleteCard(selectedCard[i]);
                }
                hands.Refresh();
                Debug.Log("UseCardSuccess");
            }
            else//使用卡牌出错
            {
                Debug.Log("位置出错");
            }
        if (isCharacterSelected) selectedCharacter.ShowNormalState();
        ResetButtonDown();
    }

    public void SendHash()
    {
        uint hash = MyHash();
        Debug.Log("Sendhash=" + hash.ToString());
        string hashstr = "";
        for (int i = 0; i < 8; i++)
        {
            uint tmp = hash%10;
            hashstr += (char)(tmp + '0');
            hash /= 10;
        }
        Debug.Log(hashstr);
        Send("BD" +  hashstr);
    }

    public bool IsHashRight(string msg)
    {
        uint hash=(uint) (msg[9]-'0');
        for(int i = 0; i < 7; i++)
        {
            hash *= 10;
            hash += (uint)(msg[8 - i] - '0');
        }
        Debug.Log("getedHash=" + hash.ToString());
        if (hash == MyHash()) return true;
        return false;
    }

    public void ResetButtonDown()//按下重置按钮
    {
        if (isTargeting)
        {
            if (isCharacterSelected)//将高亮地块恢复正常
            {
                if (!selectedCharacter.UsingCardDic.TryGetValue(GetCardString(), out UseCard useCard))
                {
                    Debug.Log("ResetError");
                    return;
                }
                ChangeUseCardPositionState(useCard, ButtonState.Normal);
                
            }
            else
            {
                if (!UsingCardDic.TryGetValue(GetCardString(), out UseCard useCard))
                {
                    Debug.Log("ResetError");
                    return;
                }
                ChangeUseCardPositionState(useCard, ButtonState.Normal);
            }
            isTargeting = false;
        }
        if (selectedCharacter != null)
        {
          ChangeAttackAreaState(selectedCharacter, ButtonState.Normal,isPlayer);
        }
        selectedCharacter = null;
        isCharacterSelected = false;
        hand.GetComponent<Hand>().Refresh();
        if (isPlayer)
        {
            characterInfo.GetComponent<CharacterInfo>().show(null);
        }
    }

    public void SelectCard(int index) {
        Hand obj = hand.GetComponent<Hand>();
        if (index >= obj.count) return;
        Card card =  obj.cardObjects[index].GetComponent<Card>();
        card.ForceChangeState(ButtonState.Selected);
    }//选择一张卡

    public void DeSelectCard(int index)
    {
        Hand obj = hand.GetComponent<Hand>();
        if (index >= obj.selectCount) return;
        Card card = obj.selectCards[index].GetComponent<Card>();
        card.ForceChangeState(ButtonState.Normal);
    }//取消选择一张卡

    public bool GetCard(int index) {
        return hand.GetComponent<Hand>().GetCard(index);
    }//抽取一张卡

    public void UpdateCard()//更新选择卡牌的情况
    {
        Card[] selectedCardtmp = hand.GetComponent<Hand>().selectCards;
        cardCount = hand.GetComponent<Hand>().selectCount;
        selectedCard = new Card[cardCount];
        for(int i = 0; i < cardCount; i++)
        {
            selectedCard[i] = selectedCardtmp[i];
        }
    }

    public CardCombination GetCardString()//获取选择的卡牌的序列
    {
        CardCombination str = new CardCombination();
        for(int i = 0; i < cardCount; i++)
        {
            str.AddCard(selectedCard[i].name);
        }
        Debug.Log(str);
        return str;
    }

    public void AddCharacter(GameObject character)//添加角色
    {
        myCharacters[characterCount++] = character.GetComponent<Character>();
    }

    public int TeaLimit = 0;

    public void DeleteTeaLimit()
    {
        TeaLimit--;
        if (TeaLimit < 1)
        {
            NormalEffect.Remove("TeaLimit");
        }
    }

    public void RemoveCharacter(Character character)
    {
        int index = characterCount - 1;
        for (; index > -1; index--)
        {
            if (myCharacters[index] == character) break;
        }
        if (index > -1)
        {
            for (; index < characterCount-1; index++)
            {
                myCharacters[index] = myCharacters[index + 1];
            }
            characterCount--;
        }
    }

    public void CharacterDead(GameObject character)//角色死亡
    {
        if(character.transform.parent = this.gameObject.transform)
        {
            Character cha = character.GetComponent<Character>();
            if (TeaLimit < 1 && hand.GetComponent<Hand>().FindCard("Item_Tea") > -1)
            {
                cha.HP = 1;
                cha.state = CharacterState.Alive;
                TeaLimit = 1;
                hand.GetComponent<Hand>().DeleteCard("Item_Tea");
                NormalEffect.Add("TeaLimit", DeleteTeaLimit);

                string log = "";
                if (isPlayer) log += "玩家 ";
                else log += "敌方玩家";
                log += " 对 " + cha.characterName + " 使用了 " + "Item_Tea";
                battleArea.GetComponent<BattleArea>().CardUsed("Item_Tea");
                Log(log);
            }
            else
            {
                if(selectedCharacter == cha)
                {
                    selectedCharacter = null;
                    isCharacterSelected = false;
                }
                RemoveCharacter(cha);
                statueEnergy--;
                ShowState();
                hand.GetComponent<Hand>().ForceAddCard(cha.characterName);
                Destroy(character);

                string log = "";
                if (isPlayer) log += "友方 ";
                else log += "敌方 ";
                log += cha.characterName + " 死亡 " ;
                Log(log);
            }
        }
    }

    public void LogCard()
    {
        string log = "当前手牌：#";
        Hand ha = hand.GetComponent<Hand>();
        for(int i = 0; i < ha.count; i++)
        {
            log += "+" + ha.cardObjects[i].GetComponent<Card>().cardName;
        }
        Log(log);
    }
    public void NextRoundDown()//下一回合按下
    {
        if (isMyRound)
        {
            isMyRound = false;
            ResetButtonDown();
            if (isPlayer)
            {
                round.GetComponent<Text>().text = "对方回合";
                Log("=====================对方回合======================");
            }
        }
        else
        {

        }
    }

    public void MyRound()//我的回合
    {
        if (isMyRound == false)
        {
            isMyRound = true;
            hand.GetComponent<Hand>().GetCard();
            hand.GetComponent<Hand>().GetCard();
            if (isPlayer)
            {
                round.GetComponent<Text>().text = "我的回合";
                Log("=====================我的回合======================");
            }
            LogCard();
            for(int i = 0; i < characterCount; i++)
            {
                myCharacters[i].NewRoundSettle();
                List<NorEff> norEffs = new List<NorEff>(NormalEffect.Values);
                foreach(NorEff norEff in norEffs)
                {
                    norEff();
                }
            }
        }
    }

    public void DeleteButtonDown()
    {
        if (isTargeting)
        {
            ResetButtonDown();
            return;
        }
        UpdateCard();
        Debug.Log("DeleteButtonDown");
        for(int i = 0; i < cardCount; i++)
        {
            if(string.CompareOrdinal(selectedCard[i].cardName, 0, "Character", 0, 9)==0)break;
            hand.GetComponent<Hand>().DeleteCard(selectedCard[i]);
        }
        hand.GetComponent<Hand>().Refresh();
        cardCount = 0;
    }

    public Hero GetCharacterWithPos(int row, int col) {
        Hero hero;
        for (int i = 0; i < characterCount; i++)
        {
            if (row == myCharacters[i].position.y && col == myCharacters[i].position.x)
            {
                hero = myCharacters[i] as Hero;
                return hero;
            }
        }
        return null;
    }
    public void AreaButtonDown(int row, int col)//当按钮按下时
    {
        if (isTargeting)
        {
            Vector2Int target = new Vector2Int(col, row);
            if (isCharacterSelected)
            {
                target -= selectedCharacter.position;
            }
            UseCard(target);
        }
        else
        {
            Hero select = GetCharacterWithPos(row, col);
            if (select != null)
            {
                if (selectedCharacter != null)
                {
                    ChangeAttackAreaState(selectedCharacter, ButtonState.Normal,isPlayer);
                }
                ChangeAttackAreaState(select, ButtonState.NormalHighlighted,isPlayer);
                selectedCharacter = select;
                isCharacterSelected = true;
                selectCharacterPosition = selectedCharacter.position;
                if (isPlayer)
                {
                    characterInfo.GetComponent<CharacterInfo>().show(select);
                }
            }
            else
            {
                Hero selectEnemy = battleArea.GetComponent<BattleArea>().GetEnemyCharacterWithPos(row, col,isPlayer);
                if (selectedCharacter != null)
                {
                    ChangeAttackAreaState(selectedCharacter, ButtonState.Normal,isPlayer);
                }
                selectedCharacter = null;
                isCharacterSelected = false;
                if (isPlayer)
                {
                    characterInfo.GetComponent<CharacterInfo>().show(selectEnemy);
                }
            }
            if(hand.TryGetComponent(out Hand ha))
            {
                if(ha.selectCount>0) UseButtonDown();
            }
            if (!isCharacterSelected && isTargeting)
            {
                AreaButtonDown(row, col);
            }
        }
    }
    public void ChangeAttackAreaState(Character character, ButtonState state,bool isPlayer) {
        if (character == null) return;
        int lenth = character.poses.Length;
        Vector2Int[] areas = new Vector2Int[lenth];
        for(int i = 0; i < lenth; i++)
        {
            areas[i] = character.poses[i]+character.position;
        }
        battleArea.GetComponent<BattleArea>().ButtonState(areas,  state , isPlayer);
    }
    public bool Send(string msg)
    {
        return battleArea.GetComponent<BattleArea>().Send(msg);
    }

    public void InitUseCard()
    {
        Vector2Int[] Poses = new Vector2Int[30];
        for(int i =1; i < 4; i++)
        {
            for(int j = 1; j < 11; j++)
            {
                Poses[(i - 1) * 10 + j - 1] = new Vector2Int(j, i);
            }
        }

        Vector2Int[] AllPosition = new Vector2Int[101];

        for(int i = 1; i < 11; i++)
        {
            for(int j = 1; j < 11; j++)
            {
                AllPosition[(i - 1) * 10 + j - 1] = new Vector2Int(j, i);
            }
        }
        AllPosition[100] = new Vector2Int(-1, -1);

        Vector2Int[] NeutalPositions = new Vector2Int[50];
        for (int i = 1; i < 6; i++)
        {
            for (int j = 1; j < 11; j++)
            {
                NeutalPositions[(i - 1) * 10 + j - 1] = new Vector2Int(j, i);
            }
        }

        void add(string str, UseFunc func, Vector2Int[] poses,bool needTag=true)
        {
            UseCard noelle = new UseCard
            {
                func = func,
                postions = poses,
                CanUse = DefaultCanUse,
                needTarget = needTag
            };
            UsingCardDic.Add(str, noelle);
        }

        add("#+Character_Noelle", PutNoelle, Poses);
        add("#+Character_Ningguang", PutNingguang, Poses);
        add("#+Character_Diluc", PutDiluc, Poses);
        add("#+Character_Lisa", PutLisa, Poses);
        add("#+Character_Keaya", PutKeaya, Poses);
        add("#+Character_Keqing", PutKeqing, Poses);
        add("#+Character_Amber", PutAmber, Poses);
        add("#+Character_Jean", PutJean, Poses);

        add("#+Item_Sunsettia", UseSunsettia, AllPosition);
        add("#+Item_Clock", UseClock, AllPosition);
        add("#+Item_Teaport", UseTeaport, AllPosition,false);
        add("#+Item_Trap", UseTrap, NeutalPositions);
        add("#+Item_Advice", UseAdvice, AllPosition,false);
        add("#+Item_Chill", UseChill, AllPosition);
        add("#+Item_Book", UseBook, AllPosition);
        add("#+Item_Sigil", UseSigl, AllPosition);
        add("#+Item_Sword", UseSword, AllPosition);
    }
    public bool TryGetCharacter(Vector2Int pos, out Hero hero)
    {
        hero = GetCharacterWithPos(pos.y, pos.x);
        if (hero == null) return false;
        return true;
    }
    public bool TryGetEnemy(Vector2Int pos, out Hero hero)
    {
        hero = null;
        pos = BattleArea.GetReverse(pos);
        Character[] characters = GetEnemyCharacters();
        foreach(Character character in characters)
        {
            if(character.position == pos)
            {
                hero = character as Hero;
                return true;
            }
        }
        return false;
    }

    public bool UseSunsettia(Vector2Int pos)
    {
        if (!TryGetCharacter(pos, out Hero hero)) return false;
        if (hero.SunsettiaLimit > 0) return false;

        hero.SelfHeal(2, 0);
        hero.AddSunsettiaLimit();
        return true;
    }

    public void UseCardLog()
    {
        string log = "";
        if (isPlayer) log += " 玩家: ";
        else log += " 敌方玩家: ";
        log += " 使用了卡  "+GetCardString();
        Log(log);
    }

    public bool UseClock(Vector2Int pos)
    {
        if (!TryGetCharacter(pos, out Hero hero)) return false;
        UseCardLog();
        hand.GetComponent<Hand>().DeleteCard("Item_Clock");
        hand.GetComponent<Hand>().Refresh();
        battleArea.GetComponent<BattleArea>().CardUsed("Item_Clock");
        hand.GetComponent<Hand>().GetCard();
        hero.stamina++;
        hero.AddClockLimit();
        return false;
    }

    public bool UseTeaport(Vector2Int pos)
    {
        Character[] characters = GetAllCharacters();
        foreach(Character character in characters)
        {
            character.SelfHeal(1, 0);
        }

        return true;
    }

    public bool DefaultCanUse()
    {
        return true;
    }

    public bool UseTrap(Vector2Int pos)
    {
        if (TryGetEnemy(pos, out Hero hero)) return false;
        Trap.CreatTrap(this.gameObject, pos).name = "Trap";
        return true;
    }

    public GameObject[] DisabledCards = new GameObject[6];
    public int disabledCardsCount = 0;

    public bool UseAdvice(Vector2Int pos)
    {
        Hand ha = hand.GetComponent<Hand>();
        UseCardLog();
        ha.DeleteCard("Item_Advice");
        hand.GetComponent<Hand>().Refresh();
        battleArea.GetComponent<BattleArea>().CardUsed("Item_Advice");
        ha.GetCard();
        DisabledCards[disabledCardsCount++] = ha.cardObjects[ha.count - 1];
        ha.GetCard();
        DisabledCards[disabledCardsCount++] = ha.cardObjects[ha.count - 1];
        DisabledCards[disabledCardsCount-2].GetComponent<Card>().ForceChangeState(ButtonState.Disabled);
        DisabledCards[disabledCardsCount-1].GetComponent<Card>().ForceChangeState(ButtonState.Disabled);
        NormalEffect.Add("Advice", EnableCard);
        return false;
    }
    public void EnableCard()
    {
        for(int i = 0; i < disabledCardsCount; i++)
        {
            if(DisabledCards[i]!=null) DisabledCards[i].GetComponent<Card>().ForceChangeState(ButtonState.Normal);
        }
        disabledCardsCount = 0;
        NormalEffect.Remove("Advice");
    }

    public bool UseChill(Vector2Int pos)
    {
        if (!TryGetEnemy(pos, out Hero hero)) return false;
        if (hero.affected == null)
        {
            hero.affected = new ElementalAffect
            {
                affectElemental = ElementType.Pyro
            };
        }
        else
        {
            GameObject obj = GameBase.CreatObject<Attack>(gameObject);
            Attack atk = obj.GetComponent<Attack>();
            atk.attackelemental = ElementType.Pyro;
            Character.ElementalReactionFunc(hero, atk);
            Destroy(obj);
        }
        hero.ShowNormalState();
        return true;
    }

    public bool UseBook(Vector2Int pos)
    {
        if (!TryGetCharacter(pos, out Hero hero)) return false;

        if (hero.heroType != HeroType.Mage) return false;

        string cardName;
        switch (hero.element)
        {
            case ElementType.Anemo:
                cardName = "Normal_Anemo";
                break;
            case ElementType.Geo:
                cardName = "Normal_Geo";
                break;
            case ElementType.Hydro:
                cardName = "Normal_Hydro";
                break;
            case ElementType.Pyro:
                cardName = "Normal_Pyro";
                break;
            case ElementType.Electro:
                cardName = "Normal_Electro";
                break;
            case ElementType.Cryo:
                cardName = "Normal_Cyro";
                break;
            case ElementType.Dendro:
                cardName = "Normal_Dendro";
                break;
            default:
                cardName = "";
                break;
        }
        UseCardLog();
        hand.GetComponent<Hand>().DeleteCard("Item_Book");
        hand.GetComponent<Hand>().Refresh();
        battleArea.GetComponent<BattleArea>().CardUsed("Item_Book");
        hand.GetComponent<Hand>().GetCard(cardName);
        hand.GetComponent<Hand>().GetCard(cardName);
        return false;
    }

    public bool UseSword(Vector2Int pos)
    {
        if (!TryGetEnemy(pos, out Hero hero)) return false;

        for(int i=0;i<characterCount;i++)
        {
            Character character = myCharacters[i];
            Hero hero1 = character as Hero;
            if(hero1.heroType == HeroType.Sword)
            {
                hero1.TryNormalAttack(BattleArea.GetReverse(hero.position + hero1.position));
            }
        }
        return true;
    }

    public bool UseSigl(Vector2Int pos)
    {
        Sigil.CreatSigil(gameObject, pos);
        return true;
    }

    public bool PutNoelle(Vector2Int pos)
    {
        if (TryGetCharacter(pos, out Hero _)) return false;
        if (TryGetEnemy(pos, out Hero _)) return false;
        GameObject obj = GameBase.CreatObject<Noelle>(gameObject);
        PutHero(pos, "Character_Noelle", obj);
        return true;
    }

    public bool PutNingguang(Vector2Int pos)
    {
        if (TryGetCharacter(pos, out Hero _)) return false;
        if (TryGetEnemy(pos, out Hero _)) return false;
        GameObject obj = GameBase.CreatObject<Ningguang>(gameObject);
        PutHero(pos, "Character_Ningguang", obj);

        return true;
    }

    public bool PutDiluc(Vector2Int pos)
    {
        if (TryGetCharacter(pos, out Hero _)) return false;
        if (TryGetEnemy(pos, out Hero _)) return false;
        GameObject obj = GameBase.CreatObject<Diluc>(gameObject);
        PutHero(pos, "Character_Diluc", obj);
        return true;
    }

    public bool PutLisa(Vector2Int pos)
    {
        if (TryGetCharacter(pos, out Hero _)) return false;
        if (TryGetEnemy(pos, out Hero _)) return false;
        GameObject obj = GameBase.CreatObject<Lisa>(gameObject);
        PutHero(pos, "Character_Lisa", obj);
        return true;
    }

    public bool PutKeaya(Vector2Int pos)
    {
        if (TryGetCharacter(pos, out Hero _)) return false;
        if (TryGetEnemy(pos, out Hero _)) return false;
        GameObject obj = GameBase.CreatObject<Keaya>(gameObject);
        PutHero(pos, "Character_Keaya", obj);
        return true;
    }

    public bool PutKeqing(Vector2Int pos)
    {
        if (TryGetCharacter(pos, out Hero _)) return false;
        if (TryGetEnemy(pos, out Hero _)) return false;
        GameObject obj = GameBase.CreatObject<Keqing>(gameObject);
        PutHero(pos, "Character_Keqing", obj);
        return true;
    }

    public bool PutAmber(Vector2Int pos)
    {
        if (TryGetCharacter(pos, out Hero _)) return false;
        if (TryGetEnemy(pos, out Hero _)) return false;
        GameObject obj = GameBase.CreatObject<Amber>(gameObject);
        PutHero(pos, "Character_Amber", obj);
        return true;
    }

    public bool PutJean(Vector2Int pos)
    {
        if (TryGetCharacter(pos, out Hero _)) return false;
        if (TryGetEnemy(pos, out Hero _)) return false;
        GameObject obj = GameBase.CreatObject<Jean>(gameObject);
        PutHero(pos, "Character_Jean", obj);
        return true;
    }

    public string StringGet()
    {
        string sms = ""+ (char)statueEnergy + (char)stamina /100+ (char)stamina %100;
        return 'A'+hand.GetComponent<Hand>().StringGet()+'|'+'B'+deck.GetComponent<Deck>().StringGet()+'|'+'C'+sms;
    }
    public void StringSet(string msg)
    {
        if (msg.Length < 2) return;
        string[] msgs = msg.Split('|');
        foreach (string str in msgs)
        {
            if (str.Length < 1) continue;
            switch (str[0])
            {
                case 'A':
                    hand.GetComponent<Hand>().StringSet(str.Substring(1));
                    break;
                case 'B':
                    deck.GetComponent<Deck>().StringSet(str.Substring(1));
                    break;
                case 'C':
                    statueEnergy = str[1];
                    stamina = str[2]*100+str[3];
                    break;
                default:
                    break;
            }
        }
    }
    public void PutHero(Vector2Int pos, string name, GameObject obj)
    {
        obj.transform.parent = this.gameObject.transform;
        obj.name = name;
        Hero hero = obj.GetComponent<Hero>();
        hero.Init(Hero.GetHeroWithString(name));
        hero.parent = this.gameObject;
        hero.sprites = GameObject.Find("Sprites");
        hero.Heroinit();
        hero.transform.localPosition = BattleArea.GetLocalPosition(pos);
        hero.MoveTo(pos);
        SpriteRenderer rend = obj.GetComponent<SpriteRenderer>();
        rend.sprite = hero.appearance;
        obj.transform.localScale = new Vector3(1, 1, 1);
        obj.transform.localRotation = new Quaternion(0, 0, 0, 0);
        if (!isPlayer) obj.transform.localRotation = new Quaternion(0,0,90,0);
        AddCharacter(obj);
        hero.ShowNormalState();
    }

    public void DestroyAll()
    {
        for(int i = 0; i < characterCount; i++)
        {
            Destroy(myCharacters[i].gameObject);
        }
        characterCount = 0;
    }

    public uint MyHash()
    {
        
        int hash = hand.GetComponent<Hand>().myHash();
        for(int i = 0; i < characterCount; i++)
        {
            hash += myCharacters[i].position.GetHashCode();
            hash += myCharacters[i].HP.GetHashCode();
            hash += myCharacters[i].shield.GetHashCode();
        }
        return (uint)(hash%100000000);
        
    }

    public Character[] GetAllCharacters()
    {
        return battleArea.GetComponent<BattleArea>().GetAllCharacters();
    }

    public Character[] GetEnemyCharacters()
    {
        return battleArea.GetComponent<BattleArea>().GetEnemyCharacters(isPlayer);
    }

}
