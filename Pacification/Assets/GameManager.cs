using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { set; get; }

    public GameObject connectionMenu;
    public GameObject hostMenu;
    public GameObject joinMenu;


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
        connectionMenu.SetActive(false);
        hostMenu.SetActive(true);
    }

    public void ConnectToServerButton()
    {

    }

    public void BackButton()
    {
        hostMenu.SetActive(false);
        joinMenu.SetActive(false);
        connectionMenu.SetActive(true);
    }
}
