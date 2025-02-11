using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public GameObject PopUpCanvas;
    public GameObject InfoArea;

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
        if (Input.GetKeyDown(KeyCode.P)) {
            if (InfoArea.activeSelf) {
                Time.timeScale = 1;
                animator.SetTrigger("HideInfo");
            }
        }
    }

    public void PauseGameTime() {
        Time.timeScale = 0;
    }

    public void ShowInfo(Item item) {
        var itemNameText = InfoArea.transform.Find("InfoPanel/NamePanel/Name").GetComponent<TMP_Text>();
        var itemDescriptionText = InfoArea.transform.Find("InfoPanel/DescriptionPanel/Description").GetComponent<TMP_Text>();
        var itemImage = InfoArea.transform.Find("InfoPanel/NamePanel/Image").GetComponent<Image>();

        itemNameText.text = item.data.name;
        itemDescriptionText.text = item.data.description;
        itemImage.sprite = item.data.icon;

        animator.SetTrigger("ShowInfo");
    }
}
