using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "StartDangerMode", story: "Start DangerMode for [Target]", category: "Action", id: "0b5990094e21ececb134bd80b775a98b")]
public partial class StartDangerModeAction : Action {
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    protected override Status OnStart() {
        Inspectable inspectable = Target.Value.GetComponentInParent<Inspectable>();

        if (inspectable == null) {
            Debug.LogError("Inspectable not found on target parent");
            return Status.Failure;
        }

        GameManager.Instance.StartDangerModeForInspectable(inspectable);
        return Status.Running;
    }

    protected override Status OnUpdate() {
        return Status.Success;
    }

    protected override void OnEnd() {
    }
}

