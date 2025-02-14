using UnityEngine;

public class ParticleOnAnimation : MonoBehaviour
{
    private Animator parentAnimator;  
    private ParticleSystem particles; 

    void Start()
    {
        parentAnimator = GetComponentInParent<Animator>();
        particles = GetComponent<ParticleSystem>();

        if (parentAnimator == null)
        {
            Debug.LogError("Nessun Animator trovato nel parent!");
        }

        if (particles == null)
        {
            Debug.LogError("Nessun ParticleSystem trovato in questo GameObject!");
        }
    }

    void Update()
    {
        if (parentAnimator != null && parentAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            if (!particles.isPlaying)
            {
                particles.Play();
            }
        }
        else
        {
            if (particles.isPlaying)
            {
                particles.Stop();
            }
        }
    }
}
