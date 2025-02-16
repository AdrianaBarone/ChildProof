using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private List<AudioSource> audioSources = new List<AudioSource>();

    public float fadeInDuration = 3f;
    public float fadeOutDuration = 3f;

    [Header("Mixer Audio")]
    public AudioMixer audioMixer;
    public AudioMixerSnapshot snapshotDefault;
    public AudioMixerSnapshot snapshotSafe;
    public AudioMixerSnapshot snapshotDanger;
    public AudioMixerSnapshot snapshotMute;

    [Header("Suoni Giocatore")]
    public AudioClip passiClip;
    private AudioSource passiSource;

    [Header("Audio cambio camera")]
    public AudioClip cameraTransitionClip;
    private AudioSource cameraAudioSource;

    [Header("Audio SoundTrack")]
    public AudioClip audioClipSafe;
    public AudioClip audioClipDanger;
    private AudioSource audioSafeSource;
    private AudioSource audioDangerSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        passiSource = CreateAudioSource(passiClip, true, "Player");
        cameraAudioSource = CreateAudioSource(cameraTransitionClip, false, "SFX");
        audioSafeSource = CreateAudioSource(audioClipSafe, true, "Music");
        audioDangerSource = CreateAudioSource(audioClipDanger, true, "Music");

        // Avvia l'audio iniziale (es. Safe)
        PlayAudioWithFadeIn(false);
    }

    private AudioSource CreateAudioSource(AudioClip audioClip, bool loop, string groupName)
    {
        if (audioClip == null) return null;

        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.clip = audioClip;
        newSource.loop = loop;
        newSource.volume = 1f;

        // Assegna il gruppo audio corretto
        AudioMixerGroup[] groups = audioMixer.FindMatchingGroups(groupName);
        if (groups.Length > 0)
        {
            newSource.outputAudioMixerGroup = groups[0];
        }

        audioSources.Add(newSource);
        return newSource;
    }

    public void PlayFootsteps(bool isMoving)
    {
        if (isMoving && !passiSource.isPlaying)
        {
            passiSource.Play();
        }
        else if (!isMoving && passiSource.isPlaying)
        {
            passiSource.Stop();
        }
    }

    public void PlayCameraTransitionSound()
    {
        if (cameraTransitionClip != null)
        {
            cameraAudioSource.Play();
        }
    }

    public void StopCameraTransitionSound()
    {
        if (cameraAudioSource.isPlaying)
        {
            cameraAudioSource.Stop();
        }
    }

    public void PlaySound(AudioClip audioClip)
    {
        AudioSource audioSource = CreateAudioSource(audioClip, false, "SFX");
        audioSource.Play();
    }

    public void StopSound(AudioClip audioClip)
    {
        foreach (var source in audioSources)
        {
            if (source.clip == audioClip)
            {
                source.Stop();
                break;
            }
        }
    }

    public void PlayAudioWithFadeIn(bool InDangerMode)
    {
        if (!InDangerMode)
        {
            StartCoroutine(FadeInAudio(audioSafeSource, audioClipSafe));
        }
        else
        {
            StartCoroutine(FadeInAudio(audioDangerSource, audioClipDanger));
        }
    }

    public void StopAudioWithFadeOut(bool InDangerMode)
    {
        if (!InDangerMode)
        {
            if (audioSafeSource.isPlaying)
            {
                StartCoroutine(FadeOutAudio(audioSafeSource, audioClipSafe));
            }
        }
        else
        {
            if (audioDangerSource.isPlaying)
            {
                StartCoroutine(FadeOutAudio(audioDangerSource, audioClipDanger));
            }
        }
    }

    public void ChangeToSnapshot(int snapshotIndex, float transitionTime)
    {
        switch (snapshotIndex)
        {
            case 1:
                snapshotDefault.TransitionTo(transitionTime);
                break;
            case 2:
                snapshotSafe.TransitionTo(transitionTime);
                break;
            case 3:
                snapshotDanger.TransitionTo(transitionTime);
                break;
            case 4:
                snapshotMute.TransitionTo(transitionTime);
                break;
            default:
                Debug.LogWarning("Snapshot index non valido.");
                break;
        }
    }

    private IEnumerator FadeInAudio(AudioSource audioSource, AudioClip clip)
    {
        if (audioSource == null || clip == null) yield break;

        audioSource.clip = clip;
        audioSource.volume = 0f;
        audioSource.loop = true;
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
        if (audioSource == null || clip == null || audioSource.clip != clip) yield break;

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
