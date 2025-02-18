using UnityEngine;

[RequireComponent(typeof(Outline))]
public class HoverOutline : MonoBehaviour
{
    private Outline outline;

    void Awake()
    {
        outline = GetComponent<Outline>();
        if(outline != null){
            outline.enabled = false;
            outline.precomputeOutline = false;
        } 
    }

    void OnMouseEnter()
    {
        // Attiva l'outline quando il mouse entra nell'area dell'oggetto
        if (outline != null && PlayerManager.Instance.IsInStateExploration())
            outline.enabled = true;
            Debug.Log("Over");
    }

    void OnMouseExit()
    {
        // Disattiva l'outline quando il mouse esce dall'area dell'oggetto
        if (outline != null)
            outline.enabled = false;
    }
}
