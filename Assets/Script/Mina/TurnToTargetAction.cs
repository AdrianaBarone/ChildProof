using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "turn_to_target", story: "[self] [turn] to [target]", category: "Action/Navigation", id: "5078194555fbf6a87adf4dc1edfc3823")]
public partial class TurnToTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Transform> Turn;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

