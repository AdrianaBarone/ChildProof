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
           { "scala_target", "Scala_Bool" },
    { "lavandino_bagno_target", "Lavandino_Bool" },
    { "libreria_target", "Libreria_Bool" },
    { "comodino_target", "Comodino_Bool" },
    { "piantana_target", "Piantana_Bool" },
    { "presa_target", "Presa_Bool" },
    { "tovaglia_target", "Tovaglia_Bool" },
    { "prolunga_target", "Prolunga_Bool" },
    { "tavolo_salotto_target", "Tavolo_salotto_Bool" },
    { "forno_target", "Forno_Bool" },
    { "mobiletto_salotto_target", "Mobiletto_salotto_Bool" },
    { "sedia_salotto_target", "Sedia_salotto_Bool" },
    { "runner_target", "Runner_Bool" },
    { "tavolino_target", "Tavolino_Bool" },
    { "boccia_pesce_rosso_target", "Boccia_pesce_rosso_Bool" },
    { "TV_target", "TV_Bool" },
    { "sedia_cucina_target", "Sedia_cucina_Bool" },
    { "camino_target", "Camino_Bool" },
    { "Sedia_Cameretta_target", "Sedia_cameretta_Bool" }

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

