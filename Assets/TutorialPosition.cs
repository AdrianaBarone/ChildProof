using UnityEngine;

public class TutorialPosition : MonoBehaviour
{
    // when  colliding with something send event to TutorialManager
    void OnTriggerEnter(Collider triggerColliderEnter)
    {
        TutorialManager.Instance.OnReceiveEvent("RaggiungiScaffale");
    }

}
