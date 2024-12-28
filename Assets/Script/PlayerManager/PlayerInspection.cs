using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInspection : MonoBehaviour
{
    public Camera fixedCamera;

    private Grabbable _pointingGrabbable;
    private Grabbable _grabbedObject = null;
    private Vector3 _grabOffset;

    private bool isFollowing = false;

    private void Update()
    {
        if (isFollowing)
        FollowMouse();
    }
    public void TryDragDrop()
    {
        if (_grabbedObject == null)
            CheckInteraction();

        if (_grabbedObject != null && Input.GetMouseButtonDown(0))
            Drop();
    }

    private void CheckInteraction()
    {
        Ray ray = fixedCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Verifica se l'oggetto a cui stiamo puntando è "grabbable"
            _pointingGrabbable = hit.transform.GetComponent<Grabbable>();
            
            if (_pointingGrabbable != null && _grabbedObject == null)
            {
                Debug.Log("Grabbable object detected: " + _pointingGrabbable.name);
                
                // Tasto destro per afferrare l'oggetto
                if (Input.GetMouseButtonDown(1))  
                {
                    _pointingGrabbable.Grab(gameObject);
                    Grab(_pointingGrabbable, hit.point); // Passa il punto di impatto come offset
                }
            }
            else
            {
                Debug.Log("No grabbable object found at mouse position.");
            }
        }
        // Se non viene rilevato nessun oggetto
        else
        {
            _pointingGrabbable = null;
            Debug.Log("No object hit by raycast.");
        }
    }

    private void Drop()
    {
        if (_grabbedObject == null)
            return;

        _grabbedObject.transform.parent = _grabbedObject.OriginalParent;
        _grabbedObject.Drop();

        //_target.enabled = true;
        isFollowing = false;
        _grabbedObject = null;
    }

    private void Grab(Grabbable grabbable, Vector3 hitPoint)
    {
        if (grabbable == null)
        {
            Debug.LogError("Grabbable object is null!");
            return;
        }

        _grabbedObject = grabbable;
        
        // Salva l'offset tra la posizione dell'oggetto e il punto dove è stato afferrato
        _grabOffset = _grabbedObject.transform.position - hitPoint;

        //_target.enabled = false;
        isFollowing = true;
    }

    private void FollowMouse()
    {
        // Lancia un raggio dalla posizione del mouse
        Ray ray = fixedCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Calcola la nuova posizione dell'oggetto in base al punto di impatto e all'offset
            Vector3 newPos = hit.point + _grabOffset;

            // Ignora la componente Z, mantenendo quella originale dell'oggetto
            newPos.z = _grabbedObject.transform.position.z;

            _grabbedObject.transform.position = newPos;
        }
    }
}
