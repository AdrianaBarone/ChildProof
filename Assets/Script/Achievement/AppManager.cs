using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AppManager : MonoBehaviour
{
    public enum PanelType
    {
        LabelMenu,
        LabelRemind,
        LabelSingleRemind,
        LabelFind,
        LabelHelp,
        LabelSettings,
    }

    public GameObject smartphoneCanvas;
    public GameObject singleRemindPanel;
    public static AppManager Instance;

       //Card contenitore REMIND del telefono
    public GameObject achievementCardPrefab;
    public Transform achievementCardParent;
    public int cardCount;

    public GameObject[] panels;

    void Awake(){
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

    public void ShowPanel(PanelType panelType)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }

        panels[(int)panelType].SetActive(true);
    }

    public void ShowPanelByIndex(int panelIndex)
    {
        ShowPanel((PanelType)panelIndex);
    }

    public void CreateAchievementCard(Achievement achievement) {
        if (achievement == null) {
            Debug.LogError("Achievement è null!");
            return;
        }
        GameObject card = Instantiate(achievementCardPrefab, achievementCardParent);

        TMP_Text nameText = card.transform.Find("NameTask").GetComponent<TMP_Text>();
        
        TMP_Text descriptionText = card.transform.Find("DescriptionTask").GetComponent<TMP_Text>();

        Button cardButton = card.GetComponent<Button>();
        cardButton.onClick.AddListener(() =>{
            // NOTE: qui funzione per aprire singleRemind e popolare i testi con i dati dell'achievement
            AppManager.Instance.ShowPanel(AppManager.PanelType.LabelSingleRemind);
            
            Text infoText = singleRemindPanel.transform.Find("ScrollView/Viewport/Content/InfoText").GetComponent<Text>();
            Text titleText = singleRemindPanel.transform.Find("TitleText").GetComponent<Text>();
            Text progressText = singleRemindPanel.transform.Find("ProgressText").GetComponent<Text>();

            titleText.text = achievement.taskName;
            infoText.text = achievement.taskFullDescription;
            int progress = achievement.GetProgress(); // AchievementManager.Instance.GetProgressFor(achievement)
            if(progress >= achievement.taskGoal){
                progressText.text = "Completato";
            } else {
                progressText.text = progress + "/" + achievement.taskGoal;
            }
        });

        nameText.text = achievement.taskName;
        descriptionText.text = achievement.taskDescription;

        cardCount++;
    }
}
