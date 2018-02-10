using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { set; get; }

    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ConnectButton()
    {
        Debug.Log("Connect");
    }

    public void HostButton()
    {
        Debug.Log("Host");
    }
}
