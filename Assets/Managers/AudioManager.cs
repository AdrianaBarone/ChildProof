using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private List<AudioSource> audioSources = new List<AudioSource>();

    public float fadeInDuration = 3f;
    public float fadeOutDuration = 3f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void PlayAudioWithFadeIn(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource availableSource = GetAvailableAudioSource();
            if (availableSource != null)
            {
                StartCoroutine(FadeInAudio(availableSource, clip));
            }
            else
            {
                AudioSource newSource = gameObject.AddComponent<AudioSource>();
                audioSources.Add(newSource);
                StartCoroutine(FadeInAudio(newSource, clip));
            }
        }
        else
        {
            Debug.LogWarning("AudioClip non assegnato.");
        }
    }

    private AudioSource GetAvailableAudioSource()
    {
        foreach (var source in audioSources)
        {
            if (!source.isPlaying) 
            {
                return source;
            }
        }
        return null;
    }

    public void StopAudioWithFadeOut(AudioClip clip)
    {
        foreach (var source in audioSources)
        {
            if (source.isPlaying && source.clip == clip)
            {
                StartCoroutine(FadeOutAudio(source, clip));
            }
        }
    }

    private IEnumerator FadeInAudio(AudioSource audioSource, AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.volume = 0f;
        audioSource.Play();

        float timeElapsed = 0f;

        while (timeElapsed < fadeInDuration)
        {
            audioSource.volume = Mathf.Lerp(0f, 1f, timeElapsed / fadeInDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = 1f;
    }

    private IEnumerator FadeOutAudio(AudioSource audioSource, AudioClip clip)
    {
        if (audioSource.clip != clip)
        {
            yield break;
        }

        float timeElapsed = 0f;

        while (timeElapsed < fadeOutDuration)
        {
            audioSource.volume = Mathf.Lerp(1f, 0f, timeElapsed / fadeOutDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = 0f;
    }
}