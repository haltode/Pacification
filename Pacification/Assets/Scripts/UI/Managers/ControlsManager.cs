using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsManager : MonoBehaviour
{
    public KeyCode chatFocus = KeyCode.T;
    public KeyCode chatSend = KeyCode.Return;

    public KeyCode unitAction = KeyCode.E;
    public KeyCode workerAddRoad = KeyCode.R;

    public KeyCode cycleCity = KeyCode.C;
    public KeyCode cycleUnit = KeyCode.U;

    public KeyCode uiMenu = KeyCode.Escape;
    public KeyCode uiBack = KeyCode.Escape;

    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
