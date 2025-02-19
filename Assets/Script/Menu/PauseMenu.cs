using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject inventoryUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        inventoryUI.SetActive(true);
        PlayerManager.Instance.ReturnToPreviousState();
    }

    void Pause()
    {
        Time.timeScale = 0f;
        GameIsPaused = true;
        inventoryUI.SetActive(false);
        PlayerManager.Instance.PrepareTransition();
    }

    public void ShowConfirmExit()
    {
        UIManager.Instance.ShowConfirmExit(true);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
