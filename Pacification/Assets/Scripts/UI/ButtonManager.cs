using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour {

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
    }

    public void Quit()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }

    public void CancelQuitting()
    {
        SceneManager.LoadScene("Menu");
    }
}
