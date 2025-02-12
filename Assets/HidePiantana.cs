using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    public GameObject piantana;
    public Animator dropzoneAnimator;

    private bool isPiantanaHidden = false;

    void Update()
    {
        if (dropzoneAnimator != null && !isPiantanaHidden)
        {
            AnimatorStateInfo stateInfo = dropzoneAnimator.GetCurrentAnimatorStateInfo(0);

            // Controlla se l'animazione è attiva
            if (stateInfo.IsName("piantana_sistemata"))
            {
                piantana.SetActive(false);
                isPiantanaHidden = true;
            }
        }
    }
}
