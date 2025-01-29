using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour {
    public static AchievementManager Instance;
    // directory to load achievements from
    public List<Achievement> achievements = new List<Achievement>();
    private int achievementCount;

    /*
    //telefono
    public GameObject smartphoneCanvas;
    */



    //PopUp achievement raggiunto
    public GameObject PopUpCanvas;

    private void Awake() {
        Instance = this;
        LoadAchievements();
        PopUpCanvas.SetActive(false);
        if (AppManager.Instance != null)
            AppManager.Instance.cardCount = 0;
    }

    /*
        void Update() {
            if (Input.GetKeyDown(KeyCode.E)) {
                ToggleSmartphone();
            }
        }

        /*
        void ToggleSmartphone() {
            bool isActive = smartphoneCanvas.gameObject.activeSelf;
            smartphoneCanvas.gameObject.SetActive(!isActive);
        }
        */

    void LoadAchievements() {
        Debug.Log("Loading Achievements");
        Debug.Log("Achievements: " + Resources.LoadAll<AchievementData>("Achievements").Length);
        foreach (var achievementData in Resources.LoadAll<AchievementData>("Achievements")) {
            Achievement achievement = new Achievement(achievementData);
            achievements.Add(achievement);
            // TODO: capire se creare anche la card nel telefono qui o in IncrementAchievement
        }
        achievementCount = achievements.Count;
    }


    public void IncrementAchievement(AchievementData completedAchievementData) {

        Achievement achievement = achievements.Find(a => a.data == completedAchievementData);

        if (achievement == null) {
            Debug.LogWarning("Achievement non trovato");
            return;
        }

        achievement.IncrementProgress(1);
        GameManager.Instance.UpdateScore(achievement.data.scoreIncrease);

        if (achievement.taskProgress == 1) {
            // NOTE: achievement appena sbloccato
            AppManager.Instance.CreateAchievementCard(achievement);
            ShowAchievementPopup(achievement);
        }
        else if (achievement.IsComplete) {
            // NOTE: achievement completato
            CheckAchievementCount();
        }
    }


    void ShowAchievementPopup(Achievement achievement) {
        var titleText = PopUpCanvas.transform.Find("PanelPopUp/titleText").GetComponent<Text>();
        var descriptionText = PopUpCanvas.transform.Find("PanelPopUp/descriptionText").GetComponent<Text>();

        descriptionText.text = "Nuovo Achievement Sbloccato!";
        titleText.text = achievement.data.name;

        PopUpCanvas.SetActive(true);
        Invoke("DisableCanvas", 3f);
    }

    private void DisableCanvas() {
        PopUpCanvas.SetActive(false);
    }

    public void CheckAchievementCount() {
        if (achievementCount == AppManager.Instance.cardCount) {
            GameManager.Instance.VictoryScreen();
        }
    }
}