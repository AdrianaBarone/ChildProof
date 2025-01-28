using System.Collections;
using UnityEngine;

public class Inspectable : MonoBehaviour {
    [SerializeField] private AchievementData achievementData;
    [SerializeField] Camera interactionCamera;
    private int interactionNumber;
    private int maxInteractionNumber;
    private bool canInteract = true;
    public DropZone[] dropZones;

    private void Awake() {
        maxInteractionNumber = dropZones.Length;
        interactionNumber = maxInteractionNumber;
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
        }
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
}