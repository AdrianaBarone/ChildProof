using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour {
    private int score;
    public void Awake() {
        score = 0;
        Debug.Log($"Score Updated: {score}");
    }

    public void UpdateScore(int value) {
        if (value > 0) {
            score += value;
            Debug.Log($"Score Updated: {score}");
        }
        else {
            Debug.LogWarning("UpdateScore accetta solo valori positivi.");
        }
    }

    public void DecreaseScore(int value) {
        score -= value;
        Debug.Log($"Score Decreased: {score}");

        if (score <= 0) {
            GameOver();
        }
    }

    private void GameOver() {
        Debug.Log("Game Over! Punteggio raggiunto: 0");
        SceneManager.LoadSceneAsync(3);
    }
}
