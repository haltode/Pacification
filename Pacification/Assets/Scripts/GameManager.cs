using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { set; get; }

    public GameObject connectionMenu;
    public GameObject hostMenu;
    public GameObject joinMenu;

    public GameObject serverPrefab;
    public GameObject clientPrefab;

    public InputField nameInput;

    public const int Port = 6321;

    private void Start()
    {
        Instance = this;
        hostMenu.SetActive(false);
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
        try
        {
            Server server = Instantiate(serverPrefab).GetComponent<Server>();
            server.Init();

            Client client = Instantiate(clientPrefab).GetComponent<Client>();
            client.name = nameInput.text;
            if (client.name == "")
            {
                client.name = "Host";
            }
            client.ConnectToServer("127.0.0.1", Port);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        connectionMenu.SetActive(false);
        hostMenu.SetActive(true);
    }

    public void ConnectToServerButton()
    {
        string hostAddress = GameObject.Find("HostAdressInput").GetComponent<InputField>().text;
        bool isConnected = false;

        if (hostAddress == "")
            hostAddress = "127.0.0.1";

        try
        {
            Client client = Instantiate(clientPrefab).GetComponent<Client>();
            client.ConnectToServer(hostAddress, Port);

            client.name = nameInput.text;
            if (client.name == "")
            {
                System.Random rnd = new System.Random();
                client.name = "Player" + (rnd.Next(100000));
            }

            connectionMenu.SetActive(false);

            isConnected = true;
        }
        catch (Exception e)
        {
            isConnected = false;
            Debug.Log(e.Message);
        }

        //      ISSUE : Appuyer plusieurs fois sur connecter sans succes cree de nouveaux client sans destroy l'ancien !!!
        //    if(!isConnected)
        //    {
        //        Client c = FindObjectOfType<Client>();
        //        if (c != null)
        //            Destroy(c.gameObject);
        //    }
    }

public void BackButton()
    {
        hostMenu.SetActive(false);
        joinMenu.SetActive(false);
        connectionMenu.SetActive(true);

        Server s = FindObjectOfType<Server>();
        if (s != null)
            Destroy(s.gameObject);

        Client c = FindObjectOfType<Client>();
        if (c != null)
            Destroy(c.gameObject);
    }
}
