using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class PlayerInteraction : MonoBehaviour
{

    // Parametri per il controllo visibilità
    [SerializeField] private float distance = 2f;
    [SerializeField] private LayerMask allLayerMask;
    [SerializeField] private LayerMask dropZoneLayerMask;
    [SerializeField] private ChildInteracted childInteractedEvent;

    private bool interactingTrigger = false;



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


    public void RaycastForInspectable()
    {
        Ray ray = new(playerCamera.transform.position, playerCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance, Color.red); // Visualizza il raycast in scena

        if (Physics.Raycast(ray, out RaycastHit hitInfo, distance, allLayerMask))
        {
            IPickable pointingPickable = hitInfo.collider.GetComponent<IPickable>();
            Inspectable pointingInspectable = hitInfo.collider.GetComponent<Inspectable>();
            CarachterAnimator pointingChild = hitInfo.collider.GetComponent<CarachterAnimator>();

            if (pointingPickable != null)
            {
                CursorManager.Instance.UpdateExplorationCursor(grabbableSprite); // Cambio del cursore per oggetto afferrabile
                if (Input.GetMouseButtonDown(0))
                {
                    pointingPickable.OnPick();
                }
                return;
            }

            if (GameManager.Instance.InDangerMode)
            {
                if (pointingChild != null)
                {
                    CursorManager.Instance.UpdateExplorationCursor(interactSprite);
                    if (Input.GetMouseButtonDown(0))
                    {
                        childInteractedEvent.SendEventMessage();
                        GameManager.Instance.EndDangerMode();

                    }
                }
                else
                {
                    CursorManager.Instance.UpdateExplorationCursor(defaultSprite);
                }
                return; // NOTE: queste return impedisce di interagire con gli oggetti in DangerMode
            }

            if (pointingInspectable != null && !pointingInspectable.IsResolved())
            {
                CursorManager.Instance.UpdateExplorationCursor(interactSprite); // Cambio del cursore per interazione
                pointingInspectable.BaseInteract();

                if (Input.GetMouseButtonDown(0) && !pointingInspectable.IsResolved())
                {
                    StartCoroutine(StartInteraction(pointingInspectable));
                }
            }
            else
            {
                CursorManager.Instance.UpdateExplorationCursor(defaultSprite); // Cambio al cursore predefinito
            }
        }
        else
        {
            CursorManager.Instance.UpdateExplorationCursor(defaultSprite); // Se non c'è nulla, cursore predefinito
        }
    }

    public Moveable RaycastForMoveable()
    {
        Camera fixedCamera = PlayerManager.Instance.GetInspectableCamera();
        Ray ray = fixedCamera.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(ray.origin, ray.direction * distance * 10, Color.blue); // Visualizza il raycast in scena

        if (Physics.Raycast(ray, out RaycastHit hitInfo, distance * 10, allLayerMask))
        {
            Moveable moveable = hitInfo.collider.GetComponent<Moveable>();

            if (moveable != null && moveable == PlayerManager.Instance.currentInspectable)
            {
                if (!interactingTrigger)
                {
                    CursorManager.Instance.PointingMoveable();
                    interactingTrigger = true;
                }

                return moveable;
            }
            else
            {
                if (interactingTrigger && !InventoryManager.Instance.isItemSelected)
                {
                    CursorManager.Instance.PointingDefault();
                    interactingTrigger = false;
                }
            }
        }

        return null;
    }

    public void TryPickUp()
    {
        Moveable moveable = RaycastForMoveable();



        if (moveable == null)
        {
            return;
        }

        if (moveable != PlayerManager.Instance.currentInspectable)
        {
            return;
        }


        if (Input.GetMouseButtonDown(0))
        {
            InventoryManager.Instance.SelectItem(moveable.gameObject);
        }
    }

    // Coroutine per ritardare il select item di un frame
    IEnumerator DelayedSelectItem(Moveable moveable)
    {
        yield return new WaitForSeconds(0.1f);
        InventoryManager.Instance.SelectItem(moveable.gameObject);
    }

    public bool TryDragAndDrop(ItemData itemData)
    {
        DropZone dropZone = RaycastForDropZone();
        Debug.Log("dropzone");
        Debug.Log(dropZone);
        if (dropZone == null)
        {
            return false;
        }

        Debug.Log("dropzone parent");
        Debug.Log(dropZone.parentInspectable);
        Debug.Log("current inspectable");
        Debug.Log(PlayerManager.Instance.currentInspectable);

        if (dropZone.parentInspectable != PlayerManager.Instance.currentInspectable)
        {
            return false;
        }



        Debug.Log("dropzone accepts item");
        Debug.Log(dropZone.AcceptsItem(itemData));
        if (dropZone.AcceptsItem(itemData))
        {
            dropZone.OnDrop();
            if (TutorialManager.Instance && gameObject.tag == "Tutorial")
            {
                TutorialManager.Instance.OnReceiveEvent("OggettoPosizionato");
            }
            return true;
        }

        return false;
    }

    public DropZone RaycastForDropZone()
    {
        Camera fixedCamera = PlayerManager.Instance.GetInspectableCamera();
        Ray ray = fixedCamera.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(ray.origin, ray.direction * distance * 50, Color.blue); // Visualizza il raycast in scena

        if (Physics.Raycast(ray, out RaycastHit hitInfo, distance * 50, dropZoneLayerMask))
        {
            DropZone dropZone = hitInfo.collider.GetComponent<DropZone>();

            return dropZone;
        }

        return null;
    }

    private IEnumerator StartInteraction(Inspectable inspectable)
    {
        // Inizio della transizione, nascondi cursore
        PlayerManager.Instance.PrepareTransition(); // NOTE: Blocca le interazioni durante la transizione
        Camera fixedCamera = inspectable.GetCamera();

        AudioManager.Instance.PlayCameraTransitionSound();

        // Interpolazione per il movimento graduale della fotocamera
        float elapsedTime = 0f;
        startPosition = playerCamera.transform.position;
        startRotation = playerCamera.transform.rotation;

        while (elapsedTime < transitionTime)
        {
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

        AudioManager.Instance.StopCameraTransitionSound();

        PlayerManager.Instance.TransitionToInspection(inspectable);
        if (TutorialManager.Instance && gameObject.tag == "Tutorial")
        {
            TutorialManager.Instance.OnReceiveEvent("TavoloCliccato");
        }
    }

    private IEnumerator EndInteraction()
    {
        PlayerManager.Instance.PrepareTransition(); // NOTE: Blocca le interazioni durante la transizione
        Camera fixedCamera = PlayerManager.Instance.GetInspectableCamera();

        AudioManager.Instance.PlayCameraTransitionSound();

        // Interpolazione per il movimento graduale della fotocamera
        float elapsedTime = 0f;
        Vector3 targetPosition = startPosition;
        Quaternion targetRotation = startRotation;

        // Disabilita la fotocamera fissa
        fixedCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);

        while (elapsedTime < transitionTime)
        {
            playerCamera.transform.position = Vector3.Lerp(fixedCamera.transform.position, targetPosition, elapsedTime / transitionTime);
            playerCamera.transform.rotation = Quaternion.Slerp(fixedCamera.transform.rotation, targetRotation, elapsedTime / transitionTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }


        playerCamera.transform.SetPositionAndRotation(targetPosition, targetRotation);

        AudioManager.Instance.StopCameraTransitionSound();

        PlayerManager.Instance.TransitionToExploration();
    }

    public void EndInteractionExternal()
    {
        StartCoroutine(EndInteraction());
    }
}
