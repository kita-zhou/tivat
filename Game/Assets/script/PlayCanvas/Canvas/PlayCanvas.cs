using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;
using System.Threading;
using System.Text;
using UnityEngine.SceneManagement;
public enum GameState {Start, Match, Play }//未加入匹配/匹配/游戏中
public class PlayCanvas : MonoBehaviour
{
    public string Version = "1001";

    public GameObject player;
    public GameObject enemyPlayer;
    
    public GameObject battleArea;

    public FileSocket client;

    public GameState state;

    public string playerPath = "save/build/select";

    public AudioSource audioSource;
    public AudioClip qingce;
    public AudioClip bar;

    public GameObject ShowMessage(string msg, Vector3 localPos, float scale = 0.01f, float liveTime = 0.5f)
    {
        return Massage.CreateMsg(msg, localPos, battleArea, scale, liveTime);
    }
    public GameObject ShowMessage(string msg, Vector3 localPos,Color color ,float scale = 0.01f, float liveTime = 0.5f)
    {
        GameObject obj =  Massage.CreateMsg(msg, localPos, battleArea, scale, liveTime);
        obj.GetComponent<Text>().color = color;
        return obj;
    }
    public GameObject ShowImage(Sprite img, Vector3 localPos, float scale = 1, float liveTime = 0.5f)
    {
        return  Massage.CreateMsg(img, localPos, battleArea, scale, liveTime);
    }

    public void ChangeState(GameState state) {
        switch (state)
        {
            case GameState.Match:
                if(state == GameState.Start)
                {
                    this.state = state;
                    audioSource.clip = bar;
                }
                break;
            case GameState.Play:
                if(state == GameState.Match)
                {
                    string msg = "BC";
                    if(ReadFile(out string[] str, playerPath))
                    {
                        this.state = state;
                        msg += str[0];
                        for(int i = 1; i < str.Length; i++)
                        {
                            msg += "+";
                            msg += str[i];
                        }
                        client.Send(msg);
                    }
                    audioSource.clip = qingce;
                }
                break;
            case GameState.Start:
                if(state == GameState.Match)
                {
                    this.state = state;
                }
                else if(state == GameState.Play)
                {
                    battleArea.GetComponent<BattleArea>().Win();
                    this.state = state;
                }
                break;
        }
    }

    public void MessageProcess()
    {
        if(client.TryGetMsg(out string msg))
        {
            Debug.Log(msg.Length);
            Debug.Log(msg);
            switch(msg[0]){
                case 'A':
                    MessageTypeA(msg);
                    break;
                case 'B':
                    MessageTypeB(msg);
                    break;
                case 'S':
                    MessageTypeS(msg);
                    break;
                default:
                    break;
            }
        }
    }

    public void MessageProcess(string msg)
    {
        Debug.Log(msg);
        switch (msg[0])
        {
            case 'A':
                MessageTypeA(msg);
                break;
            case 'B':
                MessageTypeB(msg);
                break;
            case 'S':
                MessageTypeS(msg);
                break;
            default:
                break;
        }
    }

    public void MessageTypeA(string msg) {
        switch (msg[1])
        {
            case 'A'://匹配
                ChangeState(GameState.Match);
                ChangeMessage("匹配中...");
                MessageTail(msg, 2);
                break;
            case 'B'://停止匹配
                ChangeState(GameState.Start);
                MessageTail(msg, 2);
                break;
            case 'C'://开始游戏
                ChangeState(GameState.Play);
                SendInit();
                if(msg[2] == 'A')
                {
                    battleArea.GetComponent<BattleArea>().isMianPlayer = true;
                    player.GetComponent<Player>().isMyRound = true;
                    ChangeMessage("我的回合");
                    enemyPlayer.GetComponent<Player>().NextRoundDown();
                }
                else if(msg[2] == 'B')
                {
                    battleArea.GetComponent<BattleArea>().isMianPlayer = false;
                    player.GetComponent<Player>().NextRoundDown();
                    enemyPlayer.GetComponent<Player>().isMyRound = true;
                }
                MessageTail(msg, 3);
                break;
            case 'D'://被停止
                ChangeState(GameState.Start);
                battleArea.GetComponent<BattleArea>().Win();
                MessageTail(msg, 2);
                break;
            case 'E'://统一随机数种子
                RandomSeedSet(msg);
                break;
            default:
                break;
        }

    }//服务器信号/改变状态

    public void RandomSeedSet(string msg)
    {
        int seed = 0;
        for(int i = 0; i < 6; i++)
        {
            int times = 1;
            for (int j = 0; j < i; j++) times *= 10;
            seed +=times *( msg[7 - i] - '0');
        }
        Random.InitState(seed);
        Debug.Log("SeedSetTo:" + seed.ToString());
        MessageTail(msg, 8);
    }

