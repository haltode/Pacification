using System;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour
{
    private bool isSocketReady;
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
        if (isSocketReady) // If already connected : don't try once more (in case of more than one call to this function)
            return false;

        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

            isSocketReady = true;
        }
        catch(Exception e)
        {
            Debug.Log("Socket error : " + e.Message);
        }

        return isSocketReady;
    }

    private void Update()
    {
        if (isSocketReady && stream.DataAvailable)
        {
            string data = reader.ReadLine();
            if (data != null)
                Read(data);
        }
    }

    public void Send(string data)
    {
        if (!isSocketReady)
            return;

        writer.WriteLine(data);
        writer.Flush();
    }

    private void Read(string data)
    {
        Debug.Log(data);
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
        if (!isSocketReady)
            return;

        writer.Close();
        reader.Close();
        socket.Close();
        isSocketReady = false;
    }
}

public class GameClient
{
    public string name;
    public bool isHost;
}
