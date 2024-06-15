using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    public AudioClip uiSound;
    private AudioSource uiAudioSource;
    private AudioSource[] gameAudioSources;

    private void Awake()
    {
        uiAudioSource = gameObject.AddComponent<AudioSource>();
        uiAudioSource.clip = uiSound;
        gameAudioSources = FindObjectsOfType<AudioSource>();
    }

    public void PlayUISound()
    {
        // Stop all other game sounds
        foreach (AudioSource audioSource in gameAudioSources)
        {
            if (audioSource != uiAudioSource && audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }

        // Play the UI sound
        uiAudioSource.Play();

        // Resume all other game sounds after the UI sound has finished
        Invoke("ResumeGameSounds", uiSound.length);
    }

    private void ResumeGameSounds()
    {
        foreach (AudioSource audioSource in gameAudioSources)
        {
            if (audioSource != uiAudioSource)
            {
                audioSource.UnPause();
            }
        }
    }

    private void OnEnable()
    {
        PlayUISound();
    }
}
