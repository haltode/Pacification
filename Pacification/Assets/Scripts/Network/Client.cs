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
    private HexGameUI gameUI;

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
        /* LIST OF USED COMMAND : 
         * CID
         * CIT
         * CLS
         * CNN
         * DEC
         * DED
         * 
         * EXP
         * KIK
         * KIL
         * LOD
         * 
         * MAP
         * MSE
         * MSG
         * MOV
         * ROD
         * 
         * UAA
         * UNC
         * UND
         * UNL
         * 
         * YGO
         * YOP
         * WHO
         */
        string[] receivedData = data.Split('|');
        switch(receivedData[0])
        {
            ////// UNITS : Adding
            case "SUNC":
                foreach(Player p in players)
                {
                    if(p.name == receivedData[2])
                        p.NetworkAddUnit(receivedData[1]);
                }
                break;

            //Unit Adding Multiple
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

            //Unit Remove
            case "SUND":
                foreach(Player p in players)
                {
                    if(p.name == receivedData[2])
                        p.NetworkRemoveUnit(receivedData[1]);
                }
                break;

            //Unit Movement
            case "SMOV":
                gameUI.NetworkMoveUnit(receivedData[1]);
                break;

            //Unit LevelUp
            case "SUNL":
                foreach(Player p in players)
                {
                    if(p.name == receivedData[2])
                        p.NetworkLevelUp();
                }
                break;
                
            ////// SETTLER : City creation
            case "SCIT":
                foreach(Player p in players)
                {
                    if(p.name == receivedData[3])
                    {
                        p.NetworkAddCity(receivedData[1]);
                        p.NetworkRemoveUnit(receivedData[2]);
                    }
                }
                break;

            //City destruction
            case "SCID":
                foreach(Player p in players)
                {
                    if(p.name == receivedData[2])
                        p.NetworkRemoveCity(receivedData[1]);
                }
                break;

            ///// WORKER : Build.Destroy road
            case "SROD":
                gameUI.NetworkRoad(receivedData[1]);

                string[] roadData = receivedData[1].Split('#');
                HexCell destination = player.hexGrid.GetCell(new HexCoordinates(int.Parse(roadData[0]), int.Parse(roadData[1]))).GetNeighbor((HexDirection)int.Parse(roadData[3]));
                string moveUnitData = roadData[0] + "#" + roadData[1] + "#" + destination.coordinates.X + "#" + destination.coordinates.Z;
                gameUI.NetworkMoveUnit(moveUnitData);
                break;

            //Worker : Exploit ressources
            case "SEXP":
                break;

            /////PLAYER : Take turn
            case "SYGO":
                if(GameManager.Instance.gamemode == GameManager.Gamemode.SOLO)
                {
                    if(ai == null)
                        ai = new AI(player, AI.Difficulty.HARD);
                    ai.PlayTurn();
                }
                FindObjectOfType<ButtonManager>().TakeTurn();
                break;
            
            //Player : Death
            case "SDED":
                break;
           
            //Player : Disconnected
            case "SDEC":
                chat.ChatMessage(receivedData[1] + " left the game.", ChatManager.MessageType.ALERT);
                break;

            /////// CHAT : Global Message
            case "SMSG":
                chat.ChatMessage(receivedData[2], (ChatManager.MessageType)int.Parse(receivedData[1]));
                break;

            //Chat : Private Message
            case "SMSE":
                chat.ChatMessage(receivedData[1], ChatManager.MessageType.ALERT);
                break;
            
            //Chat : Op.DeOp a player
            case "SYOP": 
                chat.OpDeop("", receivedData[1] == "1");
                break;

            //Chat : Clear units
            case "SCLS": 
                player.hexGrid.ClearUnits();
                break;

            //Chat : Kill player
            case "SKIL":
                break;

            //Chat : Kick player
            case "SKIK":
                FindObjectOfType<ButtonManager>().DeconnectionButton();
                break;

            /////// REGISTRATION ON SERVER
            case "SWHO":
                for(int i = 1; i < receivedData.Length - 1; ++i)
                    UserConnected(receivedData[i], false);

                Send("CIAM|" + clientName + "#" + ((isHost)? 1:0).ToString());
                break;

            case "SCNN":
                string[] clientStatus = receivedData[1].Split('#');
                UserConnected(clientStatus[0], (clientStatus[1]) == "1");
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
                gameUI = FindObjectOfType<HexGameUI>();
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
