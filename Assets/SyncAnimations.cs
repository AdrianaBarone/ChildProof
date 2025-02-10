using UnityEngine;

//Funzione di sincronizzazione con il fuoco
//Funzione di sincroniccazione animazioni

public class AnimationAutoStart : MonoBehaviour
{
    public Animator firstStarter;
    public Animator secondStarter; 

    private bool hasStarted = false; 

    void Update()
    {
        if (firstStarter == null || secondStarter == null) return;

        AnimatorStateInfo firstState = firstStarter.GetCurrentAnimatorStateInfo(0);

        if (!hasStarted && firstState.normalizedTime >= 1)
        {
            hasStarted = true; 
            //secondStarter.SetTrigger("activeSecondAnimation");
        }
    }
}

