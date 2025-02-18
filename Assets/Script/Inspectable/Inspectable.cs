using System.Collections;
using UnityEngine;

public class Inspectable : MonoBehaviour
{

    [SerializeField] AchievementData achievementData;
    [SerializeField] Camera interactionCamera;
    [SerializeField] GameObject[] objectsToDisable;
    [SerializeField] GameObject[] objectToEnableOnRestore;
    private int interactionNumber;
    private int maxInteractionNumber;
    private bool canInteract = true;
    public DropZone[] dropZones;


    [Header("Audio Animazione")]
    public AudioClip[] audioClips;

    private void Awake()
    {
        maxInteractionNumber = dropZones.Length;
        interactionNumber = maxInteractionNumber;
    }

    public bool IsResolved()
    {
        return interactionNumber == 0;
    }

    public Camera GetCamera()
    {
        return interactionCamera;
    }

    public void RemoveObject()
    {

        foreach (var obj in objectsToDisable)
        {
            obj.SetActive(false);
        }

        foreach (var obj in objectToEnableOnRestore)
        {
            obj.SetActive(true);
        }
    }


    public void RestoreObject()
    {
        if (!IsResolved())
        {
            foreach (var obj in objectsToDisable)
            {
                obj.SetActive(true);
            }
            foreach (var obj in objectToEnableOnRestore)
            {
                obj.SetActive(false);
            }
        }
    }

    public void BaseInteract()
    {
        // NOTE: aggiungere debounce per evitare che l'interazione venga chiamata più volte
        if (canInteract && !IsResolved())
        {
            canInteract = false;
            StartCoroutine(Debounce());
            GetComponent<Animator>().SetTrigger("isInteracting");
            for (int i = 0; i < audioClips.Length; i++)
            {
                AudioManager.Instance.CreateAudioSource(audioClips[i], false, "SFX");
                AudioManager.Instance.PlaySound(audioClips[i]);
            }
        }
    }

    public AchievementData GetAchievementData()
    {
        return achievementData;
    }

    IEnumerator Debounce()
    {
        yield return new WaitForSeconds(1);
        canInteract = true;
    }
    public void Resolve()
    {
        interactionNumber--;
        if (interactionNumber == 0)
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.tag == "NotSolvedTarget")
                {
                    child.gameObject.tag = "SolvedTarget";
                }
            }

            AchievementManager.Instance.IncrementAchievement(achievementData);
            PlayerManager.Instance.SetToExploration();
        }
    }

/*
    public void OnCameraActivated()
    {
        if (InStateInspection){
        if (!hasPlayedAchievementAudio && achievementData != null && achievementData.audioClipMum != null && achievementData.audioClipDad != null)
        {
            AudioSource audioSourceMum = AudioManager.Instance.CreateAudioSource(achievementData.audioClipMum, false, "Dialoghi");
            audioSourceMum.panStereo = 1.0f;

            AudioSource audioSourceDad = AudioManager.Instance.CreateAudioSource(achievementData.audioClipDad, false, "Dialoghi");
            audioSourceDad.panStereo = -1.0f;

            // TO DO: funzione in audio che fa partire l'audio della mamma con il corretto snapshot
            //audioSourceMum.Play();
            //audioSourceDad.PlayScheduled(AudioSettings.dspTime + audioSourceMum.clip.length);

            hasPlayedAchievementAudio = true;
        }
        }
    }
    */
}