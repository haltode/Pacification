using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Client : MonoBehaviour
{
    public string clientName;

    public bool isHost;
    private bool isSocketStarted;

    private List<GameClient> playerClients = new List<GameClient>(); 
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    private HexMapEditor mapEditor;
    private ChatManager chat;

    public Player player;
    private AI ai = null;

    public List<Player> players = new List<Player>();

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public bool ConnectToServer(string host, int port)
    {
        if(isSocketStarted)
            return false;

        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

            isSocketStarted = true;
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }

        return isSocketStarted;
    }

    void Update()
    {
        if(isSocketStarted && stream.DataAvailable)
        {
            string data = reader.ReadLine();
            if(data != null)
                Read(data);
        }
    }

    public void Send(string data)
    {
        if(!isSocketStarted)
            return;

        writer.WriteLine(data);
        writer.Flush();
    }

    void Read(string data)
    {
        Debug.Log("Client " + clientName + " received: " + data);

        string[] receivedData = data.Split('|');
        switch(receivedData[0])
        {
            /////// GAMEPLAY
            case "SMOV":
                FindObjectOfType<HexGameUI>().NetworkDoMove(receivedData[1]);
                break;

            case "SUAA":
                foreach(Player p in players)
                {
                    if(p.name == receivedData[3])
                    {
                        p.NetworkAddUnit(receivedData[1]);
                        p.NetworkAddUnit(receivedData[2]);
                    }
                }
                break;

            case "SUCD":
                foreach(Player p in players)
                {
                    if(p.name == receivedData[3])
                    {
                        p.NetworkAddCity(receivedData[1]);
                        p.NetworkRemoveUnit(receivedData[2]);
                    }
                }
                break;

            case "SUNC":
                foreach(Player p in players)
                {
                    if(p.name == receivedData[2])
                        p.NetworkAddUnit(receivedData[1]);
                }
                break;

            case "SUND":
                foreach(Player p in players)
                {
                    if(p.name == receivedData[2])
                        p.NetworkRemoveUnit(receivedData[1]);
                }
                break;

            case "SUNL":
                foreach(Player p in players)
                {
                    if(p.name == receivedData[2])
                        p.NetworkLevelUp();
                }
                break;

            case "SCID":
                foreach(Player p in players)
                {
                    if(p.name == receivedData[2])
                        p.NetworkRemoveCity(receivedData[1]);
                }
                break;


            case "SROD":
                // TODO: in solo/Multi need to handle roads + 
                break;

            case "SYGO":
                if(GameManager.Instance.gamemode == GameManager.Gamemode.SOLO)
                {
                    if(ai == null)
                        ai = new AI(player, AI.Difficulty.HARD);
                    ai.PlayTurn();
                }
                FindObjectOfType<ButtonManager>().TakeTurn();
                break;

            /////// CHAT
            case "SMSG":
                chat.ChatMessage(receivedData[2], (ChatManager.MessageType)int.Parse(receivedData[1]));
                break;

            case "SMSE":
                chat.ChatMessage(receivedData[1], ChatManager.MessageType.ALERT);
                break;

            case "SYOP":
                if(receivedData[1] == "1")
                    chat.OpDeop("", true);
                else
                    chat.OpDeop("", false);
                break;

            /////// REGISTER ON SERVER
            case "SWHO":
                for(int i = 1; i < receivedData.Length - 1; ++i)
                    UserConnected(receivedData[i], false);

                Send("CIAM|" + clientName + "#" + ((isHost)? 1:0).ToString());
                break;

            case "SCNN":
                string[] clientStatus = receivedData[1].Split('#');
                UserConnected(clientStatus[0], (clientStatus[1]) == "1");
                break;

            case "SDEC":
                chat.ChatMessage(receivedData[1] + " left the game.", ChatManager.MessageType.ALERT);
                break;

            case "SKIK":
                FindObjectOfType<ButtonManager>().DeconnectionButton();
                break;
                
            case "SLOD":
                SceneManager.LoadScene("Map");
                player.SetDisplayer();
                break;

            case "SMAP":
                if(!isHost)
                {
                    MapSenderReceiver mapLoader = FindObjectOfType<MapSenderReceiver>();
                    mapLoader.StartGame(receivedData[1]);
                }
                player.InitialSpawnUnit();
                player.UpdateMoneyDisplay();
                player.UpdateHappinessDisplay();
                player.UpdateProductionDisplay();
                player.UpdateScienceDisplay();
                chat = FindObjectOfType<ChatManager>();
                break;
        }
    }

    void UserConnected(string name, bool host)
    {
        GameClient client = new GameClient { name = name };
        playerClients.Add(client);

        if(name == clientName)
            players.Add(player);
        else
        {
            Player newPlayer = new Player(name);
            players.Add(newPlayer);
        }
    }

    void OnApplicationQuit()
    {
        CloseSocket();
    }

    void OnDisable()
    {
        CloseSocket();
    }

    public void CloseSocket()
    {
        if(!isSocketStarted)
            return;

        writer.Close();
        reader.Close();
        socket.Close();
        isSocketStarted = false;
    }
}

public class GameClient
{
    public string name;
    public bool isHost;
}
