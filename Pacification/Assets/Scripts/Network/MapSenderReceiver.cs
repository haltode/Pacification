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

        if(!server)
            return;

        saveAndLoad = Instantiate(SaveAndLoadPrefab);
        path = Path.Combine(Application.persistentDataPath, "temp");

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
