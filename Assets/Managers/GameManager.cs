using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public bool InDangerMode { get; private set; }
    public Inspectable currentDangerInspectable;
    public static GameManager Instance { get; private set; }
    Coroutine timerCoroutine;

    public int score { get; private set; }
    public int startingScore = 100;

    public int PointDecreasePercent = 5;
    public float PointDecreaseRate = 1f;
    public TMP_Text pointsText;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        score = startingScore;
        StartCoroutine(AnimatePointsText());
    }

    private void Start()
    {
        // NOTE: non ha senso ma funziona, quindi non toccare
        Time.timeScale = 1;
        InDangerMode = false;
        AudioManager.Instance.PlayAudioWithFadeIn(InDangerMode);
    }

    public void StartDangerModeForInspectable(Inspectable inspectable)
    {
        InDangerMode = true;

        AudioManager.Instance.PlayAudioWithFadeIn(InDangerMode);
        AudioManager.Instance.StopAudioWithFadeOut(InDangerMode);

        currentDangerInspectable = inspectable;
        // TODO: animazioni e suoni di attivazione
        timerCoroutine = StartCoroutine(LosePointsCoroutine());
    }

    public void EndDangerMode()
    {
        InDangerMode = false;

        AudioManager.Instance.PlayAudioWithFadeIn(InDangerMode);
        AudioManager.Instance.StopAudioWithFadeOut(InDangerMode);

        currentDangerInspectable = null;
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
    }

    IEnumerator AnimatePointsText()
    {
        int pointsStart = int.Parse(pointsText.text);
        int pointsEnd = score;
        int step = pointsStart < pointsEnd ? 1 : -1;
        float time = step > 0 ? 0.01f : 0.1f;

        for (int i = pointsStart; i != pointsEnd; i += step)
        {
            pointsText.text = i >= 0 ? i.ToString("D4") : "0000";
            yield return new WaitForSeconds(time);
        }
        pointsText.text = pointsEnd.ToString("D4");
    }

    IEnumerator LosePointsCoroutine()
    {
        while (true)
        {
            int scoreDecrease = currentDangerInspectable.GetAchievementData().scoreIncrease * PointDecreasePercent / 100;

            DecreaseScore(scoreDecrease);
            // TODO: animazioni e suoni periodici?
            StartCoroutine(AnimatePointsText());
            yield return new WaitForSeconds(PointDecreaseRate);
        }
    }

    public void UpdateScore(int value)
    {
        if (value > 0)
        {
            score += value;
            StartCoroutine(AnimatePointsText());
        }
        else
        {
            Debug.LogWarning("UpdateScore accetta solo valori positivi.");
        }
    }

    public void DecreaseScore(int value)
    {
        score -= value;

        if (score <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        //TODO: animazioni e suoni di sconfitta
        Cursor.lockState = CursorLockMode.None;
        StartCoroutine(DelayedGameOverScreen(false));
    }

    public void VictoryScreen()
    {
        // TODO: animazioni e suoni di vittoria
        StartCoroutine(DelayedGameOverScreen(true));
    }

    public IEnumerator DelayedGameOverScreen(bool win)
    {
        // NOTE: impostare qui il tempo di attesa per tutte le eventuali animazioni e suoni
        yield return new WaitForSeconds(1f);
        PlayerPrefs.SetInt("score", score);
        PlayerPrefs.SetInt("winGame", win ? 1 : 0);

        SceneManager.LoadSceneAsync("GameOver");
    }
}