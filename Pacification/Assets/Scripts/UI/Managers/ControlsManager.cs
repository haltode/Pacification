using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsManager : MonoBehaviour
{
    public KeyCode chatFocus = KeyCode.T;
    public KeyCode chatSend = KeyCode.Return;

    public KeyCode unitPrimaryAction = KeyCode.E;
    public KeyCode unitSecondaryAction = KeyCode.R;

    public KeyCode cycleCity = KeyCode.C;
    public KeyCode cycleUnit = KeyCode.U;

    public KeyCode passTurn = KeyCode.Tab;

    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
