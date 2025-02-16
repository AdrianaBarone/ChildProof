using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    [System.Serializable]
    public class AnimationObject
    {
        public GameObject[] targetObjects; // Array di oggetti da distruggere
        public Animator animator;
        public string animationStateName;
        [HideInInspector] public bool isDestroyed = false;
    }

    public AnimationObject[] animationObjects;

    void Update()
    {
        foreach (var animObj in animationObjects)
        {
            if (animObj.animator != null && !animObj.isDestroyed)
            {
                AnimatorStateInfo stateInfo = animObj.animator.GetCurrentAnimatorStateInfo(0);

                if (stateInfo.IsName(animObj.animationStateName))
                {
                    foreach (var obj in animObj.targetObjects)
                    {
                        if (obj != null)
                            Destroy(obj);
                    }
                    animObj.isDestroyed = true;
                }
            }
        }
    }
}
