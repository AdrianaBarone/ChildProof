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

    void Start()
    {
        Instance = this;

        playerInteraction = GetComponent<PlayerInteraction>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        switch (state)
        {
            case PlayerState.EXPLORATION:
            Debug.Log("EXPLORATION");
                playerInteraction.RaycastForInteractable();
                playerMovement.HandleMovement();
                break;
            case PlayerState.INSPECTION:
                // inventoryManager.enlargeUI (nella transizione)
                Debug.Log("INSPECTION");
                break;
            default:
                break;
        }
    }

    public void TransitionToInspection()
    {
        state = PlayerState.INSPECTION;
    }

    public void TransitionToExploration()
    {
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

