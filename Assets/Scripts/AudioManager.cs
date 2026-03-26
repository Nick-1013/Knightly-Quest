using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;

    public void PlayAudio()
    {
        audioSource.PlayOneShot(audioClip);
    }
}
