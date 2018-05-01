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
        if(clients.Count == playerNumber)
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

        if(!isGameStarted && clients.Count == playerNumber)
        {
            isGameStarted = true;
            Broadcast("SLOD", clients);
            Broadcast("SYGO", clients[0]);
        }
    }
    void Broadcast(string data, ServerClient client)
    {
        Broadcast(data, new List<ServerClient> { client });
    }

    void Read(ServerClient client, string data)
    {
        Debug.Log("Server received : " + data);

        string[] receivedData = data.Split('|');
        switch(receivedData[0])
        {
            /////// GAMEPLAY
            case "CMOV":
                Broadcast("SMOV|" + receivedData[1], clients);
                break;
            case "CUNI":
                Broadcast("S" + receivedData[1] +"|"+ receivedData[2] + "|" + client.clientName, clients);
                break;

            case "CUNM":
                Broadcast("SUNM|" + receivedData[1] + "|" + receivedData[2] + "|" + client.clientName, clients);
                break;

            case "CEDI":
                Broadcast("SEDI|" + receivedData[1], clients);
                break;

            case "CEND":
                if(playerNumber == 1)
                {
                    //Do stuff with AI
                }
                else
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

            /////// REGISTER A CLIENT
            case "CIAM":
                string[] clientStatus = receivedData[1].Split('#');
                client.clientName = clientStatus[0];
                client.isHost = (clientStatus[1] == "1");

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
