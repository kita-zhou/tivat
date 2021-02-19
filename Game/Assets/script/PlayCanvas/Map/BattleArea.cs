using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum CanvasState { Main, Esc, Select}

public class BattleArea : MonoBehaviour
{
    public void Initial()//初始化
    {
        area = new GameObject[xmax][];
        for (int i = 0; i < xmax; i++) area[i] = new GameObject[ymax];
        string head = "MainCanvas/战斗区域/";
        for(int i=1;i<xmax+1;i++)
            for(int j = 1; j < ymax+1; j++)
            {
                area[i-1][j-1] = GameObject.Find(head + "row" + j.ToString() + "/col" + i.ToString());
            }
        staticarea = area;
        zeropos = transform.localPosition;
        sprites = GameObject.Find("Sprites").GetComponent<AllSprites>();
        audios = GameObject.Find("Audio").GetComponent<AllAudio>();
        fonts = GameObject.Find("Fonts").GetComponent<AllFonts>();
        player = playerObject.GetComponent<Player>();
        enemy = enemyPlayerObject.GetComponent<Player>();
        battleArea = this;
    }

    public void PlayerInitial(GameObject player,string[] set)//初始化玩家类
    {
        Hand hand = player.GetComponent<Player>().hand.GetComponent<Hand>();
        Deck deck = player.GetComponent<Player>().deck.GetComponent<Deck>();
        for (int i = 0; i < 4; i++)
        {
            hand.AddCard(Card.GetCard(set[i]));
            Hero hero = Hero.GetHeroWithString(set[i]);
            for(int j= 0; j < 15; j++)
            {
                deck.PutCard(hero.BasicCards[j]);
            }
        }
        for(int i = 0; i < 20; i++)
        {
            deck.PutCard(set[i] + 4);
        }
    }

    public bool isMianPlayer=false;

    public GameObject playerObject;
    public GameObject enemyPlayerObject;
    public GameObject nuetralEnemyObject;

    public GameObject playCanvas;

    public GameObject[][] area;

    public static readonly int xmax = 10;
    public static readonly int ymax = 10;

    public static AllSprites sprites;
    public static AllAudio audios;
    public static AllFonts fonts;
    public static Player player;
    public static Player enemy;

    public static BattleArea battleArea;

    public GameObject selectButton=null;

    public GameObject enemySelectButton = null;
    public void AreaSelect(int row, int column)//按钮选择
    {
        if (selectButton == null)
        {
            selectButton = area[column - 1][row-1];
        }
        if (selectButton != area[column-1][row-1]) {
            selectButton.GetComponent<BattleButton>().ChangeState(global::ButtonState.Normal);
            selectButton = area[column-1][row-1];
        }
        playerObject.GetComponent<Player>().AreaButtonDown(row, column);
        char rowchar = (char)('0' + row);
        char columnchar =(char)('0' + column);
        Send("BAA" + rowchar+ columnchar);
        SendHash();
    }
    public void EnemyAreaSelect(int row, int column) {//敌方按钮选择
        if(enemySelectButton == null)
        {
            enemySelectButton = area[xmax-column ][ymax-row];
            enemySelectButton.GetComponent<BattleButton>().ChangeState(global::ButtonState.Selected);
        }
        if (enemySelectButton != area[xmax-column][ymax-row])
        {
            enemySelectButton.GetComponent<BattleButton>().ChangeState(global::ButtonState.Normal);
            enemySelectButton = area[xmax-column ][ymax-row];
            enemySelectButton.GetComponent<BattleButton>().ChangeState(global::ButtonState.Selected);
        }
        enemyPlayerObject.GetComponent<Player>().AreaButtonDown(row, column);
    }
    
    public void ButtonState(Vector2Int[] poses, ButtonState state,bool isPlayer)//批量更改按键状态
    {
        if (isPlayer)
        {
            Debug.Log(poses.Length);
            for (int i = 0; i < poses.Length; i++)
            {
                if(poses[i].x>0&&poses[i].x<xmax+1&&poses[i].y>0&&poses[i].y<ymax+1)
                    area[poses[i].x - 1][poses[i].y - 1].GetComponent<BattleButton>().ChangeState(state);
            }
        }
        else
        {
            for (int i = 0; i < poses.Length; i++)
            {
                if (poses[i].x > 0 && poses[i].x < xmax+1 && poses[i].y > 0 && poses[i].y < ymax+1)
                    area[xmax-poses[i].x ][ymax-poses[i].y].GetComponent<BattleButton>().ChangeState(state);
            }
        }
    }

