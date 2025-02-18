using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;
    public List<Achievement> achievements = new List<Achievement>();
    public Dictionary<Achievement, GameObject> achievementCards = new Dictionary<Achievement, GameObject>();
    private int achievementCount;
    private int completedTasks = 0;
    private int taskCount;

    [SerializeField] private Slider progressSlider;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadAchievements();
    }

    void LoadAchievements()
    {
        foreach (var achievementData in Resources.LoadAll<AchievementData>("Achievements"))
        {
            Achievement achievement = new Achievement(achievementData);
            achievements.Add(achievement);

            GameObject card = AppManager.Instance.CreateAchievementCard(achievement);
            achievementCards.Add(achievement, card);

            taskCount += achievementData.goal;
        }
        achievementCount = achievements.Count;
    }


    public void IncrementAchievement(AchievementData completedAchievementData)
    {

        Achievement achievement = achievements.Find(a => a.data == completedAchievementData);

        if (achievement == null)
        {
            Debug.LogWarning("Achievement non trovato");
            return;
        }

        if (achievement.taskProgress == 0)
        {
            AppManager.Instance.CreateRemindCard(achievement);
        }

        GameObject achievementCard = achievementCards[achievement];

        achievementCard.transform.SetAsFirstSibling();
        achievement.IncrementProgress(1);
        completedTasks++;

        StartCoroutine(AnimateProgressSlider());

        GameManager.Instance.UpdateScore(achievement.data.scoreIncrease);
        AppManager.Instance.UpdateAchievementCard(achievementCard, achievement);


        if (achievement.IsComplete)
        {
            CheckAchievementCount();
        }

        UIManager.Instance.ShowAchievement(achievement);
    }

    IEnumerator<WaitForSeconds> AnimateProgressSlider()
    {
        // coroutine per animare lo slider
        float progress = (float)completedTasks / taskCount;
        while (progressSlider.value < progress)
        {
            progressSlider.value += Time.deltaTime;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void CheckAchievementCount()
    {
        if (achievements.TrueForAll(a => a.IsComplete))
        {
            GameManager.Instance.VictoryScreen();
        }
    }
}