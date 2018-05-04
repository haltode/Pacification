using UnityEngine;

public class SoundManager : MonoBehaviour
{
    AudioSource[] gameSoundtrack;
    public AudioClip barbarianSpawn;

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
}
