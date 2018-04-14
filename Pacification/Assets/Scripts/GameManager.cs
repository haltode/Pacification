using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { set; get; }

    public GameObject connectionMenu;
    public GameObject waiting;
    public GameObject host;
    public GameObject joinMenu;

    public GameObject serverPrefab;
    public GameObject clientPrefab;

    public InputField nameInput;
    public InputField nameHostInput;

    public Slider numberOfPlayerSlider;
    public HexGrid hexGrid;
    public ControlsManager controls;

    public bool editor;

    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(this);
        waiting.SetActive(false);
        host.SetActive(false);
        joinMenu.SetActive(false);
    }

    public void StartingServer(bool isAlone = false)
    {
        try
        {
            Server server = Instantiate(serverPrefab).GetComponent<Server>();
            server.Init();

            server.playerNumber = isAlone ? 1 : (int)numberOfPlayerSlider.value;

            Client client = Instantiate(clientPrefab).GetComponent<Client>();
            client.clientName = nameHostInput.text;
            client.isHost = true;

            if(client.clientName == "")
            {
                client.clientName = "Host";
            }
            client.ConnectToServer(Server.Localhost, Server.Port);

            editor = false;
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void MainMenuEditorButton()
    {
        editor = true;
        SceneManager.LoadScene("Map");
    }

    public void MainMenuSoloButton()
    {
        StartingServer(true);
    }

    public void MainMenuHostButton()
    {
        host.SetActive(true);
        connectionMenu.SetActive(false);
    }

    public void MainMenuStartingServerButton()
    {
        StartingServer();

        host.SetActive(false);
        waiting.SetActive(true);
    }

    public void MainMenuJoinButton()
    {
        connectionMenu.SetActive(false);
        joinMenu.SetActive(true);
    }

    public void MainMenuConnectToServerButton()
    {
        string hostAddress = GameObject.Find("HostAdressInput").GetComponent<InputField>().text;
        bool isConnected = false;

        if(hostAddress == "")
            hostAddress = Server.Localhost;

        Client client = Instantiate(clientPrefab).GetComponent<Client>();

        try
        {
            if(client.ConnectToServer(hostAddress, Server.Port))
            {
                client.clientName = nameInput.text;

                if(client.clientName == "")
                {
                    System.Random rnd = new System.Random();
                    client.clientName = "Player" + (rnd.Next(1000, 10000));
                }

                connectionMenu.SetActive(false);
                isConnected = true;

                joinMenu.SetActive(false);
                waiting.SetActive(true);
            }
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }

        if(!isConnected)
            Destroy(client.gameObject);
    }

    public void MainMenuBackButton()
    {
        waiting.SetActive(false);
        host.SetActive(false);
        joinMenu.SetActive(false);
        connectionMenu.SetActive(true);

        Server server = FindObjectOfType<Server>();
        if(server != null)
        {
            server.server.Stop();
            Destroy(server.gameObject);
        }

        Client client = FindObjectOfType<Client>();
        if(client != null)
            Destroy(client.gameObject);  
    }

    public void MainMenuExitButton()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }
}
