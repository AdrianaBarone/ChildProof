using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarachterAnimator : MonoBehaviour
{
    
    public Animator animator;
    private NavMeshAgent agent;
    private float turn = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
    }

    void Update()
    {
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
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    // Face the target
                    Vector3 direction = (agent.destination - transform.position).normalized;
                    if (direction != Vector3.zero)
                    {
                        Quaternion lookRotation = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed);
                    }
                }
            }
        }

        animator.SetFloat("turn", turn);
    }
}