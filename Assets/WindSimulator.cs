using UnityEngine;
using System.Collections;

public class WindSimulator : MonoBehaviour {
    public Vector3 windDirection = new Vector3(-1, 0, 0);
    public float baseWindStrength = 3f;
    public float windVariation = 3f;
    public float windSpeed = 1f;
    public float windAcceleration = 0.1f; // Aggiunto per accelerare la velocità del vento durante l'animazione
    public float windDecaySpeed = 0.05f; // Velocità di decadimento del vento quando l'animazione è finita
    public float minimumWindStrength = 1f; // Forza minima del vento

    public Animator chairAnimator; // Riferimento all'animator della sedia
    public GameObject clothObject; // Oggetto che contiene il componente Cloth (tenda)

    private Cloth cloth;
    private Coroutine windCoroutine;
    private float currentWindStrength;

    void Start() {
        if (chairAnimator != null) {
            cloth = clothObject.GetComponent<Cloth>();
            currentWindStrength = minimumWindStrength; // Imposta il vento inizialmente al valore minimo
        }
    }

    void Update() {
        if (chairAnimator != null) {
            AnimatorStateInfo stateInfo = chairAnimator.GetCurrentAnimatorStateInfo(0);

            // Se l'animazione sta ancora giocando, aumenta la forza del vento
            if (stateInfo.normalizedTime < 1f) {
                if (currentWindStrength < baseWindStrength + windVariation) {
                    currentWindStrength += windAcceleration * Time.deltaTime;
                }

                // Se l'animazione è attiva, inizia il vento
                StartWind();
            }
            else {
                // Quando l'animazione è finita, riduci il vento progressivamente
                ReduceWindAfterAnimation();
                StopWind();
            }
        }
    }

    // Metodo che avvia il vento
    public void StartWind() {
        if (windCoroutine == null) {
            windCoroutine = StartCoroutine(ApplyWind());
        }
    }

    // Coroutine che applica il vento all'oggetto Cloth
    private IEnumerator ApplyWind() {
        while (true) {
            if (cloth == null) yield break;

            // Calcola l'effetto del vento in base alla forza attuale
            float windEffect = currentWindStrength + Mathf.PerlinNoise(Time.time * windSpeed, 0) * windVariation;
            cloth.externalAcceleration = windDirection.normalized * windEffect;

            yield return new WaitForSeconds(0.1f);
        }
    }

    // Metodo che interrompe il vento
    public void StopWind() {
        if (windCoroutine != null) {
            StopCoroutine(windCoroutine);
            windCoroutine = null;
        }

        if (cloth != null) {
            cloth.externalAcceleration = Vector3.zero;
        }
    }

    // Metodo per ridurre progressivamente la forza del vento alla fine dell'animazione
    private void ReduceWindAfterAnimation() {
        if (currentWindStrength > minimumWindStrength) {
            currentWindStrength -= windDecaySpeed * Time.deltaTime;
        }
        else {
            currentWindStrength = minimumWindStrength; // Assicurati che non scenda mai sotto il valore minimo
        }
    }
}
