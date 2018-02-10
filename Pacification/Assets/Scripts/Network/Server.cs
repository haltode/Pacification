using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Server : MonoBehaviour
{
    public const int Port = 6321;

    private List<ServerClient> clients;
    //private List<ServerClient> disconnected;

    private TcpListener server;
    private bool isServerStarted;

    public void Init()
    {
        DontDestroyOnLoad(gameObject);

        clients = new List<ServerClient>();
        //disconnected = new List<ServerClient>();

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

        //foreach(ServerClient client in clients)
        for (int i = clients.Count - 1; i >= 0; --i)
        {
            if(!IsConnected(clients[i].tcp))
            {
                clients[i].tcp.Close();
                clients.Remove(clients[i]); //
                //disconnected.Add(clients[i]);
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

        //for(int i = 0; i < disconnected.Count - 1; ++i)
        //{
        //    clients.Remove(disconnected[i]);
        //    disconnected.RemoveAt(i);
        //}
    }

    private void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }

    private void AcceptTcpClient(IAsyncResult ia)
    {
        TcpListener listener = (TcpListener) ia.AsyncState;

        ServerClient sc = new ServerClient(listener.EndAcceptTcpClient(ia));
        clients.Add(sc);

        StartListening();

        Debug.Log("Somebody has connected!");
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

    private void Read(ServerClient client, string data)
    {
        Debug.Log(client.clientName + " : " + data);
    }
}

public class ServerClient
{
    public string clientName;
    public TcpClient tcp;

    public ServerClient(TcpClient tcp)
    {
        this.tcp = tcp;
    }
}
