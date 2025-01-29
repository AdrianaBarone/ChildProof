using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

/*
    public static MainMenu Instance;
    public enum PanelType
    {
        menuPanel,
        settingsPanel,
        istructionPanel,
        confirmPanel
    }


    void Awake(){
        Instance = this;
    }
    */

    public GameObject menuPanel;
    public GameObject settingsPanel;
    
    public void Awake() {
        menuPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void ShowSettings() {
        menuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void ShowIstructions() {
        menuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void ShowMenu() {
        menuPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }
    
    /*
    public void ShowPanel(PanelType panelType)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }

        panels[(int)panelType].SetActive(true);
    }

    public void ShowPanelByIndex(int panelIndex)
    {
        ShowPanel((PanelType)panelIndex);
    }
    */

    public void PlayGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame() {
        Application.Quit();
    }

}
