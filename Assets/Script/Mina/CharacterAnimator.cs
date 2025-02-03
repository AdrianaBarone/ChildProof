using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarachterAnimator : MonoBehaviour
{
    
    private Animator animator;
    private NavMeshAgent agent;
    private float turn = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
    }

    void Update()
    {
        //if (targetTransforms.Count == 0) return;

        

        if (agent.velocity.sqrMagnitude > Mathf.Epsilon)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            float angle = Quaternion.Angle(transform.rotation, targetRotation);
            if (angle > 0.1f)
            {
                Vector3 cross = Vector3.Cross(transform.forward, agent.velocity.normalized);
                turn = Mathf.Clamp(cross.y, -1f, 1f);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * agent.angularSpeed);
            }
            else
            {
                turn = 0f;
            }
        }
        else
        {
            turn = 0f;
        }

        animator.SetFloat("turn", turn);
    }
}