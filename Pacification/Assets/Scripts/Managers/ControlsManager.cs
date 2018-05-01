using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsManager : MonoBehaviour {

    public KeyCode chatFocus = KeyCode.T;
    public KeyCode chatSend = KeyCode.Return;

    public KeyCode unitAction = KeyCode.E;
    public KeyCode unitSpawn = KeyCode.U;
    public KeyCode unitDestroy = KeyCode.LeftShift;

    public KeyCode uiMenu = KeyCode.Escape;
    public KeyCode uiBack = KeyCode.Escape;

    public KeyCode cheat = KeyCode.Tab;

    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
