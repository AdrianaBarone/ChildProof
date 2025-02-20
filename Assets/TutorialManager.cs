using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{

    public static TutorialManager Instance;
    [SerializeField] private string nextSceneName;
    [SerializeField] private Sequence[] sequences;
    [SerializeField] private GameObject inventoryCanvas;
    private int currentStep = 0;

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
            inventoryCanvas.SetActive(true);
            sequences[currentStep - 1].tutorialPanel.Hide();
        }
    }

    public void OnReceiveEvent(string eventKey)
    {
        if (eventKey == sequences[currentStep].eventKey)
        {
            if (currentStep == sequences.Length - 1)
            {
                SceneManager.LoadSceneAsync(nextSceneName);
                return;
            }


            if (sequences[currentStep].tutorialPanel != null)
            {
                sequences[currentStep].tutorialPanel.Show();
                inventoryCanvas.SetActive(false);
            }

            currentStep++;
        }
    }
}

[System.Serializable]
public struct Sequence
{
    public string eventKey;
    public TutorialPanel tutorialPanel;
}