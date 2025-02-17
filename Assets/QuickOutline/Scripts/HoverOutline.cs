using UnityEngine;

[RequireComponent(typeof(Outline))]
public class HoverOutline : MonoBehaviour
{
    private Outline outline;

    void Awake()
    {
        // Recupera il componente Outline e lo disattiva inizialmente
        outline = GetComponent<Outline>();
        if(outline != null)
            outline.enabled = false;
    }

    void OnMouseEnter()
    {
        // Attiva l'outline quando il mouse entra nell'area dell'oggetto
        if (outline != null)
            outline.enabled = true;
    }

    void OnMouseExit()
    {
        // Disattiva l'outline quando il mouse esce dall'area dell'oggetto
        if (outline != null)
            outline.enabled = false;
    }
}
