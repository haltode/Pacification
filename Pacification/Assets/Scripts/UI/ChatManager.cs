using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour {

    public Transform chatMessageContainer;
    public GameObject messagePrefab;
    public GameObject privateMessagePrefab;
    public GameObject alertMessagePrefab;

    public InputField i;
    public AudioSource notification;

    List<GameObject> allMessages;

    private void Start()
    {
        i = GameObject.Find("MessageInput").GetComponent<InputField>();
        allMessages = new List<GameObject>();

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
        if(Input.GetKeyUp(KeyCode.T))
            i.ActivateInputField();
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

        allMessages.Add(msg);

        notification.Play();
    }

    public void SendChatMessage()
    {
        if(i.text == "")
            return;
        Client client = FindObjectOfType<Client>();

        if(i.text[0] == '/')
        {
            int index = 1;
            string command = ExtractCommand(ref index, i.text);
            index++;
            switch(command)
            {
                case "msg":
                    string receiver = ExtractCommand(ref index, i.text);
                    string message = ExtractMessage(++index, i.text);
                    client.Send("CMSP|" + receiver + "|" + message);
                    break;
                case "god":
                    FindObjectOfType<ButtonManager>().CheatMode();
                    ChatMessage("GODMOD command", 2);
                    break;

                case "clear":
                    string commandClear = ExtractCommand(ref index, i.text);
                    if(commandClear == "unit" || commandClear == "units")
                        FindObjectOfType<HexGrid>().ClearUnits();
                    else if(commandClear == "" || commandClear == "msg" || commandClear == "message" || commandClear == "messages")
                    {
                        int indexMessages = allMessages.Count - 1;
                        while(indexMessages >= 0)
                        {
                            Destroy(allMessages[indexMessages]);
                            --indexMessages;
                        }
                        allMessages.Clear();
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
                    if(kicked == "")
                        ChatMessage("You didn't specified the player to kick", 2);
                    string kickMessage = ExtractMessage(++index, i.text);
                    client.Send("CKIK|" + kicked + "|" + kickMessage);
                    break;

                case "help":
                    string helpCommand = ExtractCommand(ref index, i.text);
                    if(helpCommand == "")
                        ChatMessage("The available commands are : msg, clear, unit, kick", 2);
                    else
                    {
                        switch(helpCommand)
                        {
                            case "msg":
                                ChatMessage("Use to talk with another player in private : /msg playerName message", 2);
                                break;

                            case "god":
                                ChatMessage("Use to cheat", 2);
                                break;

                            case "clear":
                                ChatMessage("Use to clear something (messages by default): /clear [msg.message.messages.unit.units]", 2);
                                break;

                            case "unit":
                                ChatMessage("Use to spawn unit : /unit ????", 2);
                                break;

                            case "kick":
                                ChatMessage("Use to kick a player : /kick playerName [reason]", 2);
                                break;

                            case "help":
                                ChatMessage("You need help...", 2);
                                break;

                            default:
                                ChatMessage("ERROR: Unknown command \"" + helpCommand + "\"", 2);
                                break;
                        }
                    }
                    break;

                case "code":
                    switch(ExtractCommand(ref index, i.text))
                    {
                        case "coinage":
                            FindObjectOfType<Client>().player.Money += 1000;
                            break;
                    }
                    break;

                default:
                    ChatMessage("ERROR: Unknown command \"" + command + "\"", 2);
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
