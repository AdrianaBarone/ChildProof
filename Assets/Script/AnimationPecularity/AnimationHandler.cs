using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    [System.Serializable]
    public class AnimationObject
    {
        public GameObject targetObject;
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
                    Destroy(animObj.targetObject);
                    animObj.isDestroyed = true;
                }
            }
        }
    }
}