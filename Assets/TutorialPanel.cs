using UnityEngine;

public class TutorialPanel : MonoBehaviour
{

    [SerializeField] private string closeEventKey;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        if (closeEventKey != null && closeEventKey != "")
        {
            TutorialManager.Instance.OnReceiveEvent(closeEventKey);
        }
    }
}
