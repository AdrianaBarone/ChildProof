using System.Collections.Generic;
using UnityEngine;

public class Inspectable : MonoBehaviour {

     private int interactionNumber;
    private int maxInteractionNumber;
    public DropZone[] dropZones;

    private void Awake() {
        maxInteractionNumber = dropZones.Length;
        interactionNumber = maxInteractionNumber;
    }

    public bool IsResolved() {
        return interactionNumber == 0;
    }
    public void Resolve(){
        interactionNumber--;
        if (interactionNumber == 0) {
            // TODO: animazione dell'oggetto
            // TODO: aggiungere riferimento all'achievement sbloccato (preso da DropZone)
            // TODO: aggiungere punti al giocatore
            PlayerManager.Instance.SetToExploration();
        }
    }
}