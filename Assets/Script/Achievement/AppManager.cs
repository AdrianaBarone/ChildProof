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
        LabelAchievements
    }

    public GameObject smartphoneCanvas;
    public GameObject singleRemindPanel;
    public static AppManager Instance;

    //Card contenitore REMIND del telefono
    public GameObject achievementCardPrefab;
    public Transform achievementCardParent;
    public int cardCount;

    public GameObject[] panels;

    [Header("Suoni Telefono")]
    public AudioClip SbloccoTelefono;
    public AudioClip BloccoTelefono;


    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && Time.timeScale != 0)
        {
            ToggleSmartphone();
        }
    }

    void ToggleSmartphone()
    {
        bool isActive = smartphoneCanvas.gameObject.activeSelf;
        smartphoneCanvas.gameObject.SetActive(!isActive);

        if (!isActive)
        {
            // NOTE: Apre il telefono
            PlayerManager.Instance.SetToPhoneUp();
            UIManager.Instance.ShowInventory(false);
            // UIMManager.Instance.ShowPhoneAnimation();
            // AudioManager.Instance.PlaySound(SbloccoTelefono);
        }
        else
        {
            // NOTE: Chiude il telefono
            PlayerManager.Instance.TransitionToExploration();
            UIManager.Instance.ShowInventory(true);
            // UIMManager.Instance.HidePhoneAnimation();
            // AudioManager.Instance.PlaySound(BloccoTelefono);
        }

    }

    public void HidePhone()
    {
        PlayerManager.Instance.TransitionToExploration();
        // UIMManager.Instance.HidePhoneAnimation();
        AudioManager.Instance.PlaySound(BloccoTelefono);
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

    public GameObject CreateRemindCard(Achievement achievement)
    {
        if (achievement == null)
        {
            Debug.LogError("Achievement è null!");
            return null;
        }
        GameObject card = Instantiate(achievementCardPrefab, achievementCardParent);

        TMP_Text nameText = card.transform.Find("NameTask").GetComponent<TMP_Text>();

        TMP_Text descriptionText = card.transform.Find("DescriptionTask").GetComponent<TMP_Text>();

        Button cardButton = card.GetComponent<Button>();
        cardButton.interactable = false;
        cardButton.onClick.AddListener(() =>
        {
            ScrollRect scrollRect = singleRemindPanel.transform.Find("ScrollView").GetComponent<ScrollRect>();
            scrollRect.verticalNormalizedPosition = 1f; // Torna in cima
            ShowPanel(PanelType.LabelSingleRemind);

            TMP_Text infoText = singleRemindPanel.transform.Find("ScrollView/Viewport/Content/InfoText").GetComponent<TMP_Text>();
            TMP_Text titleText = singleRemindPanel.transform.Find("Header/TitleText").GetComponent<TMP_Text>();
            TMP_Text progressText = singleRemindPanel.transform.Find("Header/ProgressText").GetComponent<TMP_Text>();

            titleText.text = achievement.data.name;
            infoText.text = achievement.data.fullDescription;
            int progress = achievement.taskProgress;
            if (progress >= achievement.data.goal)
            {
                progressText.text = "Completato";
            }
            else
            {
                progressText.text = progress + "/" + achievement.data.goal;
            }
        });

        nameText.text = achievement.data.name;
        descriptionText.text = achievement.data.description;

        cardCount++;

        return card;
    }
}
