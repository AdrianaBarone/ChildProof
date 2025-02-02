using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AppManager : MonoBehaviour {
    public enum PanelType {
        LabelMenu,
        LabelRemind,
        LabelSingleRemind,
        LabelFind,
        LabelHelpBuy,

        LabelHelp,
    }

    public GameObject smartphoneCanvas;
    public GameObject singleRemindPanel;
    public static AppManager Instance;

    //Card contenitore REMIND del telefono
    public GameObject achievementCardPrefab;
    public Transform achievementCardParent;
    public int cardCount;
    int helpPrice = 0;
    bool helpBought = false;
    public Button helpButton;

    public GameObject[] panels;

    void Awake() {
        Instance = this;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            ToggleSmartphone();
        }
    }

    void ToggleSmartphone() {
        bool isActive = smartphoneCanvas.gameObject.activeSelf;
        smartphoneCanvas.gameObject.SetActive(!isActive);
    }

    public void ShowPanel(PanelType panelType) {
        for (int i = 0; i < panels.Length; i++) {
            panels[i].SetActive(false);
        }

        panels[(int)panelType].SetActive(true);
    }

    public void ShowPanelByIndex(int panelIndex) {
        ShowPanel((PanelType)panelIndex);
    }

    public void ShowHelpPanel() {
        if (helpBought) {
            ShowPanel(PanelType.LabelHelp);
        }
        else {
            ShowPanel(PanelType.LabelHelpBuy);
        }
    }

    public void EnableHelpBuyPanel(Inspectable dangerInspectable) {
        Debug.Log(dangerInspectable);
        AchievementData achievement = dangerInspectable.GetAchievementData();
        helpButton.interactable = true;
        helpPrice = achievement.scoreIncrease * GameManager.Instance.HelpPricePercent / 100;

        panels[(int)PanelType.LabelHelpBuy].transform.Find("PanelBuy/PriceText").GetComponent<Text>().text = helpPrice.ToString() + " Punti";
        FillHelpPanel(dangerInspectable);
    }

    public void DisableHelpBuyPanel() {
        helpPrice = 0;
        helpBought = false;
        helpButton.interactable = false;
    }

    public void TryBuyHelp() {
        GameManager.Instance.DecreaseScore(helpPrice);
        helpBought = true;
        ShowPanel(PanelType.LabelHelp);
    }

    void FillHelpPanel(Inspectable dangerInspectable) {
        AchievementData achievement = dangerInspectable.GetAchievementData();
        Transform panelHelp = panels[(int)PanelType.LabelHelp].transform.Find("PanelHelp");

        panelHelp.Find("TextHelp").GetComponent<Text>().text = achievement.helpDescription;
        panelHelp.Find("ImageHelp").GetComponent<Image>().sprite = achievement.solutionImage;


    }

    public GameObject CreateAchievementCard(Achievement achievement) {
        if (achievement == null) {
            Debug.LogError("Achievement è null!");
            return null;
        }
        GameObject card = Instantiate(achievementCardPrefab, achievementCardParent);

        TMP_Text nameText = card.transform.Find("NameTask").GetComponent<TMP_Text>();

        TMP_Text descriptionText = card.transform.Find("DescriptionTask").GetComponent<TMP_Text>();

        Button cardButton = card.GetComponent<Button>();
        cardButton.interactable = false;
        cardButton.onClick.AddListener(() => {
            // TODO: reset scroll position of singleRemindPanel
            ScrollRect scrollRect = singleRemindPanel.transform.Find("ScrollView").GetComponent<ScrollRect>();
            scrollRect.verticalNormalizedPosition = 1f; // Torna in cima
            ShowPanel(PanelType.LabelSingleRemind);

            Text infoText = singleRemindPanel.transform.Find("ScrollView/Viewport/Content/InfoText").GetComponent<Text>();
            Text titleText = singleRemindPanel.transform.Find("TitleText").GetComponent<Text>();
            Text progressText = singleRemindPanel.transform.Find("ProgressText").GetComponent<Text>();

            titleText.text = achievement.data.name;
            infoText.text = achievement.data.fullDescription;
            int progress = achievement.taskProgress;
            if (progress >= achievement.data.goal) {
                progressText.text = "Completato";
            }
            else {
                progressText.text = progress + "/" + achievement.data.goal;
            }
        });

        nameText.text = achievement.data.name;
        descriptionText.text = achievement.data.description;

        cardCount++;

        return card;
    }
}
