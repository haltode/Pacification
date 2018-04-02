using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class SaveLoadMapMenu : MonoBehaviour
{
    public const int MapFormatVersion = 3;
    private bool saveMode;

    public Text menuLabel, actionButtonLabel;
    public InputField nameInput;

    public RectTransform listContent;
    public SaveLoadMenuItem itemPrefab;

    public void Open(bool saveMode)
    {
        this.saveMode = saveMode;
        if(saveMode)
        {
            menuLabel.text = "Save Map";
            actionButtonLabel.text = "Save";
        }
        else
        {
            menuLabel.text = "Load map";
            actionButtonLabel.text = "Load";
        }
        FillList();
        gameObject.SetActive(true);
        HexMapCamera.Locked = true;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        HexMapCamera.Locked = false;
    }

    public void Action()
    {
        string path = GetSelectedPath();
        if(path == null)
            return;

        if(saveMode)
            Save(path);
        else
            Load(path);

        Close();
    }

    public void Save(string path)
    {
        HexGrid hexGrid = FindObjectOfType<HexGrid>();

        using(BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
        {
            writer.Write(MapFormatVersion);
            hexGrid.Save(writer);
        }
    }

    public void Load(string path)
    {
        HexGrid hexGrid = FindObjectOfType<HexGrid>();

        if(!File.Exists(path))
        {
            Debug.LogError("File does not exist " + path);
            return;
        }

        using(BinaryReader reader = new BinaryReader(File.OpenRead(path)))
        {
            int header = reader.ReadInt32();
            if(header == MapFormatVersion)
            {
                hexGrid.Load(reader);
                HexMapCamera.ValidatePosition();
            }
            else
                Debug.LogWarning("Unknown map format " + header);
        }
    }

    public void Delete()
    {
        string path = GetSelectedPath();
        if(path == null)
            return;
        if(File.Exists(path))
            File.Delete(path);
        nameInput.text = "";
        FillList();
    }

    string GetSelectedPath()
    {
        string mapName = nameInput.text;
        if(mapName.Length == 0)
            return null;
        return Path.Combine(Application.persistentDataPath, mapName + ".map");
    }

    public string GetTempPath()
    {
        return Path.Combine(Application.persistentDataPath, "temp");
    }

    public void SelectItem(string name)
    {
        nameInput.text = name;
    }

    void FillList()
    {
        for(int i = 0; i < listContent.childCount; ++i)
            Destroy(listContent.GetChild(i).gameObject);

        string[] paths = Directory.GetFiles(Application.persistentDataPath, "*.map");
        Array.Sort(paths);
        for(int i = 0; i < paths.Length; ++i)
        {
            SaveLoadMenuItem item = Instantiate(itemPrefab);
            item.menu = this;
            item.MapName = Path.GetFileNameWithoutExtension(paths[i]);
            item.transform.SetParent(listContent, false);
        }
    }
}