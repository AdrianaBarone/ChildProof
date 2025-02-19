using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;

    private List<AudioSource> audioSources = new List<AudioSource>();

    public float fadeInDuration = 3f;
    public float fadeOutDuration = 3f;


    [Header("AudioMixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("AudioMixerGroups")]
    [SerializeField] private List<AudioMixerGroup> mixerGroups = new List<AudioMixerGroup>();
    private Dictionary<string, AudioMixerGroup> mixerGroupsDict = new Dictionary<string, AudioMixerGroup>();

    [Header("Suoni Giocatore")]
    public AudioClip passiClip;
    private AudioSource passiSource;

    [Header("Audio cambio camera")]
    public AudioClip cameraTransitionClip;
    private AudioSource cameraAudioSource;

/*
    [Header("Audio Punti")]
    public AudioClip pointsGainClip;
    public AudioClip pointsLossClip;
    private AudioSource pointsGainSource;
    private AudioSource pointsLossSource;
    */

    [Header("Audio SoundTrack")]
    public AudioClip audioClipSafe;
    public AudioClip audioClipDanger;
    private AudioSource audioSafeSource;
    private AudioSource audioDangerSource;

    private SnapshotManager snapshotManager;

    private void Awake() {
        Instance = this;

        // Costruisce il dizionario con i gruppi mixer
        foreach (var group in mixerGroups) {
            mixerGroupsDict[group.name] = group;
        }

        passiSource = CreateAudioSource(passiClip, true, "Player");
        cameraAudioSource = CreateAudioSource(cameraTransitionClip, false, "SFX");
        audioSafeSource = CreateAudioSource(audioClipSafe, true, "Music");
        audioDangerSource = CreateAudioSource(audioClipDanger, true, "Music");
        //pointsGainSource = CreateAudioSource(pointsGainClip, false, "SFX");
        //pointsLossSource = CreateAudioSource(pointsLossClip, false, "SFX");
    }

    private void Start() {
        snapshotManager = GetComponent<SnapshotManager>();
        snapshotManager.ChangeSnapshot(SnapshotState.Player, 0);
    }

    public AudioSource CreateAudioSource(AudioClip audioClip, bool loop, string groupName) {
        if (audioClip == null) {
            Debug.Log(" AudioClip è NULL! Controlla gli assegnamenti in AudioManager.");
            return null;
        }

        if (!mixerGroupsDict.ContainsKey(groupName)) {
            Debug.LogError($" Il gruppo AudioMixer '{groupName}' non esiste! Controlla il nome.");
            return null;
        }

        foreach (var source in audioSources) {
            if (source.clip == audioClip) {
                return source;
            }
        }

        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.clip = audioClip;
        newSource.loop = loop;
        newSource.outputAudioMixerGroup = mixerGroupsDict[groupName];
        newSource.playOnAwake = false;

        audioSources.Add(newSource);

        Debug.Log($"Creato AudioSource per '{audioClip.name}' nel gruppo '{groupName}'");

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
        if (cameraTransitionClip != null) {
            snapshotManager.ChangeSnapshot(SnapshotState.SFX, 1f);
            cameraAudioSource.Play();
        }
    }

    public void StopCameraTransitionSound() {
        if (cameraAudioSource.isPlaying) {
            cameraAudioSource.Stop();
        }
    }

    public void PlaySound(AudioClip audioClip) {
        AudioSource audioSource = CreateAudioSource(audioClip, false, "SFX");
        snapshotManager.ChangeSnapshot(SnapshotState.SFX, 1f);
        audioSource.Play();
        StartCoroutine(WaitForSoundToFinish(audioSource));
    }

/*
    public void PlayPointsSound(bool isGain) {
        snapshotManager.ChangeSnapshot(SnapshotState.SFX, 1f);
        if (isGain) {
            if (pointsGainSource != null) {
                pointsGainSource.volume = 1;
                pointsGainSource.Play();
            }
        }
        else {
            if (pointsLossSource != null) pointsLossSource.Play();
        }
    }
*/
    private IEnumerator WaitForSoundToFinish(AudioSource audioSource) {
        yield return new WaitForSeconds(audioSource.clip.length);
        yield return new WaitForSeconds(1f);
        if (!audioSource.isPlaying && snapshotManager.GetCurrentSnapshot() != SnapshotState.Voice) {
            snapshotManager.ChangeSnapshot(SnapshotState.Player, 3f);
        }
    }

    public void PlayDialogs(AudioSource audioSourceQuestion, AudioSource audioSourceAnswer) {
        snapshotManager.ChangeSnapshot(SnapshotState.Voice, 0);
        audioSourceQuestion.Play();
        audioSourceAnswer.PlayScheduled(AudioSettings.dspTime + audioSourceQuestion.clip.length);
        StartCoroutine(ResetSnapshotAfterDialog(audioSourceQuestion, audioSourceAnswer));
    }

    private IEnumerator ResetSnapshotAfterDialog(AudioSource audioSourceQuestion, AudioSource audioSourceAnswer) {
        yield return new WaitForSeconds(audioSourceQuestion.clip.length);
        yield return new WaitForSeconds(audioSourceAnswer.clip.length);

        if (!audioSourceQuestion.isPlaying && !audioSourceAnswer.isPlaying) {
            snapshotManager.ChangeSnapshot(SnapshotState.Player, 3f);
        }
    }



    public void PlayAudioWithFadeIn(bool InDangerMode) {
        if (!InDangerMode) {
            StartCoroutine(FadeInAudio(audioSafeSource, audioClipSafe));
        }
        else {
            StartCoroutine(FadeInAudio(audioDangerSource, audioClipDanger));
        }
    }

    public void StopAudioWithFadeOut(bool InDangerMode) {
        if (!InDangerMode) {
            if (audioDangerSource.isPlaying) {
                StartCoroutine(FadeOutAudio(audioDangerSource, audioClipDanger));
            }
        }
        else {
            if (audioSafeSource.isPlaying) {
                StartCoroutine(FadeOutAudio(audioSafeSource, audioClipSafe));
            }
        }
    }

    private IEnumerator FadeInAudio(AudioSource audioSource, AudioClip clip) {
        if (audioSource == null || clip == null) {
            Debug.LogError($"FadeInAudio fallito: AudioSource o AudioClip è NULL!");
            yield break;
        }

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
        if (audioSource == null || clip == null) {
            Debug.LogError($"FadeOutAudio fallito: AudioSource o AudioClip è NULL!");
            yield break;
        }

        float timeElapsed = 0f;
        while (timeElapsed < fadeOutDuration) {
            audioSource.volume = Mathf.Lerp(1f, 0f, timeElapsed / fadeOutDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
    }

}
