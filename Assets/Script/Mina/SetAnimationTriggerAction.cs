using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Set Animation Trigger", story: "Set Animation [Trigger] from [Target]", category: "Action", id: "4fa82efa9e9127cd5e717b92118f8147")]
public partial class SetAnimationTriggerAction : Action
{
    // Dizionario che mappa i tag ai trigger dell'Animator
    private Dictionary<string, string> triggerMap = new Dictionary<string, string>
    {
        { "scala", "Scala_Bool" },
        { "lavandino_bagno", "Lavandino_Bool" },
        { "libreria", "Libreria_Bool" },
        { "comodino", "Comodino_Bool" },
        { "piantana", "Piantana_Bool" },
        { "presa", "Presa_Bool" },
        { "tovaglia", "Tovaglia_Bool" },
        { "prolunga", "Prolunga_Bool" },
        { "tavolo_salotto", "Tavolo_salotto_Bool" },
        { "forno", "Forno_Bool" },
        { "mobiletto_salotto", "Mobiletto_salotto_Bool" },
        { "sedia_salotto", "Sedia_salotto_Bool" },
        { "runner", "Runner_Bool" },
        { "tavolino", "Tavolino_Bool" },
        { "boccia_pesce_rosso", "Boccia_pesce_rosso_Bool" },
        { "TV", "TV_Bool" },
        { "sedia_cucina", "Sedia_cucina_Bool"},
        { "camino", "Camino_Bool"},
        { "Sedia_Cameretta", "Sedia_cameretta_Bool"},
    };
    private string currentTarget;
    [SerializeReference] public BlackboardVariable<string> Trigger;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Trigger == null || Target == null)
        {
            return Status.Failure;
        }
        Trigger.Value = triggerMap[Target.Value.name];
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

