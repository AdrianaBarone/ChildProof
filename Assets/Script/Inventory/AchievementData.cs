using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New AchievementData", menuName = "ScriptableObject/Create New AchievementData")]
public class AchievementData : ScriptableObject
{
    public new string name;
    public string description;
    public string fullDescription;
    public Sprite achievementIcon;
    public int goal;
    public int scoreIncrease;
    public AudioClip audioClipQuestion;
    public AudioClip audioClipAnswer;
}