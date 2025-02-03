using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Tag check", story: "[Target] tag is [tag]", category: "Conditions", id: "2296aad4a3434ee4831e589d591ff2ac")]
public partial class TagChangeCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<string> Tag;

    public override bool IsTrue()
    {
        return Target.Value.CompareTag(Tag.Value);
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
