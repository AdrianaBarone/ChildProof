using UnityEngine;

public enum PositionType {
    Above,
    Forward,
    Side
}

public abstract class Interactable : MonoBehaviour {
    public PositionType positionType;

    public void BaseInteract() {
        Interact();
    }

    public (Vector3, Quaternion) GetTargetPositionAndRotation() {
        Vector3 targetPosition = positionType switch {
            PositionType.Above => transform.position + transform.up * 5f,// Posizione sopra l'oggetto
            PositionType.Forward => transform.position + transform.forward * 5f,// Posizione davanti all'oggetto
            PositionType.Side => transform.position + transform.right * 5f,// Posizione di lato all'oggetto
            _ => transform.position + transform.forward * 5f,// Default, fotocamera davanti
        };
        Quaternion targetRotation = Quaternion.LookRotation(transform.position - targetPosition);  // orientamento verso l'oggetto

        return (targetPosition, targetRotation);
    }

    protected virtual void Interact() {
        //Funzione vuota che sarà sovrascritta di volta in volta dalle sottoclassi
    }
}
