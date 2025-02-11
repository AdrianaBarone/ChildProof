using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class PlayerInteraction : MonoBehaviour {

    // Parametri per il controllo visibilità
    [SerializeField] private float distance = 2f;
    [SerializeField] private LayerMask allLayerMask;
    [SerializeField] private LayerMask dropZoneLayerMask;
    [SerializeField] private ChildInteracted childInteractedEvent;



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


    [Header("Audio cambio camera")]
    //AUDIO
    private AudioSource audioSource;
    public AudioClip audioClip;

    void Start() {
        //AUDIO
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (audioClip != null) {
            audioSource.clip = audioClip;
        }
        else {
            Debug.LogWarning("Nessun audio clip assegnato a " + gameObject.name);
        }
    }

    public void RaycastForInspectable() {
        Ray ray = new(playerCamera.transform.position, playerCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance, Color.red); // Visualizza il raycast in scena

        if (Physics.Raycast(ray, out RaycastHit hitInfo, distance, allLayerMask)) {
            IPickable pointingPickable = hitInfo.collider.GetComponent<IPickable>();
            Inspectable pointingInspectable = hitInfo.collider.GetComponent<Inspectable>();
            CarachterAnimator pointingChild = hitInfo.collider.GetComponent<CarachterAnimator>();

            if (GameManager.Instance.InDangerMode) {
                if (pointingChild != null) {
                    CursorManager.Instance.UpdateExplorationCursor(interactSprite);
                    if (Input.GetMouseButtonDown(0)) {
                        childInteractedEvent.SendEventMessage();
                        GameManager.Instance.EndDangerMode();

                    }
                }
                else {
                    CursorManager.Instance.UpdateExplorationCursor(defaultSprite);
                }
                return; // NOTE: queste return impedisce di interagire con gli oggetti in DangerMode
            }

            if (pointingInspectable != null && !pointingInspectable.IsResolved()) {
                CursorManager.Instance.UpdateExplorationCursor(interactSprite); // Cambio del cursore per interazione
                pointingInspectable.BaseInteract();

                if (Input.GetMouseButtonDown(0) && !pointingInspectable.IsResolved()) {
                    StartCoroutine(StartInteraction(pointingInspectable));
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
        Camera fixedCamera = PlayerManager.Instance.GetInspectableCamera();
        Ray ray = fixedCamera.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(ray.origin, ray.direction * distance * 10, Color.blue); // Visualizza il raycast in scena

        if (Physics.Raycast(ray, out RaycastHit hitInfo, distance * 10, allLayerMask)) {
            Moveable moveable = hitInfo.collider.GetComponent<Moveable>();

            if (moveable != null && moveable == PlayerManager.Instance.currentInspectable) {
                CursorManager.Instance.PointingMoveable();
                return moveable;
            }
            else {
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

        if (moveable != PlayerManager.Instance.currentInspectable) {
            return;
        }


        if (Input.GetMouseButtonDown(0)) {
            StartCoroutine(DelayedSelectItem(moveable));
        }
    }

    // Coroutine per ritardare il select item di un frame
    IEnumerator DelayedSelectItem(Moveable moveable) {
        yield return null;
        InventoryManager.Instance.SelectItem(moveable.gameObject);
    }

    public bool TryDragAndDrop(ItemData itemData) {
        DropZone dropZone = RaycastForDropZone();

        if (dropZone == null) {
            return false;
        }

        if (dropZone.parentInspectable != PlayerManager.Instance.currentInspectable) {
            return false;
        }


        if (dropZone.AcceptsItem(itemData)) {
            dropZone.OnDrop();
            return true;
        }

        return false;
    }

    public DropZone RaycastForDropZone() {
        Camera fixedCamera = PlayerManager.Instance.GetInspectableCamera();
        Ray ray = fixedCamera.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(ray.origin, ray.direction * distance * 50, Color.blue); // Visualizza il raycast in scena

        if (Physics.Raycast(ray, out RaycastHit hitInfo, distance * 50, dropZoneLayerMask)) {
            DropZone dropZone = hitInfo.collider.GetComponent<DropZone>();

            return dropZone;
        }

        return null;
    }

    private IEnumerator StartInteraction(Inspectable inspectable) {
        // Inizio della transizione, nascondi cursore
        PlayerManager.Instance.PrepareTransition(); // NOTE: Blocca le interazioni durante la transizione
        Camera fixedCamera = inspectable.GetCamera();

        //AUDIO
        audioSource.Play();

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

        //AUDIO
        audioSource.Stop();

        PlayerManager.Instance.TransitionToInspection(inspectable);
    }

    private IEnumerator EndInteraction() {
        PlayerManager.Instance.PrepareTransition(); // NOTE: Blocca le interazioni durante la transizione
        Camera fixedCamera = PlayerManager.Instance.GetInspectableCamera();

        //AUDIO
        audioSource.Play();

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

        //AUDIO
        audioSource.Stop();

        PlayerManager.Instance.TransitionToExploration();
    }

    public void EndInteractionExternal() {
        StartCoroutine(EndInteraction());
    }
}
