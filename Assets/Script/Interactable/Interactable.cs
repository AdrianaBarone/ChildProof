using UnityEngine;

public abstract class Interactable : MonoBehaviour {

    [SerializeField] Camera interactableCamera;
        
    public void BaseInteract() {
        Interact();
    }

    public Camera GetCamera() {
        return interactableCamera;
    }

    protected virtual void Interact() {
        //Funzione vuota che sarà sovrascritta di volta in volta dalle sottoclassi
    }
}