    public void MessageTail(string msg, int n)
    {
        if (msg.Length > n)
        {
            msg = msg.Remove(0, n + 1);
            if (msg.Length > 0)
                MessageProcess(msg);
        }
    }

    public void PosSelect(string msg) {
        int row = msg[3]-'0';
        int column = msg[4]-'0';
        battleArea.GetComponent<BattleArea>().EnemyAreaSelect(row, column);
        MessageTail(msg, 5);
    }

    public void ControlButton(string msg) {
        switch (msg[3])
        {
            case 'A':
                battleArea.GetComponent<BattleArea>().EnemyUIButtonClick(UIButtonType.NextRound);
                break;
            case 'B':
                battleArea.GetComponent<BattleArea>().EnemyUIButtonClick(UIButtonType.Reset);
                break;
            case 'C':
                battleArea.GetComponent<BattleArea>().EnemyUIButtonClick(UIButtonType.Use);
                break;
            case 'D':
                battleArea.GetComponent<BattleArea>().EnemyMoveClick();
                break;
            case 'E':
                battleArea.GetComponent<BattleArea>().EnemyDeleteClick();
                break;
            default:
                break;
        }
        MessageTail(msg, 4);
    }

    public void CardSelect(string msg) {
        int index = msg[4]-'0';
        switch (msg[3])
        {
            case 'A':
                enemyPlayer.GetComponent<Player>().SelectCard(index);
                break;
            case 'B':
                enemyPlayer.GetComponent<Player>().DeSelectCard(index);
                break;
            default:
                break;
        }
        MessageTail(msg, 5);
    }

    public void CardGet(string msg) {
        int index10 = msg[3]-'0';
        int index1 = msg[4] - '0';
        int index = index10 * 10 + index1;
        enemyPlayer.GetComponent<Player>().GetCard(index);
        MessageTail(msg, 5);
    }

    public void InitialMsg(string msg) {
        msg =  msg.Remove(0, 2);
        string[] enemy = msg.Split('+');
        if (ReadFile(out string[] player, playerPath)) InitPlayer(player, enemy);
    }

    public void MessageTypeB(string msg) {
        switch (msg[1])
        {
            case 'A':
                switch (msg[2])
                {
                    case 'A':
                        PosSelect(msg);
                        break;
                    case 'B':
                        ControlButton(msg);
                        break;
                    default:
                        break;
                }
                break;
            case 'B':
                switch (msg[2])
                {
                    case 'A':
                        CardSelect(msg);
                        break;
                    case 'B':
                        CardGet(msg);
                        break;
                    default:
                        break;
                }
                break;
            case 'C':
                InitialMsg(msg);
                break;
            case 'D':
                HashCompare(msg);
                break;
            case 'E':
                SendString();
                break;
            case 'F':
                SetString(msg.Substring(2));
                break;
            default:
                break;
        }

    }//客户端信号/敌方操作

    public void SendString() {
        string msg = "D";
        client.Send("BF" + msg);
        Thread.Sleep(100);
        msg =""+ 'A' + BattleArea.player.StringGet();
        client.Send("BF" + msg);
        for (int i = 0; i < BattleArea.player.characterCount; i++)
        {
            Thread.Sleep(100);
            msg = "C" + BattleArea.player.myCharacters[i].StringGet();
            client.Send("BF" + msg);
        }
        Thread.Sleep(100);
        msg = ""+'B' + BattleArea.enemy.StringGet();
        client.Send("BF" + msg);
        for (int i = 0; i < BattleArea.enemy.characterCount; i++)
        {
            Thread.Sleep(100);
            msg = "C" + BattleArea.enemy.myCharacters[i].StringGet();
            client.Send("BF" + msg);
        }
        Thread.Sleep(100);
        client.Send("BFE");
    }
    public void SetString(string msg)
    {
        Debug.Log("canvas        ");
            if (msg.Length < 1) return;
            Debug.Log("canvasstr      " + msg);
            switch (msg[0])
            {
                case 'B':
                    BattleArea.player.StringSet(msg.Substring(1));
                    break;
                case 'A':
                    BattleArea.enemy.StringSet(msg.Substring(1));
                    break;
                case 'C':
                    string str = msg.Substring(1);
                Character.StaticStringSet(str);
                    break;
            case 'D':
                BattleArea.player.DestroyAll();
                BattleArea.enemy.DestroyAll();
                break;
            case 'E':
                break;
                default:
                    break;
            }
    }

    public bool hashIncorrect = false;
    public void HashCompare(string msg)
    {
        uint hash = battleArea.GetComponent<BattleArea>().MyHash();
        Debug.Log("MyHash=" + hash.ToString());
        bool is1 = battleArea.GetComponent<BattleArea>().IsHashRight(msg);
        Debug.Log("IsHashRight=" + is1.ToString());
        if (!is1)
        {
            if (hashIncorrect)
            {
                ChangeMessage("失去同步.....");
            }
            else
                hashIncorrect = true;
        }
        else
        {
            if (hashIncorrect)
            {
                ChangeMessage("重新同步");
                hashIncorrect = false;
            }
        }
    }

