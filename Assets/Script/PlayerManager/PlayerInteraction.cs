using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public enum PlayerState
{
    STATO_A,
    STATO_B,
}

public class PlayerInteraction : MonoBehaviour
{
    private PlayerState state = PlayerState.STATO_A;

    // Parametri per il controllo visibilità
    [SerializeField] private float distance = 2f;
    [SerializeField] private LayerMask layerMask;

    // Parametri cursore
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D interactCursor;
    [SerializeField] private Texture2D grabbableCursor;

    //Oggetti con cui interagire
    private Interactable pointingInteractable;
    private Grabbable pointingGrabbable;
    private Pickable pointingPickable;

    // Riferimento alla fotocamera mobile e fissa
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera fixedCamera;

    // Movimento del giocatore
    [SerializeField] private PlayerMovement playerMovement;

    // Tempo di transizione tra le fotocamere
    [SerializeField] private float transitionTime = 1f;
    private bool isTransitioning = false;
    private Vector3 startPosition;
    private Quaternion startRotation;

    void Start()
    {
        if (fixedCamera != null)
            fixedCamera.gameObject.SetActive(false);
    }

    void TransitionState(PlayerState toState)
    {

        if (state == PlayerState.STATO_A)
        {


        }
        else if (state == PlayerState.STATO_B)
        {


        }

        state = toState;
    }

    void Update()
    {


        if (state == PlayerState.STATO_A)
        {
            Cursor.visible = true;

            // Raycast per l'interazione
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hitInfo;
            Debug.DrawRay(ray.origin, ray.direction * distance, Color.red); // Visualizza il raycast in scena

            if (Physics.Raycast(ray, out hitInfo, distance, layerMask))
            {
                pointingInteractable = hitInfo.collider.GetComponent<Interactable>();
                pointingGrabbable = hitInfo.collider.GetComponent<Grabbable>();
                pointingPickable = hitInfo.collider.GetComponent<Pickable>();


                if (pointingInteractable)
                {
                    UpdateCursor(interactCursor); // Cambio del cursore per interazione
                    pointingInteractable.BaseInteract();

                    if (Input.GetMouseButtonDown(0))
                    {
                        StartCoroutine(StartInteraction(hitInfo.collider.transform));
                    }
                }
                else if (pointingGrabbable)
                {
                    UpdateCursor(grabbableCursor); // Cambio del cursore per oggetto afferrabile
                    if (Input.GetMouseButtonDown(0))
                    {
                        StartCoroutine(StartInteraction(hitInfo.collider.transform));
                    }
                }
                else if (pointingPickable)
                {
                    UpdateCursor(grabbableCursor); // Cambio del cursore per oggetto afferrabile
                    if (Input.GetMouseButtonDown(0))
                    {
                        pointingPickable.BasePick();
                    }
                }
                else
                {
                    UpdateCursor(defaultCursor); // Cambio al cursore predefinito
                }
            }
            else
            {
                UpdateCursor(defaultCursor); // Se non c'è nulla, cursore predefinito
            }

        }
        else if (state == PlayerState.STATO_B)
        {
            // camera fissa, drag/grab and drop per singolo oggetto

        }


        if (playerCamera.gameObject.activeSelf && !isTransitioning)
        {

        }
        else if (isTransitioning)
        {
            Cursor.visible = false;  // Nascondi il cursore durante la transizione
        }
        else Cursor.visible = true;
    }


    // Funzione per cambiare il cursore
    private void UpdateCursor(Texture2D newCursor)
    {
        if (newCursor != null)
        {
            Cursor.SetCursor(newCursor, Vector2.zero, CursorMode.Auto);
        }
    }

    private IEnumerator StartInteraction(Transform interactableTransform)
    {
        // Inizio della transizione
        isTransitioning = true;

        // Calcolare la posizione frontale all'oggetto (senza inclinazione)
        // La fotocamera "fixed" sarà posizionata a una certa distanza davanti all'oggetto, lungo il suo "forward"
        Vector3 targetPosition = interactableTransform.position + interactableTransform.forward * 5f; // posizioniamo la fotocamera "fixed" davanti all'oggetto
        Quaternion targetRotation = Quaternion.LookRotation(interactableTransform.position - targetPosition);  // orientamento verso l'oggetto

        // Interpolazione per il movimento graduale della fotocamera
        float elapsedTime = 0f;
        startPosition = playerCamera.transform.position;
        startRotation = playerCamera.transform.rotation;

        while (elapsedTime < transitionTime)
        {
            playerCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / transitionTime);
            playerCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / transitionTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Assicurarsi che la posizione finale e la rotazione siano precise
        fixedCamera.transform.position = targetPosition;
        fixedCamera.transform.rotation = targetRotation;

        // Attiva la fotocamera fissa
        fixedCamera.gameObject.SetActive(true);
        playerCamera.gameObject.SetActive(false);
        fixedIsActive();

        // Disabilita il movimento del giocatore
        playerMovement.EnablePlayerMovement(false);

        // Esegui l'interazione
        //--- interazione con inventario ---//

        isTransitioning = false;
    }

    private IEnumerator EndInteraction(Vector3 startPosition, Quaternion startRotation)
    {
        // Inizio della transizione inversa
        isTransitioning = true;

        // Interpolazione per il movimento graduale della fotocamera
        float elapsedTime = 0f;
        Vector3 targetPosition = startPosition;
        Quaternion targetRotation = startRotation;

        while (elapsedTime < transitionTime)
        {
            fixedCamera.transform.position = Vector3.Lerp(fixedCamera.transform.position, targetPosition, elapsedTime / transitionTime);
            fixedCamera.transform.rotation = Quaternion.Slerp(fixedCamera.transform.rotation, targetRotation, elapsedTime / transitionTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        playerCamera.transform.position = targetPosition;
        playerCamera.transform.rotation = targetRotation;

        // Disabilita la fotocamera fissa
        fixedCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);
        fixedIsActive();

        // Riabilita il movimento del giocatore
        playerMovement.EnablePlayerMovement(true);

        isTransitioning = false;
    }

    public bool fixedIsActive()
    {
        return fixedCamera.gameObject.activeSelf;
    }
}
