using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour {
    public static AchievementManager Instance;
    public List<Achievement> achievements = new List<Achievement>();
    // dictionary Card, GameObject for the cards
    public Dictionary<Achievement, GameObject> achievementCards = new Dictionary<Achievement, GameObject>();
    private int achievementCount;
    public GameObject PopUpCanvas;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        LoadAchievements();
        PopUpCanvas.SetActive(false);
        if (AppManager.Instance != null)
            AppManager.Instance.cardCount = 0;


        IncrementAchievement(GameManager.Instance.currentDangerInspectable.GetAchievementData());
    }

    void LoadAchievements() {
        foreach (var achievementData in Resources.LoadAll<AchievementData>("Achievements")) {
            Achievement achievement = new Achievement(achievementData);
            achievements.Add(achievement);
            GameObject card = AppManager.Instance.CreateAchievementCard(achievement);
            achievementCards.Add(achievement, card);
        }
        achievementCount = achievements.Count;
    }


    public void IncrementAchievement(AchievementData completedAchievementData) {

        Achievement achievement = achievements.Find(a => a.data == completedAchievementData);

        if (achievement == null) {
            Debug.LogWarning("Achievement non trovato");
            return;
        }

        if (achievement.taskProgress == 0) {
            GameObject achievementCard = achievementCards[achievement];
            achievementCard.transform.SetAsFirstSibling();
            achievementCard.GetComponent<Button>().interactable = true;

            // TODO: popup solo se il task non ha 1 interazione massima
            if (achievement.data.goal != 1) {
                ShowUnlockAchievementPopup(achievement);
            }
        }

        achievement.IncrementProgress(1);
        GameManager.Instance.UpdateScore(achievement.data.scoreIncrease);

        if (achievement.IsComplete) {
            ShowCompleteAchievementPopup(achievement);
            CheckAchievementCount();
        }
    }


    void ShowUnlockAchievementPopup(Achievement achievement) {
        var titleText = PopUpCanvas.transform.Find("PanelPopUp/titleText").GetComponent<Text>();
        var descriptionText = PopUpCanvas.transform.Find("PanelPopUp/descriptionText").GetComponent<Text>();

        descriptionText.text = "Nuovo Achievement Sbloccato!";
        titleText.text = achievement.data.name;

        PopUpCanvas.SetActive(true);
        Invoke("DisableCanvas", 3f);
    }

    void ShowCompleteAchievementPopup(Achievement achievement) {
        var titleText = PopUpCanvas.transform.Find("PanelPopUp/titleText").GetComponent<Text>();
        var descriptionText = PopUpCanvas.transform.Find("PanelPopUp/descriptionText").GetComponent<Text>();

        descriptionText.text = "Nuovo Achievement Completato!";
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