using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public bool InDangerMode { get; private set; }
    public Inspectable currentDangerInspectable;
    public static GameManager Instance { get; private set; }
    Coroutine timerCoroutine;

    public int score { get; private set; }

    public int PointDecreasePercent = 5;
    public float PointDecreaseRate = 1f;
    public int HelpPricePercent = 50;


    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        score = 0;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.D)) {
            if (!InDangerMode) {

                StartDangerModeForInspectable(currentDangerInspectable);
            }
            else {
                EndDangerMode();

            }
        }
    }

    public void StartDangerModeForInspectable(Inspectable inspectable) {
        InDangerMode = true;
        currentDangerInspectable = inspectable;
        // TODO: animazioni e suoni di attivazione
        AppManager.Instance.EnableHelpBuyPanel(currentDangerInspectable);
        timerCoroutine = StartCoroutine(LosePointsCoroutine());
    }

    public void EndDangerMode() {
        InDangerMode = false;
        currentDangerInspectable = null;
        AppManager.Instance.DisableHelpBuyPanel();
        if (timerCoroutine != null) {
            StopCoroutine(timerCoroutine);
        }
    }

    IEnumerator LosePointsCoroutine() {
        while (true) {
            int scoreDecrease = currentDangerInspectable.GetAchievementData().scoreIncrease * PointDecreasePercent / 100;

            DecreaseScore(scoreDecrease);
            // TODO: animazioni e suoni periodici? collegamentu UI
            Debug.Log(score);
            yield return new WaitForSeconds(PointDecreaseRate);
        }
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

        if (score <= 0) {
            GameOver();
        }
    }

    private void GameOver() {
        Debug.Log("Game Over! Punteggio raggiunto: 0");
        SceneManager.LoadSceneAsync(3);
    }

    public void VictoryScreen() {
        Debug.Log("Hai vinto!");
        // TODO: animazioni e suoni di vittoria
        // TODO: cambio scena
    }
}