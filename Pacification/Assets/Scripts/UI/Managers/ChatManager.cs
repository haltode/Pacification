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
    ControlsManager controls;

    List<GameObject> allMessages;
    Transform chat;

    bool isChatActive = false;
    bool isOp = false;

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
        if(Input.GetKeyUp(controls.passTurn))
            buttonManager.EndTurnButton();
    }

    public void ChatAppearManager()
    {
        if(isChatActive && input.text == "")
            HideCHat();
        else if(!isChatActive)
            SpawnChat();
    }

    public void SpawnChat()
    {
        foreach(Transform t in chat)
            t.gameObject.SetActive(true);
        input.ActivateInputField();
        isChatActive = true;
    }

    public void HideCHat()
    {
        foreach(Transform t in chat)
            t.gameObject.SetActive(false);
        isChatActive = false;
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
                    ChatMessage("You whispered to " + receiver + " >> " + message, MessageType.PRIVATE);
                    break;

                case "clear":
                    string commandClear = ExtractCommand(ref index, input.text);
                    if(commandClear == "unit" || commandClear == "units")
                    {
                        if(!isOp)
                        {
                            NoPermission();
                            break;
                        }
                        client.Send("CUNI|CLS|0|0");
                    }
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
                    if(!isOp)
                    {
                        NoPermission();
                        break;
                    }

                    string commandUnit = ExtractCommand(ref index, input.text);
                    
                    //Add unit spawning code on location pointed by player

                    ChatMessage("ERROR: Unknown command \"unit " + commandUnit + "\"", MessageType.ALERT);
                    break;

                case "kick":
                    if(!isOp)
                    {
                        NoPermission();
                        break;
                    }

                    string kickedPlayer = ExtractCommand(ref index, input.text);
                    if(kickedPlayer == "")
                        ChatMessage("You didn't specified the player to kick", MessageType.ALERT);
                    string kickMessage = ExtractMessage(++index, input.text);
                    client.Send("CKIK|" + kickedPlayer + "|" + (kickMessage == "" ? "NO_REASON_GIVEN": kickMessage));
                    break;

                case "code":
                    switch(ExtractCommand(ref index, input.text))
                    {
                        case "coinage":
                            ++index;
                            string beRich = ExtractCommand(ref index, input.text);
                            if(beRich == "")
                                client.player.money += 1000;
                            else
                                client.player.money += int.Parse(beRich);
                            client.player.displayer.DisplayResources();
                            break;

                        case "epenser":
                            ++index;
                            string beSmart = ExtractCommand(ref index, input.text);
                            if(beSmart == "")
                                client.player.science += 1000;
                            else
                                client.player.science += int.Parse(beSmart);
                            client.player.displayer.DisplayResources();
                            client.Send("CUNI|COD|" + (beSmart == "" ? "1000":beSmart));
                            break;

                        case "pizza":
                            ++index;
                            string beHungry = ExtractCommand(ref index, input.text);
                            if(beHungry == "")
                                client.player.resources[5] += 1000;
                            else
                                client.player.resources[5] += int.Parse(beHungry);
                            client.player.displayer.DisplayResources();
                            break;

                        case "dada":
                            ++index;
                            string beEquitation = ExtractCommand(ref index, input.text);
                            if(beEquitation == "")
                                client.player.resources[3] += 1000;
                            else
                                client.player.resources[3] += int.Parse(beEquitation);
                            client.player.displayer.DisplayResources();
                            break;

                        case "minecraft":
                            ++index;
                            string beRock = ExtractCommand(ref index, input.text);
                            if(beRock == "")
                            {
                                client.player.resources[0] += 1000;
                                client.player.resources[1] += 1000;
                                client.player.resources[2] += 1000;
                            }
                            else
                            {
                                client.player.resources[0] += int.Parse(beRock);
                                client.player.resources[1] += int.Parse(beRock);
                                client.player.resources[2] += int.Parse(beRock);
                            }
                            client.player.displayer.DisplayResources();
                            break;

                        case "ohmygod":
                            ++index;
                            string beGod = ExtractCommand(ref index, input.text);
                            if(beGod == "")
                            {
                                client.player.resources[0] += 1000;
                                client.player.resources[1] += 1000;
                                client.player.resources[2] += 1000;
                                client.player.resources[3] += 1000;
                                client.player.resources[4] += 1000;
                                client.player.resources[5] += 1000;
                                client.player.money += 1000;
                                client.player.science += 1000;
                            }
                            else
                            {
                                client.player.resources[0] += int.Parse(beGod);
                                client.player.resources[1] += int.Parse(beGod);
                                client.player.resources[2] += int.Parse(beGod);
                                client.player.resources[3] += int.Parse(beGod);
                                client.player.resources[4] += int.Parse(beGod);
                                client.player.resources[5] += int.Parse(beGod);
                                client.player.money += int.Parse(beGod);
                                client.player.science += int.Parse(beGod);
                            }
                            client.player.displayer.DisplayResources();
                            break;

                        case "fog":
                            ++index;
                            string toggle = ExtractCommand(ref index, input.text);
                            if(toggle == "off")
                            {
                                Shader.EnableKeyword("HEX_MAP_EDITOR");
                                client.player.hexGrid.ShowUI(false);
                            }
                            else if(toggle == "on")
                            {
                                Shader.DisableKeyword("HEX_MAP_EDITOR");
                                client.player.hexGrid.ShowUI(true);
                            }
                            break;
                    }
                    break;

                case "op":
                    if(!isOp && !client.isHost)
                        NoPermission();
                    else
                        OpDeop(ExtractCommand(ref index, input.text), true);
                    break;

                case "deop":
                    if(!isOp)
                        NoPermission();
                    else
                        OpDeop(ExtractCommand(ref index, input.text), false);
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
                                ChatMessage("Use to talk with another player in private : /msg [playerName] [message]", MessageType.ALERT);
                                break;

                            case "clear":
                                ChatMessage("Use to clear something (messages by default): /clear [msg.unit]", MessageType.ALERT);
                                break;

                            case "unit":
                                ChatMessage("Use to spawn a unit : /unit [type]", MessageType.ALERT);
                                break;

                            case "kick":
                                ChatMessage("Use to kick a player : /kick [playerName] [reason]", MessageType.ALERT);
                                break;

                            case "help":
                                ChatMessage("You need help...", MessageType.ALERT);
                                break;

                            case "code":
                                ChatMessage("No help is provided for the cheating!", MessageType.ALERT);
                                break;

                            default:
                                ChatMessage("ERROR: Unknown command \"" + helpCommand + "\"", MessageType.ALERT);
                                break;
                        }
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
        input.ActivateInputField();
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

    void NoPermission()
    {
        ChatMessage("You do not have permission to use this command", MessageType.ALERT);
    }

    public void OpDeop(string playerName, bool toOp)
    {
        if(toOp == isOp && playerName == "")
            return;

        if(playerName == "")
        {
            isOp = toOp;
            string message = toOp ? "now op" : "no longer op";
            client.Send("CMSG|3|The player " + client.clientName + " is " + message);
        }
        else
        {
            string opMSG = toOp ? "1" : "0";
            client.Send("CYOP|" + opMSG + "|" + playerName);
        }
    }
}
