using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { set; get; }

    public GameObject connectionMenu;
    public GameObject hostMenu;
    public GameObject toHosting;
    public GameObject joinMenu;

    public GameObject serverPrefab;
    public GameObject clientPrefab;

    public InputField nameInput;
    public InputField nameInputHost;

    public Slider numberOfPlayerSlider;

    private void Start()
    {
        Instance = this;
        hostMenu.SetActive(false);
        toHosting.SetActive(false);
        joinMenu.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }

    public void MenuJoinButton()
    {
        connectionMenu.SetActive(false);
        joinMenu.SetActive(true);
    }

    public void MenuHostButton()
    {
        StartingServer();

        toHosting.SetActive(false);
        hostMenu.SetActive(true);
    }

    public void StartingServer(bool isAlone = false)
    {
        try
        {
            Server server = Instantiate(serverPrefab).GetComponent<Server>();
            server.Init();

            server.playerNumber = isAlone ? 1:(int)numberOfPlayerSlider.value;

            Client client = Instantiate(clientPrefab).GetComponent<Client>();
            client.clientName = nameInputHost.text;
            client.isHost = true;

            if(client.clientName == "")
            {
                client.clientName = "Host";
            }
            client.ConnectToServer(Server.Localhost, Server.Port);
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void ConnectToServerButton()
    {
        string hostAddress = GameObject.Find("HostAdressInput").GetComponent<InputField>().text;
        bool isConnected = false;

        if (hostAddress == "")
            hostAddress = Server.Localhost;

        Client client = Instantiate(clientPrefab).GetComponent<Client>();

        try
        {
            if(client.ConnectToServer(hostAddress, Server.Port))
            {
                client.clientName = nameInput.text;

                if (client.clientName == "")
                {
                    System.Random rnd = new System.Random();
                    client.clientName = "Player" + (rnd.Next(1000, 10000));
                }

                connectionMenu.SetActive(false);
                isConnected = true;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        if (!isConnected)
            Destroy(client.gameObject);
    }

    public void BackButton()
    {
        hostMenu.SetActive(false);
        toHosting.SetActive(false);
        joinMenu.SetActive(false);
        connectionMenu.SetActive(true);

        Server server = FindObjectOfType<Server>();
        if (server != null)
        {
            server.server.Stop();
            Destroy(server.gameObject);
        }

        Client client = FindObjectOfType<Client>();
        if (client != null)
            Destroy(client.gameObject);
    }

    public void MenutoHostingButton()
    {
        toHosting.SetActive(true);
        connectionMenu.SetActive(false);
    }

    public void SoloButton()
    {
        StartingServer(true);
        SceneManager.LoadScene("Map");
    }

    public void ExitButton()
    {
        SceneManager.LoadScene("Exit");
    }

    public void StartGame(string map)
    {
        SceneManager.LoadScene("Map");
    }
}
