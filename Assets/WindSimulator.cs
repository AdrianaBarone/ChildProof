using UnityEngine;
using System.Collections;

public class WindSimulator : MonoBehaviour
{
    public Vector3 windDirection = new Vector3(-1, 0, 0);
    public float baseWindStrength = 3f;
    public float windVariation = 3f;
    public float windSpeed = 1f;
    public float windAcceleration = 0.1f; // Aggiunto per accelerare la velocità del vento durante l'animazione

    public Animator chairAnimator; // Riferimento all'animator della sedia
    public GameObject clothObject; // Oggetto che contiene il componente Cloth (tenda)
    
    private Cloth cloth;
    private Coroutine windCoroutine;
    private float currentWindStrength;

    void Start()
    {
        if (chairAnimator != null)
        {
            cloth = clothObject.GetComponent<Cloth>();
            currentWindStrength = baseWindStrength; 
        }
    }

    void Update()
    {
        if (chairAnimator != null)
        {
            AnimatorStateInfo stateInfo = chairAnimator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.normalizedTime < 1f)
            {
                if (currentWindStrength < baseWindStrength + windVariation)
                {
                    currentWindStrength += windAcceleration * Time.deltaTime;
                }

                StartWind();
            }
            else
            {
                StopWind();
            }
        }
    }

    public void StartWind()
    {
        if (windCoroutine == null)
        {
            windCoroutine = StartCoroutine(ApplyWind());
        }
    }

    private IEnumerator ApplyWind()
    {
        while (true)
        {
            if (cloth == null) yield break;

            float windEffect = currentWindStrength + Mathf.PerlinNoise(Time.time * windSpeed, 0) * windVariation;
            cloth.externalAcceleration = windDirection.normalized * windEffect;

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void StopWind()
    {
        if (windCoroutine != null)
        {
            StopCoroutine(windCoroutine);
            windCoroutine = null;
        }

        if (cloth != null)
        {
            cloth.externalAcceleration = Vector3.zero;
        }

        currentWindStrength = baseWindStrength; 
    }
}
