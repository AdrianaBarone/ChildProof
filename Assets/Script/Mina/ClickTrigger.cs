using UnityEngine;

public class ClickTriggerWithAnimatorCondition : MonoBehaviour
{
    public Animator animator; // Riferimento all'Animator del personaggio
    public float cooldownTime = 5f; // Tempo di cooldown in secondi
    private bool canBeClicked = true; // Indica se il personaggio è cliccabile
    private float cooldownTimer = 0f; // Timer per il cooldown

    void Update()
    {
        // Gestione del cooldown
        if (!canBeClicked)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= cooldownTime)
            {
                canBeClicked = true; // Resetta la possibilità di cliccare
                cooldownTimer = 0f;
            }
        }

        // Controlla se il tasto sinistro del mouse è stato premuto
        if (Input.GetMouseButtonDown(0) && canBeClicked && IsTargetReached())
        {
            // Lancia un raggio dalla posizione del mouse
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Verifica se il raggio colpisce questo oggetto
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform) // Se il raggio colpisce questo GameObject
                {
                    Debug.Log("Oggetto cliccato: " + gameObject.name);

                    // Imposta il trigger "clicked" nell'Animator
                    if (animator != null)
                    {
                        animator.SetTrigger("Clicked");
                    }

                    // Avvia il cooldown
                    canBeClicked = false;
                }
            }
        }
    }

    // Metodo per controllare se TargetReached è true nell'Animator
    private bool IsTargetReached()
    {
        if (animator != null)
        {
            // Verifica il valore del parametro TargetReached
            return animator.GetBool("TargetReached");
        }
        return false;
    }
}
