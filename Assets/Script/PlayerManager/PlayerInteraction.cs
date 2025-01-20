using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class PlayerInteraction : MonoBehaviour {

    // Parametri per il controllo visibilità
    [SerializeField] private float distance = 2f;
    [SerializeField] private LayerMask layerMask;

    //Oggetti con cui interagire
    private Interactable pointingInteractable;
    private IPickable pointingPickable;

    // Riferimento alla fotocamera mobile e fissa
    [SerializeField] private Camera playerCamera;
    public CameraPosition cameraPosition;

    // Tempo di transizione tra le fotocamere
    [SerializeField] private float transitionTime = 1f;
    private Vector3 startPosition;
    private Quaternion startRotation;

    //Parametri Cursore
    public Sprite defaultSprite;
    [SerializeField] private Sprite interactSprite;
    [SerializeField] private Sprite grabbableSprite;

    void Start() {
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
                CursorManager.Instance.UpdateExplorationCursor(interactSprite); // Cambio del cursore per interazione
                pointingInteractable.BaseInteract();

                if (Input.GetMouseButtonDown(0) && pointingInspectable && !pointingInspectable.IsResolved()) {
                    StartCoroutine(StartInteraction(pointingInteractable));
                }
            }
            else {
                CursorManager.Instance.UpdateExplorationCursor(defaultSprite); // Cambio al cursore predefinito
            }

            if (pointingPickable != null) {
                CursorManager.Instance.UpdateExplorationCursor(grabbableSprite); // Cambio del cursore per oggetto afferrabile
                if (Input.GetMouseButtonDown(0)) {
                    pointingPickable.OnPick();
                }
            }
        }
        else {
            CursorManager.Instance.UpdateExplorationCursor(defaultSprite); // Se non c'è nulla, cursore predefinito
        }
    }

    public Moveable RaycastForMoveable() {
        Camera fixedCamera = PlayerManager.Instance.GetInteractableCamera();
        Ray ray = fixedCamera.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(ray.origin, ray.direction * distance * 10, Color.blue); // Visualizza il raycast in scena

        if (Physics.Raycast(ray, out RaycastHit hitInfo, distance * 10, layerMask)) {
            Moveable moveable = hitInfo.collider.GetComponent<Moveable>();

            if (moveable != null) {
                CursorManager.Instance.PointingMoveable();
                return moveable;
            } else {
                CursorManager.Instance.PointingDefault();
            }
        }

        return null;
    }

    public void TryPickUp() {
        Moveable moveable = RaycastForMoveable();
        if (moveable == null) {
            return;
        }


        if (Input.GetMouseButtonDown(0)) {
            StartCoroutine(DelayedSelectItem(moveable));
        }

        // ItemData picked = moveable.PickupItem(); -> also disable the object
        // set selected item to picked
        // 
    }

    // ienumerator per ritardare il select item di un frame
    IEnumerator DelayedSelectItem(Moveable moveable) {
        yield return null;
        InventoryManager.Instance.SelectItem(moveable.gameObject);
    }

    public bool TryDragAndDrop(ItemData itemData) {
        DropZone dropZone = RaycastForDropZone();

        if (dropZone == null) {
            return false;
        }


        if (dropZone.AcceptsItem(itemData)) {
            dropZone.OnDrop();
            return true;
        }

        return false;
    }

    public DropZone RaycastForDropZone() {
        Camera fixedCamera = PlayerManager.Instance.GetInteractableCamera();
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

    private IEnumerator StartInteraction(Interactable interactable) {
        // Inizio della transizione, nascondi cursore
        PlayerManager.Instance.PrepareTransition(); // NOTE: Blocca le interazioni durante la transizione
        Camera fixedCamera = interactable.GetCamera();



        // Interpolazione per il movimento graduale della fotocamera
        float elapsedTime = 0f;
        startPosition = playerCamera.transform.position;
        startRotation = playerCamera.transform.rotation;


        while (elapsedTime < transitionTime) {
            Vector3 newPosition = Vector3.Lerp(startPosition, fixedCamera.transform.position, elapsedTime / transitionTime);
            Quaternion newRotation = Quaternion.Slerp(startRotation, fixedCamera.transform.rotation, elapsedTime / transitionTime);
            playerCamera.transform.SetPositionAndRotation(newPosition, newRotation);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Assicurarsi che la posizione finale e la rotazione siano precise
        playerCamera.transform.SetPositionAndRotation(fixedCamera.transform.position, fixedCamera.transform.rotation);


        // Attiva la fotocamera fissa
        fixedCamera.gameObject.SetActive(true);
        playerCamera.gameObject.SetActive(false);


        PlayerManager.Instance.TransitionToInspection(interactable);
    }

    private IEnumerator EndInteraction() {
        PlayerManager.Instance.PrepareTransition(); // NOTE: Blocca le interazioni durante la transizione
        Camera fixedCamera = PlayerManager.Instance.GetInteractableCamera();

        // Interpolazione per il movimento graduale della fotocamera
        float elapsedTime = 0f;
        Vector3 targetPosition = startPosition;
        Quaternion targetRotation = startRotation;

        // Disabilita la fotocamera fissa
        fixedCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);

        while (elapsedTime < transitionTime) {
            playerCamera.transform.position = Vector3.Lerp(fixedCamera.transform.position, targetPosition, elapsedTime / transitionTime);
            playerCamera.transform.rotation = Quaternion.Slerp(fixedCamera.transform.rotation, targetRotation, elapsedTime / transitionTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }


        playerCamera.transform.SetPositionAndRotation(targetPosition, targetRotation);

        PlayerManager.Instance.TransitionToExploration();
    }

    public void EndInteractionExternal() {
        StartCoroutine(EndInteraction());
    }
}