    public void ButtonState(Vector2Int pos, ButtonState state, bool isPlayer)//更改单个按键状态
    {
        if (isPlayer)
        {
            if(pos.x>0&&pos.x<7&&pos.y>0&&pos.y<7)
                area[pos.x - 1][pos.y - 1].GetComponent<BattleButton>().ChangeState(state);
        }
        else
        {
            if (pos.x > 0 && pos.x < 7 && pos.y > 0 && pos.y < 7)
                area[6-pos.x ][6-pos.y ].GetComponent<BattleButton>().ChangeState(state);
        }
    }

    public static Vector2 zeropos;
    public static GameObject[][] staticarea;
    public static Vector3 GetLocalPosition(Vector2Int pos) {
        return new Vector3(pos.x-5.5f,pos.y-5.5f,0);
    }
    public static Vector3 GetLocalPosition(Vector2 pos)
    {
        return GetLocalPosition(new Vector2Int((int)pos.x,(int)pos.y));
    }
    public static Vector2Int GetReverse(Vector2Int pos)
    {
        return new Vector2Int(xmax - pos.x+1, ymax - pos.y+1);
    }

    public static Vector2Int GetPos(Vector2 pos)
    {
        return new Vector2Int((int)pos.x,(int)pos.y);
    }

    void Start()
    {
        Initial();
    }

    public GameObject mainCamera;

    public string GetArea()
    {
        return "";
    }

    public void SetArea(string msg)
    {

    }

    public void Log(string msg)
    {
        playCanvas.GetComponent<PlayCanvas>().Log(msg);
    }

    void ChangeCanvas(CanvasState state)
    {
        switch (state)
        {
            case CanvasState.Main:
                mainCamera.transform.localPosition = new Vector3(0, 0, 0);
                break;
            case CanvasState.Esc:
                mainCamera.transform.localPosition = new Vector3(0, 200, 0);
                break;
            default:
                break;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            UIButtonClick(UIButtonType.Reset);
        }
    }

    public Hero GetEnemyCharacterWithPos(int row, int column,bool isPlayer)//获取敌方是否在该位置有角色
    {
        Vector2Int pos = GetReverse(new Vector2Int(column, row));
        if (isPlayer)
        {
            return enemyPlayerObject.GetComponent<Player>().GetCharacterWithPos(pos.y,pos.x);
        }
        else
        {
            return playerObject.GetComponent<Player>().GetCharacterWithPos(pos.y, pos.x);
        }
    }

    public Character[] GetAllCharacters()//获取所有角色
    {
        int playerCount = playerObject.GetComponent<Player>().characterCount;
        int enemyCount = enemyPlayerObject.GetComponent<Player>().characterCount;
        int neutralCount = nuetralEnemyObject.GetComponent<NeutralEnemy>().characterCount;
        Character[] plyers = playerObject.GetComponent<Player>().myCharacters;
        Character[] enemys = enemyPlayerObject.GetComponent<Player>().myCharacters;
        Character[] neutals = nuetralEnemyObject.GetComponent<NeutralEnemy>().characters;

        Character[] all = new Character[playerCount + enemyCount + neutralCount];
        for (int i = 0; i < playerCount; i++) all[i] = plyers[i];
        for (int i = 0; i < enemyCount; i++) all[i + playerCount] = enemys[i];
        for (int i = 0; i < neutralCount; i++) all[i + playerCount + enemyCount] = neutals[i];

        return all;
    }

    public Character[] GetEnemyCharacters(bool isPlayer)
    {
        int neutralCount = nuetralEnemyObject.GetComponent<NeutralEnemy>().characterCount;
        Character[] neutals = nuetralEnemyObject.GetComponent<NeutralEnemy>().characters;
        int playerCount;
        Character[] players;
        if (isPlayer)
        {
            playerCount = enemyPlayerObject.GetComponent<Player>().characterCount;
            players = enemyPlayerObject.GetComponent<Player>().myCharacters;
        }
        else
        {
            playerCount = playerObject.GetComponent<Player>().characterCount;
            players = playerObject.GetComponent<Player>().myCharacters;
        }
        Character[] all = new Character[playerCount + neutralCount];
        for (int i = 0; i < playerCount; i++) all[i] = players[i];
        for (int i = 0; i < neutralCount; i++) all[i + playerCount] = neutals[i];

        return all;
    }//获取所有敌方+中立角色
    public void UIButtonClick(UIButtonType type)
    {
        Debug.Log(type.ToString());
        switch (type)
        {
            case UIButtonType.Reset:
                playerObject.GetComponent<Player>().ResetButtonDown();
                Send("BABB");
                break;
            case UIButtonType.Use:
                playerObject.GetComponent<Player>().UseButtonDown();
                Send("BABC");
                break;
            case UIButtonType.NextRound:
                playerObject.GetComponent<Player>().NextRoundDown();
                enemyPlayerObject.GetComponent<Player>().MyRound();
                Send("BABA");
                break;
            default:
                break;
        }
        SendHash();
    }

