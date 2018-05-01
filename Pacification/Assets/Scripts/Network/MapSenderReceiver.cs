using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSenderReceiver : MonoBehaviour {

    public string path;

    public SaveLoadMapMenu SaveAndLoadPrefab;
    public SaveLoadMapMenu saveAndLoad;

    void Start ()
    {
        Server server = FindObjectOfType<Server>();
        saveAndLoad = Instantiate(SaveAndLoadPrefab);
        path = Path.Combine(Application.persistentDataPath, "temp");

        if(!server)
            return;

        HexMapGenerator generator = FindObjectOfType<HexMapGenerator>();
        generator.GenerateMap(50, 50);

        saveAndLoad.Save(path);
        string map = File.ReadAllText(path);
        File.Delete(path);

        server.Broadcast("SMAP|" + map, server.clients);
    }

    public void StartGame(string map)
    {
        File.WriteAllText(path, map);
        saveAndLoad.Load(path);
        File.Delete(path);
    }
}
