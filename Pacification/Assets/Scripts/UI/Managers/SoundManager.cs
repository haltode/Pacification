using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip barbarianSpawn;

    public void PlayBarbarianSpawn()
    {
        AudioSource.PlayClipAtPoint(barbarianSpawn, Camera.main.transform.position);
    }
}
