using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour
{
    public string clientName;

    public bool isHost;
    private bool isSocketStarted;

    private List<GameClient> players = new List<GameClient>(); 
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    private void Start()
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
            Debug.Log("Socket error : " + e.Message);
        }

        return isSocketStarted;
    }

    private void Update()
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

    private void Read(string data)
    {
        Debug.Log("Client " + clientName + " received: " + data);

        string[] receivedData = data.Split('|');
        switch(receivedData[0])
        {
            /////// GAMEPLAY
            case "SMOV":
                // Display another player move
                break;

            case "SEDI":
                HexMapEditor mapEditor = FindObjectOfType<HexMapEditor>();
                mapEditor.NetworkEditedCell(receivedData[1]);
                break;

            case "SYGO":
                // It's your turn
                break;

            /////// CHAT
            case "SMSG":
                // Received a global message 
                break;

            case "SMSP":
                // Received a private message 
                break;

            case "SMSE":
                // The private message you sent couldn't find a target
                break;

            /////// REGISTER ON SERVER
            case "SWHO":
                for(int i = 1; i < receivedData.Length - 1; ++i)
                    UserConnected(receivedData[i], false);

                Debug.Log("Client " + clientName + " send: " + clientName + "#" + ((isHost) ? 1 : 0).ToString() );

                Send("CIAM|" + clientName + "#" + ((isHost)? 1:0).ToString());
                break;

            case "SCNN":
                string[] clientStatus = receivedData[1].Split('#');
                UserConnected(clientStatus[0], (clientStatus[1]) == "1");
                break;

            case "SDEC":
                // One user has disconnected
                break;

            case "SMAP":
                GameManager.Instance.StartGame(receivedData[1]);
                break;
        }
    }

    private void UserConnected(string name, bool host)
    {
        GameClient client = new GameClient { name = name };
        players.Add(client);
    }

    private void OnApplicationQuit()
    {
        CloseSocket();
    }

    private void OnDisable()
    {
        CloseSocket();
    }

    private void CloseSocket()
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
