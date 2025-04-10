using UnityEngine;

public class KarakasaAudioController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hopSound;

    public void PlayHopSound()
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(hopSound);
        }
        else
        {
            Debug.LogWarning("AudioSource is not assigned.");
        }
    }
}
