using System.Collections;
using UnityEngine;

public enum PlayerState {
    EXPLORATION,
    INSPECTION,
    TRANSITION, // NOTE: stato dummy, per bloccare le interazioni durante le transizioni
}

public class PlayerManager : MonoBehaviour {
    public static PlayerManager Instance;
    private PlayerState state = PlayerState.EXPLORATION;
    private PlayerState lastState = PlayerState.EXPLORATION;
    public Interactable currentInteractable;


    public PlayerInteraction playerInteraction;
    private PlayerMovement playerMovement;

    void Awake() {
        Instance = this;

        playerInteraction = GetComponent<PlayerInteraction>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Start() {
        CursorManager.Instance.ExplorationCursor();
    }

    void Update(){
        switch (state) {
            case PlayerState.EXPLORATION:
                CursorManager.Instance.ExplorationCursor();
                playerInteraction.RaycastForInteractable();
                playerMovement.HandleRotation();
                break;
            case PlayerState.INSPECTION:
                CursorManager.Instance.InspectionCursor();
                InventoryManager.Instance.HandleInventory();
                playerInteraction.TryPickUp();
                if (Input.GetKeyDown(KeyCode.Tab)) {
                    InventoryManager.Instance.ClearSelection();
                    SetToExploration();
                }
                break;
            default:
                break;
        }
    }

    void FixedUpdate() {
        switch (state) {
            case PlayerState.EXPLORATION:
                playerMovement.HandleMovement();
                break;
            case PlayerState.INSPECTION:
                break;
            default:
                break;
        }
    }

    public void TransitionToInspection(Interactable interactable) {
        Cursor.lockState = CursorLockMode.None;
        state = PlayerState.INSPECTION;
        currentInteractable = interactable;
    }

    public void TransitionToExploration() {
        Cursor.lockState = CursorLockMode.Locked;
        state = PlayerState.EXPLORATION;
        currentInteractable = null;
    }

    public Camera GetInteractableCamera() {
        return currentInteractable.GetCamera();
    }

    public void SetToExploration() {
        playerInteraction.EndInteractionExternal();
    }

    public void PrepareTransition(){
        lastState = state;
        state = PlayerState.TRANSITION;
    }

    public void ReturnToPreviousState() {
        state = lastState;
    }

    public bool InStateInspection() {
        return state == PlayerState.INSPECTION;
    }

    public bool IsInStateExploration() {
        return state == PlayerState.EXPLORATION;
    }
}

