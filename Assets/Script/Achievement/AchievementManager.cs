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

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        LoadAchievements();
        if (AppManager.Instance != null)
            AppManager.Instance.cardCount = 0;
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

            if (achievement.data.goal != 1) {
                UIManager.Instance.ShowUnlockAchievementPopup(achievement);
            }
        }

        achievement.IncrementProgress(1);
        GameManager.Instance.UpdateScore(achievement.data.scoreIncrease);

        /*
        if (achievement.IsComplete) {
            UIManager.Instance.ShowCompleteAchievementPopup(achievement);
            CheckAchievementCount();
        }
        */

    }

    public void CheckAchievementCount() {
        if (achievements.TrueForAll(a => a.IsComplete)) {
            GameManager.Instance.VictoryScreen();
        }
    }
}