using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSender : MonoBehaviour {

    public HexGrid hexgrid;
    public string path;

    void Start () {
        Server server = FindObjectOfType<Server>();
        if(!server)
            return;

        path = Path.Combine(Application.persistentDataPath, "temp");
        hexgrid = FindObjectOfType<HexGrid>();

        Save();
        string map = File.ReadAllText(path + "S");
        server.Broadcast("SMAP|" + map, server.clients);
        File.Delete(path + "S");
    }

    public void StartGame(string map)
    {
        File.WriteAllText(path + "R", map);
        Load();
        File.Delete(path + "R");

        SceneManager.LoadScene("Map");
    }

    //##################### TEMPORAIRE

    public void Save()
    {
        using(BinaryWriter writer = new BinaryWriter(File.Open(path + "S", FileMode.Create)))
        {
            writer.Write(SaveLoadMapMenu.MapFormatVersion);
            hexgrid.Save(writer);
        }
    }

    public void Load()
    {
        hexgrid = FindObjectOfType<HexGrid>();

        if(!File.Exists(path + "R"))
        {
            Debug.LogError("File does not exist " + path + "R");
            return;
        }

        using(BinaryReader reader = new BinaryReader(File.OpenRead(path + "R")))
        {
            int header = reader.ReadInt32();
            if(header == SaveLoadMapMenu.MapFormatVersion)
            {
                hexgrid.Load(reader);
                HexMapCamera.ValidatePosition();
            }
            else
                Debug.LogWarning("Unknown map format " + header);
        }
    }
}
