using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public GameObject PopUpCanvas;
    public GameObject PauseCanvas;
    public GameObject CursorCanvas;
    public GameObject InventoryCanvas;
    public GameObject ItemPopup;
    public GameObject achievementCard;

    public GameObject confirmExit;

    [SerializeField] private Sprite completedAchievementSprite;

    public Image tooltipSprite;

    public static UIManager Instance;

    private Animator animator;
    private bool isInspecting = false;

    [Header("Suoni UI")]
    public AudioClip InfoItem;
    public AudioClip SbloccoAchivement;

    private void Awake() {
        Instance = this;
    }

    void Start() {
        animator = GetComponent<Animator>();
        AudioManager.Instance.CreateAudioSource(InfoItem, false, "SFX");
        AudioManager.Instance.CreateAudioSource(SbloccoAchivement, false, "SFX");
    }

    public void ShowUnlockAchievementPopup(Achievement achievement) {
        var titleText = PopUpCanvas.transform.Find("PanelPopUp/titleText").GetComponent<TMP_Text>();
        var descriptionText = PopUpCanvas.transform.Find("PanelPopUp/descriptionText").GetComponent<TMP_Text>();

        descriptionText.text = "Nuovo Achievement Sbloccato!";
        titleText.text = achievement.data.name;

        animator.SetTrigger("PopUp");
        AudioManager.Instance.PlaySound(SbloccoAchivement);
    }


    public void ShowCompleteAchievementPopup(Achievement achievement) {
        var titleText = PopUpCanvas.transform.Find("PanelPopUp/titleText").GetComponent<TMP_Text>();
        var descriptionText = PopUpCanvas.transform.Find("PanelPopUp/descriptionText").GetComponent<TMP_Text>();

        descriptionText.text = "Nuovo Achievement Completato!";
        titleText.text = achievement.data.name;

        animator.SetTrigger("PopUp");
    }

    public void ShowInspectionTooltip(bool show) {
        tooltipSprite.gameObject.SetActive(show);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Q) && isInspecting) {
            PlayerManager.Instance.ReturnToPreviousState();
            CloseInfo();
        }

        if (InventoryCanvas.activeSelf && !isInspecting) {
            // NOTE: probabilmente esiste un modo più efficiente per fare questa cosa
            for (int i = 0; i < 10; i++) {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i)) {
                    Debug.Log("Premuto " + i);
                    Item item = InventoryManager.Instance.GetItem((i - 1 + 10) % 10);
                    if (item != null) {
                        ShowInfo(item);
                        //Aggiunta per tutorial
                        if (TutorialManager.Instance && gameObject.tag == "Tutorial") {
                            StartCoroutine(WaitForQAndTriggerEvent());
                        }
                        //
                    }
                }
            }
        }


        if (Input.GetKeyDown(KeyCode.Escape) && !PauseCanvas.activeSelf) {

            if (PlayerManager.Instance.InStatePhoneUp()) {
                AppManager.Instance.ClosePhone();
                PlayerManager.Instance.ReturnToPreviousState();
            }

            if (isInspecting) {
                CloseInfo();
                PlayerManager.Instance.ReturnToPreviousState();
            }

            Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;
            InventoryCanvas.SetActive(false);
            CursorCanvas.SetActive(false);
            PauseCanvas.SetActive(true);

            Time.timeScale = 0;
            PlayerManager.Instance.PrepareTransition();
        }
    }

    public void ShowConfirmExit(bool show) {
        confirmExit.SetActive(show);
    }

    public void HidePhoneAnimation() {
        animator.SetTrigger("HidePhone");
    }

    public void ShowPhoneAnimation() {
        animator.SetTrigger("ShowPhone");
    }

    public void ShowInventory(bool show) {
        InventoryCanvas.SetActive(show);
    }

    public void Resume() {
        //Cursor.visible = false;
        CursorCanvas.SetActive(true);
        InventoryCanvas.SetActive(true);
        PauseCanvas.SetActive(false);
        Time.timeScale = 1;
        PlayerManager.Instance.ReturnToPreviousState();
    }

    public void ExitGame() {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void ShowInfo(Item item) {
        isInspecting = true;
        PlayerManager.Instance.PrepareTransition();
        var itemNameText = ItemPopup.transform.Find("ImageTitle/Title").GetComponent<TMP_Text>();
        var itemDescriptionText = ItemPopup.transform.Find("Description").GetComponent<TMP_Text>();
        var itemImage = ItemPopup.transform.Find("ImageTitle/Image").GetComponent<Image>();

        itemNameText.text = item.data.name;
        itemDescriptionText.text = item.data.description;
        itemImage.sprite = item.data.icon;

        animator.SetTrigger("ShowInfo");
        Time.timeScale = 0;
        AudioManager.Instance.PlaySound(InfoItem);
    }

    public void CloseInfo() {
        isInspecting = false;
        animator.SetTrigger("HideInfo");
        Time.timeScale = 1;
    }

    public void ShowAchievement(Achievement achievement) {
        if (achievement == null) {
            Debug.LogError("Achievement è null!");
            return;
        }

        achievementCard.transform.Find("Body/Titolo").GetComponent<TMP_Text>().text = achievement.data.name;
        if (achievement.taskProgress >= achievement.data.goal) {

            achievementCard.GetComponent<Image>().sprite = completedAchievementSprite;
            achievementCard.transform.Find("Body/Titolo").GetComponent<TMP_Text>().color = new Color(0.8396226f, 0.9176471f, 0.972549f);
            achievementCard.transform.Find("Body/Progresso/Testo").GetComponent<TMP_Text>().color = new Color(0.8396226f, 0.9176471f, 0.972549f);
            achievementCard.transform.Find("Icon").GetComponent<Image>().sprite = achievement.data.achievementIcon;
        }

        TMP_Text progressText = achievementCard.transform.Find("Body/Progresso/Testo").GetComponent<TMP_Text>();
        Slider progressSlider = achievementCard.transform.Find("Body/Progresso/Slider").GetComponent<Slider>();

        progressText.text = achievement.taskProgress + "/" + achievement.data.goal;
        progressSlider.value = (float)achievement.taskProgress / achievement.data.goal;

        animator.SetTrigger("ShowAchievementPopup");
    }

    //Aggiunta per tutorial
    private IEnumerator WaitForQAndTriggerEvent() {
        bool qPressed = false;

        while (!qPressed) {
            if (Input.GetKeyDown(KeyCode.Q)) {
                qPressed = true;
                Debug.Log("Q premuto");
            }
            yield return null;
        }

        if (TutorialManager.Instance && gameObject.tag == "Tutorial") {
            TutorialManager.Instance.OnReceiveEvent("ChiusuraPopupOggetto");
        }
    }
}
