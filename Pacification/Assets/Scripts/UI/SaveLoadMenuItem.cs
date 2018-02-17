using UnityEngine;
using UnityEngine.UI;

public class SaveLoadMenuItem : MonoBehaviour
{
    public SaveLoadMapMenu menu;

    string mapName;
    
    public string MapName
    {
        get { return mapName; }
        set
        {
            mapName = value;
            transform.GetChild(0).GetComponent<Text>().text = value;
        }
    }

    public void Select()
    {
        menu.SelectItem(mapName);
    }
}