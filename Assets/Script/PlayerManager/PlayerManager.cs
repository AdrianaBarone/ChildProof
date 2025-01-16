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
    [SerializeField] private CursorManager cursorManager;

    void Awake() {
        Instance = this;

        playerInteraction = GetComponent<PlayerInteraction>();
        playerMovement = GetComponent<PlayerMovement>();
        cursorManager = FindObjectOfType<CursorManager>();
    }

    void Start() {
        cursorManager.ExplorationCursor();
    }

    void Update() {
        switch (state) {
            case PlayerState.EXPLORATION:
                cursorManager.ExplorationCursor();
                playerInteraction.RaycastForInteractable();
                playerMovement.HandleMovement();
                break;
            case PlayerState.INSPECTION:
                cursorManager.InspectionCursor();
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