    public void FreeMoveClick()
    {
        playerObject.GetComponent<Player>().MoveButtonDown();
        Send("BABD");
        SendHash();
    }

    public void EnemyMoveClick()
    {
        enemyPlayerObject.GetComponent<Player>().MoveButtonDown();
    }

    public void DeleteCardClick() {
        playerObject.GetComponent<Player>().DeleteButtonDown();
        Send("BABE");
        SendHash();
    }
    public void EnemyDeleteClick() {
        enemyPlayerObject.GetComponent<Player>().DeleteButtonDown();
    }

    public GameObject cardUsed;

    public void ShowMessage(string msg)
    {

    }

    public void CardUsed(string[] cards, int num)
    {
        Card[] cards1 = new Card[num];
        for (int i = 0; i < num; i++) cards1[i] = Card.GetCard(cards[i]);
        CardUsed(cards1, num);
    }

    public void CardUsed(Card[] cards,int num)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject obj = GameBase.CreatObject<UsedCard>(cardUsed);
            UsedCard usedCard = obj.GetComponent<UsedCard>();
            usedCard.aliveTime = i * 0.5f + 0.5f;
            usedCard.ChangeApprence(cards[i].front);
            int count = cardUsed.transform.childCount;
            usedCard.target = new Vector3(3.5f, 0, -count*0.1f);
        }
    }

    public void CardUsed(Card card)
    {
        GameObject obj = GameBase.CreatObject<UsedCard>(cardUsed);
        UsedCard usedCard = obj.GetComponent<UsedCard>();
        usedCard.aliveTime = 0.5f;
        usedCard.ChangeApprence(card.front);
        int count = cardUsed.transform.childCount;
        usedCard.target = new Vector3(3.5f, 0, -count * 0.1f);
    }

    public void CardUsed(string name)
    {
        Card card = Card.GetCard(name);
        CardUsed(card);
    }

    public void EnemyUIButtonClick(UIButtonType type)
    {
        switch (type)
        {
            case UIButtonType.Reset:
                enemyPlayerObject.GetComponent<Player>().ResetButtonDown();
                break;
            case UIButtonType.Use:
                enemyPlayerObject.GetComponent<Player>().UseButtonDown();
                break;
            case UIButtonType.NextRound:
                enemyPlayerObject.GetComponent<Player>().NextRoundDown();
                playerObject.GetComponent<Player>().MyRound();
                break;
            default:
                break;
        }
    }

    public bool Send(string msg)
    {
        return playCanvas.GetComponent<PlayCanvas>().client.Send(msg);
    }

    public void SendBE()
    {
        Send("BE");
    }

    public bool IsHashRight(string msg)
    {
        uint hash = (uint)(msg[9] - '0');
        for (int i = 0; i < 7; i++)
        {
            hash *= 10;
            hash += (uint)(msg[8 - i] - '0');
        }
        Debug.Log("getedHash=" + hash.ToString());
        if (hash == MyHash()) return true;
        return false;
    }

    public void SendHash()
    {
        uint hash = MyHash();
        Debug.Log("Sendhash=" + hash.ToString());
        string hashstr = "";
        for (int i = 0; i < 8; i++)
        {
            uint tmp = hash % 10;
            hashstr += (char)(tmp + '0');
            hash /= 10;
        }
        Debug.Log(hashstr);
        Send("BD" + hashstr);
    }
    public uint MyHash()
    {
        return (playerObject.GetComponent<Player>().MyHash() + enemyPlayerObject.GetComponent<Player>().MyHash())%(uint)1000000;
    }

    public static GameObject ShowMessage(string msg, Vector3 localPos, float scale = 0.01f, float liveTime = 0.5f)
    {
        return Massage.CreateMsg(msg, localPos, battleArea.gameObject, scale, liveTime);
    }
    public static GameObject ShowMessage(string msg, Vector3 localPos, Color color, float scale = 0.01f, float liveTime = 0.5f)
    {
        GameObject obj = Massage.CreateMsg(msg, localPos, battleArea.gameObject, scale, liveTime);
        obj.GetComponent<TMPro.TextMeshPro>().color = color;
        return obj;
    }
    public static GameObject ShowImage(Sprite img, Vector3 localPos, float scale = 1, float liveTime = 0.5f)
    {
        return Massage.CreateMsg(img, localPos,battleArea.gameObject, scale, liveTime);
    }


    public void ExitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Defeat()
    {
        Send("AD");
        ShowMessage("Defeat", new Vector3(4, 0, -10), new Color(1, 0.5f, 0), 0.6f, 2);
        Invoke("ExitGame", 2);
    }

    public void Win()
    {
        ShowMessage("Win", new Vector3(4, 0, -10), new Color(1,0.5f,0),0.6f, 2);
        Invoke("ExitGame", 2);
    }
}//战斗区域类
