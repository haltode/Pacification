using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListManager : MonoBehaviour {

    public Text player1;
    public Text player2;
    public Text player3;
    public Text player4;
    public Text player5;
    public Text player6;
    public Text player7;
    public Text player8;

    bool[] isUsed = { false, false, false, false, false, false, false, false };
    Text[] names;

    private void Start()
    {
        names = new Text[] { player1, player2, player3, player4, player5, player6, player7, player8 };
    }

    public void AddPlayer(string name)
    {
        if(player1 == null)
            return;

        bool done = false;
        int i = 0;

        while(!done && i < 8)
        {
            if(!isUsed[i])
            {
                names[i].text = name;
                isUsed[i] = true;
                done = true;
            }
            ++i;
        }
    }

    public void RemovePlayer(string name)
    {
        if(player1 == null)
            return;

        for(int i = 0; i < 8; ++i)
        {
            if(names[i].text == name)
            {
                names[i].text = "Available Slot";
                isUsed[i] = false;
            }
        }
    }

    public void Clear()
    {
        for(int i = 0; i < 8; ++i)
        {
            names[i].text = "Available Slot";
            isUsed[i] = false;
        }
    }
}
