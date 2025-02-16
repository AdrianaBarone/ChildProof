using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private Sprite winImage;
    [SerializeField] private Sprite loseImage; 

    [SerializeField] private Image gameOverImage;
    [SerializeField] private TMP_Text pointText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private string[] loseMessages;
    void Awake() {
        int score = PlayerPrefs.GetInt("score", 0);
        bool winGame = PlayerPrefs.GetInt("winGame", 0) == 1;


        if (winGame) {
            gameOverImage.sprite = winImage;
            messageText.transform.parent.gameObject.SetActive(false);
            pointText.transform.parent.gameObject.SetActive(true);
            pointText.text = score.ToString();
        } else {
            gameOverImage.sprite = loseImage;
            string message = loseMessages[Random.Range(0, loseMessages.Length)];
            messageText.transform.parent.gameObject.SetActive(true);
            pointText.transform.parent.gameObject.SetActive(false);
            messageText.text = message;
        }

        
    }

    private void Update() {
        if (Input.anyKeyDown) {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
