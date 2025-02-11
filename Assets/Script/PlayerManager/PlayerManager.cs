using System.Collections;
using UnityEngine;

public enum PlayerState {
    EXPLORATION,
    INSPECTION,
    PHONE_UP,
    TRANSITION, // NOTE: stato dummy, per bloccare le interazioni durante le transizioni
}

public class PlayerManager : MonoBehaviour {
    public static PlayerManager Instance;
    private PlayerState state = PlayerState.EXPLORATION;
    private PlayerState lastState = PlayerState.EXPLORATION;
    public Inspectable currentInspectable;


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

    void Update() {
        switch (state) {
            case PlayerState.EXPLORATION:
                InventoryManager.Instance.ShowInventory();
                CursorManager.Instance.ExplorationCursor();
                playerInteraction.RaycastForInspectable();
                playerMovement.HandleRotation();
                break;
            case PlayerState.INSPECTION:
                InventoryManager.Instance.ShowInventory();
                CursorManager.Instance.InspectionCursor();
                InventoryManager.Instance.HandleInventory();
                playerInteraction.TryPickUp();
                if (Input.GetKeyDown(KeyCode.Tab)) {
                    InventoryManager.Instance.ClearSelection();
                    SetToExploration();
                }
                break;
            case PlayerState.PHONE_UP:
                CursorManager.Instance.InspectionCursor();
                playerMovement.StopMovement();
                InventoryManager.Instance.HideInventory();
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

    public void TransitionToInspection(Inspectable inspectable) {
        Cursor.lockState = CursorLockMode.None;
        state = PlayerState.INSPECTION;
        currentInspectable = inspectable;
        inspectable.RemoveObject();
    }

    public void TransitionToExploration() {
        Cursor.lockState = CursorLockMode.Locked;
        state = PlayerState.EXPLORATION;

        if (currentInspectable != null)
            currentInspectable.RestoreObject();
        currentInspectable = null;
    }

    public Camera GetInspectableCamera() {
        return currentInspectable.GetCamera();
    }

    public void SetToExploration() {
        playerInteraction.EndInteractionExternal();
    }

    public void SetToPhoneUp() {
        state = PlayerState.PHONE_UP;
    }

    public void PrepareTransition() {
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

