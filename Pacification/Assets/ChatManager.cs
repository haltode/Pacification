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
            string command = ExtractCommand(ref index, i.text);
            index++;
            switch (command)
            {
                case "msg":
                    string receiver = ExtractCommand(ref index, i.text);
                    string message = ExtractMessage(++index, i.text);
                    client.Send("CMSP|"  + receiver + "|" + message);
                    break;
                case "god":
                    FindObjectOfType<ButtonManager>().CheatMode();
                    ChatMessage("GODMOD command", 2);
                    break;

                case "clear":
                    string commandClear = ExtractCommand(ref index, i.text);
                    if(commandClear == "unit" || commandClear == "units")
                        FindObjectOfType<HexGrid>().ClearUnits();
                    else if (commandClear == "" || commandClear == "msg" || commandClear == "message" || commandClear == "messages")
                    {
                        GameObject[] messages = GameObject.FindGameObjectsWithTag("Message");
                        foreach(GameObject bubble in messages)
                            bubble.SetActive(false);
                    }
                    else
                        ChatMessage("ERROR: Unknown command \"/clear " + commandClear + "\"", 2);
                    break;

                case "unit":
                    string commandUnit = ExtractCommand(ref index, i.text);


                    ChatMessage("ERROR: Unknown command \"unit " + commandUnit + "\"", 2);
                    break;

                case "kick":
                    string kicked = ExtractCommand(ref index, i.text);
                    string kickMessage = ExtractMessage(++index, i.text);
                    client.Send("CKIK|" + kicked + "|" + kickMessage);
                    break;

                case "help":
                    break;

                default:
                    ChatMessage("ERROR: Unknown command \""+ command + "\"", 2);
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
