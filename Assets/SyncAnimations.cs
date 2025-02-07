using UnityEngine;

//Funzione di sincronizzazione con il fuoco
public class AnimationAutoStart : MonoBehaviour
{
    public Animator firstStarter;
    public Animator secondStarter; 

    private bool isPlaying = false; 

    void Update()
    {
        if (firstStarter == null || secondStarter == null) return;

        AnimatorStateInfo firstState = firstStarter.GetCurrentAnimatorStateInfo(0);

        if (!isPlaying && firstState.normalizedTime > 0)
        {
            isPlaying = true; 
            secondStarter.SetTrigger("fireActive");
        }

        if (firstState.normalizedTime >= 1)
        {
            isPlaying = false;
        }
    }
}
