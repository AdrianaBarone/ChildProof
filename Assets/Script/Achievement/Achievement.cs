using System;

[Serializable]
public class Achievement
{
    public string taskName;
    public string taskDescription;
    public string taskFullDescription;
    public string targetObject;
    public int taskGoal;
    public int taskProgress;

    public bool IsComplete => taskProgress >= taskGoal;

    public void IncrementProgress(int amount = 1) {
        if (!IsComplete) {
            taskProgress += amount;
            if (taskProgress > taskGoal) {
                taskProgress = taskGoal;
            }
        }
    }
}