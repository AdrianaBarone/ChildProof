using UnityEngine;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{

    public static TutorialManager Instance;
    [SerializeField] private Sequence[] sequences;
    private int currentStep = -1;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        OnReceiveEvent("Inizio");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            sequences[currentStep].tutorialPanel.Hide();
        }
    }

    public void OnReceiveEvent(string eventKey)
    {
        if (currentStep > 0 && sequences[currentStep].tutorialPanel == null)
        {
            Debug.Log("End of tutorial");
            return;
        }


        if (eventKey == sequences[currentStep + 1].eventKey)
        {
            currentStep++;
            sequences[currentStep].tutorialPanel.Show();
        }
    }
}

[System.Serializable]
public struct Sequence
{
    public string eventKey;
    public TutorialPanel tutorialPanel;
}