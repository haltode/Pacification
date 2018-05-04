using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip barbarianSpawn;

    private void Start()
    {
        AudioSource[] audio = GetComponents<AudioSource>();
        if(GameManager.Instance.gamemode == GameManager.Gamemode.EDITOR)
            audio[1].Play();
        else
            audio[0].Play();
    }

    public void PlayBarbarianSpawn()
    {
        AudioSource.PlayClipAtPoint(barbarianSpawn, Camera.main.transform.position);
    }
}
