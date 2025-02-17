using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;

    private List<AudioSource> audioSources = new List<AudioSource>();

    public float fadeInDuration = 3f;
    public float fadeOutDuration = 3f;

    [Header("Info Mixer Audio")]
    public AudioMixer audioMixer;
    public AudioMixerSnapshot sfx;
    public AudioMixerSnapshot player;
    public AudioMixerSnapshot voice;
    //public AudioMixerSnapshot music;

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

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        passiSource = CreateAudioSource(passiClip, true);
        cameraAudioSource = CreateAudioSource(cameraTransitionClip, false);
        audioSafeSource = CreateAudioSource(audioClipSafe, true);
        audioDangerSource = CreateAudioSource(audioClipDanger, true);
    }

    public AudioSource CreateAudioSource(AudioClip audioClip, bool loop) {
        foreach (var source in audioSources) {
            if (source.clip == audioClip) {
                return source;
            }
        }

        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.clip = audioClip;
        newSource.loop = loop;
        newSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[0];

        audioSources.Add(newSource);

        return newSource;
    }


    public void PlayFootsteps(bool isMoving) {
        if (isMoving && !passiSource.isPlaying) {
            passiSource.Play();
        }
        else if (!isMoving && passiSource.isPlaying) {
            passiSource.Stop();
        }
    }

    public void PlayCameraTransitionSound() {
        ChangeToSnapshot(1,3f);
        if (cameraTransitionClip != null) {
            cameraAudioSource.Play();
        }
    }

    public void StopCameraTransitionSound() {
        if (cameraAudioSource.isPlaying) {
            cameraAudioSource.Stop();
        }
    }

    public void PlaySound(AudioClip audioClip) {
        AudioSource audioSource = CreateAudioSource(audioClip, false);
        audioSource.Play();
    }

    public void StopSound(AudioClip audioClip) {
        AudioSource audioSource = CreateAudioSource(audioClip, false);
        audioSource.Stop();
    }

    public void PlayAudioWithFadeIn(bool InDangerMode) {
        if (!InDangerMode) {
            StartCoroutine(FadeInAudio(audioSafeSource, audioClipSafe));
        }
        else if (InDangerMode) {
            StartCoroutine(FadeInAudio(audioDangerSource, audioClipDanger));
        }
    }

    public void StopAudioWithFadeOut(bool InDangerMode) {
        if (!InDangerMode) {
            if (audioDangerSource.isPlaying) {
                StartCoroutine(FadeOutAudio(audioDangerSource, audioClipDanger));
            }
        }
        else if (InDangerMode) {
            if (audioDangerSource.isPlaying) {
                StartCoroutine(FadeOutAudio(audioSafeSource, audioClipSafe));
            }
        }
    }

    private IEnumerator FadeInAudio(AudioSource audioSource, AudioClip clip) {
        audioSource.clip = clip;
        audioSource.volume = 0f;
        audioSource.loop = true;
        audioSource.Play();

        float timeElapsed = 0f;

        while (timeElapsed < fadeInDuration) {
            audioSource.volume = Mathf.Lerp(0f, 1f, timeElapsed / fadeInDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = 1f;
    }

    private IEnumerator FadeOutAudio(AudioSource audioSource, AudioClip clip) {
        if (audioSource.clip != clip) {
            yield break;
        }

        float timeElapsed = 0f;

        while (timeElapsed < fadeOutDuration) {
            audioSource.volume = Mathf.Lerp(1f, 0f, timeElapsed / fadeOutDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = 0f;
    }

    public void ChangeToSnapshot(int snapshotIndex, float transitionTime) {
        switch (snapshotIndex) {
            case 1:
                sfx.TransitionTo(transitionTime);
                break;
            case 2:
                player.TransitionTo(transitionTime);
                break;
            case 3:
                voice.TransitionTo(transitionTime);
                break;
            
            /*
            case 4:
                music.TransitionTo(transitionTime);
                break;
            */
            default:
                Debug.LogWarning("Snapshot index non valido.");
                break;
        }
    }

}
