using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour {

    public Transform chatMessageContainer;
    public GameObject messagePrefab;

    public void ChatMessage(string msg)
    {
        GameObject go = Instantiate(messagePrefab) as GameObject;

        go.transform.SetParent(chatMessageContainer);

        go.GetComponentInChildren<Text>().text = msg;
    }

    public void SendChatMessage()
    {
        InputField i = GameObject.Find("MessageInput").GetComponent<InputField>();

        if(i.text == "")
            return;

        Client client = FindObjectOfType<Client>();
        client.Send("CMSG|" + i.text);

        i.text = "";
    }
}
