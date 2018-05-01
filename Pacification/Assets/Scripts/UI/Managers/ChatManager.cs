using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{

    public Transform chatMessageContainer;
    public GameObject messagePrefab;
    public GameObject privateMessagePrefab;
    public GameObject alertMessagePrefab;

    public InputField input;
    public AudioSource notification;

    public Client client;
    public ButtonManager buttonManager;
    private ControlsManager controls;

    List<GameObject> allMessages;
    Transform chat;

    bool active;

    public enum MessageType
    {
        NORMAL,
        PRIVATE,
        ALERT
    }

    void Start()
    {
        controls = FindObjectOfType<ControlsManager>();

        input = GameObject.Find("MessageInput").GetComponent<InputField>();
        allMessages = new List<GameObject>();

        client = FindObjectOfType<Client>();
        buttonManager = FindObjectOfType<ButtonManager>();

        notification = GetComponent<AudioSource>();

        chat = GameObject.Find("Chat").transform;
        HideCHat();
    }

    void Update()
    {
        if(Input.GetKeyUp(controls.chatFocus))
            ChatAppearManager();
        if(Input.GetKey(controls.chatSend))
            SendChatMessage();
    }

    public void ChatAppearManager()
    {
        if(active && input.text == "")
            HideCHat();
        else if(!active)
            SpawnChat();
    }

    public void SpawnChat()
    {
        foreach(Transform t in chat)
            t.gameObject.SetActive(true);
        input.ActivateInputField();
        active = true;
    }

    public void HideCHat()
    {
        foreach(Transform t in chat)
            t.gameObject.SetActive(false);
        active = false;
    }

    public void ChatMessage(string message, MessageType type)
    {
        GameObject msg;
        if(type == MessageType.NORMAL)
            msg = Instantiate(messagePrefab) as GameObject;
        else if(type == MessageType.PRIVATE)
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
        if(input.text == "")
            return;

        if(input.text[0] == '/')
        {
            int index = 1;
            string command = ExtractCommand(ref index, input.text);
            index++;
            switch(command)
            {
                case "msg":
                    string receiver = ExtractCommand(ref index, input.text);
                    string message = ExtractMessage(++index, input.text);
                    client.Send("CMSP|" + receiver + "|" + message);
                    break;
                case "god":
                    buttonManager.CheatMode();
                    ChatMessage("GODMOD command", ChatManager.MessageType.ALERT);
                    break;

                case "clear":
                    string commandClear = ExtractCommand(ref index, input.text);
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
                        ChatMessage("ERROR: Unknown command \"/clear " + commandClear + "\"", MessageType.ALERT);
                    break;

                case "unit":
                    string commandUnit = ExtractCommand(ref index, input.text);


                    ChatMessage("ERROR: Unknown command \"unit " + commandUnit + "\"", MessageType.ALERT);
                    break;

                case "kick":
                    string kicked = ExtractCommand(ref index, input.text);
                    if(kicked == "")
                        ChatMessage("You didn't specified the player to kick", MessageType.ALERT);
                    string kickMessage = ExtractMessage(++index, input.text);
                    client.Send("CKIK|" + kicked + "|" + kickMessage);
                    break;

                case "help":
                    string helpCommand = ExtractCommand(ref index, input.text);
                    if(helpCommand == "")
                        ChatMessage("The available commands are : msg, clear, unit, kick", MessageType.ALERT);
                    else
                    {
                        switch(helpCommand)
                        {
                            case "msg":
                                ChatMessage("Use to talk with another player in private : /msg playerName message", MessageType.ALERT);
                                break;

                            case "god":
                                ChatMessage("Use to open the editor menu", MessageType.ALERT);
                                break;

                            case "code":
                                ChatMessage("Use this command to enter cheat code", MessageType.ALERT);
                                break;

                            case "clear":
                                ChatMessage("Use to clear something (messages by default): /clear [msg.message.messages.unit.units]", MessageType.ALERT);
                                break;

                            case "unit":
                                ChatMessage("Use to spawn unit : /unit ????", MessageType.ALERT);
                                break;

                            case "kick":
                                ChatMessage("Use to kick a player : /kick playerName [reason]", MessageType.ALERT);
                                break;

                            case "help":
                                ChatMessage("You need help...", MessageType.ALERT);
                                break;

                            default:
                                ChatMessage("ERROR: Unknown command \"" + helpCommand + "\"", MessageType.ALERT);
                                break;
                        }
                    }
                    break;

                case "code":
                    switch(ExtractCommand(ref index, input.text))
                    {
                        case "coinage":
                            client.player.money += 1000;
                            client.player.UpdateMoneyDisplay();
                            break;
                    }
                    break;

                default:
                    ChatMessage("ERROR: Unknown command \"" + command + "\"", MessageType.ALERT);
                    break;
            }
        }
        else
            client.Send("CMSG|0|" + input.text);
        input.text = "";
    }

    string ExtractCommand(ref int index, string data)
    {
        string command = "";
        while(index < data.Length && data[index] != ' ')
        {
            command += data[index];
            index++;
        }
        return command;
    }

    string ExtractMessage(int index, string data)
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
