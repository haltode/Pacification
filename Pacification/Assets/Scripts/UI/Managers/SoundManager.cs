using UnityEngine;

public class SoundManager : MonoBehaviour
{
    AudioSource[] gameSoundtrack;
    public AudioClip barbarianSpawn;
    public AudioClip newBuilding;
    public AudioClip newCity;
    public AudioClip newAttacker;

    private void Start()
    {
        gameSoundtrack = GetComponents<AudioSource>();
        gameSoundtrack[0].Play();
    }

    public void PlayTheSimsMusic()
    {
        if(gameSoundtrack[0].isPlaying)
            gameSoundtrack[0].Stop();
        gameSoundtrack[1].Play();
    }

    public void PlayBarbarianSpawn()
    {
        AudioSource.PlayClipAtPoint(barbarianSpawn, Camera.main.transform.position);
    }

    public void PlayNewBuilding()
    {
        AudioSource.PlayClipAtPoint(newBuilding, Camera.main.transform.position);
    }

    public void PlayNewCity()
    {
        AudioSource.PlayClipAtPoint(newCity, Camera.main.transform.position);
    }

    public void PlayNewAttacker()
    {
        AudioSource.PlayClipAtPoint(newAttacker, Camera.main.transform.position);
    }
}
