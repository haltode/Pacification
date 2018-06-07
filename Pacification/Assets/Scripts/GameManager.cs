using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { set; get; }

    public GameObject serverPrefab;
    public GameObject clientPrefab;

    public InputField nameInput;
    public InputField nameHostInput;

    public HexGrid hexGrid;
    public ControlsManager controls;

    public Transform errorMount;
    public Transform waitingPannel;
    public Text errorLog;

    public Slider AILevel;
    
    public enum Gamemode
    {
        SOLO,
        MULTI,
        EDITOR
    }
    public Gamemode gamemode;

    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void StartingServer()
    {
        bool isConnected = false;
        Server server = Instantiate(serverPrefab).GetComponent<Server>();
        Client client = Instantiate(clientPrefab).GetComponent<Client>();

        try
        {
            server.Init();
            
            client.clientName = nameHostInput.text;
            client.isHost = true;

            if(client.clientName == "")
                client.clientName = "Host";
            client.player = new Player(client.clientName);

            isConnected = client.ConnectToServer(Server.Localhost, Server.Port);
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }

        if(!isConnected)
        {
            Destroy(server.gameObject);
            Destroy(client.gameObject);
            FindObjectOfType<LUI_MenuCamControl>().setMount(errorMount);
            errorLog.text = "ERR_SERVER_SLOT_UNAVAILABLE";
        }
        else if(gamemode == Gamemode.MULTI)
            FindObjectOfType<LUI_MenuCamControl>().setMount(waitingPannel);
    }

    public void MainMenuEditorButton()
    {
        gamemode = Gamemode.EDITOR;
        SceneManager.LoadScene("Map");
    }

    public void MainMenuSoloButton()
    {
        gamemode = Gamemode.SOLO;
        StartingServer();
    }

    public void MainMenuStartingServerButton()
    {
        gamemode = Gamemode.MULTI;
        StartingServer();
    }

    public void StartingGame()
    {
        FindObjectOfType<HexMapGenerator>().Save();
        FindObjectOfType<Server>().StartingGame();
    }

    public void MainMenuConnectToServerButton()
    {
        gamemode = Gamemode.MULTI;
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

                client.player = new Player(client.clientName);
                isConnected = true;
            }
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }

        if(!isConnected)
        {
            Destroy(client.gameObject);
            FindObjectOfType<LUI_MenuCamControl>().setMount(errorMount);
            errorLog.text = "ERR_SERVER_UNREACHABLE";
        }
        else
            FindObjectOfType<LUI_MenuCamControl>().setMount(waitingPannel);
    }

    public void MainMenuBackButton()
    {
        Server server = FindObjectOfType<Server>();
        if(server != null)
        {
            server.server.Stop();
            Destroy(server.gameObject);
        }

        Client client = FindObjectOfType<Client>();
        if(client != null)
        {
            client.playerListDisplay.Clear();
            Destroy(client.gameObject);
        }
    }

    public void MainMenuExitButton()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }
}
