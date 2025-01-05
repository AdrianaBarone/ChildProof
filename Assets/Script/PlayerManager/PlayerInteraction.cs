using System.Collections;
using UnityEngine;



public class PlayerInteraction : MonoBehaviour {

    // Parametri per il controllo visibilità
    [SerializeField] private float distance = 2f;
    [SerializeField] private LayerMask layerMask;

    // Parametri cursore
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D interactCursor;
    [SerializeField] private Texture2D grabbableCursor;

    //Oggetti con cui interagire
    private Interactable pointingInteractable;
    private IPickable pointingPickable;

    // Riferimento alla fotocamera mobile e fissa
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera fixedCamera;
    public CameraPosition cameraPosition;

    // Tempo di transizione tra le fotocamere
    [SerializeField] private float transitionTime = 1f;
    private Vector3 startPosition;
    private Quaternion startRotation;

    void Start() {
        if (fixedCamera != null)
            fixedCamera.gameObject.SetActive(false);
    }

    public void RaycastForInteractable() {
        Ray ray = new(playerCamera.transform.position, playerCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance, Color.red); // Visualizza il raycast in scena

        if (Physics.Raycast(ray, out RaycastHit hitInfo, distance, layerMask)) {
            pointingInteractable = hitInfo.collider.GetComponent<Interactable>();
            pointingPickable = hitInfo.collider.GetComponent<IPickable>();
            Inspectable pointingInspectable = hitInfo.collider.GetComponent<Inspectable>();

            // TODO: capire bene come gestire queste interazioni

            if (pointingInteractable != null) {
                UpdateCursor(interactCursor); // Cambio del cursore per interazione
                pointingInteractable.BaseInteract();

                if (Input.GetMouseButtonDown(0) && pointingInspectable && !pointingInspectable.IsResolved()) {
                    StartCoroutine(StartInteraction(pointingInteractable));
                }
            }
            else {
                UpdateCursor(defaultCursor); // Cambio al cursore predefinito
            }
            
            if (pointingPickable != null) {
                UpdateCursor(grabbableCursor); // Cambio del cursore per oggetto afferrabile
                if (Input.GetMouseButtonDown(0)) {
                    pointingPickable.OnPick();
                }
            }
        }
        else {
            UpdateCursor(defaultCursor); // Se non c'è nulla, cursore predefinito
        }
    }


    public bool TryDragAndDrop(Item item) {
        DropZone dropZone = RaycastForDropZone();
        if (dropZone == null) {
            return false;
        }


        if (dropZone.AcceptsItem(item)) {
            dropZone.OnDrop();
            return true;
        }

        return false;
    }

    public DropZone RaycastForDropZone() {
        // ray is from mouse position to the world
        Ray ray = fixedCamera.ScreenPointToRay(Input.mousePosition);
        
        Debug.DrawRay(ray.origin, ray.direction * distance * 10, Color.blue); // Visualizza il raycast in scena

        if (Physics.Raycast(ray, out RaycastHit hitInfo, distance * 10, layerMask)) {
            DropZone dropZone = hitInfo.collider.GetComponent<DropZone>();

            if (dropZone != null) {
                // TODO: ritorna true? cambia colore?
            }

            return dropZone;
        }

        return null;
    }

    // Funzione per cambiare il cursore
    private void UpdateCursor(Texture2D newCursor) {
        if (newCursor != null) {
            Vector2 cursorHotspot = new Vector2(newCursor.width / 2, newCursor.height / 2);
            Cursor.SetCursor(newCursor, cursorHotspot, CursorMode.Auto);
        }
    }

    private IEnumerator StartInteraction(Interactable interactable) {
        // Inizio della transizione, nascondi cursore
        PlayerManager.Instance.PrepareTransition(); // NOTE: Blocca le interazioni durante la transizione

        (Vector3 targetPosition, Quaternion targetRotation) = interactable.GetTargetPositionAndRotation();


        // Interpolazione per il movimento graduale della fotocamera
        float elapsedTime = 0f;
        startPosition = playerCamera.transform.position;
        startRotation = playerCamera.transform.rotation;

        while (elapsedTime < transitionTime) {
            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / transitionTime);
            Quaternion newRotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / transitionTime);
            playerCamera.transform.SetPositionAndRotation(newPosition, newRotation);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Assicurarsi che la posizione finale e la rotazione siano precise
        fixedCamera.transform.SetPositionAndRotation(targetPosition, targetRotation);


        // Attiva la fotocamera fissa
        fixedCamera.gameObject.SetActive(true);
        playerCamera.gameObject.SetActive(false);


        PlayerManager.Instance.TransitionToInspection();
    }

    private IEnumerator EndInteraction() {
        PlayerManager.Instance.PrepareTransition(); // NOTE: Blocca le interazioni durante la transizione
        // Interpolazione per il movimento graduale della fotocamera
        float elapsedTime = 0f;
        Vector3 targetPosition = startPosition;
        Quaternion targetRotation = startRotation;

        while (elapsedTime < transitionTime) {
            fixedCamera.transform.position = Vector3.Lerp(fixedCamera.transform.position, targetPosition, elapsedTime / transitionTime);
            fixedCamera.transform.rotation = Quaternion.Slerp(fixedCamera.transform.rotation, targetRotation, elapsedTime / transitionTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }


        playerCamera.transform.SetPositionAndRotation(targetPosition, targetRotation);

        // Disabilita la fotocamera fissa
        fixedCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);


        PlayerManager.Instance.TransitionToExploration();
    }

    public void EndInteractionExternal() {
        StartCoroutine(EndInteraction());
    }
}
