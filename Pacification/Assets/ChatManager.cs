using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour {

    public Transform chatMessageContainer;
    public GameObject messagePrefab;

    public AudioSource notification;

    private void Start()
    {
        if(!GameManager.Instance.editor)
            notification = GetComponent<AudioSource>();
        else
        {
            Transform chat = GameObject.Find("Chat").transform;
            foreach(Transform t in chat)
            {
                t.gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        if(Input.GetKey("return"))
            SendChatMessage();
    }

    public void ChatMessage(string message)
    {
        GameObject msg = Instantiate(messagePrefab) as GameObject;
        msg.transform.SetParent(chatMessageContainer);
        msg.GetComponentInChildren<Text>().text = message;

        notification.Play();
    }

    public void SendChatMessage()
    {
        InputField i = GameObject.Find("MessageInput").GetComponent<InputField>();

        if(i.text == "")
            return;

        Client client = FindObjectOfType<Client>();
        if(i.text[0] == '/')
        {
            string command = "" + i.text[1] + i.text[2] + i.text[3];

            if(command == "msg")
            {
                string[] message = i.text.Split(' ');
                string toSend = "";
                for(int j = 2; j < message.Length; j++)
                    toSend += message[j] + " ";
                client.Send("CMSP|"  + i.text.Split(' ')[1] + "|" + toSend);
            }
        }
        else
            client.Send("CMSG|" + i.text);

        i.text = "";
    }
}
