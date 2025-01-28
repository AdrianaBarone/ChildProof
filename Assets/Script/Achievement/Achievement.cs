using System;

[Serializable]
public class Achievement {
    public AchievementData data;
    public int taskProgress;

    public bool IsComplete => taskProgress >= data.goal;

    public Achievement(AchievementData data) {
        this.data = data;
        taskProgress = 0;
    }

    public void IncrementProgress(int amount = 1) {
        if (!IsComplete) {
            taskProgress += amount;
            if (taskProgress > data.goal) {
                taskProgress = data.goal;
            }
        }
    }
}