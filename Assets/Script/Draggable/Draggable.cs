using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Draggable : MonoBehaviour, IDragHandler
{
    private Transform parentToReturnTo = null;

    // Questo viene chiamato all'inizio del drag
    public void OnBeginDrag(PointerEventData eventData)
    {
        parentToReturnTo = this.transform.parent;
        this.transform.SetParent(this.transform.parent.parent); // Modifica il parent temporaneamente
    }

    // Questo viene chiamato durante il drag
    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position; // Imposta la posizione dell'oggetto
    }

    // Questo viene chiamato al termine del drag
    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.SetParent(parentToReturnTo); // Ripristina il parent originale
    }
}
