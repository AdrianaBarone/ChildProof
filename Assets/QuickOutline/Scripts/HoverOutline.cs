using UnityEngine;

[RequireComponent(typeof(Outline))]
public class HoverOutline : MonoBehaviour
{
    private Outline outline;

    [Tooltip("Distanza massima entro la quale l'outline sarà visibile")]
    public float maxDistance = 10f;

    // Flag per tenere traccia se il mouse è sopra l'oggetto
    private bool mouseOver = false;

    void Awake()
    {
        // Recupera il componente Outline e lo disabilita all'avvio
        outline = GetComponent<Outline>();
        if (outline != null)
            outline.enabled = false;
        outline.precomputeOutline = false;
    }

    void OnMouseEnter()
    {
        mouseOver = true;
        // Attiva l'outline solo se l'oggetto è entro la distanza desiderata
        if (IsWithinDistance())
        {
            outline.enabled = true;
        }
    }

    void OnMouseExit()
    {
        mouseOver = false;
        // Disabilita l'outline quando il mouse esce
        if (outline != null)
            outline.enabled = false;
    }

    void Update()
    {
        // Se il mouse è sopra l'oggetto, controlla continuamente la distanza dalla camera
        if (mouseOver && PlayerManager.Instance.IsInStateExploration())
        {
            if (IsWithinDistance())
            {
                if (!outline.enabled)
                    outline.enabled = true;
            }
            else
            {
                if (outline.enabled)
                    outline.enabled = false;
            }
        }
    }

    bool IsWithinDistance()
    {
        if (Camera.main == null)
            return false;

        float distance = Vector3.Distance(Camera.main.transform.position, transform.position);
        return distance <= maxDistance;
    }
}