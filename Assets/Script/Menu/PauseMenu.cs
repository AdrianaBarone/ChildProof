using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour{
    public static bool GameIsPaused = false;
    public GameObject inventoryUI;
    //public GameObject pauseMenuUI;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G)){
            if (GameIsPaused){
                Resume();
            }else{
                Pause();
            }
        }
    }

    void Resume(){
        //pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        inventoryUI.SetActive(true);
        PlayerManager.Instance.ReturnToPreviousState();
    }

    void Pause(){
        //pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        inventoryUI.SetActive(false);
        PlayerManager.Instance.PrepareTransition();
    }
}
