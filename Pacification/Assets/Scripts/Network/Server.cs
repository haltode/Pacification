using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Server : MonoBehaviour
{
    public int port = 6321;

    private List<ServerClient> clients;
    private List<ServerClient> disconnectList;

    private TcpListener server;
    private bool isServerStarted;

    public void Init()
    {
        DontDestroyOnLoad(gameObject);

        clients = new List<ServerClient>();
        disconnectList = new List<ServerClient>();

        try
        {
            server = new TcpListener(IPAddress.Any, port);
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
        if (!isServerStarted)
            return;

        foreach(ServerClient client in clients)
        {
            if(!IsConnected(client.tcp))
            {
                client.tcp.Close();
                disconnectList.Add(client);
                continue;
            }
            else
            {
                NetworkStream s = client.tcp.GetStream();
                if (s.DataAvailable)
                {
                    StreamReader reader = new StreamReader(s, true);
                    string data = reader.ReadLine();

                    if(data != null)
                    {
                        Read(client, data);
                    }
                }
            }
        }

        for(int i = 0; i < disconnectList.Count - 1; ++i)
        {
            clients.Remove(disconnectList[i]);
            disconnectList.RemoveAt(i);
        }
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
            if (client != null && client.Client != null && client.Client.Connected)
            {
                if (client.Client.Poll(0, SelectMode.SelectRead))
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

    // Server Send
    private void Broadcast(string data, List<ServerClient> clientList)
    {
        foreach(ServerClient servclient in clientList)
        {
            try
            {
                StreamWriter writer = new StreamWriter(servclient.tcp.GetStream());
                writer.WriteLine(data);
                writer.Flush();
            }
            catch(Exception e)
            {
                Debug.Log("Error : " + e.Message);
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