    public void MessageTypeS(string msg) {
        ChangeMessage("服务器拒绝，请检查版本更新");
    }//停止信号
    public bool InitPlayer(string[] plyer, string[] enemy)//从字符串数组初始化两个player的数据
    {
        if (player.GetComponent<Player>().InitFromString(plyer) &&
        enemyPlayer.GetComponent<Player>().InitFromString(enemy)) return true;//如果初始化成功
        return false;
    }

    public bool InitMyPlayer(string[] vs)
    {
        if (player.GetComponent<Player>().InitFromString(vs)) return true;
        return false;
    }

    public bool InitEnemy(string[] vs)
    {
        if (enemyPlayer.GetComponent<Player>().InitFromString(vs)) return true;
        return false;
    }

    public bool SendInit()
    {
        if( ReadFile(out string[] vs, playerPath))
        {
            string msg = "BC";
            msg += vs[0];
            for (int i = 1; i < vs.Length; i++)
            {
                msg +="+" + vs[i];
            }
            Debug.Log(msg);
            Debug.Log(msg.Length);
            client.Send(msg);
            return true;
        }
        return false;
    }

    public bool ReadFile(out string[] str,string path)
    {
        Debug.Log("InitStart");
        str = null;
        if (!File.Exists(path)) return false;
        BinaryReader br = new BinaryReader(File.OpenRead(path));
        if((ListType)br.ReadInt32()!=ListType.Selected)return false ;
        if(br.ReadInt32()!=24)return false;
        Debug.Log("Point1");
        str = new string[24];
        for(int i = 0; i < 24; i++)
        {
            str[i] = br.ReadString();
        }
        if (br.ReadInt32() != 4) return false;
        if (br.ReadInt32() != 20) return false;
        Debug.Log("InitEnd");
        return true;
    }

    public void SocketExit()
    {
        client.Send("AD");
        client.Send("AA");
    }

    public void ChangeMessage(string msg)
    {
        player.GetComponent<Player>().round.GetComponent<Text>().text = msg;
    }
    public string logpath;
    public string filename;
    public FileStream logStream;
    public void Log(string msg) {
        TextWriter writer = null;
        while (writer == null)
        {
            try
            {
                writer = File.AppendText(Path.Combine(logpath, filename));
            }
            catch
            {
                writer = null;
                Thread.Sleep(100);
            }
        }
        writer.Write(msg);
        writer.Write('\n');
        writer.Close();
        writer.Dispose();
    }
    ~PlayCanvas()
    {
        client.Send("S");
    }

    public void ExitScene()
    {
        client.Send("S");
        SceneManager.LoadScene("MainMenu");
    }

    private void Start()
    {
        if(Application.platform.Equals(RuntimePlatform.WindowsEditor) || Application.platform.Equals(RuntimePlatform.WindowsPlayer))
        {
            logpath = "save/log";
            playerPath = "save/build/select";
        }
        else
        {
            logpath = Application.persistentDataPath + "/save/log";
            playerPath = Application.persistentDataPath + "/save/build/select";
        }
        filename = $"Log{System.DateTime.Now:yyyy-MM-dd HH-mm}.txt";
        if (!Directory.Exists(logpath))
            Directory.CreateDirectory(logpath);
        Version = "1001";
        try
        {
            client = new FileSocket();
            client.Connect(client.serverIP, 12346);
            client.Send("AA"+Version);
        }
        catch
        {
            ChangeMessage("服务器出错，请退出重试");
        }
         /*if (ReadFile(out string[] str, "save/build/select")) InitPlayer(str, str);
         player.GetComponent<Player>().hand.GetComponent<Hand>().GetCard("Normal_Burst");
         player.GetComponent<Player>().hand.GetComponent<Hand>().GetCard("Normal_Electro");
         player.GetComponent<Player>().hand.GetComponent<Hand>().GetCard("Item_Clock");
         player.GetComponent<Player>().hand.GetComponent<Hand>().GetCard("Normal_Geo");
        /*enemyPlayer.GetComponent<Player>().hand.GetComponent<Hand>().GetCard("Normal_Burst");
        enemyPlayer.GetComponent<Player>().hand.GetComponent<Hand>().GetCard("Normal_Pyro");
        enemyPlayer.GetComponent<Player>().hand.GetComponent<Hand>().GetCard("Item_Clock");
        enemyPlayer.GetComponent<Player>().hand.GetComponent<Hand>().GetCard("Normal_Geo");
        //ShowImage(player.GetComponent<Player>().sprites.GetComponent<AllSprites>().cardback_Anemo, new Vector3(0, 0,-1),1,1000);
        //ShowMessage("测试", new Vector3(0, 0, -1), 0.1f, 1000);*/
    }
    
    private void Update()
    {
        MessageProcess();
    }

}
