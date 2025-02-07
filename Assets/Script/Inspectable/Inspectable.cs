using System.Collections;
using UnityEngine;

public class Inspectable : MonoBehaviour {

    [SerializeField] AchievementData achievementData;
    [SerializeField] Camera interactionCamera;
    private int interactionNumber;
    private int maxInteractionNumber;
    private bool canInteract = true;
    public DropZone[] dropZones;

    //AUDIO
    private AudioSource[] audioSources;
    public AudioClip[] audioClips;


    private void Awake() {
        maxInteractionNumber = dropZones.Length;
        interactionNumber = maxInteractionNumber;
    }

private void Start()
    {
        // Rimuove AudioSource esistenti per evitarne la duplicazione
        foreach (var source in GetComponents<AudioSource>())
        {
            Destroy(source);
        }

        audioSources = new AudioSource[audioClips.Length];

        for (int i = 0; i < audioClips.Length; i++)
        {
            audioSources[i] = gameObject.AddComponent<AudioSource>();
            audioSources[i].clip = audioClips[i];
        }

        if (audioClips.Length == 0)
        {
            Debug.LogWarning("Nessun audio clip assegnato.");
        }
    }

    public bool IsResolved() {
        return interactionNumber == 0;
    }

    public Camera GetCamera() {
        return interactionCamera;
    }

    public void BaseInteract() {
        // NOTE: aggiungere debounce per evitare che l'interazione venga chiamata più volte
        if (canInteract && !IsResolved()) {
            canInteract = false;
            StartCoroutine(Debounce());
            GetComponent<Animator>().SetTrigger("isInteracting");
            PlayAudioOnAnimation();
        }
    }

    public AchievementData GetAchievementData() {
        return achievementData;
    }

    IEnumerator Debounce() {
        yield return new WaitForSeconds(1);
        canInteract = true;
    }
    public void Resolve() {
        interactionNumber--;
        if (interactionNumber == 0) {
            AchievementManager.Instance.IncrementAchievement(achievementData);
            PlayerManager.Instance.SetToExploration();
        }
    }

    // AUDIO
    public void PlayAudioOnAnimation()
    {
        foreach (var source in audioSources)
        {
            if (source.clip != null && !source.isPlaying)
            {
                source.Play();
            }
        }
    }

}