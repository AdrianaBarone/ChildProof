using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool InDangerMode { get; private set; }
    public Inspectable currentDangerInspectable;
    public static GameManager Instance { get; private set; }
    Coroutine timerCoroutine;

    public int score { get; private set; }

    public int PointDecreasePercent = 5;
    public float PointDecreaseRate = 1f;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        score = 0;
    }

    private void Start()
    {
        // NOTE: non ha senso ma funziona, quindi non toccare
        Time.timeScale = 1;
        InDangerMode = false;
        //AudioManager.Instance.ChangeToSnapshot(2,3f)
        // AudioManager.Instance.PlayAudioWithFadeIn(InDangerMode);
    }

    public void StartDangerModeForInspectable(Inspectable inspectable)
    {
        InDangerMode = true;

        //AudioManager.Instance.ChangeToSnapshot(2,3f)
        AudioManager.Instance.PlayAudioWithFadeIn(InDangerMode);
        AudioManager.Instance.StopAudioWithFadeOut(InDangerMode);
        
        currentDangerInspectable = inspectable;
        // TODO: animazioni e suoni di attivazione
        timerCoroutine = StartCoroutine(LosePointsCoroutine());
    }

    public void EndDangerMode()
    {
        InDangerMode = false;

        //AudioManager.Instance.ChangeToSnapshot(2,3f)
        AudioManager.Instance.PlayAudioWithFadeIn(InDangerMode);
        AudioManager.Instance.StopAudioWithFadeOut(InDangerMode);

        currentDangerInspectable = null;
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
    }

    IEnumerator LosePointsCoroutine()
    {
        while (true)
        {
            int scoreDecrease = currentDangerInspectable.GetAchievementData().scoreIncrease * PointDecreasePercent / 100;

            DecreaseScore(scoreDecrease);
            // TODO: animazioni e suoni periodici? collegamentu UI
            yield return new WaitForSeconds(PointDecreaseRate);
        }
    }

    public void UpdateScore(int value)
    {
        if (value > 0)
        {
            score += value;
            Debug.Log($"Score Updated: {score}");
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
        StartCoroutine(DelayedGameOverScreen(false));
    }

    public void VictoryScreen()
    {
        // TODO: animazioni e suoni di vittoria
        StartCoroutine(DelayedGameOverScreen(true));
    }

    public IEnumerator DelayedGameOverScreen(bool win) {
        // NOTE: impostare qui il tempo di attesa per tutte le eventuali animazioni e suoni
        yield return new WaitForSeconds(1f);
        PlayerPrefs.SetInt("score", score);
        PlayerPrefs.SetInt("winGame", win ? 1 : 0);

        SceneManager.LoadSceneAsync("GameOver");
    }
}