using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/ChildInteracted")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "ChildInteracted", message: "Touch", category: "Events", id: "a5f059df021a4d5ec3d84c7527935700")]
public partial class ChildInteracted : EventChannelBase {
    public delegate void ChildInteractedEventHandler();
    public event ChildInteractedEventHandler Event;

    public void SendEventMessage() {
        Debug.Log("ChildInteracted");
        Event?.Invoke();
    }

    public override void SendEventMessage(BlackboardVariable[] messageData) {
        Event?.Invoke();
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback) {
        ChildInteractedEventHandler del = () => {
            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del) {
        Event += del as ChildInteractedEventHandler;
    }

    public override void UnregisterListener(Delegate del) {
        Event -= del as ChildInteractedEventHandler;
    }
}

