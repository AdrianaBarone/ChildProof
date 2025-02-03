using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Random Target from List", story: "[Agent] navigates to target in [Targets] tagged [Tag]", category: "Action/Navigation", id: "d2cc4a9478937690277e62c238db96f4")]
public partial class RandomTargetFromListAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<List<GameObject>> Targets;
    [SerializeReference] public BlackboardVariable<string> Tag;
    [SerializeReference] public BlackboardVariable<float> Speed = new BlackboardVariable<float>(1.0f);
    [SerializeReference] public BlackboardVariable<float> DistanceThreshold = new BlackboardVariable<float>(0.2f);
    [SerializeReference] public BlackboardVariable<string> AnimatorSpeedParam = new BlackboardVariable<string>("SpeedParam");

    // This will only be used in movement without a navigation agent.
    [SerializeReference] public BlackboardVariable<float> SlowDownDistance = new BlackboardVariable<float>(1.0f);

    private NavMeshAgent m_NavMeshAgent;
    private Animator m_Animator;
    private float m_PreviousStoppingDistance;
    private Vector3 m_LastTargetPosition;
    private Vector3 m_ColliderAdjustedTargetPosition;

    protected override Status OnStart()
    {
        if (Agent.Value == null || Targets.Value == null || Targets.Value.Count == 0)
        {
            return Status.Failure;
        }

        // Filter targets to only include those with the tag "NotSolvedTarget"
        List<GameObject> validTargets = Targets.Value.FindAll(target => target.CompareTag("NotSolvedTarget"));
        if (validTargets.Count == 0)
        {
            return Status.Failure;
        }

        // Choose a random target from the valid targets
        GameObject chosenTarget = validTargets[UnityEngine.Random.Range(0, validTargets.Count)];

        m_NavMeshAgent = Agent.Value.GetComponent<NavMeshAgent>();
        m_Animator = Agent.Value.GetComponent<Animator>();

        if (m_NavMeshAgent == null || m_Animator == null)
        {
            return Status.Failure;
        }

        m_PreviousStoppingDistance = m_NavMeshAgent.stoppingDistance;
        m_NavMeshAgent.stoppingDistance = DistanceThreshold.Value;
        m_LastTargetPosition = chosenTarget.transform.position;
        m_ColliderAdjustedTargetPosition = m_LastTargetPosition;

        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        if (m_NavMeshAgent == null)
        {
            return Status.Failure;
        }

        m_NavMeshAgent.speed = Speed.Value;
        m_NavMeshAgent.SetDestination(m_ColliderAdjustedTargetPosition);

        if (Vector3.Distance(m_NavMeshAgent.transform.position, m_ColliderAdjustedTargetPosition) <= DistanceThreshold.Value)
        {
            return Status.Success;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (m_NavMeshAgent != null)
        {
            m_NavMeshAgent.stoppingDistance = m_PreviousStoppingDistance;
        }
    }
}