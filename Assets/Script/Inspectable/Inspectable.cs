using UnityEngine;

public abstract class Inspectable : MonoBehaviour {

     private int interactionNumber;
    private int maxInteractionNumber;

    private void Awake() {
        foreach (Transform child in transform) {
            if (child.GetComponent<DropZone>() != null) {
                maxInteractionNumber++;
            }
        }
        interactionNumber = maxInteractionNumber;
    }

    public bool IsResolved() {
        return interactionNumber == 0;
    }
    public void Resolve(){
        interactionNumber--;
        if (interactionNumber == 0) {
            // TODO: animazione dell'oggetto
            // TODO: cambio stato di player manager
            PlayerManager.Instance.SetToExploration();
        }
    }
}