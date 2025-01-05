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


    private PlayerInteraction playerInteraction;
    private PlayerMovement playerMovement;

    void Awake() {
        Instance = this;

        playerInteraction = GetComponent<PlayerInteraction>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() {
        switch (state) {
            case PlayerState.EXPLORATION:
                playerInteraction.RaycastForInteractable();
                playerMovement.HandleMovement();
                break;
            case PlayerState.INSPECTION:
                if (Input.GetKeyDown(KeyCode.Tab)) {
                    SetToExploration();
                }

                break;
            default:
                break;
        }
    }

    public void TransitionToInspection() {
        Cursor.lockState = CursorLockMode.None;
        state = PlayerState.INSPECTION;
    }

    public void TransitionToExploration() {
        Cursor.lockState = CursorLockMode.Locked;
        state = PlayerState.EXPLORATION;
    }

    public void SetToExploration() {
        playerInteraction.EndInteractionExternal();
    }

    public void PrepareTransition(){
        state = PlayerState.TRANSITION;
    }

    public bool InStateInspection() {
        return state == PlayerState.INSPECTION;
    }

    public bool IsInStateExploration() {
        return state == PlayerState.EXPLORATION;
    }
}

