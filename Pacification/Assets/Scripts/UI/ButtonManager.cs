using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour {

    public GameObject leftPannel;
    public GameObject rightPannel;

    private bool cheatmode = false;
    private float time = 0F;

    public void Update()
    {
        if(Input.GetKey(KeyCode.Tab) && Time.time >= time)
        {
            if(cheatmode)
            {
                Debug.Log("CheatMode deactivated");
                leftPannel.SetActive(false);
                rightPannel.SetActive(false);
                cheatmode = false;
            }
            else
            {
                Debug.Log("CheatMode activated");
                leftPannel.SetActive(true);
                rightPannel.SetActive(true);
                cheatmode = true;
            }
            time = Time.time + 0.5F;
        }
    }

    public void DeconnectionButton()
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
            Destroy(client.gameObject);
        }
        SceneManager.LoadScene("Menu");

        HexGrid hexGrid= FindObjectOfType<HexGrid>();
        Destroy(hexGrid.gameObject);

        HexMapCamera hexCam = FindObjectOfType<HexMapCamera>();
        Destroy(hexCam.gameObject);

        Destroy(GameObject.Find("GameManager"));
    }

    public void Quit()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }

    public void CancelQuitting()
    {
        Destroy(GameObject.Find("GameManager"));
        SceneManager.LoadScene("Menu");
    }
}
