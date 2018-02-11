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

    private bool isGameStarted;
    private bool isServerStarted;

    private List<ServerClient> clients;
    private TcpListener server;


    public void Init()
    {
        DontDestroyOnLoad(gameObject);

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
            Debug.Log("Socket error: " + e.Message);
        }
    }

    public void Update()
    {
        if(!isServerStarted)
            return;

        for (int i = clients.Count - 1; i >= 0; --i)
        {
            if(!IsConnected(clients[i].tcp))
            {
                clients[i].tcp.Close();
                clients.Remove(clients[i]);
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

        if(!isGameStarted && clients.Count == playerNumber)
        {
            Debug.Log("Sending Map");
            isGameStarted = true;

            ////// Appel au constructeur + enregistreur en string de Map
            string map = "empty_map_for_now";
            /////////

            Broadcast("SMAP|" + map, clients);
        }
    }

    private void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }

    private void AcceptTcpClient(IAsyncResult ia)
    {
        if(clients.Count == playerNumber)
            return;

        TcpListener listener = (TcpListener) ia.AsyncState;

        string allUsers = "";
        foreach(ServerClient serverClient in clients)
        {
            allUsers += serverClient.clientName + '|';
        }

        ServerClient sc = new ServerClient(listener.EndAcceptTcpClient(ia));
        clients.Add(sc);

        StartListening();

        Broadcast("SWHO|"+ allUsers, clients[clients.Count - 1]);
    }

    private bool IsConnected(TcpClient client)
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

    private void Broadcast(string data, List<ServerClient> clientList)
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
    private void Broadcast(string data, ServerClient client)
    {
        Broadcast(data, new List<ServerClient> { client });
    }

    private void Read(ServerClient client, string data)
    {
        Debug.Log("Server received : " + data);

        string[] receivedData = data.Split('|');
        switch(receivedData[0])
        {
            case "CMOV":
                Broadcast("SMOV|" + receivedData[1], clients);
                break;

            case "CEND":
                ///////// One player ended his turn, let the next one play
                /*//*/ServerClient newActivclient = clients[0];
                /////////////////////

                Broadcast("SYGO|", newActivclient);
                break;

            case "CMSG":
                Broadcast("SMSG|" + client.clientName + " >> " + receivedData[1], clients);
                break;

            case "CMSP":
                ServerClient clientMSGreceiver = clients.Find(clientMsp => clientMsp.clientName == receivedData[1]);
                if (clientMSGreceiver != null)
                    Broadcast("SMSP|" + client.clientName + " >> " + receivedData[2], clients.Find(clientMsp => clientMsp.clientName == receivedData[1]));
                else
                    Broadcast("SMSE|The player receivedData[2] is unreacheable", client);
                break;

            case "CIAM":
                string[] clientStatus = receivedData[1].Split('#');
                client.clientName = clientStatus[0];
                client.isHost = (clientStatus[1] == "1");

                Broadcast("SCNN|" + receivedData[1], clients);
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
