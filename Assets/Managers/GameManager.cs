using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public bool InDangerMode { get; private set; }
    public static GameManager Instance { get; private set; }
    Coroutine timerCoroutine;

    public int score { get; private set; }

    [SerializeField] private float losePointsFrequency = 1f;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        score = 0;
    }

    // TODO: aggiungere il riferimento opzionale all'oggetto che ha attivato la modalitàì
    public void StartDangerModeForItem() {
        InDangerMode = true;
        // TODO: animazioni e suoni di attivazione
        // TODO: attiva la sezione 'help' del menu
        timerCoroutine = StartCoroutine(StartDangerMode());
    }

    public void EndDangerMode() {
        InDangerMode = false;
        if (timerCoroutine != null) {
            StopCoroutine(timerCoroutine);
        }
    }

    IEnumerator StartDangerMode() {
        while (true) {
            DecreaseScore(1);
            // TODO: perdi punti in base all'oggetto, animazioni e suoni periodici?
            Debug.Log(score);
            yield return new WaitForSeconds(losePointsFrequency);
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
        Debug.Log($"Score Decreased: {score}");

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