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

    [Header("Find")]
    public TMP_Text roomText;

    [Header("Reminders")]

    public GameObject remindCardPrefab;
    public Transform remindCardParent;

    [Header("Achievements")]

    public GameObject achievementCardPrefab;
    public Transform achievementCardParent;
    [SerializeField] private Sprite completedAchievementSprite;
    public GameObject[] panels;

    [Header("Suoni Telefono")]
    public AudioClip SbloccoTelefono;
    public AudioClip BloccoTelefono;

    public bool isPauseMenu;


    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // 
        if (Input.GetKeyDown(KeyCode.E) && !(UIManager.Instance.isPaused || UIManager.Instance.isInspecting))
        {
            ToggleSmartphone();
        }
    }

    void ToggleSmartphone()
    {
        bool isActive = smartphoneCanvas.gameObject.activeSelf;
        InventoryManager.Instance.ClearSelection();

        if (!isActive)
        {
            // NOTE: Apre il telefono
            PlayerManager.Instance.SetToPhoneUp();
            Time.timeScale = 0;
            UIManager.Instance.ShowInventory(false);
            UIManager.Instance.ShowPhoneAnimation();
            AudioManager.Instance.PlaySound(SbloccoTelefono);
        }
        else
        {
            // NOTE: Chiude il telefono
            ClosePhone();
        }

    }

    public void ClosePhone()
    {
        Time.timeScale = 1;
        PlayerManager.Instance.ReturnToPreviousState();
        UIManager.Instance.ShowInventory(true);
        UIManager.Instance.HidePhoneAnimation();
        AudioManager.Instance.PlaySound(BloccoTelefono);
    }

    public void ShowPanel(PanelType panelType)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }

        panels[(int)panelType].SetActive(true);
        // remindScrollView.ScrollTo(remindScrollView.ElementAt(0));
        // achievementScrollView.ScrollTo(achievementScrollView.ElementAt(0));
    }

    public void ShowPanelByIndex(int panelIndex)
    {
        ShowPanel((PanelType)panelIndex);
    }

    public GameObject CreateAchievementCard(Achievement achievement)
    {
        if (achievement == null)
        {
            Debug.LogError("Achievement è null!");
            return null;
        }
        GameObject card = Instantiate(achievementCardPrefab, achievementCardParent);

        TMP_Text titleText = card.transform.Find("Body/Titolo").GetComponent<TMP_Text>();
        TMP_Text progressText = card.transform.Find("Body/Progresso/Testo").GetComponent<TMP_Text>();
        Slider progressSlider = card.transform.Find("Body/Progresso/Slider").GetComponent<Slider>();

        titleText.text = achievement.data.name;
        progressText.text = achievement.taskProgress + "/" + achievement.data.goal;
        progressSlider.value = (float)achievement.taskProgress / achievement.data.goal;


        return card;
    }

    public void UpdateAchievementCard(GameObject card, Achievement achievement)
    {
        if (achievement == null)
        {
            Debug.LogError("Achievement è null!");
            return;
        }

        if (achievement.taskProgress >= achievement.data.goal)
        {

            card.GetComponent<Image>().sprite = completedAchievementSprite;
            card.transform.Find("Body/Titolo").GetComponent<TMP_Text>().color = new Color(0.8396226f, 0.9176471f, 0.972549f);
            card.transform.Find("Body/Progresso/Testo").GetComponent<TMP_Text>().color = new Color(0.8396226f, 0.9176471f, 0.972549f);
            card.transform.Find("Icon").GetComponent<Image>().sprite = achievement.data.achievementIcon;
        }

        TMP_Text progressText = card.transform.Find("Body/Progresso/Testo").GetComponent<TMP_Text>();
        Slider progressSlider = card.transform.Find("Body/Progresso/Slider").GetComponent<Slider>();

        progressText.text = achievement.taskProgress + "/" + achievement.data.goal;
        progressSlider.value = (float)achievement.taskProgress / achievement.data.goal;
    }

    public void UpdateChildPosition(string roomName)
    {
        Debug.Log("Room: " + roomName);
        roomText.text = roomName;
    }

    public GameObject CreateRemindCard(Achievement achievement)
    {
        if (achievement == null)
        {
            Debug.LogError("Achievement è null!");
            return null;
        }
        GameObject card = Instantiate(remindCardPrefab, remindCardParent);

        TMP_Text nameText = card.transform.Find("NameTask").GetComponent<TMP_Text>();

        TMP_Text descriptionText = card.transform.Find("DescriptionTask").GetComponent<TMP_Text>();

        Button cardButton = card.GetComponent<Button>();
        cardButton.onClick.AddListener(() =>
        {
            ShowPanel(PanelType.LabelSingleRemind);

            TMP_Text infoText = singleRemindPanel.transform.Find("ScrollView/Viewport/Content/InfoText").GetComponent<TMP_Text>();
            TMP_Text titleText = singleRemindPanel.transform.Find("Header/TitleText").GetComponent<TMP_Text>();
            TMP_Text progressText = singleRemindPanel.transform.Find("Header/ProgressText").GetComponent<TMP_Text>();

            titleText.text = achievement.data.name;
            infoText.text = achievement.data.fullDescription;
            int progress = achievement.taskProgress;
            progressText.text = progress + "/" + achievement.data.goal;
        });

        nameText.text = achievement.data.name;
        descriptionText.text = achievement.data.description;

        return card;
    }
}
