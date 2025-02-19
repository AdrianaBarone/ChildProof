using UnityEngine;

[RequireComponent(typeof(Outline))]
public class HoverOutline : MonoBehaviour
{
    private Outline outline;

    [Tooltip("Distanza massima entro la quale l'outline sarà visibile")]
    public float maxDistance = 10f;

    [Tooltip("Seleziona l'oggetto (o il gruppo) da escludere dall'outline. Se l'oggetto corrente o un suo discendente appartiene a questo oggetto, l'outline non verrà attivato.")]
    public GameObject objectToExclude;

    // Flag per tenere traccia se il mouse è sopra l'oggetto
    private bool mouseOver = false;

    void Awake()
    {
        // Recupera il componente Outline e lo disabilita all'avvio
        outline = GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = false;
            outline.precomputeOutline = false;
        }
    }

    // Restituisce true se l'oggetto corrente (o uno dei suoi antenati) è quello da escludere
    bool IsExcluded()
    {
        if (objectToExclude == null)
            return false;

        // Se il GameObject corrente è l'oggetto escluso
        if (gameObject == objectToExclude)
            return true;

        // Se il GameObject corrente è un discendente dell'oggetto escluso
        if (transform.IsChildOf(objectToExclude.transform))
            return true;

        return false;
    }

    void OnMouseEnter()
    {
        if (IsExcluded())
            return; // Se appartiene all'oggetto da escludere, non attiviamo l'outline

        mouseOver = true;
        // Attiva l'outline solo se l'oggetto è entro la distanza desiderata
        if (IsWithinDistance())
            outline.enabled = true;
    }

    void OnMouseExit()
    {
        if (IsExcluded())
            return;

        mouseOver = false;
        // Disabilita l'outline quando il mouse esce
        if (outline != null)
            outline.enabled = false;
    }

    void Update()
    {
        if (IsExcluded())
            return;

        // Se il mouse è sopra l'oggetto e il giocatore è in stato di esplorazione, controlla continuamente la distanza dalla camera
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
