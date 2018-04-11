using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour {

    public Transform chatMessageContainer;
    public GameObject messagePrefab;
    public GameObject privateMessagePrefab;
    public GameObject alertMessagePrefab;

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

    public void ChatMessage(string message, int type)
    {
        GameObject msg;
        if(type == 0)
            msg = Instantiate(messagePrefab) as GameObject;
        else if(type == 1)
            msg = Instantiate(privateMessagePrefab) as GameObject;
        else
            msg = Instantiate(alertMessagePrefab) as GameObject;

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
            int index = 1;
            string test = ExtractCommand(ref index, i.text);
            Debug.Log(test);
            switch (test)
            {
                case "msg":
                    index++;
                    string receiver = ExtractCommand(ref index, i.text);
                    string message = ExtractMessage(++index, i.text);
                    client.Send("CMSP|"  + receiver + "|" + message);
                    break;
                case "god":
                    FindObjectOfType<ButtonManager>().CheatMode();
                    break;

                case "clear":
                    break;

                case "unit":
                    break;

                case "kick":
                    break;

                case "help":
                    break;

                default:
                    ChatMessage("ERROR: Unknown command", 2);
                    break;
            }
        }
        else
            client.Send("CMSG|0|" + i.text);

        i.text = "";
    }

    private string ExtractCommand(ref int index, string data)
    {
        string command = "";
        while(index < data.Length && data[index] != ' ')
        {
            command += data[index];
            index++;
        }
        return command;
    }

    private string ExtractMessage(int index, string data)
    {
        string message = "";
        while (index < data.Length)
        {
            message += data[index];
            index++;
        }
        return message;
    }
}
