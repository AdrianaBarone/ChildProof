using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public GameObject PopUpCanvas;
    public GameObject PauseCanvas;
    public GameObject CursorCanvas;
    public GameObject InfoArea;
    public GameObject InventoryCanvas;

    public static UIManager Instance;

    private Animator animator;

    private void Awake() {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        animator = GetComponent<Animator>();
    }

    public void ShowUnlockAchievementPopup(Achievement achievement) {
        var titleText = PopUpCanvas.transform.Find("PanelPopUp/titleText").GetComponent<TMP_Text>();
        var descriptionText = PopUpCanvas.transform.Find("PanelPopUp/descriptionText").GetComponent<TMP_Text>();

        descriptionText.text = "Nuovo Achievement Sbloccato!";
        titleText.text = achievement.data.name;

        animator.SetTrigger("PopUp");
    }


    public void ShowCompleteAchievementPopup(Achievement achievement) {
        var titleText = PopUpCanvas.transform.Find("PanelPopUp/titleText").GetComponent<TMP_Text>();
        var descriptionText = PopUpCanvas.transform.Find("PanelPopUp/descriptionText").GetComponent<TMP_Text>();

        descriptionText.text = "Nuovo Achievement Completato!";
        titleText.text = achievement.data.name;

        animator.SetTrigger("PopUp");
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            if (InfoArea.activeSelf) {
                Time.timeScale = 1;
                PlayerManager.Instance.TransitionToExploration();
                animator.SetTrigger("HideInfo");
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (PauseCanvas.activeSelf) {
                Cursor.lockState = CursorLockMode.Locked;
                //Cursor.visible = false;
                InventoryCanvas.SetActive(true);
                CursorCanvas.SetActive(true);
                PauseCanvas.SetActive(false);
                Time.timeScale = 1;
                PlayerManager.Instance.ReturnToPreviousState();
            }
            else if (Time.timeScale == 1) {
                Cursor.lockState = CursorLockMode.None;
                //Cursor.visible = true;
                InventoryCanvas.SetActive(false);
                CursorCanvas.SetActive(false);
                PauseCanvas.SetActive(true);
                Debug.Log("Pause");
                Time.timeScale = 0;
                PlayerManager.Instance.PrepareTransition();
            }
        }
    }

    public void UnpauseGame() {
        Cursor.lockState = CursorLockMode.Locked;
        PauseCanvas.SetActive(false);
        CursorCanvas.SetActive(true);
        InventoryCanvas.SetActive(true);
        PlayerManager.Instance.ReturnToPreviousState();
        Time.timeScale = 1;
    }

    public void ExitGame() {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void PauseGameTime() {
        Debug.Log("PauseGameTime");
        Time.timeScale = 0;
    }

    public void ShowInfo(Item item) {
        PlayerManager.Instance.PrepareTransition();
        var itemNameText = InfoArea.transform.Find("InfoPanel/NamePanel/Name").GetComponent<TMP_Text>();
        var itemDescriptionText = InfoArea.transform.Find("InfoPanel/DescriptionPanel/Description").GetComponent<TMP_Text>();
        var itemImage = InfoArea.transform.Find("InfoPanel/NamePanel/Image").GetComponent<Image>();

        itemNameText.text = item.data.name;
        itemDescriptionText.text = item.data.description;
        itemImage.sprite = item.data.icon;

        animator.SetTrigger("ShowInfo");
    }
}
