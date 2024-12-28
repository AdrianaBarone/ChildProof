using UnityEngine;

public enum PlayerState
{
    EXPLORATION,
    INSPECTION,
}

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    private PlayerState state = PlayerState.EXPLORATION;


    private PlayerInteraction playerInteraction;
    private PlayerMovement playerMovement;
    private PlayerInspection playerInspection;

    void Start()
    {
        Instance = this;

        playerInteraction = GetComponent<PlayerInteraction>();
        playerMovement = GetComponent<PlayerMovement>();
        playerInspection = GetComponent<PlayerInspection>();
    }

    void Update()
    {
        switch (state)
        {
            case PlayerState.EXPLORATION:
            Cursor.lockState = CursorLockMode.Locked;
                playerInteraction.RaycastForInteractable();
                playerMovement.HandleMovement();
                break;
            case PlayerState.INSPECTION:
                Cursor.lockState = CursorLockMode.None;
                //InventoryManager.Instance.ListItems();
                //playerInspection.TryDragDrop();
                break;
            default:
                break;
        }
    }

    public void TransitionToInspection()
    {
        Cursor.lockState = CursorLockMode.Locked;
        state = PlayerState.INSPECTION;
    }

    public void TransitionToExploration()
    {
        Cursor.lockState = CursorLockMode.None;
        state = PlayerState.EXPLORATION;
    }

    public bool InStateInspection()
    {
        return state == PlayerState.INSPECTION;
    }

    public bool IsInStateExploration()
    {
        return state == PlayerState.EXPLORATION;
    }
}

