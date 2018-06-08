using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Server : MonoBehaviour
{
    public const int Port = 6321;
    public const string Localhost = "127.0.0.1";
    public int playerNumber;
    public int playerPlaying;

    private bool isGameStarted;
    private bool isServerStarted;

    public List<ServerClient> clients;
    public TcpListener server;

    public void Init()
    {
        DontDestroyOnLoad(gameObject);
        playerPlaying = 0;

        clients = new List<ServerClient>();

        try
        {
            server = new TcpListener(IPAddress.Any, Port);
            server.Start();

            StartListening();
            isServerStarted = true;
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            Destroy(gameObject);
            FindObjectOfType<LUI_MenuCamControl>().setMount(GameManager.Instance.errorMount);
            GameManager.Instance.errorLog.text = "ERR_PORT_ALREADY_USED";
        }
    }

    public void Update()
    {
        if(!isServerStarted)
            return;

        for(int i = clients.Count - 1; i >= 0; --i)
        {
            if(!IsConnected(clients[i].tcp))
            {
                string deconnected = clients[i].clientName;
                clients[i].tcp.Close();
                clients.Remove(clients[i]);
                Broadcast("SDEC|" + deconnected, clients);
            }
            else
            {
                NetworkStream stream = clients[i].tcp.GetStream();
                if(stream.DataAvailable)
                {
                    StreamReader reader = new StreamReader(stream, true);
                    string data = reader.ReadLine();
                    if(data != null)
                        Read(clients[i], data);
                }
            }
        }
    }

    void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }

    void AcceptTcpClient(IAsyncResult ia)
    {
        if(isGameStarted)
            return;

        TcpListener listener = (TcpListener) ia.AsyncState;

        string allUsers = "";
        foreach(ServerClient serverClient in clients)
            allUsers += serverClient.clientName + '|';

        ServerClient sc = new ServerClient(listener.EndAcceptTcpClient(ia));
        clients.Add(sc);

        StartListening();

        Broadcast("SWHO|"+ allUsers, clients[clients.Count - 1]);
    }

    bool IsConnected(TcpClient client)
    {
        try
        {
            if(client != null && client.Client != null && client.Client.Connected)
            {
                if(client.Client.Poll(0, SelectMode.SelectRead))
                    return (client.Client.Receive(new byte[1], SocketFlags.Peek) != 0);
                else
                    return true;
            }
            else
                return false;
        }
        catch
        {
            return false;
        }
    }

    public void Broadcast(string data, List<ServerClient> clientList)
    {
        foreach(ServerClient client in clientList)
        {
            try
            {
                StreamWriter writer = new StreamWriter(client.tcp.GetStream());
                writer.WriteLine(data);
                writer.Flush();
            }
            catch(Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }
    void Broadcast(string data, ServerClient client)
    {
        Broadcast(data, new List<ServerClient> { client });
    }

    public void StartingGame()
    {
        isGameStarted = true;
        playerNumber = clients.Count;
        Broadcast("SLOD", clients);

        Broadcast("SYGO", clients[0]);
    }

    void Read(ServerClient client, string data)
    {
        Debug.Log("Server received : " + data);

        string[] receivedData = data.Split('|');
        switch(receivedData[0])
        {
            /////// GAMEPLAY
            case "CMOV":
                Broadcast("SMOV|" + receivedData[1] + "|" + client.clientName, clients);
                break;

            case "CUNI":
                string toSend = "";
                for(int i = 2; i < receivedData.Length; i++)
                    toSend += "|" + receivedData[i];
                Broadcast("S" + receivedData[1] + toSend + "|" + client.clientName, clients);
                break;

            case "CEND":
                playerPlaying = (playerPlaying + 1) % playerNumber;
                Broadcast("SYGO|", clients[playerPlaying]);
                break;

            /////// CHAT
            case "CMSG":
                Broadcast("SMSG|" + receivedData[1] + "|"+ client.clientName + " >> " + receivedData[2], clients);
                break;

            case "CMSP":
                ServerClient clientMSGreceiver = clients.Find(clientMsp => clientMsp.clientName == receivedData[1]);
                if(clientMSGreceiver != null)
                    Broadcast("SMSG|1|" + client.clientName + " whispered >> " + receivedData[2], clients.Find(clientMsp => clientMsp.clientName == receivedData[1]));
                else
                    Broadcast("SMSE| ERROR : The player " + receivedData[1] + " is unreacheable", client);
                break;

            case "CYOP":
                ServerClient clientToOp = clients.Find(clientOpped => clientOpped.clientName == receivedData[2]);
                if(clientToOp != null)
                    Broadcast("SYOP|" + receivedData[1], clients.Find(clientOpped => clientOpped.clientName == receivedData[2]));
                else
                    Broadcast("SMSE| ERROR : The player " + receivedData[2] + " is unreacheable", client);
                break;

            /////// REGISTER A CLIENT
            case "CIAM":
                string[] clientStatus = receivedData[1].Split('#');
                client.clientName = clientStatus[0];
                client.isHost = (clientStatus[1] == "1");
                
                if(client.isHost && clients.Count > 1)
                {
                    client.tcp.Close();
                    clients.Remove(client);
                }
                else
                    Broadcast("SCNN|" + receivedData[1], clients);

                break;

            case "CKIK":
                ServerClient clientKicked = clients.Find(clientMsp => clientMsp.clientName == receivedData[1]);
                if(clientKicked != null)
                    if(!clientKicked.isHost)
                        Broadcast("SKIK|" + receivedData[2], clients.Find(clientMsp => clientMsp.clientName == receivedData[1]));
                    else
                        Broadcast("SMSE|The Host is unkickable! Long live the Host!", client);
                else
                    Broadcast("SMSE| ERROR : The player " + receivedData[1] + " is unreacheable", client);
                break;
        }
    }
}

public class ServerClient
{
    public string clientName;
    public TcpClient tcp;
    public bool isHost;

    public ServerClient(TcpClient tcp)
    {
        this.tcp = tcp;
    }
}
