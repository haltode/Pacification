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
    bool isSocketStarted;

    List<GameClient> playerClients = new List<GameClient>(); 
    TcpClient socket;
    NetworkStream stream;
    StreamWriter writer;
    StreamReader reader;

    HexGameUI gameUI;
    public ChatManager chat;
    public PlayerListManager playerListDisplay;

    public Player player;
    public List<Player> players = new List<Player>();
    AI ai = null;

    public List<GameObject> DeadList;

    int roundNb = 0;

    void Start()
    {
        playerListDisplay = FindObjectOfType<PlayerListManager>();
        DontDestroyOnLoad(gameObject);
        DeadList = new List<GameObject>();
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
         * COD
         * CID
         * CIT
         * CLS
         * CNN
         * CTD 
         * CUP
         * DEC
         * DED
         * 
         * KIK
         * KIL
         * LOD
         * 
         * MAP
         * MSE
         * MSG
         * MOV
         * 
         * UNC
         * UTD
         * 
         * YGO
         * YOP 
         * WEX
         * WHO
         * WRD
         */
        string[] receivedData = data.Split('|');
        switch(receivedData[0])
        {
            ////// UNITS : Add
            case "SUNC":
                for(int i = 1; i < receivedData.Length - 1; i++)
                {
                    foreach(Player p in players)
                    {
                        if(p.name == receivedData[receivedData.Length - 1])
                            p.NetworkAddUnit(receivedData[i]);
                    }
                }
                break;

            //Unit TakeDamage
            case "SUTD":
                for(int i = 1; i < receivedData.Length - 2; i+=2)
                {
                    string[] resTD = receivedData[i].Split('#');
                    string[] resTD2 = receivedData[i].Split('#');
                    HexCell attackerRes = player.hexGrid.GetCell(new HexCoordinates(int.Parse(resTD[0]), int.Parse(resTD[1])));
                    HexCell unit = player.hexGrid.GetCell(new HexCoordinates(int.Parse(resTD[0]), int.Parse(resTD[1])));

                    attackerRes.Unit.Rotation(unit);
                    attackerRes.Unit.Unit.anim.animator.SetTrigger("ActionTrigger");

                    foreach(Player p in players)
                    {
                        if(p.name == receivedData[receivedData.Length - 2])
                            p.NetworkTakeDamageUnit(receivedData[i+1]);
                    }

                    attackerRes.Unit.Unit.anim.animator.SetTrigger("IdleTrigger");
                }
                break;

            //Unit Movement
            case "SMOV":
                foreach(Player p in players)
                    if(p.name == receivedData[2])
                        p.NetworkMoveUnit(receivedData[1]);
                break;
                
            ////// CITY : City Creation
            case "SCIT":
                for(int i = 1; i < receivedData.Length - 1; i++)
                {
                    foreach(Player p in players)
                    {
                        if(p.name == receivedData[receivedData.Length - 1])
                        {
                            p.NetworkAddCity(receivedData[i]);

                            string[] remover = receivedData[i].Split('#');
                            Unit unit = player.hexGrid.GetCell(new HexCoordinates(int.Parse(remover[1]), int.Parse(remover[2]))).Unit.Unit;
                            p.RemoveUnit(unit);
                        }
                    }
                }
                break;

            //City Take Damage
            case "SCTD":
                for(int i = 1; i < receivedData.Length - 2; i+=2)
                {
                    string[] resTD = receivedData[i].Split('#');
                    string[] resTD2 = receivedData[i].Split('#');
                    HexCell attackerRes = player.hexGrid.GetCell(new HexCoordinates(int.Parse(resTD[0]), int.Parse(resTD[1])));
                    HexCell city = player.hexGrid.GetCell(new HexCoordinates(int.Parse(resTD[0]), int.Parse(resTD[1])));

                    attackerRes.Unit.Rotation(city);

                    attackerRes.Unit.Unit.anim.animator.SetTrigger("ActionTrigger");
                    foreach(Player p in players)
                    {
                        if(p.name == receivedData[receivedData.Length - 2])
                            p.NetworkTakeDamageCity(receivedData[i+1]);
                    }
                    attackerRes.Unit.Unit.anim.animator.SetTrigger("IdleTrigger");
                }
                break;

            //City Levelup
            case "SCUP":
                for(int i = 1; i < receivedData.Length - 1; i++)
                {
                    string[] cityData = receivedData[i].Split('#');
                    City city = (City)player.hexGrid.GetCell(new HexCoordinates(int.Parse(cityData[0]), int.Parse(cityData[1]))).Feature;
                    city.LevelUp(cityData[2]);
                }
                break;

            //Resources TakeDamage
            case "SRTD":
                for(int i = 1; i < receivedData.Length - 2; i+=2)
                {
                    string[] resTD = receivedData[i].Split('#');
                    string[] resTD2 = receivedData[i].Split('#');
                    HexCell attackerRes = player.hexGrid.GetCell(new HexCoordinates(int.Parse(resTD[0]), int.Parse(resTD[1])));
                    HexCell ressource = player.hexGrid.GetCell(new HexCoordinates(int.Parse(resTD[0]), int.Parse(resTD[1])));

                    attackerRes.Unit.Rotation(ressource);
                    attackerRes.Unit.Unit.anim.animator.SetTrigger("ActionTrigger");
                    foreach(Player p in players)
                    {
                        if(p.name == receivedData[receivedData.Length - 2])
                            p.NetworkTakeDamageResource(receivedData[i+1]);
                    }
                    attackerRes.Unit.Unit.anim.animator.SetTrigger("IdleTrigger");
                }
                break;

            ///// WORKER : Build.Destroy road
            case "SWRD":
                gameUI.NetworkRoad(receivedData[1]);

                string[] roadData = receivedData[1].Split('#');
                HexCell destination = player.hexGrid.GetCell(new HexCoordinates(int.Parse(roadData[0]), int.Parse(roadData[1]))).GetNeighbor((HexDirection)int.Parse(roadData[3]));
                string moveUnitData = roadData[0] + "#" + roadData[1] + "#" + destination.coordinates.X + "#" + destination.coordinates.Z;
                player.NetworkMoveUnit(moveUnitData);
                break;

            //Worker : Exploit ressources
            case "SWEX":
                FindObjectOfType<SoundManager>().PlayNewBuilding();
                string[] exploitData = receivedData[1].Split('#');
                HexCell exploit = player.hexGrid.GetCell(new HexCoordinates(int.Parse(exploitData[0]), int.Parse(exploitData[1])));
                exploit.Unit.Unit.anim.animator.SetTrigger("ActionTrigger");
                exploit.Unit.Unit.anim.animator.SetTrigger("IdleTrigger");

                foreach(Player p in players)
                {
                    if(p.name == receivedData[2])
                    {
                        Resource resource = new Resource(p, exploit, (Resource.ResourceType)int.Parse(exploitData[2]));
                        exploit.Feature = resource;
                        exploit.Feature.Type = Feature.FeatureType.RESOURCE;
                        exploit.FeatureIndex += 6;
                        p.playerResources.Add(resource);
                    }
                }
                break;

            case "SLVP":
                foreach(Player p in players)
                    if(p.name == receivedData[2])
                        p.NetworkLevelUp(receivedData[1]);
                break;

            /////PLAYER : Take turn
            case "SYGO":
                ++roundNb;
                if(GameManager.Instance.gamemode == GameManager.Gamemode.SOLO)
                {
                    if(ai == null)
                    {
                        ai = new AI(player, (AI.Difficulty)GameManager.Instance.AILevel.value);
                        players.Add(ai.aiPlayer);
                    }
                    ai.NewTurn();
                    ai.PlayTurn();
                }
                if(player.displayer != null)
                {
                    foreach(Player p in players)
                    {
                        p.Newturn(p == player);
                        foreach(Unit u in p.playerUnits)
                            u.Update();
                    }
                    player.displayer.UpdateRoundDisplay(roundNb);

                    
                    if(player.isDead)
                    {
                        Send("CEND|");
                        player.displayer.defeat.SetActive(true);
                        return;
                    }
                    else if (GameManager.Instance.gamemode != GameManager.Gamemode.SOLO )
                    {
                        bool victory = true;
                        foreach(Player p in players)
                            if(p != player)
                                victory &= p.isDead;

                        if(victory)
                            player.displayer.victory.SetActive(true);
                    }
                }
                
                FindObjectOfType<ButtonManager>().TakeTurn();
                break;
           
            //Player : Disconnected
            case "SDEC":
                if(chat != null)
                    chat.ChatMessage(receivedData[1] + " left the game.", ChatManager.MessageType.ALERT);
                else
                    playerListDisplay.RemovePlayer(receivedData[1]);
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

            //Chat : Kick player
            case "SKIK":
                FindObjectOfType<ButtonManager>().DeconnectionButton();
                break;

            //Chat : Use science cheatcode
            case "SCOD":
                foreach(Player p in players)
                    if(p.name == receivedData[2] && player.name != p.name)
                        p.science += int.Parse(receivedData[1]);
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

                playerListDisplay.SetHost(isHost);
                break;

            case "SLOD":
                SceneManager.LoadScene("Map");
                player.SetDisplayer();
                break;

            case "SMAP":
                player.hexGrid = FindObjectOfType<HexGrid>();
                player.displayer = FindObjectOfType<DisplayInformationManager>();

                if(!isHost)
                {
                    MapSenderReceiver mapLoader = FindObjectOfType<MapSenderReceiver>();
                    mapLoader.StartGame(receivedData[1]);
                }
                else
                    player.hexGrid.InitialSpawnUnit();

                player.displayer.player = player;
                player.displayer.DisplayResources();
                chat = FindObjectOfType<ChatManager>();
                gameUI = FindObjectOfType<HexGameUI>();
                player.displayer.KillLoading();
                break;

            case "SINI":
                if(isHost)
                {
                    HexMapCamera.FocusOnPosition(player.playerUnits[0].HexUnit.location.Position);
                    break;
                }

                for(int i = 1; i < receivedData.Length - 1; i += 4)
                    foreach(Player p in players)
                        if(p.name == receivedData[i])
                        {
                            p.color = p.PlayerColors[int.Parse(receivedData[i + 3])];
                            p.InitialSpawnUnit(receivedData[i + 1], receivedData[i + 2], p.name == player.name);
                        }
                break;
        }

        if(player != null && player.displayer != null)
        {
            player.displayer.UpdateInformationPannels();
            player.displayer.UpdateUpgradePannel();
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

        playerListDisplay.AddPlayer(name);
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
