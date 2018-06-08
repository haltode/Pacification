using System.IO;
using UnityEngine;

public class MapSenderReceiver : MonoBehaviour {

    public string path = "";
    
    public SaveLoadMapMenu SaveAndLoadPrefab;
    public SaveLoadMapMenu saveAndLoad;

    void Start()
    {
        Server server = FindObjectOfType<Server>();
        saveAndLoad = Instantiate(SaveAndLoadPrefab);

        if(!server)
            return;

        string map;
        path = GameManager.Instance.path;
        if(path == "")
        {
            path = Path.Combine(Application.persistentDataPath, "31uikx83y54gnt661zf651_646a89knz7984dt13a");
            HexMapGenerator generator = FindObjectOfType<HexMapGenerator>();
            generator.GenerateMap();
            saveAndLoad.Save(path);
            map = File.ReadAllText(path);
            File.Delete(path);
        }
        else
        {
            map = File.ReadAllText(path);
            saveAndLoad.Load(path);
            Debug.Log(path);
        }

        server.Broadcast("SMAP|" + map, server.clients);
    }

    public void StartGame(string map)
    {
        path = Path.Combine(Application.persistentDataPath, "31uikx83y54gnt661zf651_646a89knz7984dt13a");
        File.WriteAllText(path, map);
        saveAndLoad.Load(path);
        File.Delete(path);
    }
}
