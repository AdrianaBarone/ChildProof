using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public enum PanelType {
        MenuPanel,
        SettingsPanel,
        InstructionsPanel,
        ConfirmPanel
    }
    
    public GameObject[] panels;

    void Awake() {
        ShowPanel(PanelType.MenuPanel);
    }

    public void ShowPanel(PanelType panelType) {
        for (int i = 0; i < panels.Length; i++) {
            panels[i].SetActive(false);
        }
        panels[(int)panelType].SetActive(true);
    }

    public void ShowPanelByIndex(int panelIndex) {
        ShowPanel((PanelType)panelIndex);
    }

    public void PlayGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
